using System;
using ErsatzCivLib.Model.Static;

namespace ErsatzCivLib.Model.Units.Sea
{
    /// <summary>
    /// Represents a submarine unit.
    /// </summary>
    /// <remarks>Invisible to land units, can be spotted only when adjacent to a sea or an air unit (not when two spaces away).</remarks>
    /// <seealso cref="SeaUnitPivot"/>
    [Serializable]
    public class SubmarinePivot : SeaUnitPivot
    {
        private SubmarinePivot(CityPivot city, MapSquarePivot location, PlayerPivot player) :
            base(city, 8, 2, 3, 50, AdvancePivot.MassProduction, null, 450, null, location, player, false, false, 2, 0, false, 1)
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
