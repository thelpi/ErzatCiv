using System;

namespace ErsatzCivLib.Model.Events
{
    /// <summary>
    /// Event informations when a barbarian <see cref="Units.Land.DiplomatPivot"/> is killed.
    /// </summary>
    /// <seealso cref="EventArgs"/>
    public class BarbarianDiplomatKilledEventArgs : EventArgs
    {
        /// <summary>
        /// The treasure gain.
        /// </summary>
        public int TreasureGain { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="treasureGain">The <see cref="TreasureGain"/> value.</param>
        internal BarbarianDiplomatKilledEventArgs(int treasureGain)
        {
            TreasureGain = treasureGain;
        }
    }
}
