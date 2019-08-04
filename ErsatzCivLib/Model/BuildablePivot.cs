using System;

namespace ErsatzCivLib.Model
{
    /// <summary>
    /// Represents an item a city can build.
    /// </summary>
    /// <remarks>
    /// Each concrete implementation must implement :
    /// - A <c>static internal readonly</c> field "Default" used as a template.
    /// - A <c>static internal</c> method "CreateAtLocation" used to create instances, based on the default template. The method must have a single parameter, of type <see cref="MapSquarePivot"/>.
    /// - Every constructors must be <c>private</c>.
    /// </remarks>
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
        /// <param name="productivityCost">The <see cref="ProductivityCost"/> value.</param>
        /// <param name="name">Optionnal; the <see cref="Name"/> value.
        /// IF <c>Null</c>, the class name is used without the "Pivot" suffix.</param>
        protected BuildablePivot(int productivityCost, string name = null)
        {
            Name = name ?? GetType().Name.Replace("Pivot", string.Empty);
            ProductivityCost = productivityCost;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Name} ({ProductivityCost})";
        }

        /// <summary>
        /// Indicates the instance is <see cref="CapitalizationPivot"/> typed.
        /// </summary>
        /// <returns><c>True</c> if <see cref="CapitalizationPivot"/>; <c>False</c> otherwise.</returns>
        public bool IsCapitalization()
        {
            return GetType() == typeof(CapitalizationPivot);
        }

        /// <summary>
        /// Indicates the instance is <see cref="UnitPivot"/> typed.
        /// </summary>
        /// <returns><c>True</c> if <see cref="UnitPivot"/>; <c>False</c> otherwise.</returns>
        public bool IsUnit()
        {
            return GetType().IsSubclassOf(typeof(UnitPivot));
        }
    }
}
