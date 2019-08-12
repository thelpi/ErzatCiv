using System;
using System.Collections.Generic;
using ErsatzCivLib.Model.Static;

namespace ErsatzCivLib.Model
{
    /// <summary>
    /// Represents consequences of a new turn on a civilization.
    /// </summary>
    [Serializable]
    public class TurnConsequencesPivot
    {
        #region Embedded properties

        /// <summary>
        /// Indicates a revolution has ended.
        /// </summary>
        public bool EndOfRevolution { get; private set; }
        /// <summary>
        /// Indicates some <see cref="CityPivot"/> have finished to produce their <see cref="BuildablePivot"/>.
        /// </summary>
        public IReadOnlyDictionary<CityPivot, BuildablePivot> EndOfProduction { get; private set; }
        /// <summary>
        /// Indicates the discovery of a <see cref="Static.AdvancePivot"/> is completed.
        /// </summary>
        public AdvancePivot EndOfAdvance { get; private set; }

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="endOfRevolution">The <see cref="EndOfRevolution"/> value.</param>
        /// <param name="endOfProduction">The <see cref="EndOfProduction"/> value.</param>
        /// <param name="endOfAdvance">The <see cref="EndOfAdvance"/> value.</param>
        internal TurnConsequencesPivot(bool endOfRevolution, Dictionary<CityPivot, BuildablePivot> endOfProduction, AdvancePivot endOfAdvance)
        {
            EndOfRevolution = endOfRevolution;
            EndOfProduction = endOfProduction;
            EndOfAdvance = endOfAdvance;
        }
    }
}
