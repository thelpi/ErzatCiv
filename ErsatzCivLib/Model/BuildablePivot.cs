using System;

namespace ErsatzCivLib.Model
{
    /// <summary>
    /// Represents an item a city can build.
    /// </summary>
    [Serializable]
    public abstract class BuildablePivot
    {
        /// <summary>
        /// Name.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Productivity amount required to build the item.
        /// </summary>
        public int ProductivityCost { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The <see cref="Name"/> value.</param>
        /// <param name="productivityCost">The <see cref="ProductivityCost"/> value.</param>
        protected BuildablePivot(string name, int productivityCost)
        {
            Name = name;
            ProductivityCost = productivityCost;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Name;
        }
    }
}
