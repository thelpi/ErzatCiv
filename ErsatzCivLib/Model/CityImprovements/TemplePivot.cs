using System;

namespace ErsatzCivLib.Model.CityImprovements
{
    /// <summary>
    /// Represents a temple.
    /// </summary>
    /// <seealso cref="CityImprovementPivot"/>
    [Serializable]
    public class TemplePivot : CityImprovementPivot
    {
        private const int PRODUCTIVITY_COST = 40;
        private const int MAINTENANCE_COST = 1;
        private const int PURCHASE_PRICE = 160;
        private const int SELL_VALUE = 40;

        private TemplePivot() :
            base(PRODUCTIVITY_COST, MAINTENANCE_COST, PURCHASE_PRICE, SELL_VALUE, null, true)
        { }

        /// <summary>
        /// Default instance.
        /// </summary>
        internal static readonly TemplePivot Default = new TemplePivot();

        /// <summary>
        /// Static constructior.
        /// </summary>
        /// <param name="location">Builder location.</param>
        /// <returns>An instance of <see cref="TemplePivot"/>.</returns>
        internal static TemplePivot CreateAtLocation(MapSquarePivot location)
        {
            return Default;
        }
    }
}
