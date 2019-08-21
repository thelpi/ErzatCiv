using System;
using ErsatzCivLib.Model.Static;

namespace ErsatzCivLib.Model.Units.Land
{
    /// <summary>
    /// Represents a unit of militia.
    /// </summary>
    /// <seealso cref="LandUnitPivot"/>
    [Serializable]
    public class MilitiaPivot : LandUnitPivot
    {
        private MilitiaPivot(CityPivot city, MapSquarePivot location, PlayerPivot player) :
            base(city, 1, 1, 1, 10, null, AdvancePivot.Gunpowder, 50, null, 0, location, player)
        { }

        /// <summary>
        /// Default instance.
        /// </summary>
        internal static readonly MilitiaPivot Default = new MilitiaPivot(null, null, null);

        /// <summary>
        /// Static constructior.
        /// </summary>
        /// <param name="city">The <see cref="UnitPivot.City"/> value.</param>
        /// <param name="location">The <see cref="UnitPivot.MapSquareLocation"/> value if <paramref name="city"/> is <c>Null</c>.</param>
        /// <param name="player">The <see cref="UnitPivot.Player"/> value if <paramref name="city"/> is <c>Null</c>.</param>
        /// <returns>An instance of <see cref="MilitiaPivot"/>.</returns>
        internal static MilitiaPivot CreateAtLocation(CityPivot city, MapSquarePivot location, PlayerPivot player)
        {
            return new MilitiaPivot(city, location, player);
        }
    }
}
