using System;
using ErsatzCivLib.Model.Static;

namespace ErsatzCivLib.Model.Units.Air
{
    /// <summary>
    /// Represents a fighter plane unit.
    /// </summary>
    /// <seealso cref="AirUnitPivot"/>
    [Serializable]
    public class FighterPivot : AirUnitPivot
    {
        private FighterPivot(CityPivot city, MapSquarePivot location) :
            base(city, 4, 2, 10, 60, AdvancePivot.Flight, null, 600, null, location)
        { }

        /// <summary>
        /// Default instance.
        /// </summary>
        internal static readonly FighterPivot Default = new FighterPivot(null, null);

        /// <summary>
        /// Static constructior.
        /// </summary>
        /// <param name="city">The <see cref="UnitPivot.City"/> value.</param>
        /// <param name="location">The <see cref="UnitPivot.MapSquareLocation"/> value, if <paramref name="city"/> is <c>Null</c>.</param>
        /// <returns>An instance of <see cref="FighterPivot"/>.</returns>
        internal static FighterPivot CreateAtLocation(CityPivot city, MapSquarePivot location)
        {
            return new FighterPivot(city, location);
        }
    }
}
