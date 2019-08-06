using System;

namespace ErsatzCivLib.Model.Events
{
    [Serializable]
    public class SquareChangedEventArgs : EventArgs
    {
        public MapSquarePivot MapSquare { get; private set; }

        public SquareChangedEventArgs(MapSquarePivot mapSquare)
        {
            MapSquare = mapSquare;
        }
    }
}
