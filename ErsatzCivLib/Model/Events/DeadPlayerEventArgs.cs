using System;

namespace ErsatzCivLib.Model.Events
{
    /// <summary>
    /// Event informations when an opponent is defeated.
    /// </summary>
    /// <seealso cref="EventArgs"/>
    public class DeadPlayerEventArgs : EventArgs
    {
        /// <summary>
        /// The dead <see cref="PlayerPivot"/>.
        /// </summary>
        public PlayerPivot Player { get; private set; }
        /// <summary>
        /// The killer <see cref="PlayerPivot"/>.
        /// </summary>
        public PlayerPivot Killer { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> value.</param>
        /// <param name="killer">The <see cref="Killer"/> value.</param>
        internal DeadPlayerEventArgs(PlayerPivot player, PlayerPivot killer)
        {
            Player = player;
            Killer = killer;
        }
    }
}
