﻿using System;
using ErsatzCivLib.Model.Static;

namespace ErsatzCivLib.Model
{
    /// <summary>
    /// Represents a unit who navigates only on land.
    /// </summary>
    [Serializable]
    public abstract class LandUnitPivot : UnitPivot
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="city">The <see cref="UnitPivot.City"/> value.</param>
        /// <param name="defensePoints">The <see cref="UnitPivot.DefensePoints"/> value.</param>
        /// <param name="offensePoints">The <see cref="UnitPivot.OffensePoints"/> value.</param>
        /// <param name="speed">The <see cref="UnitPivot.Speed"/> value.</param>
        /// <param name="productivityCost">The <see cref="BuildablePivot.ProductivityCost"/> value.</param>
        /// <param name="advancePrerequisite">The <see cref="BuildablePivot.AdvancePrerequisite"/> value.</param>
        /// <param name="advanceObsolescence">The <see cref="BuildablePivot.AdvanceObsolescence"/> value.</param>
        /// <param name="purchasePrice">The <see cref="BuildablePivot.PurchasePrice"/> value.</param>
        /// <param name="name">The <see cref="BuildablePivot.Name"/> value.</param>
        /// <param name="citizenCostToProduce">The <see cref="UnitPivot.CitizenCostToProduce"/> value.</param>
        /// <param name="location">The <see cref="UnitPivot.MapSquareLocation"/> value, if <paramref name="city"/> is <c>Null</c>.</param>
        /// <param name="player">The <see cref="UnitPivot.Player"/> value, if <paramref name="city"/> is <c>Null</c>.</param>
        /// <param name="ignoreControlZone">Optional; the <see cref="UnitPivot.IgnoreControlZone"/> value.</param>
        /// <param name="ignoreCityWalls">Optional; the <see cref="UnitPivot.IgnoreCityWalls"/> value.</param>
        /// <param name="maintenanceCost">Optional; the <see cref="UnitPivot.MaintenanceCost"/> value.</param>
        protected LandUnitPivot(CityPivot city, int offensePoints, int defensePoints, int speed, int productivityCost,
            AdvancePivot advancePrerequisite, AdvancePivot advanceObsolescence, int purchasePrice, string name, int citizenCostToProduce,
            MapSquarePivot location, PlayerPivot player, bool ignoreControlZone = false, bool ignoreCityWalls = false, int maintenanceCost = 1) :
            base(city, offensePoints, defensePoints, speed, productivityCost, advancePrerequisite, advanceObsolescence,
                purchasePrice, name, citizenCostToProduce, location, player, ignoreControlZone, ignoreCityWalls, 1, maintenanceCost)
        { }
    }
}
