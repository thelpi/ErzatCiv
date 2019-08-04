using System;

namespace ErsatzCivLib.Model.CityImprovements
{
    /// <summary>
    /// Represents a marketplace.
    /// </summary>
    /// <seealso cref="CityImprovementPivot"/>
    [Serializable]
    public class MarketplacePivot : CityImprovementPivot
    {
        private const int PRODUCTIVITY_COST = 80;
        private const int MAINTENANCE_COST = 1;
        private const int PURCHASE_PRICE = 320;
        private const int SELL_VALUE = 80;

        private MarketplacePivot() :
            base(PRODUCTIVITY_COST, MAINTENANCE_COST, PURCHASE_PRICE, SELL_VALUE)
        { }

        /// <summary>
        /// Default instance.
        /// </summary>
        internal static readonly MarketplacePivot Default = new MarketplacePivot();

        /// <summary>
        /// Static constructior.
        /// </summary>
        /// <param name="location">Builder location.</param>
        /// <returns>An instance of <see cref="MarketplacePivot"/>.</returns>
        internal static MarketplacePivot CreateAtLocation(MapSquarePivot location)
        {
            return Default;
        }
    }
}
