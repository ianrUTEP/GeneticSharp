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
        private string m_destFolder;
        private System.Linq.Expressions.Expression<Func<double, int, double>> m_fitnessEquation;
        private System.Linq.Expressions.Expression<Func<DemoPt, DemoPt, double>> m_weightEquation;
        #endregion

        #region Constructors
        public TspDemoPart()
        {
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
            ga.TaskExecutor = new LinearTaskExecutor();
            ga.CrossoverProbability = 0.75f; //default
            ga.MutationProbability = 0.1f;  //default
            base.ConfigGA(ga);
        }

        public override ITermination CreateTermination()
        {
            return new OrTermination(new TimeEvolvingTermination(TimeSpan.FromMinutes(240)), new FitnessStagnationTermination(1000));
        }       

        public override IFitness CreateFitness()
        {
            // m_fitness = new DemoFitness(m_numberOfCities, 0, 1000, 0, 1000);

            return m_fitness;
        }
        public override void Initialize()
        {
            base.Initialize();

            Console.WriteLine("Input information file:");
            var inputPointsFile = Console.ReadLine();

            //TODO: figure out how to put the reader and csvreader in using statements
            var reader = new StreamReader(inputPointsFile);
            var csv = new CsvHelper.CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture);
            //Keep this though
            var points = csv.GetRecords<DemoPt>().ToList();

            m_numberOfCities = points.Count;

            //Create the fitness object for evaluation of solutions
            m_fitness = new DemoFitness(points, m_fitnessEquation.Compile(), m_weightEquation.Compile());

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

            // var cities = bestChromosome.GetGenes().Select(g => g.Value.ToString()).ToArray();
            // Console.WriteLine("City tour: {0}");///, string.Join(", ", cities));
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
            Object[] dataFields = {GA.Population.GenerationsNumber.ToString(), GA.Termination.ToString()};
            String[] dataNames = {"Number of populations: ", "Termination conditions: "};
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
