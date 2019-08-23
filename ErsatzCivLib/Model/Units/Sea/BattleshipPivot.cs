using System;
using ErsatzCivLib.Model.Static;

namespace ErsatzCivLib.Model.Units.Sea
{
    /// <summary>
    /// Represents a battleship unit.
    /// </summary>
    /// <seealso cref="SeaUnitPivot"/>
    [Serializable]
    public class BattleshipPivot : SeaUnitPivot
    {
        private BattleshipPivot(CityPivot city, MapSquarePivot location, PlayerPivot player) :
            base(city, 18, 12, 4, 160, AdvancePivot.Steel, null, 3200, null, location, player, false, false, 2, 0, true, 1)
        { }

        /// <summary>
        /// Default instance.
        /// </summary>
        internal static readonly BattleshipPivot Default = new BattleshipPivot(null, null, null);

        /// <summary>
        /// Static constructior.
        /// </summary>
        /// <param name="city">The <see cref="UnitPivot.City"/> value.</param>
        /// <param name="location">The <see cref="UnitPivot.MapSquareLocation"/> value, if <paramref name="city"/> is <c>Null</c>.</param>
        /// <param name="player">The <see cref="UnitPivot.Player"/> value, if <paramref name="city"/> is <c>Null</c>.</param>
        /// <returns>An instance of <see cref="BattleshipPivot"/>.</returns>
        internal static BattleshipPivot CreateAtLocation(CityPivot city, MapSquarePivot location, PlayerPivot player)
        {
            return new BattleshipPivot(city, location, player);
        }
    }
}
