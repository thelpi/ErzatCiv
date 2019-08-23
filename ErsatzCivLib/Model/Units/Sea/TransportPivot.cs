using System;
using ErsatzCivLib.Model.Static;

namespace ErsatzCivLib.Model.Units.Sea
{
    /// <summary>
    /// Represents a transport unit.
    /// </summary>
    /// <seealso cref="SeaUnitPivot"/>
    [Serializable]
    public class TransportPivot : SeaUnitPivot
    {
        private TransportPivot(CityPivot city, MapSquarePivot location, PlayerPivot player) :
            base(city, 0, 3, 4, 50, AdvancePivot.Industrialization, null, 450, null, location, player, false, false, 1, 8, false, 1)
        { }

        /// <summary>
        /// Default instance.
        /// </summary>
        internal static readonly TransportPivot Default = new TransportPivot(null, null, null);

        /// <summary>
        /// Static constructior.
        /// </summary>
        /// <param name="city">The <see cref="UnitPivot.City"/> value.</param>
        /// <param name="location">The <see cref="UnitPivot.MapSquareLocation"/> value, if <paramref name="city"/> is <c>Null</c>.</param>
        /// <param name="player">The <see cref="UnitPivot.Player"/> value, if <paramref name="city"/> is <c>Null</c>.</param>
        /// <returns>An instance of <see cref="TransportPivot"/>.</returns>
        internal static TransportPivot CreateAtLocation(CityPivot city, MapSquarePivot location, PlayerPivot player)
        {
            return new TransportPivot(city, location, player);
        }
    }
}
