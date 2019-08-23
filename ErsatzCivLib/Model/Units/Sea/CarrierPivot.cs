using System;
using ErsatzCivLib.Model.Static;

namespace ErsatzCivLib.Model.Units.Sea
{
    /// <summary>
    /// Represents a carrier unit.
    /// </summary>
    /// <remarks>Unlike others <see cref="SeaUnitPivot"/>, can carry (and only) <see cref="AirUnitPivot"/>.</remarks>
    /// <seealso cref="SeaUnitPivot"/>
    [Serializable]
    public class CarrierPivot : SeaUnitPivot
    {
        private CarrierPivot(CityPivot city, MapSquarePivot location, PlayerPivot player) :
            base(city, 1, 12, 5, 160, AdvancePivot.AdvancedFlight, null, 3200, null, location, player, false, false, 2, 8, false, 1)
        { }

        /// <summary>
        /// Default instance.
        /// </summary>
        internal static readonly CarrierPivot Default = new CarrierPivot(null, null, null);

        /// <summary>
        /// Static constructior.
        /// </summary>
        /// <param name="city">The <see cref="UnitPivot.City"/> value.</param>
        /// <param name="location">The <see cref="UnitPivot.MapSquareLocation"/> value, if <paramref name="city"/> is <c>Null</c>.</param>
        /// <param name="player">The <see cref="UnitPivot.Player"/> value, if <paramref name="city"/> is <c>Null</c>.</param>
        /// <returns>An instance of <see cref="CarrierPivot"/>.</returns>
        internal static CarrierPivot CreateAtLocation(CityPivot city, MapSquarePivot location, PlayerPivot player)
        {
            return new CarrierPivot(city, location, player);
        }
    }
}
