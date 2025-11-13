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
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Extensions.TspFitness"/> class.
        /// </summary>
        /// <param name="numberOfCities">The number of cities.</param>
        /// <param name="minX">The minimum city x coordinate.</param>
        /// <param name="maxX">The maximum city x coordinate.</param>
        /// <param name="minY">The minimum city y coordinate.</param>
        /// <param name="maxY">The maximum city y coordinate..</param>
        public DemoFitness(int numberOfCities, int minX, int maxX, int minY, int maxY)
        {
            points = new List<DemoPt>(numberOfCities);
            MinX = minX;
            MaxX = maxX;
            MinY = minY;
            MaxY = maxY;

            if (maxX >= int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(maxX));
            }

            if (maxY >= int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(maxY));
            }

            for (int i = 0; i < numberOfCities; i++)
            {
                var city = new DemoPt(RandomizationProvider.Current.GetDouble(MinX, maxX + 1), RandomizationProvider.Current.GetDouble(MinY, maxY + 1),
                    RandomizationProvider.Current.GetDouble(MinX, maxX + 1), RandomizationProvider.Current.GetDouble(MinY, maxY + 1));
                points.Add(city);
            }
        }
        
        /// <summary>
        /// Gets the cities.
        /// </summary>
        /// <value>The cities.</value>
        public IList<DemoPt> points { get; private set; }

        /// <summary>
        /// Gets the minimum x.
        /// </summary>
        /// <value>The minimum x.</value>
        public int MinX { get; private set; }

        /// <summary>
        /// Gets the max x.
        /// </summary>
        /// <value>The max x.</value>
        public int MaxX { get; private set; }

        /// <summary>
        /// Gets the minimum y.
        /// </summary>
        /// <value>The minimum y.</value>
        public int MinY { get; private set; }

        /// <summary>
        /// Gets the max y.
        /// </summary>
        /// <value>The max y.</value>
        public int MaxY { get; private set; }        
        
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

            for (int i = 0, genesLength = genes.Length; i < genesLength; i++)
            {
                var currentCityIndex = Convert.ToInt32(genes[i].Value, CultureInfo.InvariantCulture);
                distanceSum += CalcDistanceTwoCities(points[currentCityIndex], points[lastCityIndex]);
                lastCityIndex = currentCityIndex;

                citiesIndexes.Add(lastCityIndex);
            }

            distanceSum += CalcDistanceTwoCities(points[citiesIndexes.Last()], points[citiesIndexes.First()]);

            var fitness = 1.0 - (distanceSum / (points.Count * 1000.0));

            ((TspChromosome)chromosome).Distance = distanceSum;

            // There is repeated cities on the indexes?
            var diff = points.Count - citiesIndexes.Distinct().Count();

            if (diff > 0)
            {
                fitness /= diff;
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
        private static double CalcDistanceTwoCities(DemoPt one, DemoPt two)
        {
            return Math.Sqrt(Math.Pow(two.X - one.X, 2) + Math.Pow(two.Y - one.Y, 2));
        }        
    }
}