using System;
using ErsatzCivLib.Model.Static;

namespace ErsatzCivLib.Model.SpaceShipParts
{
    /// <summary>
    /// Represents a space ship structural.
    /// </summary>
    /// <seealso cref="SpaceShipPivot"/>
    [Serializable]
    public class StructuralPivot : SpaceShipPivot
    {
        private StructuralPivot() :
            base(80, AdvancePivot.SpaceFlight, 640, "SS Structural", 39)
        { }

        /// <summary>
        /// Default instance.
        /// </summary>
        internal static readonly StructuralPivot Default = new StructuralPivot();

        /// <summary>
        /// Static constructior.
        /// </summary>
        /// <returns>An instance of <see cref="StructuralPivot"/>.</returns>
        internal static StructuralPivot Create()
        {
            return new StructuralPivot();
        }
    }
}
