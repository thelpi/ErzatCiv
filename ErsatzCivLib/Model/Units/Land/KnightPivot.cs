﻿using System;
using ErsatzCivLib.Model.Static;

namespace ErsatzCivLib.Model.Units.Land
{
    /// <summary>
    /// Represents a unit of knight.
    /// </summary>
    /// <seealso cref="LandUnitPivot"/>
    [Serializable]
    public class KnightPivot : LandUnitPivot
    {
        private KnightPivot(CityPivot city, MapSquarePivot location) :
            base(city, 4, 2, 2, 40, AdvancePivot.Chivalry, AdvancePivot.Automobile, 320, null, 0, location)
        { }

        /// <summary>
        /// Default instance.
        /// </summary>
        internal static readonly KnightPivot Default = new KnightPivot(null, null);

        /// <summary>
        /// Static constructior.
        /// </summary>
        /// <param name="city">The <see cref="UnitPivot.City"/> value.</param>
        /// <param name="location">The <see cref="UnitPivot.MapSquareLocation"/> value if <paramref name="city"/> is <c>Null</c>.</param>
        /// <returns>An instance of <see cref="KnightPivot"/>.</returns>
        internal static KnightPivot CreateAtLocation(CityPivot city, MapSquarePivot location)
        {
            return new KnightPivot(city, location);
        }
    }
}
