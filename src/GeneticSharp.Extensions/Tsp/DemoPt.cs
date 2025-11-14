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
        public DemoPt(/*double x, double y, double fieldx, double fieldy*/)
        {
            // X = x;
            // Y = y;
            // FieldX = fieldx;
            // FieldY = fieldy;
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
        public double FieldX { get; set; }

        /// <summary>
        /// Gets or sets the interpolated Y function value.
        /// </summary>
        /// <value>The interpolated Y function value.</value>
        public double FieldY { get; set; }
        #endregion
    }
}