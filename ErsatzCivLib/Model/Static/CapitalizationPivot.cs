using System;

namespace ErsatzCivLib.Model.Static
{
    /// <summary>
    /// Capitalization (transforms city productivity in treasure).
    /// </summary>
    /// <seealso cref="BuildablePivot"/>
    [Serializable]
    public class CapitalizationPivot : BuildablePivot
    {
        private const int PRODUCTIVITY_COST = 0;

        /// <summary>
        /// Constructor.
        /// </summary>
        private CapitalizationPivot() : base(PRODUCTIVITY_COST, null, null) { }

        /// <summary>
        /// Default instance.
        /// </summary>
        public static readonly CapitalizationPivot Default = new CapitalizationPivot();
    }
}
