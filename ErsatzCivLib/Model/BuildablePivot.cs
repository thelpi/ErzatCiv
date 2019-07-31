namespace ErsatzCivLib.Model
{
    /// <summary>
    /// Represents an item a city can build.
    /// </summary>
    public abstract class BuildablePivot
    {
        /// <summary>
        /// Productivity amount required to build the item.
        /// </summary>
        public int ProductivityCost { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="productivityCost">The <see cref="ProductivityCost"/> value.</param>
        protected BuildablePivot(int productivityCost)
        {
            ProductivityCost = productivityCost;
        }
    }
}
