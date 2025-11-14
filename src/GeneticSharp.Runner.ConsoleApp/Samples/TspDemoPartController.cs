using GeneticSharp.Extensions;  //Install with Install-Package GeneticSharp.Extensions
using System;
using System.ComponentModel;
using System.Linq;
using System.IO;
//Install with Install-Package CSVHelper

namespace GeneticSharp.Runner.ConsoleApp.Samples
{
    [DisplayName("TSP for Demo Part")]
    public class TspDemoPart : SampleControllerBase
    {
        #region Fields
        private int m_numberOfCities;
        private DemoFitness m_fitness;
        private string m_destFolder;
        #endregion

        #region Constructors
        public TspDemoPart()
        {
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

            // var targetBitmap = Bitmap.FromFile(inputImageFile) as Bitmap;
            m_fitness = new DemoFitness(points);

            var folder = Path.Combine(Path.GetDirectoryName(inputPointsFile), "results");
            m_destFolder = "{0}_{1:yyyyMMdd_HHmmss}".With(folder, DateTime.Now);
            Directory.CreateDirectory(m_destFolder);
            Console.WriteLine("Result path will be written to '{0}'.", m_destFolder);
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public override IChromosome CreateChromosome()
        {
            return new TspChromosome(m_numberOfCities);
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
            var c = bestChromosome as TspChromosome;
            Console.WriteLine("Cities: {0:n0}", c.Length);
            Console.WriteLine("Distance: {0:n2}", c.Distance);

            // var cities = bestChromosome.GetGenes().Select(g => g.Value.ToString()).ToArray();
            // Console.WriteLine("City tour: {0}");///, string.Join(", ", cities));
        }

        /// <summary>
        /// Exports the sample at the end.
        /// </summary>
        /// <param name="bestChromosome">The current best chromosome</param>
        public override void Export(IChromosome bestChromosome)
        {
            using (var writer = new StreamWriter(m_destFolder + "/solution.csv"))
            using (var csv = new CsvHelper.CsvWriter(writer, System.Globalization.CultureInfo.InvariantCulture))
            {
                var solution = bestChromosome.GetGenes().Select(g => g.Value).ToList(); //.ToArray() was default, trying .ToList() and also not convert to string;
                csv.WriteRecords(solution);
            }
        }
        #endregion
    }
}
