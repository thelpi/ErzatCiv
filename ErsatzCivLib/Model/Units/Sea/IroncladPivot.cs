using System;
using ErsatzCivLib.Model.Static;

namespace ErsatzCivLib.Model.Units.Air
{
    /// <summary>
    /// Represents a bomber plane unit.
    /// </summary>
    /// <seealso cref="SeaUnitPivot"/>
    [Serializable]
    public class IroncladPivot : SeaUnitPivot
    {
        private IroncladPivot(CityPivot city, MapSquarePivot location, PlayerPivot player) :
            base(city, 4, 4, 4, 60, AdvancePivot.SteamEngine, AdvancePivot.Combustion, 600, null, location, player)
        { }

        /// <summary>
        /// Default instance.
        /// </summary>
        internal static readonly IroncladPivot Default = new IroncladPivot(null, null, null);

        /// <summary>
        /// Static constructior.
        /// </summary>
        /// <param name="city">The <see cref="UnitPivot.City"/> value.</param>
        /// <param name="location">The <see cref="UnitPivot.MapSquareLocation"/> value, if <paramref name="city"/> is <c>Null</c>.</param>
        /// <param name="player">The <see cref="UnitPivot.Player"/> value if <paramref name="city"/> is <c>Null</c>.</param>
        /// <returns>An instance of <see cref="IroncladPivot"/>.</returns>
        internal static IroncladPivot CreateAtLocation(CityPivot city, MapSquarePivot location, PlayerPivot player)
        {
            return new IroncladPivot(city, location, player);
        }
    }
}
