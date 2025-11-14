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
        #endregion

        #region Constructors
        public TspDemoPart() : this(8000)
        {
        }

        public TspDemoPart(int numberOfCities)
        {
            m_numberOfCities = numberOfCities;
        }
        #endregion

        #region Methods
        public override void ConfigGA(GeneticAlgorithm ga)
        {
            ga.TaskExecutor = new LinearTaskExecutor();
            base.ConfigGA(ga);
        }

        public override ITermination CreateTermination()
        {
            return new OrTermination(new TimeEvolvingTermination(TimeSpan.FromMinutes(1)), new FitnessStagnationTermination(500));
        }       

        public override IFitness CreateFitness()
        {
            m_fitness = new DemoFitness(m_numberOfCities, 0, 1000, 0, 1000);

            return m_fitness;
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

            var cities = bestChromosome.GetGenes().Select(g => g.Value.ToString()).ToArray();
            Console.WriteLine("City tour: {0}");///, string.Join(", ", cities));
        }
        #endregion
    }
}
