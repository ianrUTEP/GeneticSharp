namespace GeneticSharp.Extensions
{
    /// <summary>
    /// Point in a DemoPart.
    /// </summary>
    public class DemoPt
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Extensions.DemoPt"/> class.
        /// </summary>
        public DemoPt()
        {
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