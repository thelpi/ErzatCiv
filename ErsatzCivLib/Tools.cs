using System;
using ErsatzCivLib.Model;
using ErsatzCivLib.Model.Persistent;

namespace ErsatzCivLib
{
    internal static class Tools
    {
        internal static readonly Random Randomizer =
            new Random(DateTime.Now.Millisecond * DateTime.Now.Second * DateTime.Now.Minute * DateTime.Now.Hour);

        internal static int Column(this DirectionPivot direction, int column)
        {
            switch (direction)
            {
                case DirectionPivot.BottomLeft:
                case DirectionPivot.Left:
                case DirectionPivot.TopLeft:
                    return column - 1;
                case DirectionPivot.BottomRight:
                case DirectionPivot.Right:
                case DirectionPivot.TopRight:
                    return column + 1;
            }

            return column;
        }

        internal static int Row(this DirectionPivot direction, int row)
        {
            switch (direction)
            {
                case DirectionPivot.Top:
                case DirectionPivot.TopLeft:
                case DirectionPivot.TopRight:
                    return row - 1;
                case DirectionPivot.Bottom:
                case DirectionPivot.BottomLeft:
                case DirectionPivot.BottomRight:
                    return row + 1;
            }

            return row;
        }
    }
}
