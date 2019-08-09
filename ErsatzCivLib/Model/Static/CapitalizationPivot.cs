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
        /// <summary>
        /// Constructor.
        /// </summary>
        private CapitalizationPivot() : base(0, null, null) { }

        /// <summary>
        /// Default instance.
        /// </summary>
        public static readonly CapitalizationPivot Default = new CapitalizationPivot();
    }
}
