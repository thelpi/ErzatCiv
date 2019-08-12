﻿using System;
using ErsatzCivLib.Model.Static;

namespace ErsatzCivLib.Model.Units.Land
{
    /// <summary>
    /// Represents a unit of phalanx.
    /// </summary>
    /// <seealso cref="LandUnitPivot"/>
    [Serializable]
    public class PhalanxPivot : LandUnitPivot
    {
        private PhalanxPivot(CityPivot city, MapSquarePivot location) :
            base(city, 1, 2, 1, 20, AdvancePivot.BronzeWorking, AdvancePivot.Gunpowder, 120, null, 0, location)
        { }

        /// <summary>
        /// Default instance.
        /// </summary>
        internal static readonly PhalanxPivot Default = new PhalanxPivot(null, null);

        /// <summary>
        /// Static constructior.
        /// </summary>
        /// <param name="city">The <see cref="UnitPivot.City"/> value.</param>
        /// <param name="location">The <see cref="UnitPivot.MapSquareLocation"/> value if <paramref name="city"/> is <c>Null</c>.</param>
        /// <returns>An instance of <see cref="PhalanxPivot"/>.</returns>
        internal static PhalanxPivot CreateAtLocation(CityPivot city, MapSquarePivot location)
        {
            return new PhalanxPivot(city, location);
        }
    }
}
