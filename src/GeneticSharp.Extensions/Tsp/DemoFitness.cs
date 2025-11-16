using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace GeneticSharp.Extensions
{
    /// <summary>
    /// Travelling Salesman Problem fitness function.
    /// <remarks>
    /// The travelling salesman problem (TSP) or travelling salesperson problem asks the following question: 
    /// Given a list of cities and the distances between each pair of cities, what is the shortest possible 
    /// route that visits each city exactly once and returns to the origin city?
    /// <see href="http://en.wikipedia.org/wiki/Travelling_salesman_problem">Wikipedia</see> 
    /// </remarks>
    /// </summary>
    public class DemoFitness : IFitness
    {
        #region Fields
        private readonly Func<DemoPt, DemoPt, double> m_weightFunction;
        private readonly Func<double, int, double> m_fitFunc;
        #endregion
        
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Extensions.TspFitness"/> class.
        /// </summary>
        /// <param name="numberPts">The number of cities.</param>
        public DemoFitness(List<DemoPt> pointsList, Func<double, int, double> fitnessEquation, Func<DemoPt,DemoPt,double> weightEquation)
        {
            Points = pointsList;
            m_fitFunc = fitnessEquation;
            m_weightFunction = weightEquation;
        }
        #endregion
        
        #region Properties
        /// <summary>
        /// Gets the points.
        /// </summary>
        /// <value>The points.</value>
        public IList<DemoPt> Points { get; private set; }      
        #endregion
        
        #region Methods
        /// <summary>
        /// Performs the evaluation against the specified chromosome.
        /// </summary>
        /// <param name="chromosome">The chromosome to be evaluated.</param>
        /// <returns>The fitness of the chromosome.</returns>
        public double Evaluate(IChromosome chromosome)
        {
            var genes = chromosome.GetGenes();
            var distanceSum = 0.0;
            var lastCityIndex = Convert.ToInt32(genes[0].Value, CultureInfo.InvariantCulture);
            var citiesIndexes = new List<int>
            {
                lastCityIndex
            };

            //Get cost between every internal edge
            for (int i = 0, genesLength = genes.Length; i < genesLength; i++)
            {
                var currentCityIndex = Convert.ToInt32(genes[i].Value, CultureInfo.InvariantCulture);
                distanceSum += CalcDistanceTwoCities(Points[currentCityIndex], Points[lastCityIndex]);
                lastCityIndex = currentCityIndex;

                citiesIndexes.Add(lastCityIndex);
            }

            //Add cost to return to starting city
            distanceSum += CalcDistanceTwoCities(Points[citiesIndexes.Last()], Points[citiesIndexes.First()]);

            var fitness = m_fitFunc(distanceSum, Points.Count);

            ((DemoPtChromosome)chromosome).Distance = distanceSum;
            ((DemoPtChromosome)chromosome).Unique = citiesIndexes.Distinct().Count();

            // There is repeated cities on the indexes?
            var diff = Points.Count - ((DemoPtChromosome)chromosome).Unique;

            //High damage to repeated cities
            if (diff > 0)
            {
                fitness /= diff+1;
            }

            if (fitness < 0)
            {
                fitness = 0;
            }

            return fitness;
        }

        /// <summary>
        /// Calculates the distance between two cities.
        /// </summary>
        /// <returns>The distance two cities.</returns>
        /// <param name="one">City one.</param>
        /// <param name="two">City two.</param>
        private double CalcDistanceTwoCities(DemoPt one, DemoPt two)
        {
            return m_weightFunction(one,two);
        }
        #endregion
    }
}