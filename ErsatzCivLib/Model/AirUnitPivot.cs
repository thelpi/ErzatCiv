﻿using System;
using ErsatzCivLib.Model.Static;

namespace ErsatzCivLib.Model
{
    /// <summary>
    /// Represents a unit who navigates both on alnd and sea.
    /// </summary>
    [Serializable]
    public abstract class AirUnitPivot : UnitPivot
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
        /// <param name="location">The <see cref="UnitPivot.MapSquareLocation"/> value, if <paramref name="city"/> is <c>Null</c>.</param>
        protected AirUnitPivot(CityPivot city, int offensePoints, int defensePoints, int speed, int productivityCost,
            AdvancePivot advancePrerequisite, AdvancePivot advanceObsolescence, int purchasePrice, string name, MapSquarePivot location) :
            base(city, offensePoints, defensePoints, speed, productivityCost,
                advancePrerequisite, advanceObsolescence, purchasePrice, name, 0, location)
        { }
    }
}