using System;

namespace GeneticSharp.Extensions
{
    /// <summary>
    /// Travelling Salesman Problem chromosome.
    /// <remarks>
    /// Each gene represents a city index.
    /// </remarks>
    /// </summary>
    [Serializable]
    public class DemoPtChromosome : ChromosomeBase
    {
        #region Fields
        private readonly int m_numberOfCities;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Extensions.DemoPtChromosome"/> class.
        /// </summary>
        /// <param name="numberOfCities">Number of cities.</param>
        // public DemoPtChromosome(int numberOfCities) : base(numberOfCities)
        // {
        //     m_numberOfCities = numberOfCities;
        //     var citiesIndexes = RandomizationProvider.Current.GetUniqueInts(numberOfCities, 0, numberOfCities);

        //     for (int i = 0; i < numberOfCities; i++)
        //     {
        //         ReplaceGene(i, new Gene(citiesIndexes[i]));
        //     }
        // }
        public DemoPtChromosome(int numberOfCities) : base(numberOfCities)
        {
            m_numberOfCities = numberOfCities;
            // var citiesIndexes = RandomizationProvider.Current.GetUniqueInts(numberOfCities, 0, numberOfCities);

            //Basic first case: just go in numeric order
            for (int i = 0; i < numberOfCities; i++)
            {
                ReplaceGene(i, new Gene(i));
            }
            // m_numberOfCities = numberOfCities;
            // var citiesIndexes = RandomizationProvider.Current.GetUniqueInts(numberOfCities, 0, numberOfCities);

            // for (int i = 0; i < numberOfCities; i++)
            // {
            //     ReplaceGene(i, new Gene(citiesIndexes[i]));
            // }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the distance.
        /// </summary>
        /// <value>The distance.</value>
        public double Distance { get; internal set; }

        /// <summary>
        /// Gets the number of unique points on the path.
        /// Hides the default chromosome length
        /// </summary>
        /// <value>The number of unique points on the path.</value>
        public int Unique { get; internal set; }
        #endregion

        #region implemented abstract members of ChromosomeBase
        /// <summary>
        /// Generates the gene for the specified index.
        /// </summary>
        /// <returns>The gene.</returns>
        /// <param name="geneIndex">Gene index.</param>
        public override Gene GenerateGene(int geneIndex)
        {
            return new Gene(RandomizationProvider.Current.GetInt(0, m_numberOfCities));
        }

        /// <summary>
        /// Creates a new chromosome using the same structure of this.
        /// </summary>
        /// <returns>The new chromosome.</returns>
        public override IChromosome CreateNew()
        {
            return new DemoPtChromosome(m_numberOfCities);
        }

        /// <summary>
        /// Creates a clone.
        /// </summary>
        /// <returns>The chromosome clone.</returns>
        public override IChromosome Clone()
        {
            var clone = base.Clone() as DemoPtChromosome;
            clone.Distance = Distance;

            return clone;
        }
        #endregion
    }
}