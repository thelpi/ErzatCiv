using System;
using ErsatzCivLib.Model;

namespace ErsatzCivLib
{
    internal static class Tools
    {
        internal static readonly Random Randomizer =
            new Random(DateTime.Now.Millisecond * DateTime.Now.Second * DateTime.Now.Minute * DateTime.Now.Hour);

        internal static int Column(this DirectionEnumPivot direction, int column)
        {
            switch (direction)
            {
                case DirectionEnumPivot.BottomLeft:
                case DirectionEnumPivot.Left:
                case DirectionEnumPivot.TopLeft:
                    return column - 1;
                case DirectionEnumPivot.BottomRight:
                case DirectionEnumPivot.Right:
                case DirectionEnumPivot.TopRight:
                    return column + 1;
            }

            return column;
        }

        internal static int Row(this DirectionEnumPivot direction, int row)
        {
            switch (direction)
            {
                case DirectionEnumPivot.Top:
                case DirectionEnumPivot.TopLeft:
                case DirectionEnumPivot.TopRight:
                    return row - 1;
                case DirectionEnumPivot.Bottom:
                case DirectionEnumPivot.BottomLeft:
                case DirectionEnumPivot.BottomRight:
                    return row + 1;
            }

            return row;
        }
    }
}
