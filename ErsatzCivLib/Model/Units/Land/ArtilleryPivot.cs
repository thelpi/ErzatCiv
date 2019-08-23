using System;
using ErsatzCivLib.Model.Static;

namespace ErsatzCivLib.Model.Units.Land
{
    /// <summary>
    /// Represents a unit of artillery.
    /// </summary>
    /// <seealso cref="LandUnitPivot"/>
    [Serializable]
    public class ArtilleryPivot : LandUnitPivot
    {
        private ArtilleryPivot(CityPivot city, MapSquarePivot location, PlayerPivot player) :
            base(city, 12, 2, 2, 60, AdvancePivot.Robotics, null, 600, null, 0, location, player, ignoreCityWalls: true)
        { }

        /// <summary>
        /// Default instance.
        /// </summary>
        internal static readonly ArtilleryPivot Default = new ArtilleryPivot(null, null, null);

        /// <summary>
        /// Static constructior.
        /// </summary>
        /// <param name="city">The <see cref="UnitPivot.City"/> value.</param>
        /// <param name="location">The <see cref="UnitPivot.MapSquareLocation"/> value if <paramref name="city"/> is <c>Null</c>.</param>
        /// <param name="player">The <see cref="UnitPivot.Player"/> value, if <paramref name="city"/> is <c>Null</c>.</param>
        /// <returns>An instance of <see cref="ArtilleryPivot"/>.</returns>
        internal static ArtilleryPivot CreateAtLocation(CityPivot city, MapSquarePivot location, PlayerPivot player)
        {
            return new ArtilleryPivot(city, location, player);
        }
    }
}
