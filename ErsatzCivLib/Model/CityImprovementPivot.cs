using System;

namespace ErsatzCivLib.Model
{
    /// <summary>
    /// Represents a city improvement.
    /// </summary>
    /// <seealso cref="BuildablePivot"/>
    [Serializable]
    public abstract class CityImprovementPivot : BuildablePivot
    {
        /// <summary>
        /// The maintenance cost, in gold by turn.
        /// </summary>
        public int MaintenanceCost { get; private set; }
        /// <summary>
        /// The purchse price, in gold.
        /// </summary>
        public int PurchasePrice { get; private set; }
        /// <summary>
        /// The sell value, in gold.
        /// </summary>
        public int SellValue { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="productivityCost">The <see cref="BuildablePivot.ProductivityCost"/> value.</param>
        /// <param name="maintenanceCost">The <see cref="MaintenanceCost"/> value.</param>
        /// <param name="purchasePrice">The <see cref="PurchasePrice"/> value.</param>
        /// <param name="sellValue">The <see cref="SellValue"/> value.</param>
        /// <param name="name">Optionnal; the <see cref="BuildablePivot.Name"/> value.</param>
        protected CityImprovementPivot(int productivityCost, int maintenanceCost, int purchasePrice, int sellValue, string name = null) :
            base (productivityCost, name)
        {
            MaintenanceCost = maintenanceCost;
            PurchasePrice = purchasePrice;
            SellValue = sellValue;
        }
    }
}
