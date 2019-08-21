using System;
using ErsatzCivLib.Model.Static;

namespace ErsatzCivLib.Model.Units.Air
{
    /// <summary>
    /// Represents a bomber plane unit.
    /// </summary>
    /// <seealso cref="SeaUnitPivot"/>
    [Serializable]
    public class SubmarinePivot : SeaUnitPivot
    {
        private SubmarinePivot(CityPivot city, MapSquarePivot location, PlayerPivot player) :
            base(city, 8, 2, 3, 50, AdvancePivot.MassProduction, null, 450, null, location, player)
        { }

        /// <summary>
        /// Default instance.
        /// </summary>
        internal static readonly SubmarinePivot Default = new SubmarinePivot(null, null, null);

        /// <summary>
        /// Static constructior.
        /// </summary>
        /// <param name="city">The <see cref="UnitPivot.City"/> value.</param>
        /// <param name="location">The <see cref="UnitPivot.MapSquareLocation"/> value, if <paramref name="city"/> is <c>Null</c>.</param>
        /// <param name="player">The <see cref="UnitPivot.Player"/> value, if <paramref name="city"/> is <c>Null</c>.</param>
        /// <returns>An instance of <see cref="SubmarinePivot"/>.</returns>
        internal static SubmarinePivot CreateAtLocation(CityPivot city, MapSquarePivot location, PlayerPivot player)
        {
            return new SubmarinePivot(city, location, player);
        }
    }
}
