using System;
using ErsatzCivLib.Model.Static;

namespace ErsatzCivLib.Model.Units.Air
{
    /// <summary>
    /// Represents a fighter plane unit.
    /// </summary>
    /// <seealso cref="AirUnitPivot"/>
    [Serializable]
    public class FighterPivot : AirUnitPivot
    {
        private FighterPivot(CityPivot city, MapSquarePivot location, PlayerPivot player) :
            base(city, 4, 2, 10, 60, AdvancePivot.Flight, null, 600, null, location, player)
        { }

        /// <summary>
        /// Default instance.
        /// </summary>
        internal static readonly FighterPivot Default = new FighterPivot(null, null, null);

        /// <summary>
        /// Static constructior.
        /// </summary>
        /// <param name="city">The <see cref="UnitPivot.City"/> value.</param>
        /// <param name="location">The <see cref="UnitPivot.MapSquareLocation"/> value, if <paramref name="city"/> is <c>Null</c>.</param>
        /// <param name="player">The <see cref="UnitPivot.Player"/> value, if <paramref name="city"/> is <c>Null</c>.</param>
        /// <returns>An instance of <see cref="FighterPivot"/>.</returns>
        internal static FighterPivot CreateAtLocation(CityPivot city, MapSquarePivot location, PlayerPivot player)
        {
            return new FighterPivot(city, location, player);
        }
    }
}
