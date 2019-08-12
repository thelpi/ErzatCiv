using System;
using ErsatzCivLib.Model.Static;

namespace ErsatzCivLib.Model.Units.Land
{
    /// <summary>
    /// Represents a unit of chariot.
    /// </summary>
    /// <seealso cref="LandUnitPivot"/>
    [Serializable]
    public class ChariotPivot : LandUnitPivot
    {
        private ChariotPivot(CityPivot city, MapSquarePivot location) :
            base(city, 4, 1, 2, 40, AdvancePivot.Wheel, AdvancePivot.Chivalry, 320, null, 0, location)
        { }

        /// <summary>
        /// Default instance.
        /// </summary>
        internal static readonly ChariotPivot Default = new ChariotPivot(null, null);

        /// <summary>
        /// Static constructior.
        /// </summary>
        /// <param name="city">The <see cref="UnitPivot.City"/> value.</param>
        /// <param name="location">The <see cref="UnitPivot.MapSquareLocation"/> value if <paramref name="city"/> is <c>Null</c>.</param>
        /// <returns>An instance of <see cref="ChariotPivot"/>.</returns>
        internal static ChariotPivot CreateAtLocation(CityPivot city, MapSquarePivot location)
        {
            return new ChariotPivot(city, location);
        }
    }
}
