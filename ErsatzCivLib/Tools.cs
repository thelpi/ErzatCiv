using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ErsatzCivLib.Model.Persistent;

namespace ErsatzCivLib
{
    internal static class Tools
    {
        private static readonly object _locker = new object();
        private static readonly Random _randomizer =
            new Random(DateTime.Now.Millisecond * DateTime.Now.Second * DateTime.Now.Minute * DateTime.Now.Hour);

        public static Random Randomizer
        {
            get
            {
                lock (_locker)
                {
                    return _randomizer;
                }
            }
        }

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

        internal static List<T> GetInstancesOfTypeFromStaticFields<T>() where T : class
        {
            return typeof(T)
                .GetFields(BindingFlags.Static | BindingFlags.Public)
                .Select(f => f.GetValue(null) as T)
                .Where(v => !(v is null))
                .ToList();
        }
    }
}
