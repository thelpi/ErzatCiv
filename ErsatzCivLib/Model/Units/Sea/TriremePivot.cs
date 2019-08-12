using System;
using ErsatzCivLib.Model.Static;

namespace ErsatzCivLib.Model.Units.Air
{
    /// <summary>
    /// Represents a bomber plane unit.
    /// </summary>
    /// <seealso cref="SeaUnitPivot"/>
    [Serializable]
    public class TriremePivot : SeaUnitPivot
    {
        private TriremePivot(CityPivot city, MapSquarePivot location) :
            base(city, 1, 0, 3, 40, AdvancePivot.MapMaking, AdvancePivot.Navigation, 320, null, location)
        { }

        /// <summary>
        /// Default instance.
        /// </summary>
        internal static readonly TriremePivot Default = new TriremePivot(null, null);

        /// <summary>
        /// Static constructior.
        /// </summary>
        /// <param name="city">The <see cref="UnitPivot.City"/> value.</param>
        /// <param name="location">The <see cref="UnitPivot.MapSquareLocation"/> value, if <paramref name="city"/> is <c>Null</c>.</param>
        /// <returns>An instance of <see cref="TriremePivot"/>.</returns>
        internal static TriremePivot CreateAtLocation(CityPivot city, MapSquarePivot location)
        {
            return new TriremePivot(city, location);
        }
    }
}
