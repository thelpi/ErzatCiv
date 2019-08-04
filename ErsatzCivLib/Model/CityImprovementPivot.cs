using System;

namespace ErsatzCivLib.Model
{
    /// <summary>
    /// Represents a city improvement.
    /// </summary>
    /// <seealso cref="BuildablePivot"/>
    [Serializable]
    public abstract class CityImprovementPivot : BuildablePivot, IEquatable<CityImprovementPivot>
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
        /// <param name="hasCitizenMoodEffect">Optionnal; the <see cref="BuildablePivot.HasCitizenMoodEffect"/> value.</param>
        protected CityImprovementPivot(int productivityCost, int maintenanceCost,
            int purchasePrice, int sellValue,
            string name = null, bool hasCitizenMoodEffect = false) :
            base (productivityCost, name, hasCitizenMoodEffect)
        {
            MaintenanceCost = maintenanceCost;
            PurchasePrice = purchasePrice;
            SellValue = sellValue;
        }

        public bool Equals(CityImprovementPivot other)
        {
            return Name == other?.Name;
        }

        public static bool operator ==(CityImprovementPivot ms1, CityImprovementPivot ms2)
        {
            if (ms1 is null)
            {
                return ms2 is null;
            }

            return ms1.Equals(ms2) == true;
        }

        public static bool operator ==(CityImprovementPivot ms1, BuildablePivot ms2)
        {
            if (ms1 is null)
            {
                return ms2 is null;
            }

            return ms1.Equals(ms2) == true;
        }

        public static bool operator !=(CityImprovementPivot ms1, CityImprovementPivot ms2)
        {
            return !(ms1 == ms2);
        }

        public static bool operator !=(CityImprovementPivot ms1, BuildablePivot ms2)
        {
            return !(ms1 == ms2);
        }

        public override bool Equals(object obj)
        {
            return obj is CityImprovementPivot && Equals(obj as CityImprovementPivot);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
