using System;
using ErsatzCivLib.Model.Static;

namespace ErsatzCivLib.Model.Units.Sea
{
    /// <summary>
    /// Represents a trireme unit.
    /// </summary>
    /// <remarks>Must finish turn on a coast square.</remarks>
    /// <seealso cref="SeaUnitPivot"/>
    [Serializable]
    public class TriremePivot : SeaUnitPivot
    {
        private TriremePivot(CityPivot city, MapSquarePivot location, PlayerPivot player) :
            base(city, 1, 0, 3, 40, AdvancePivot.MapMaking, AdvancePivot.Navigation, 320, null, location, player, false, false, 1, 2, false, 1)
        { }

        /// <summary>
        /// Default instance.
        /// </summary>
        internal static readonly TriremePivot Default = new TriremePivot(null, null, null);

        /// <summary>
        /// Static constructior.
        /// </summary>
        /// <param name="city">The <see cref="UnitPivot.City"/> value.</param>
        /// <param name="location">The <see cref="UnitPivot.MapSquareLocation"/> value, if <paramref name="city"/> is <c>Null</c>.</param>
        /// <param name="player">The <see cref="UnitPivot.Player"/> value, if <paramref name="city"/> is <c>Null</c>.</param>
        /// <returns>An instance of <see cref="TriremePivot"/>.</returns>
        internal static TriremePivot CreateAtLocation(CityPivot city, MapSquarePivot location, PlayerPivot player)
        {
            return new TriremePivot(city, location, player);
        }
    }
}
