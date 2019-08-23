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
        private BomberPivot(CityPivot city, MapSquarePivot location, PlayerPivot player) :
            base(city, 12, 1, 8, 120, AdvancePivot.AdvancedFlight, null, 1920, null, location, player, false, false, true, 2, 2, 1)
        { }

        /// <summary>
        /// Default instance.
        /// </summary>
        internal static readonly BomberPivot Default = new BomberPivot(null, null, null);

        /// <summary>
        /// Static constructior.
        /// </summary>
        /// <param name="city">The <see cref="UnitPivot.City"/> value.</param>
        /// <param name="location">The <see cref="UnitPivot.MapSquareLocation"/> value, if <paramref name="city"/> is <c>Null</c>.</param>
        /// <param name="player">The <see cref="UnitPivot.Player"/> value, if <paramref name="city"/> is <c>Null</c>.</param>
        /// <returns>An instance of <see cref="BomberPivot"/>.</returns>
        internal static BomberPivot CreateAtLocation(CityPivot city, MapSquarePivot location, PlayerPivot player)
        {
            return new BomberPivot(city, location, player);
        }
    }
}
