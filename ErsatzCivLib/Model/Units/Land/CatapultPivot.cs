﻿using System;
using ErsatzCivLib.Model.Static;

namespace ErsatzCivLib.Model.Units.Land
{
    /// <summary>
    /// Represents a unit of catapult.
    /// </summary>
    /// <seealso cref="LandUnitPivot"/>
    [Serializable]
    public class CatapultPivot : LandUnitPivot
    {
        private CatapultPivot(CityPivot city, MapSquarePivot location, PlayerPivot player) :
            base(city, 6, 1, 1, 40, AdvancePivot.Mathematics, AdvancePivot.Metallurgy, 320, null, 0, location, player)
        { }

        /// <summary>
        /// Default instance.
        /// </summary>
        internal static readonly CatapultPivot Default = new CatapultPivot(null, null, null);

        /// <summary>
        /// Static constructior.
        /// </summary>
        /// <param name="city">The <see cref="UnitPivot.City"/> value.</param>
        /// <param name="location">The <see cref="UnitPivot.MapSquareLocation"/> value if <paramref name="city"/> is <c>Null</c>.</param>
        /// <param name="player">The <see cref="UnitPivot.Player"/> value, if <paramref name="city"/> is <c>Null</c>.</param>
        /// <returns>An instance of <see cref="CatapultPivot"/>.</returns>
        internal static CatapultPivot CreateAtLocation(CityPivot city, MapSquarePivot location, PlayerPivot player)
        {
            return new CatapultPivot(city, location, player);
        }
    }
}
