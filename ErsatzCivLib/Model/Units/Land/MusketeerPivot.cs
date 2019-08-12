using System;
using ErsatzCivLib.Model.Static;

namespace ErsatzCivLib.Model.Units.Land
{
    /// <summary>
    /// Represents a unit of musketeer.
    /// </summary>
    /// <seealso cref="LandUnitPivot"/>
    [Serializable]
    public class MusketeerPivot : LandUnitPivot
    {
        private MusketeerPivot(CityPivot city, MapSquarePivot location) :
            base(city, 2, 3, 1, 30, AdvancePivot.Gunpowder, AdvancePivot.Conscription, 210, null, 0, location)
        { }

        /// <summary>
        /// Default instance.
        /// </summary>
        internal static readonly MusketeerPivot Default = new MusketeerPivot(null, null);

        /// <summary>
        /// Static constructior.
        /// </summary>
        /// <param name="city">The <see cref="UnitPivot.City"/> value.</param>
        /// <param name="location">The <see cref="UnitPivot.MapSquareLocation"/> value if <paramref name="city"/> is <c>Null</c>.</param>
        /// <returns>An instance of <see cref="MusketeerPivot"/>.</returns>
        internal static MusketeerPivot CreateAtLocation(CityPivot city, MapSquarePivot location)
        {
            return new MusketeerPivot(city, location);
        }
    }
}
