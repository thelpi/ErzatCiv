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
        protected SeaUnitPivot(CityPivot city, int defensePoints, int offensePoints, int speed, int productivityCost,
            AdvancePivot advancePrerequisite, AdvancePivot advanceObsolescence, int purchasePrice, string name, MapSquarePivot location) :
            base(city, defensePoints, offensePoints, speed, productivityCost,
                advancePrerequisite, advanceObsolescence, purchasePrice, name, 0, location)
        { }

        /// <summary>
        /// Computes the real <see cref="UnitPivot.Speed"/>.
        /// </summary>
        /// <returns>The speed.</returns>
        protected override int ComputeRealSpeed()
        {
            var bonus = 0;

            if (City != null)
            {
                if (City.Player.WonderIsActive(WonderPivot.MagellanExpedition))
                {
                    bonus += MAGELLAN_WONDER_INCREASE_SPEED;
                }

                if (City.Player.WonderIsActive(WonderPivot.Lighthouse))
                {
                    bonus += LIGHTHOUSE_WONDER_INCREASE_SPEED;
                }
            }

            return Speed + bonus;
        }
    }
}
