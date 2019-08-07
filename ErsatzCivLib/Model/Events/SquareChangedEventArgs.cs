using System;

namespace ErsatzCivLib.Model.Events
{
    /// <summary>
    /// Event informations when a part of the map is discovered.
    /// </summary>
    /// <seealso cref="EventArgs"/>
    public class SquareChangedEventArgs : EventArgs
    {
        /// <summary>
        /// List of discovered <see cref="MapSquarePivot"/>.
        /// </summary>
        public MapSquarePivot MapSquare { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="mapSquare">The <see cref="MapSquare"/> value.</param>
        public SquareChangedEventArgs(MapSquarePivot mapSquare)
        {
            MapSquare = mapSquare;
        }
    }
}
