using System;
using ErsatzCivLib.Model.Static;

namespace ErsatzCivLib.Model.SpaceShipParts
{
    /// <summary>
    /// Represents a space ship component.
    /// </summary>
    /// <seealso cref="SpaceShipPivot"/>
    [Serializable]
    public class ComponentPivot : SpaceShipPivot
    {
        private ComponentPivot() :
            base(160, AdvancePivot.Plastics, 1280, "SS Component", 8)
        { }

        /// <summary>
        /// Default instance.
        /// </summary>
        internal static readonly ComponentPivot Default = new ComponentPivot();

        /// <summary>
        /// Static constructior.
        /// </summary>
        /// <returns>An instance of <see cref="ComponentPivot"/>.</returns>
        internal static ComponentPivot Create()
        {
            return new ComponentPivot();
        }
    }
}
