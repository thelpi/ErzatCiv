using System;

namespace ErsatzCivLib.Model.CityImprovements
{
    /// <summary>
    /// Represents military barracks.
    /// </summary>
    /// <seealso cref="CityImprovementPivot"/>
    [Serializable]
    public class BarracksPivot : CityImprovementPivot
    {
        private const int PRODUCTIVITY_COST = 40;
        private const int MAINTENANCE_COST = 2;
        private const int PURCHASE_PRICE = 160;
        private const int SELL_VALUE = 40;

        private BarracksPivot() :
            base(PRODUCTIVITY_COST, MAINTENANCE_COST, PURCHASE_PRICE, SELL_VALUE)
        { }

        /// <summary>
        /// Default instance.
        /// </summary>
        internal static readonly BarracksPivot Default = new BarracksPivot();

        /// <summary>
        /// Static constructior.
        /// </summary>
        /// <param name="location">Builder location.</param>
        /// <returns>An instance of <see cref="BarracksPivot"/>.</returns>
        internal static BarracksPivot CreateAtLocation(MapSquarePivot location)
        {
            return Default;
        }
    }
}
