using System;
using ErsatzCivLib.Model.Static;

namespace ErsatzCivLib.Model.Units.Land
{
    /// <summary>
    /// Represents a unit of caravan.
    /// </summary>
    /// <seealso cref="LandUnitPivot"/>
    [Serializable]
    public class CaravanPivot : LandUnitPivot
    {
        private CaravanPivot(CityPivot city, MapSquarePivot location) :
            base(city, 0, 1, 1, 50, AdvancePivot.Trade, null, 450, null, 0, location)
        { }

        /// <summary>
        /// Default instance.
        /// </summary>
        internal static readonly CaravanPivot Default = new CaravanPivot(null, null);

        /// <summary>
        /// Static constructior.
        /// </summary>
        /// <param name="city">The <see cref="UnitPivot.City"/> value.</param>
        /// <param name="location">The <see cref="UnitPivot.MapSquareLocation"/> value if <paramref name="city"/> is <c>Null</c>.</param>
        /// <returns>An instance of <see cref="CaravanPivot"/>.</returns>
        internal static CaravanPivot CreateAtLocation(CityPivot city, MapSquarePivot location)
        {
            return new CaravanPivot(city, location);
        }
    }
}
