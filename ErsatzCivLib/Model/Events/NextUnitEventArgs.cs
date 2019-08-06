using System;

namespace ErsatzCivLib.Model.Events
{
    /// <summary>
    /// Event informations when the current unit changes.
    /// </summary>
    /// <seealso cref="EventArgs"/>
    public class NextUnitEventArgs : EventArgs
    {
        /// <summary>
        /// Indicates if there is still at least one moveable unit.
        /// </summary>
        public bool MoreUnit { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="moreUnit">The <see cref="MoreUnit"/> value.</param>
        internal NextUnitEventArgs(bool moreUnit)
        {
            MoreUnit = moreUnit;
        }
    }
}
