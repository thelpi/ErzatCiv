using System;

namespace ErsatzCivLib.Model.MapEnums
{
    /// <summary>
    /// Represents the land coverage of the map.
    /// </summary>
    [Serializable]
    public enum LandCoveragePivot
    {
        /// <summary>
        /// Very low.
        /// </summary>
        VeryLow,
        /// <summary>
        /// Low.
        /// </summary>
        Low,
        /// <summary>
        /// Medium.
        /// </summary>
        Medium,
        /// <summary>
        /// High.
        /// </summary>
        High,
        /// <summary>
        /// Very high.
        /// </summary>
        VeryHigh
    }
}
