using System;
using ErsatzCivLib.Model.Static;

namespace ErsatzCivLib.Model.Events
{
    /// <summary>
    /// Event informations when the <see cref="WonderPivot.GreatLibrary"/> discovers an advance.
    /// </summary>
    /// <seealso cref="EventArgs"/>
    public class ForcedAdvanceEventArgs : EventArgs
    {
        /// <summary>
        /// The discovered <see cref="AdvancePivot"/>.
        /// </summary>
        public AdvancePivot Advance { get; private set; }
        /// <summary>
        /// <c>True</c> if the advance found was the one in progress.
        /// </summary>
        public bool WasInProgressAdvance { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="advance">The <see cref="Advance"/> value.</param>
        /// <param name="wasInProgressAdvance">The <see cref="WasInProgressAdvance"/> value.</param>
        internal ForcedAdvanceEventArgs(AdvancePivot advance, bool wasInProgressAdvance)
        {
            Advance = advance;
            WasInProgressAdvance = wasInProgressAdvance;
        }
    }
}
