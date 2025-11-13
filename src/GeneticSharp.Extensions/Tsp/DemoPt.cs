namespace GeneticSharp.Extensions
{
    /// <summary>
    /// Travelling Salesman city.
    /// </summary>
    public class DemoPt
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Extensions.TspCity"/> class.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        public DemoPt(double x, double y, double interpx, double interpy)
        {
            X = x;
            Y = y;
            InterpX = interpx;
            InterpY = interpy;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the x.
        /// </summary>
        /// <value>The x.</value>
        public double X { get; set; }

        /// <summary>
        /// Gets or sets the y.
        /// </summary>
        /// <value>The y.</value>
        public double Y { get; set; }

        /// <summary>
        /// Gets or sets the interpolated X function value.
        /// </summary>
        /// <value>The interpolated X function value.</value>
        public double InterpX { get; set; }

        /// <summary>
        /// Gets or sets the interpolated Y function value.
        /// </summary>
        /// <value>The interpolated Y function value.</value>
        public double InterpY { get; set; }
        #endregion
    }
}