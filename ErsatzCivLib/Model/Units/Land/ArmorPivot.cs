using System;
using ErsatzCivLib.Model.Static;

namespace ErsatzCivLib.Model.Units.Land
{
    /// <summary>
    /// Represents a unit of blinded armor.
    /// </summary>
    /// <seealso cref="LandUnitPivot"/>
    [Serializable]
    public class ArmorPivot : LandUnitPivot
    {
        private ArmorPivot(CityPivot city, MapSquarePivot location) :
            base(city, 10, 5, 3, 80, AdvancePivot.Automobile, null, 960, null, 0, location)
        { }

        /// <summary>
        /// Default instance.
        /// </summary>
        internal static readonly ArmorPivot Default = new ArmorPivot(null, null);

        /// <summary>
        /// Static constructior.
        /// </summary>
        /// <param name="city">The <see cref="UnitPivot.City"/> value.</param>
        /// <param name="location">The <see cref="UnitPivot.MapSquareLocation"/> value if <paramref name="city"/> is <c>Null</c>.</param>
        /// <returns>An instance of <see cref="ArmorPivot"/>.</returns>
        internal static ArmorPivot CreateAtLocation(CityPivot city, MapSquarePivot location)
        {
            return new ArmorPivot(city, location);
        }
    }
}
