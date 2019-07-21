using System;
using System.Collections.Generic;
using ErsatzCiv.Model;

namespace ErsatzCiv
{
    public static class Tools
    {
        public static readonly Random Randomizer =
            new Random(DateTime.Now.Millisecond * DateTime.Now.Second * DateTime.Now.Minute * DateTime.Now.Hour);

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        public static int Column(this DirectionEnumPivot direction, int column)
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

        public static int Row(this DirectionEnumPivot direction, int row)
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

        public static DirectionEnumPivot? Move(this System.Windows.Input.Key key)
        {
            switch (key)
            {
                case System.Windows.Input.Key.NumPad1:
                    return DirectionEnumPivot.BottomLeft;
                case System.Windows.Input.Key.NumPad2:
                    return DirectionEnumPivot.Bottom;
                case System.Windows.Input.Key.NumPad3:
                    return DirectionEnumPivot.BottomRight;
                case System.Windows.Input.Key.NumPad6:
                    return DirectionEnumPivot.Right;
                case System.Windows.Input.Key.NumPad9:
                    return DirectionEnumPivot.TopRight;
                case System.Windows.Input.Key.NumPad8:
                    return DirectionEnumPivot.Top;
                case System.Windows.Input.Key.NumPad7:
                    return DirectionEnumPivot.TopLeft;
                case System.Windows.Input.Key.NumPad4:
                    return DirectionEnumPivot.Left;
                default:
                    return null;
            }
        }
    }
}
