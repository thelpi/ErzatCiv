using System;

namespace ErsatzCivLib.Model.CityImprovements
{
    /// <summary>
    /// Represents city walls.
    /// </summary>
    /// <seealso cref="CityImprovementPivot"/>
    [Serializable]
    public class CityWallsPivot : CityImprovementPivot
    {
        private const int PRODUCTIVITY_COST = 120;
        private const int MAINTENANCE_COST = 2;
        private const int PURCHASE_PRICE = 480;
        private const int SELL_VALUE = 120;

        private CityWallsPivot() :
            base(PRODUCTIVITY_COST, MAINTENANCE_COST, PURCHASE_PRICE, SELL_VALUE)
        { }

        /// <summary>
        /// Default instance.
        /// </summary>
        internal static readonly CityWallsPivot Default = new CityWallsPivot();

        /// <summary>
        /// Static constructior.
        /// </summary>
        /// <param name="location">Builder location.</param>
        /// <returns>An instance of <see cref="CityWallsPivot"/>.</returns>
        internal static CityWallsPivot CreateAtLocation(MapSquarePivot location)
        {
            return Default;
        }
    }
}
