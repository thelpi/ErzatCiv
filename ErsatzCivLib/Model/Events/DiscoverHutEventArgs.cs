using System;

namespace ErsatzCivLib.Model.Events
{
    /// <summary>
    /// Event informations when new <see cref="HutPivot"/> is discoverd.
    /// </summary>
    /// <seealso cref="EventArgs"/>
    public class DiscoverHutEventArgs : EventArgs
    {
        /// <summary>
        /// The <see cref="HutPivot"/> discovered.
        /// </summary>
        public HutPivot Hut { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="hut">The <see cref="Hut"/> value.</param>
        internal DiscoverHutEventArgs(HutPivot hut)
        {
            Hut = hut;
        }
    }
}
