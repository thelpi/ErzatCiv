using System;
using System.Collections.Generic;

namespace ErsatzCivLib.Model
{
    /// <summary>
    /// Represents consequences of a new turn on a civilization.
    /// </summary>
    [Serializable]
    public class TurnConsequencesPivot
    {
        /// <summary>
        /// Indicates a revolution has ended.
        /// </summary>
        public bool EndOfRevolution { get; internal set; }
        /// <summary>
        /// Indicates some <see cref="CityPivot"/> have finished to produce their <see cref="BuildablePivot"/>.
        /// </summary>
        public IReadOnlyDictionary<CityPivot, BuildablePivot> EndOfProduction { get; internal set; }
        /// <summary>
        /// Indicates the discovery of a <see cref="Static.AdvancePivot"/> is completed.
        /// </summary>
        public bool EndOfAdvance { get; internal set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        internal TurnConsequencesPivot() { }
    }
}
