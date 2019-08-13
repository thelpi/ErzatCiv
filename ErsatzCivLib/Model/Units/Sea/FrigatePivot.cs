﻿using System;
using ErsatzCivLib.Model.Static;

namespace ErsatzCivLib.Model.Units.Air
{
    /// <summary>
    /// Represents a bomber plane unit.
    /// </summary>
    /// <seealso cref="SeaUnitPivot"/>
    [Serializable]
    public class FrigatePivot : SeaUnitPivot
    {
        private FrigatePivot(CityPivot city, MapSquarePivot location) :
            base(city, 2, 2, 3, 40, AdvancePivot.Magnetism, AdvancePivot.Industrialization, 320, null, location)
        { }

        /// <summary>
        /// Default instance.
        /// </summary>
        internal static readonly FrigatePivot Default = new FrigatePivot(null, null);

        /// <summary>
        /// Static constructior.
        /// </summary>
        /// <param name="city">The <see cref="UnitPivot.City"/> value.</param>
        /// <param name="location">The <see cref="UnitPivot.MapSquareLocation"/> value, if <paramref name="city"/> is <c>Null</c>.</param>
        /// <returns>An instance of <see cref="FrigatePivot"/>.</returns>
        internal static FrigatePivot CreateAtLocation(CityPivot city, MapSquarePivot location)
        {
            return new FrigatePivot(city, location);
        }
    }
}