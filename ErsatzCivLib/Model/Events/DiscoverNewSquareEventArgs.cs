using System;
using System.Collections.Generic;

namespace ErsatzCivLib.Model.Events
{
    /// <summary>
    /// Event informations when new <see cref="MapSquarePivot"/> is discoverd.
    /// </summary>
    /// <seealso cref="EventArgs"/>
    public class DiscoverNewSquareEventArgs : EventArgs
    {
        /// <summary>
        /// Collection of new <see cref="MapSquarePivot"/>.
        /// </summary>
        public IReadOnlyCollection<MapSquarePivot> MapSquares { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="mapSquares">The <see cref="MapSquares"/> value.</param>
        internal DiscoverNewSquareEventArgs(IEnumerable<MapSquarePivot> mapSquares)
        {
            MapSquares = new List<MapSquarePivot>(mapSquares);
        }
    }
}
