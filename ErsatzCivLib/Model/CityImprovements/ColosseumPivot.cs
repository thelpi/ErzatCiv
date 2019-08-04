using System;

namespace ErsatzCivLib.Model.CityImprovements
{
    /// <summary>
    /// Represents colosseum.
    /// </summary>
    /// <seealso cref="CityImprovementPivot"/>
    [Serializable]
    public class ColosseumPivot : CityImprovementPivot
    {
        private const int PRODUCTIVITY_COST = 100;
        private const int MAINTENANCE_COST = 4;
        private const int PURCHASE_PRICE = 400;
        private const int SELL_VALUE = 100;

        private ColosseumPivot() :
            base(PRODUCTIVITY_COST, MAINTENANCE_COST, PURCHASE_PRICE, SELL_VALUE)
        { }

        /// <summary>
        /// Default instance.
        /// </summary>
        internal static readonly ColosseumPivot Default = new ColosseumPivot();

        /// <summary>
        /// Static constructior.
        /// </summary>
        /// <param name="location">Builder location.</param>
        /// <returns>An instance of <see cref="ColosseumPivot"/>.</returns>
        internal static ColosseumPivot CreateAtLocation(MapSquarePivot location)
        {
            return Default;
        }
    }
}
