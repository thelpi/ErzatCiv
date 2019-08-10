using System;
using ErsatzCivLib.Model.Static;

namespace ErsatzCivLib.Model.Units
{
    /// <summary>
    /// Represents a nuclear strike unit.
    /// </summary>
    [Serializable]
    public class NuclearPivot : UnitPivot
    {
        private NuclearPivot(CityPivot city) :
            base(city, true, true, 0, 99, 1, 16, 160, AdvancePivot.Rocketry, null, 3200)
        { }

        /// <summary>
        /// Default instance.
        /// </summary>
        internal static readonly NuclearPivot Default = new NuclearPivot(null);

        /// <summary>
        /// Static constructior.
        /// </summary>
        /// <param name="city">The <see cref="UnitPivot.City"/> value..</param>
        /// <returns>An instance of <see cref="NuclearPivot"/>.</returns>
        internal static NuclearPivot CreateAtLocation(CityPivot city)
        {
            return new NuclearPivot(city);
        }
    }
}
