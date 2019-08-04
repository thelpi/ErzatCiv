using System;

namespace ErsatzCivLib.Model.CityImprovements
{
    /// <summary>
    /// Represents a granary.
    /// </summary>
    /// <seealso cref="CityImprovementPivot"/>
    [Serializable]
    public class LibraryPivot : CityImprovementPivot
    {
        public const double SCIENCE_INCREASE_RATIO = 1.5;

        private const int PRODUCTIVITY_COST = 80;
        private const int MAINTENANCE_COST = 1;
        private const int PURCHASE_PRICE = 320;
        private const int SELL_VALUE = 80;

        private LibraryPivot() :
            base(PRODUCTIVITY_COST, MAINTENANCE_COST, PURCHASE_PRICE, SELL_VALUE)
        { }

        /// <summary>
        /// Default instance.
        /// </summary>
        internal static readonly LibraryPivot Default = new LibraryPivot();

        /// <summary>
        /// Static constructior.
        /// </summary>
        /// <param name="location">Builder location.</param>
        /// <returns>An instance of <see cref="LibraryPivot"/>.</returns>
        internal static LibraryPivot CreateAtLocation(MapSquarePivot location)
        {
            return Default;
        }
    }
}
