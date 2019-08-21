using System;
using ErsatzCivLib.Model.Static;

namespace ErsatzCivLib.Model.Units.Land
{
    /// <summary>
    /// Represents a unit of cavalry.
    /// </summary>
    /// <seealso cref="LandUnitPivot"/>
    [Serializable]
    public class CavalryPivot : LandUnitPivot
    {
        private CavalryPivot(CityPivot city, MapSquarePivot location, PlayerPivot player) :
            base(city, 2, 1, 2, 20, AdvancePivot.HorsebackRiding, AdvancePivot.Conscription, 120, null, 0, location, player)
        { }

        /// <summary>
        /// Default instance.
        /// </summary>
        internal static readonly CavalryPivot Default = new CavalryPivot(null, null, null);

        /// <summary>
        /// Static constructior.
        /// </summary>
        /// <param name="city">The <see cref="UnitPivot.City"/> value.</param>
        /// <param name="location">The <see cref="UnitPivot.MapSquareLocation"/> value if <paramref name="city"/> is <c>Null</c>.</param>
        /// <param name="player">The <see cref="UnitPivot.Player"/> value, if <paramref name="city"/> is <c>Null</c>.</param>
        /// <returns>An instance of <see cref="CavalryPivot"/>.</returns>
        internal static CavalryPivot CreateAtLocation(CityPivot city, MapSquarePivot location, PlayerPivot player)
        {
            return new CavalryPivot(city, location, player);
        }
    }
}
