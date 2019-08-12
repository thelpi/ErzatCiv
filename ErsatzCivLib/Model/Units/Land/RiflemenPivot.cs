using System;
using ErsatzCivLib.Model.Static;

namespace ErsatzCivLib.Model.Units.Land
{
    /// <summary>
    /// Represents a unit of riflemen.
    /// </summary>
    /// <seealso cref="LandUnitPivot"/>
    [Serializable]
    public class RiflemenPivot : LandUnitPivot
    {
        private RiflemenPivot(CityPivot city, MapSquarePivot location) :
            base(city, 3, 5, 1, 30, AdvancePivot.Conscription, null, 210, null, 0, location)
        { }

        /// <summary>
        /// Default instance.
        /// </summary>
        internal static readonly RiflemenPivot Default = new RiflemenPivot(null, null);

        /// <summary>
        /// Static constructior.
        /// </summary>
        /// <param name="city">The <see cref="UnitPivot.City"/> value.</param>
        /// <param name="location">The <see cref="UnitPivot.MapSquareLocation"/> value if <paramref name="city"/> is <c>Null</c>.</param>
        /// <returns>An instance of <see cref="RiflemenPivot"/>.</returns>
        internal static RiflemenPivot CreateAtLocation(CityPivot city, MapSquarePivot location)
        {
            return new RiflemenPivot(city, location);
        }
    }
}
