using System;
using ErsatzCivLib.Model.Static;

namespace ErsatzCivLib.Model.Units
{
    /// <summary>
    /// Represents a nuclear strike unit.
    /// </summary>
    [Serializable]
    public class NuclearPivot : UnitPivot
    {
        private NuclearPivot(CityPivot city, MapSquarePivot location) :
            base(city, true, true, 0, 99, 16, 160, AdvancePivot.Rocketry, null, 3200, null, 0, location)
        { }

        /// <summary>
        /// Default instance.
        /// </summary>
        internal static readonly NuclearPivot Default = new NuclearPivot(null, null);

        /// <summary>
        /// Static constructior.
        /// </summary>
        /// <param name="city">The <see cref="UnitPivot.City"/> value.</param>
        /// <param name="location">The <see cref="UnitPivot.MapSquareLocation"/> value, if <paramref name="city"/> is <c>Null</c>.</param>
        /// <returns>An instance of <see cref="NuclearPivot"/>.</returns>
        internal static NuclearPivot CreateAtLocation(CityPivot city, MapSquarePivot location)
        {
            return new NuclearPivot(city, location);
        }
    }
}
