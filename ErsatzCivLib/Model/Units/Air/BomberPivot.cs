using System;
using ErsatzCivLib.Model.Static;

namespace ErsatzCivLib.Model.Units.Air
{
    /// <summary>
    /// Represents a bomber plane unit.
    /// </summary>
    /// <seealso cref="AirUnitPivot"/>
    [Serializable]
    public class BomberPivot : AirUnitPivot
    {
        private BomberPivot(CityPivot city, MapSquarePivot location) :
            base(city, 12, 1, 8, 120, AdvancePivot.AdvancedFlight, null, 1920, null, location)
        { }

        /// <summary>
        /// Default instance.
        /// </summary>
        internal static readonly BomberPivot Default = new BomberPivot(null, null);

        /// <summary>
        /// Static constructior.
        /// </summary>
        /// <param name="city">The <see cref="UnitPivot.City"/> value.</param>
        /// <param name="location">The <see cref="UnitPivot.MapSquareLocation"/> value, if <paramref name="city"/> is <c>Null</c>.</param>
        /// <returns>An instance of <see cref="BomberPivot"/>.</returns>
        internal static BomberPivot CreateAtLocation(CityPivot city, MapSquarePivot location)
        {
            return new BomberPivot(city, location);
        }
    }
}
