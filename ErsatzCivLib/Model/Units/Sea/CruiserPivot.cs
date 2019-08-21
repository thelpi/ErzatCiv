using System;
using ErsatzCivLib.Model.Static;

namespace ErsatzCivLib.Model.Units.Air
{
    /// <summary>
    /// Represents a bomber plane unit.
    /// </summary>
    /// <seealso cref="SeaUnitPivot"/>
    [Serializable]
    public class CruiserPivot : SeaUnitPivot
    {
        private CruiserPivot(CityPivot city, MapSquarePivot location, PlayerPivot player) :
            base(city, 6, 6, 6, 80, AdvancePivot.Combustion, null, 960, null, location, player)
        { }

        /// <summary>
        /// Default instance.
        /// </summary>
        internal static readonly CruiserPivot Default = new CruiserPivot(null, null, null);

        /// <summary>
        /// Static constructior.
        /// </summary>
        /// <param name="city">The <see cref="UnitPivot.City"/> value.</param>
        /// <param name="location">The <see cref="UnitPivot.MapSquareLocation"/> value, if <paramref name="city"/> is <c>Null</c>.</param>
        /// <param name="player">The <see cref="UnitPivot.Player"/> value, if <paramref name="city"/> is <c>Null</c>.</param>
        /// <returns>An instance of <see cref="CruiserPivot"/>.</returns>
        internal static CruiserPivot CreateAtLocation(CityPivot city, MapSquarePivot location, PlayerPivot player)
        {
            return new CruiserPivot(city, location, player);
        }
    }
}
