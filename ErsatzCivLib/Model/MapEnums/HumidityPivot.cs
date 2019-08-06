using System;

namespace ErsatzCivLib.Model.MapEnums
{
    /// <summary>
    /// Levels of humidity.
    /// </summary>
    [Serializable]
    public enum HumidityPivot
    {
        /// <summary>
        /// Dry.
        /// </summary>
        Dry,
        /// <summary>
        /// Average.
        /// </summary>
        Average,
        /// <summary>
        /// Wet.
        /// </summary>
        Wet
    }
}
