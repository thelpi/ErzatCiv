using System;

namespace ErsatzCivLib.Model.Events
{
    /// <summary>
    /// Event informations when trying to attack a peaceful opponent.
    /// </summary>
    /// <seealso cref="EventArgs"/>
    public class AttackInPeaceEventArgs : EventArgs
    {
        /// <summary>
        /// The attacked <see cref="PlayerPivot"/>.
        /// </summary>
        public PlayerPivot Opponent { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="opponent">The <see cref="Opponent"/> value.</param>
        internal AttackInPeaceEventArgs(PlayerPivot opponent)
        {
            Opponent = opponent;
        }
    }
}
