using System;
using ErsatzCivLib.Model.Static;

namespace ErsatzCivLib.Model.Units.Land
{
    /// <summary>
    /// Represents a unit of diplomat.
    /// </summary>
    /// <seealso cref="LandUnitPivot"/>
    [Serializable]
    public class DiplomatPivot : LandUnitPivot
    {
        private DiplomatPivot(CityPivot city, MapSquarePivot location, PlayerPivot player) :
            base(city, 0, 0, 2, 30, AdvancePivot.Writing, null, 210, null, 0, location, player)
        { }

        /// <summary>
        /// Default instance.
        /// </summary>
        internal static readonly DiplomatPivot Default = new DiplomatPivot(null, null, null);

        /// <summary>
        /// Static constructior.
        /// </summary>
        /// <param name="city">The <see cref="UnitPivot.City"/> value.</param>
        /// <param name="location">The <see cref="UnitPivot.MapSquareLocation"/> value if <paramref name="city"/> is <c>Null</c>.</param>
        /// <param name="player">The <see cref="UnitPivot.Player"/> value, if <paramref name="city"/> is <c>Null</c>.</param>
        /// <returns>An instance of <see cref="DiplomatPivot"/>.</returns>
        internal static DiplomatPivot CreateAtLocation(CityPivot city, MapSquarePivot location, PlayerPivot player)
        {
            return new DiplomatPivot(city, location, player);
        }
    }
}
