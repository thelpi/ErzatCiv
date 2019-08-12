using System;
using ErsatzCivLib.Model.Static;

namespace ErsatzCivLib.Model.Units.Land
{
    /// <summary>
    /// Represents a unit of mechanical infantry.
    /// </summary>
    /// <seealso cref="LandUnitPivot"/>
    [Serializable]
    public class MechInfPivot : LandUnitPivot
    {
        private MechInfPivot(CityPivot city, MapSquarePivot location) :
            base(city, 6, 6, 3, 50, AdvancePivot.LaborUnion, null, 450, "Mechanical infantry", 0, location)
        { }

        /// <summary>
        /// Default instance.
        /// </summary>
        internal static readonly MechInfPivot Default = new MechInfPivot(null, null);

        /// <summary>
        /// Static constructior.
        /// </summary>
        /// <param name="city">The <see cref="UnitPivot.City"/> value.</param>
        /// <param name="location">The <see cref="UnitPivot.MapSquareLocation"/> value if <paramref name="city"/> is <c>Null</c>.</param>
        /// <returns>An instance of <see cref="MechInfPivot"/>.</returns>
        internal static MechInfPivot CreateAtLocation(CityPivot city, MapSquarePivot location)
        {
            return new MechInfPivot(city, location);
        }
    }
}
