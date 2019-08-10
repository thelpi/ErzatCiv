using System;
using ErsatzCivLib.Model.Static;

namespace ErsatzCivLib.Model
{
    /// <summary>
    /// Base class for spaceship items.
    /// </summary>
    /// <seealso cref="BuildablePivot"/>
    [Serializable]
    public abstract class SpaceShipPivot : BuildablePivot
    {
        #region Embedded properties

        /// <summary>
        /// Number of items of this type for a space ship.
        /// </summary>
        public int SpaceShipItemCount { get; private set; }

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="productivityCost">The <see cref="BuildablePivot.ProductivityCost"/> value.</param>
        /// <param name="advancePrerequisite">The <see cref="BuildablePivot.AdvancePrerequisite"/> value.</param>
        /// <param name="purchasePrice">The <see cref="BuildablePivot.PurchasePrice"/> value.</param>
        /// <param name="name">Optionnal; the <see cref="BuildablePivot.Name"/> value.</param>
        /// <param name="spaceShipItemCount">Optionnal; the <see cref="SpaceShipItemCount"/> value.</param>
        protected SpaceShipPivot(int productivityCost, AdvancePivot advancePrerequisite,
            int purchasePrice, string name, int spaceShipItemCount) :
            base(productivityCost, advancePrerequisite, null, purchasePrice, name)
        {
            SpaceShipItemCount = spaceShipItemCount;
        }
    }
}
