using System;
using ErsatzCivLib.Model.Static;

namespace ErsatzCivLib.Model.Units.Land
{
    /// <summary>
    /// Represents a unit of cannon.
    /// </summary>
    /// <seealso cref="LandUnitPivot"/>
    [Serializable]
    public class CannonPivot : LandUnitPivot
    {
        private CannonPivot(CityPivot city, MapSquarePivot location, PlayerPivot player) :
            base(city, 8, 1, 1, 40, AdvancePivot.Metallurgy, AdvancePivot.Robotics, 320, null, 0, location, player)
        { }

        /// <summary>
        /// Default instance.
        /// </summary>
        internal static readonly CannonPivot Default = new CannonPivot(null, null, null);

        /// <summary>
        /// Static constructior.
        /// </summary>
        /// <param name="city">The <see cref="UnitPivot.City"/> value.</param>
        /// <param name="location">The <see cref="UnitPivot.MapSquareLocation"/> value if <paramref name="city"/> is <c>Null</c>.</param>
        /// <param name="player">The <see cref="UnitPivot.Player"/> value, if <paramref name="city"/> is <c>Null</c>.</param>
        /// <returns>An instance of <see cref="CannonPivot"/>.</returns>
        internal static CannonPivot CreateAtLocation(CityPivot city, MapSquarePivot location, PlayerPivot player)
        {
            return new CannonPivot(city, location, player);
        }
    }
}
