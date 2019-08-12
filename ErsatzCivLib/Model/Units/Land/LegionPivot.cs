using System;
using ErsatzCivLib.Model.Static;

namespace ErsatzCivLib.Model.Units.Land
{
    /// <summary>
    /// Represents a unit of legion.
    /// </summary>
    /// <seealso cref="LandUnitPivot"/>
    [Serializable]
    public class LegionPivot : LandUnitPivot
    {
        private LegionPivot(CityPivot city, MapSquarePivot location) :
            base(city, 3, 1, 1, 20, AdvancePivot.IronWorking, AdvancePivot.Conscription, 120, null, 0, location)
        { }

        /// <summary>
        /// Default instance.
        /// </summary>
        internal static readonly LegionPivot Default = new LegionPivot(null, null);

        /// <summary>
        /// Static constructior.
        /// </summary>
        /// <param name="city">The <see cref="UnitPivot.City"/> value.</param>
        /// <param name="location">The <see cref="UnitPivot.MapSquareLocation"/> value if <paramref name="city"/> is <c>Null</c>.</param>
        /// <returns>An instance of <see cref="LegionPivot"/>.</returns>
        internal static LegionPivot CreateAtLocation(CityPivot city, MapSquarePivot location)
        {
            return new LegionPivot(city, location);
        }
    }
}
