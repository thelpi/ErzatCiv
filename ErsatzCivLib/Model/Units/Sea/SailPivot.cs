using System;
using ErsatzCivLib.Model.Static;

namespace ErsatzCivLib.Model.Units.Air
{
    /// <summary>
    /// Represents a bomber plane unit.
    /// </summary>
    /// <seealso cref="SeaUnitPivot"/>
    [Serializable]
    public class SailPivot : SeaUnitPivot
    {
        private SailPivot(CityPivot city, MapSquarePivot location, PlayerPivot player) :
            base(city, 1, 1, 3, 40, AdvancePivot.Navigation, AdvancePivot.Magnetism, 320, null, location, player)
        { }

        /// <summary>
        /// Default instance.
        /// </summary>
        internal static readonly SailPivot Default = new SailPivot(null, null, null);

        /// <summary>
        /// Static constructior.
        /// </summary>
        /// <param name="city">The <see cref="UnitPivot.City"/> value.</param>
        /// <param name="location">The <see cref="UnitPivot.MapSquareLocation"/> value, if <paramref name="city"/> is <c>Null</c>.</param>
        /// <param name="player">The <see cref="UnitPivot.Player"/> value, if <paramref name="city"/> is <c>Null</c>.</param>
        /// <returns>An instance of <see cref="SailPivot"/>.</returns>
        internal static SailPivot CreateAtLocation(CityPivot city, MapSquarePivot location, PlayerPivot player)
        {
            return new SailPivot(city, location, player);
        }
    }
}
