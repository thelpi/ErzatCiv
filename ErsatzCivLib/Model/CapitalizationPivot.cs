using System;

namespace ErsatzCivLib.Model
{
    /// <summary>
    /// Capitalization (city production).
    /// </summary>
    /// <seealso cref="BuildablePivot"/>
    [Serializable]
    public class CapitalizationPivot : BuildablePivot
    {
        private const int PRODUCTIVITY_COST = 0;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="mapSquare">Not used.</param>
        private CapitalizationPivot() : base(PRODUCTIVITY_COST) { }

        /// <summary>
        /// Default instance.
        /// </summary>
        internal static readonly CapitalizationPivot Default = new CapitalizationPivot();

        /// <summary>
        /// Static constructior.
        /// </summary>
        /// <param name="location">Builder location.</param>
        /// <returns>An instance of <see cref="CapitalizationPivot"/>.</returns>
        internal static CapitalizationPivot CreateAtLocation(MapSquarePivot location)
        {
            return new CapitalizationPivot();
        }
    }
}
