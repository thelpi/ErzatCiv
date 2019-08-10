using System;
using ErsatzCivLib.Model.Static;

namespace ErsatzCivLib.Model.SpaceShipParts
{
    /// <summary>
    /// Represents a space ship module.
    /// </summary>
    /// <seealso cref="SpaceShipPivot"/>
    [Serializable]
    public class ModulePivot : SpaceShipPivot
    {
        private ModulePivot() :
            base(320, AdvancePivot.Robotics, 2560, "SS Module", 4)
        { }

        /// <summary>
        /// Default instance.
        /// </summary>
        internal static readonly ModulePivot Default = new ModulePivot();

        /// <summary>
        /// Static constructior.
        /// </summary>
        /// <returns>An instance of <see cref="ModulePivot"/>.</returns>
        internal static ModulePivot Create()
        {
            return new ModulePivot();
        }
    }
}
