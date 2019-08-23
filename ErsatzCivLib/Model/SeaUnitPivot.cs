using System;
using ErsatzCivLib.Model.Static;

namespace ErsatzCivLib.Model
{
    /// <summary>
    /// Represents a unit who navigates only on sea.
    /// </summary>
    [Serializable]
    public abstract class SeaUnitPivot : UnitPivot
    {
        private const int MAGELLAN_WONDER_INCREASE_SPEED = 1;
        private const int LIGHTHOUSE_WONDER_INCREASE_SPEED = 1;

        #region Embedded properties

        /// <summary>
        /// Transport capacity (<see cref="LandUnitPivot"/> only).
        /// </summary>
        public int TransportCapacity { get; private set; }
        /// <summary>
        /// If <c>True</c>, can attack opponent <see cref="LandUnitPivot"/>.
        /// </summary>
        public bool CanAttackCoastUnit { get; private set; }

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
        /// <param name="ignoreControlZone">Optional; the <see cref="UnitPivot.IgnoreControlZone"/> value.</param>
        /// <param name="ignoreCityWalls">Optional; the <see cref="UnitPivot.IgnoreCityWalls"/> value.</param>
        /// <param name="squareSight">Optional; the <see cref="UnitPivot.SquareSight"/> value.</param>
        /// <param name="transportCapacity">Optional; the <see cref="TransportCapacity"/> value.</param>
        /// <param name="canAttackCoastUnit">Optional; the <see cref="CanAttackCoastUnit"/> value.</param>
        /// <param name="maintenanceCost">The <see cref="UnitPivot.MaintenanceCost"/> value.</param>
        protected SeaUnitPivot(CityPivot city, int offensePoints, int defensePoints, int speed, int productivityCost,
            AdvancePivot advancePrerequisite, AdvancePivot advanceObsolescence, int purchasePrice, string name, MapSquarePivot location,
            PlayerPivot player, bool ignoreControlZone, bool ignoreCityWalls, int squareSight, int transportCapacity, bool canAttackCoastUnit, int maintenanceCost) :
            base(city, offensePoints, defensePoints, speed, productivityCost, advancePrerequisite, advanceObsolescence, purchasePrice, name, 0,
                location, player, ignoreControlZone, ignoreCityWalls, squareSight, maintenanceCost)
        {
            TransportCapacity = transportCapacity;
            CanAttackCoastUnit = canAttackCoastUnit;
        }

        /// <summary>
        /// Computes the real <see cref="UnitPivot.Speed"/>.
        /// </summary>
        /// <returns>The speed.</returns>
        protected override int ComputeRealSpeed()
        {
            var bonus = 0;

            if (Player != null)
            {
                if (Player.WonderIsActive(WonderPivot.MagellanExpedition))
                {
                    bonus += MAGELLAN_WONDER_INCREASE_SPEED;
                }

                if (Player.WonderIsActive(WonderPivot.Lighthouse))
                {
                    bonus += LIGHTHOUSE_WONDER_INCREASE_SPEED;
                }
            }

            return Speed + bonus;
        }
    }
}
