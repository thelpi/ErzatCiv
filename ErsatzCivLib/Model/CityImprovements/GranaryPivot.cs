using System;

namespace ErsatzCivLib.Model.CityImprovements
{
    /// <summary>
    /// Represents a granary.
    /// </summary>
    /// <seealso cref="CityImprovementPivot"/>
    [Serializable]
    public class GranaryPivot : CityImprovementPivot
    {
        private const int PRODUCTIVITY_COST = 60;
        private const int MAINTENANCE_COST = 1;
        private const int PURCHASE_PRICE = 240;
        private const int SELL_VALUE = 60;

        private GranaryPivot() :
            base(PRODUCTIVITY_COST, MAINTENANCE_COST, PURCHASE_PRICE, SELL_VALUE)
        { }

        /// <summary>
        /// Default instance.
        /// </summary>
        internal static readonly GranaryPivot Default = new GranaryPivot();

        /// <summary>
        /// Static constructior.
        /// </summary>
        /// <param name="location">Builder location.</param>
        /// <returns>An instance of <see cref="GranaryPivot"/>.</returns>
        internal static GranaryPivot CreateAtLocation(MapSquarePivot location)
        {
            return Default;
        }
    }
}
