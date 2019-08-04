using System;

namespace ErsatzCivLib.Model.CityImprovements
{
    /// <summary>
    /// Represents an aqueduc.
    /// </summary>
    /// <seealso cref="CityImprovementPivot"/>
    [Serializable]
    public class AqueducPivot : CityImprovementPivot
    {
        private const int PRODUCTIVITY_COST = 120;
        private const int MAINTENANCE_COST = 2;
        private const int PURCHASE_PRICE = 480;
        private const int SELL_VALUE = 120;

        private AqueducPivot() :
            base(PRODUCTIVITY_COST, MAINTENANCE_COST, PURCHASE_PRICE, SELL_VALUE)
        { }

        /// <summary>
        /// Default instance.
        /// </summary>
        internal static readonly AqueducPivot Default = new AqueducPivot();

        /// <summary>
        /// Static constructior.
        /// </summary>
        /// <param name="location">Builder location.</param>
        /// <returns>An instance of <see cref="AqueducPivot"/>.</returns>
        internal static AqueducPivot CreateAtLocation(MapSquarePivot location)
        {
            return Default;
        }
    }
}
