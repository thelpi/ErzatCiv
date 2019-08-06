using System;

namespace ErsatzCivLib.Model.Enums
{
    /// <summary>
    /// Represents the map size.
    /// </summary>
    [Serializable]
    public enum SizePivot
    {
        /// <summary>
        /// Very small.
        /// </summary>
        VerySmall = 1, // do not change the indice !
        /// <summary>
        /// Small.
        /// </summary>
        Small,
        /// <summary>
        /// Medium.
        /// </summary>
        Medium,
        /// <summary>
        /// Large.
        /// </summary>
        Large,
        /// <summary>
        /// Very large.
        /// </summary>
        VeryLarge
    }
}
