using System;

namespace ErsatzCivLib.Model.Enums
{
    /// <summary>
    /// Represents the land organization inside the map.
    /// </summary>
    [Serializable]
    public enum LandShapePivot
    {
        /// <summary>
        /// Single pangaea.
        /// </summary>
        Pangaea,
        /// <summary>
        /// Few continents.
        /// </summary>
        Continent,
        /// <summary>
        /// Several islands.
        /// </summary>
        Island
    }
}
