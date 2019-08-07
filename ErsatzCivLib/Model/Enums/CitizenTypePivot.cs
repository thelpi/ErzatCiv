using System;

namespace ErsatzCivLib.Model.Enums
{
    /// <summary>
    /// Represents a type of citizen.
    /// </summary>
    [Serializable]
    public enum CitizenTypePivot
    {
        /// <summary>
        /// Scientist.
        /// </summary>
        Scientist,
        /// <summary>
        /// TaxCollector.
        /// </summary>
        TaxCollector,
        /// <summary>
        /// Entertainer.
        /// </summary>
        Entertainer
    }
}
