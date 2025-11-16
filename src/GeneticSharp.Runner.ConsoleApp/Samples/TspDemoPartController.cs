using GeneticSharp.Extensions;  //Install with Install-Package GeneticSharp.Extensions
using System;
using System.ComponentModel; 
using System.Linq;      //Lambda expressions
using System.IO;        //File r/w
using System.Text;      //Building parameter output
using System.Collections.Generic;   //List of points
//Install CSVHelper package through Install-Package or other package methods

namespace GeneticSharp.Runner.ConsoleApp.Samples
{
    [DisplayName("TSP for Demo Part")]
    public class TspDemoPart : SampleControllerBase
    {
        #region Fields
        private int m_numberOfCities;
        private DemoFitness m_fitness;
        private string m_sourceFile;
        private string m_destFolder;
        private float m_crossProb;
        private float m_mutateProb;
        private System.Linq.Expressions.Expression<Func<double, int, double>> m_fitnessEquation;
        private System.Linq.Expressions.Expression<Func<DemoPt, DemoPt, double>> m_weightEquation;
        #endregion

        #region Constructors
        public TspDemoPart()
        {
            // Configurable values for the system - only population size is not controllable through this file
            m_crossProb = 0.20f;    //default 0.75 - minimal impact but slows down generation time as increases
            m_mutateProb = 0.90f;   //default 0.1 - increasing has large impact for our purposes with little decrease in generation time

            // Fitness function is kind of arbitrary
            // By having the incorrect factor be divided by smaller ratio it increases the resolution
            // of the fitness and is less likely to stagnate due to floating point
            // Too small and fitness is zero'd out, no idea how to solve
            m_fitnessEquation = (double totalDist, int pointCount) //Given the total distance of a solution and the point count, how fit is it?
                => 1.0 - (totalDist / (pointCount * 2.4));
            // Weight equation:
            // This runs for every edge considered and is given the two points to consider. DemoPt objects contain their X, Y, FieldX, and FieldY components
            m_weightEquation = (DemoPt one, DemoPt two) 
                => Math.Sqrt(Math.Pow(two.X - one.X, 2) + Math.Pow(two.Y - one.Y, 2));
        }
        #endregion

        #region Methods
        public override void ConfigGA(GeneticAlgorithm ga)
        {
            ga.TaskExecutor = new ParallelTaskExecutor();//LinearTaskExecutor();
            ga.CrossoverProbability = m_crossProb; 
            ga.MutationProbability = m_mutateProb;  
            base.ConfigGA(ga);
        }

        public override ITermination CreateTermination()
        {
            return new OrTermination(new TimeEvolvingTermination(TimeSpan.FromMinutes(240)), new FitnessStagnationTermination(1000));
        }       

        public override IFitness CreateFitness()
        {
            return m_fitness;
        }
        public override void Initialize()
        {
            base.Initialize();

            ///The user will input the file at runtime
            Console.WriteLine("Input information file:");
            var inputPointsFile = Console.ReadLine();
            m_sourceFile = inputPointsFile;

            //Read the points from a CSV file
            List<DemoPt> points;
            using (var reader = new StreamReader(inputPointsFile))
            using (var csv = new CsvHelper.CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture))
            {
                points = csv.GetRecords<DemoPt>().ToList();
            }
            m_numberOfCities = points.Count;

            //Create the fitness object for evaluation of solutions
            m_fitness = new DemoFitness(points, m_fitnessEquation.Compile(), m_weightEquation.Compile());

            //Where information will be stored at the end
            var folder = Path.Combine(Path.GetDirectoryName(inputPointsFile), "results");
            m_destFolder = "{0}_{1:yyyyMMdd_HHmmss}".With(folder, DateTime.Now);
            Directory.CreateDirectory(m_destFolder);
            Console.WriteLine("Result path will be written to '{0}'.", m_destFolder);
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public override IChromosome CreateChromosome()
        {
            return new DemoPtChromosome(m_numberOfCities);
        }

        public override ICrossover CreateCrossover()
        {
            return new OrderedCrossover();
        }

        public override IMutation CreateMutation()
        {
            return new ReverseSequenceMutation();
        }

        /// <summary>
        /// Draws the sample.
        /// </summary>
        /// <param name="bestChromosome">The current best chromosome</param>
        public override void Draw(IChromosome bestChromosome)
        {
            var c = bestChromosome as DemoPtChromosome;
            Console.WriteLine("Total points: {0:n0}", c.Length);
            Console.WriteLine("Unique points on path: {0:n0}", c.Unique);
            Console.WriteLine("Distance: {0:n2}", c.Distance);
        }

        /// <summary>
        /// Exports the sample at the end and extra data
        /// </summary>
        public override void Export()
        {
            using (var writer = new StreamWriter(m_destFolder + "/solution.csv"))
            using (var csv = new CsvHelper.CsvWriter(writer, System.Globalization.CultureInfo.InvariantCulture))
            {
                var solution = GA.BestChromosome.GetGenes().Select(g => g.Value).ToList(); //ToString().ToArray() was default, trying .ToList() and also not convert to string;
                csv.WriteRecords(solution);
            }
            StringBuilder sb = new StringBuilder();
            var chrome = GA.BestChromosome as DemoPtChromosome;
            object[] dataFields = { 
                                    m_sourceFile.ToString(),
                                    GA.Termination.ToString(), 
                                    GA.Population.GenerationsNumber.ToString(), 
                                    GA.Population.MinSize.ToString(), 
                                    GA.Population.MaxSize.ToString(), 
                                    CreateCrossover().ToString(),
                                    m_crossProb.ToString(), 
                                    CreateMutation().ToString(),
                                    m_mutateProb.ToString(),
                                    m_fitnessEquation.ToString(),
                                    m_weightEquation.ToString(),
                                    (GA.Population.GenerationsNumber / GA.TimeEvolving.TotalSeconds).ToString(),
                                    chrome.Unique.ToString(),
                                    chrome.Fitness.ToString(),
                                    chrome.Distance.ToString(),
                                    };
            string[] dataNames = {  
                                    "Source points file: ",
                                    "Termination conditions: ", 
                                    "Number of generations: ", 
                                    "Minimum population size: ", 
                                    "Maximum population size: ", 
                                    "Crossover type: ",
                                    "Crossover propability: ", 
                                    "Mutation type: ",
                                    "Mutation probability: ",
                                    "Fitness equation: ",
                                    "Weight equation: ",
                                    "Average speed gen/sec: ",
                                    "Best chromosome unique: ",
                                    "Best chromosome fitness: ",
                                    "Best chromosome distance: "
                                    };
            for (int ctr = 0; ctr <= dataNames.GetUpperBound(0); ctr++)
            {
                sb.Append(dataNames[ctr]);
                sb.Append(dataFields[ctr]);
                sb.AppendLine();
            }
            using StreamWriter outputData = new(Path.Combine(m_destFolder, "runParams.txt"));
            outputData.Write(sb.ToString());
        }
        #endregion
    }
}
