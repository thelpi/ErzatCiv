﻿using System;
using ErsatzCivLib.Model.Static;

namespace ErsatzCivLib.Model.Units.Air
{
    /// <summary>
    /// Represents a bomber plane unit.
    /// </summary>
    /// <seealso cref="SeaUnitPivot"/>
    [Serializable]
    public class TransportPivot : SeaUnitPivot
    {
        private TransportPivot(CityPivot city, MapSquarePivot location) :
            base(city, 0, 3, 4, 50, AdvancePivot.Industrialization, null, 450, null, location)
        { }

        /// <summary>
        /// Default instance.
        /// </summary>
        internal static readonly TransportPivot Default = new TransportPivot(null, null);

        /// <summary>
        /// Static constructior.
        /// </summary>
        /// <param name="city">The <see cref="UnitPivot.City"/> value.</param>
        /// <param name="location">The <see cref="UnitPivot.MapSquareLocation"/> value, if <paramref name="city"/> is <c>Null</c>.</param>
        /// <returns>An instance of <see cref="TransportPivot"/>.</returns>
        internal static TransportPivot CreateAtLocation(CityPivot city, MapSquarePivot location)
        {
            return new TransportPivot(city, location);
        }
    }
}
