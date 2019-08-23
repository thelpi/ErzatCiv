using System;
using ErsatzCivLib.Model.Static;

namespace ErsatzCivLib.Model
{
    /// <summary>
    /// Represents a unit who navigates both on alnd and sea.
    /// </summary>
    [Serializable]
    public abstract class AirUnitPivot : UnitPivot
    {
        #region Embedded properties

        /// <summary>
        /// If <c>True</c>, can attack opponent's <see cref="AirUnitPivot"/>.
        /// </summary>
        public bool CanAttackAirUnit { get; private set; }
        /// <summary>
        /// Number of turn before goiing back to city or <see cref="Units.Sea.CarrierPivot"/>.
        /// </summary>
        public int TurnsInAir { get; private set; }

        #endregion

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
        /// <param name="player">The <see cref="UnitPivot.Player"/> value, if <paramref name="city"/> is <c>Null</c>.</param>
        /// <param name="ignoreControlZone">The <see cref="UnitPivot.IgnoreControlZone"/> value.</param>
        /// <param name="canAttackAirUnit">The <see cref="CanAttackAirUnit"/> value.</param>
        /// <param name="ignoreCityWalls">The <see cref="UnitPivot.IgnoreCityWalls"/> value.</param>
        /// <param name="squareSight">The <see cref="UnitPivot.SquareSight"/> value.</param>
        /// <param name="turnsInAir">The <see cref="TurnsInAir"/> value.</param>
        /// <param name="maintenanceCost">The <see cref="UnitPivot.MaintenanceCost"/> value.</param>
        protected AirUnitPivot(CityPivot city, int offensePoints, int defensePoints, int speed, int productivityCost,
            AdvancePivot advancePrerequisite, AdvancePivot advanceObsolescence, int purchasePrice, string name, MapSquarePivot location,
            PlayerPivot player, bool ignoreControlZone, bool canAttackAirUnit, bool ignoreCityWalls, int squareSight, int turnsInAir, int maintenanceCost) :
            base(city, offensePoints, defensePoints, speed, productivityCost, advancePrerequisite, advanceObsolescence, purchasePrice, name,
                0, location, player, ignoreControlZone, ignoreCityWalls, squareSight, maintenanceCost)
        {
            CanAttackAirUnit = canAttackAirUnit;
            TurnsInAir = turnsInAir;
        }
    }
}
