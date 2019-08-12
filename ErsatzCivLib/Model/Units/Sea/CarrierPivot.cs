using System;
using ErsatzCivLib.Model.Static;

namespace ErsatzCivLib.Model.Units.Air
{
    /// <summary>
    /// Represents a bomber plane unit.
    /// </summary>
    /// <seealso cref="SeaUnitPivot"/>
    [Serializable]
    public class CarrierPivot : SeaUnitPivot
    {
        private CarrierPivot(CityPivot city, MapSquarePivot location) :
            base(city, 1, 12, 5, 160, AdvancePivot.AdvancedFlight, null, 3200, null, location)
        { }

        /// <summary>
        /// Default instance.
        /// </summary>
        internal static readonly CarrierPivot Default = new CarrierPivot(null, null);

        /// <summary>
        /// Static constructior.
        /// </summary>
        /// <param name="city">The <see cref="UnitPivot.City"/> value.</param>
        /// <param name="location">The <see cref="UnitPivot.MapSquareLocation"/> value, if <paramref name="city"/> is <c>Null</c>.</param>
        /// <returns>An instance of <see cref="CarrierPivot"/>.</returns>
        internal static CarrierPivot CreateAtLocation(CityPivot city, MapSquarePivot location)
        {
            return new CarrierPivot(city, location);
        }
    }
}
