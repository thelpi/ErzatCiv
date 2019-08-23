using System;
using ErsatzCivLib.Model.Static;

namespace ErsatzCivLib.Model.Units.Air
{
    /// <summary>
    /// Represents a nuclear strike unit.
    /// </summary>
    /// <remarks>
    /// Disappears on use.
    /// Can't be seen by opponent.
    /// Causes pollution on land, but not on sea.
    /// </remarks>
    /// <seealso cref="AirUnitPivot"/>
    [Serializable]
    public class NuclearPivot : AirUnitPivot
    {
        private NuclearPivot(CityPivot city, MapSquarePivot location, PlayerPivot player) :
            base(city, 99, 0, 16, 160, AdvancePivot.Rocketry, null, 3200, null, location, player, true, false, false, 1, 1, 1)
        { }

        /// <summary>
        /// Default instance.
        /// </summary>
        internal static readonly NuclearPivot Default = new NuclearPivot(null, null, null);

        /// <summary>
        /// Static constructior.
        /// </summary>
        /// <param name="city">The <see cref="UnitPivot.City"/> value.</param>
        /// <param name="location">The <see cref="UnitPivot.MapSquareLocation"/> value, if <paramref name="city"/> is <c>Null</c>.</param>
        /// <param name="player">The <see cref="UnitPivot.Player"/> value, if <paramref name="city"/> is <c>Null</c>.</param>
        /// <returns>An instance of <see cref="NuclearPivot"/>.</returns>
        internal static NuclearPivot CreateAtLocation(CityPivot city, MapSquarePivot location, PlayerPivot player)
        {
            return new NuclearPivot(city, location, player);
        }
    }
}
