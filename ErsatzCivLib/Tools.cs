using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ErsatzCivLib.Model;
using ErsatzCivLib.Model.Enums;

namespace ErsatzCivLib
{
    /// <summary>
    /// Extensions and static methods.
    /// </summary>
    internal static class Tools
    {
        private static readonly object _locker = new object();
        private static readonly Random _randomizer =
            new Random(DateTime.Now.Millisecond * DateTime.Now.Second * DateTime.Now.Minute * DateTime.Now.Hour);

        /// <summary>
        /// Global <see cref="Random"/> instance; thread-safe.
        /// </summary>
        internal static Random Randomizer
        {
            get
            {
                lock (_locker)
                {
                    return _randomizer;
                }
            }
        }

        /// <summary>
        /// Extension; gets the next column index related to a <see cref="DirectionPivot"/> from a specified column index.
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <param name="column">The column index.</param>
        /// <returns>The next column index.</returns>
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

        /// <summary>
        /// Extension; gets the next row index related to a <see cref="DirectionPivot"/> from a specified row index.
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <param name="column">The row index.</param>
        /// <returns>The next row index.</returns>
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

        /// <summary>
        /// Gets every instances of a specified type, if the instance exists as a <c>public static readonly</c> field in the class.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <returns>List of <typeparamref name="T"/> instances; no <c>null</c> value.</returns>
        internal static List<T> GetInstancesOfTypeFromStaticFields<T>() where T : class
        {
            return typeof(T)
                .GetFields(BindingFlags.Static | BindingFlags.Public)
                .Select(f => f.GetValue(null) as T)
                .Where(v => !(v is null))
                .ToList();
        }

        /// <summary>
        /// Extension; gets the opposite direction of a <see cref="DirectionPivot"/>.
        /// </summary>
        /// <param name="dir">The direction.</param>
        /// <returns>The opposite direction.</returns>
        internal static DirectionPivot Opposite(this DirectionPivot dir)
        {
            switch (dir)
            {
                case DirectionPivot.Bottom:
                    return DirectionPivot.Top;
                case DirectionPivot.BottomLeft:
                    return DirectionPivot.TopRight;
                case DirectionPivot.BottomRight:
                    return DirectionPivot.TopLeft;
                case DirectionPivot.Left:
                    return DirectionPivot.Right;
                case DirectionPivot.Right:
                    return DirectionPivot.Left;
                case DirectionPivot.Top:
                    return DirectionPivot.Bottom;
                case DirectionPivot.TopLeft:
                    return DirectionPivot.BottomRight;
                case DirectionPivot.TopRight:
                default:
                    return DirectionPivot.BottomLeft;
            }
        }

        /// <summary>
        /// Extension; gets the intermediate direction between two <see cref="DirectionPivot"/>, if any.
        /// </summary>
        /// <param name="dir1">The first direction.</param>
        /// <param name="dir2">The second direction.</param>
        /// <returns>The intermediate direction. <c>Null</c> if there are more than one direction between the two.</returns>
        internal static DirectionPivot? Intermediate(this DirectionPivot dir1, DirectionPivot dir2)
        {
            switch (dir1)
            {
                case DirectionPivot.Bottom:
                    if (dir2 == DirectionPivot.Right)
                    {
                        return DirectionPivot.BottomRight;
                    }
                    else if (dir2 == DirectionPivot.Right)
                    {
                        return DirectionPivot.BottomLeft;
                    }
                    break;
                case DirectionPivot.BottomLeft:
                    if (dir2 == DirectionPivot.BottomRight)
                    {
                        return DirectionPivot.Bottom;
                    }
                    else if (dir2 == DirectionPivot.TopLeft)
                    {
                        return DirectionPivot.Left;
                    }
                    break;
                case DirectionPivot.BottomRight:
                    if (dir2 == DirectionPivot.BottomLeft)
                    {
                        return DirectionPivot.Bottom;
                    }
                    else if (dir2 == DirectionPivot.TopRight)
                    {
                        return DirectionPivot.Right;
                    }
                    break;
                case DirectionPivot.Left:
                    if (dir2 == DirectionPivot.Top)
                    {
                        return DirectionPivot.TopLeft;
                    }
                    else if (dir2 == DirectionPivot.Bottom)
                    {
                        return DirectionPivot.BottomLeft;
                    }
                    break;
                case DirectionPivot.Right:
                    if (dir2 == DirectionPivot.Top)
                    {
                        return DirectionPivot.TopRight;
                    }
                    else if (dir2 == DirectionPivot.Bottom)
                    {
                        return DirectionPivot.BottomRight;
                    }
                    break;
                case DirectionPivot.Top:
                    if (dir2 == DirectionPivot.Right)
                    {
                        return DirectionPivot.TopRight;
                    }
                    else if (dir2 == DirectionPivot.Right)
                    {
                        return DirectionPivot.TopLeft;
                    }
                    break;
                case DirectionPivot.TopLeft:
                    if (dir2 == DirectionPivot.BottomLeft)
                    {
                        return DirectionPivot.Left;
                    }
                    else if (dir2 == DirectionPivot.TopRight)
                    {
                        return DirectionPivot.Top;
                    }
                    break;
                case DirectionPivot.TopRight:
                default:
                    if (dir2 == DirectionPivot.TopLeft)
                    {
                        return DirectionPivot.Top;
                    }
                    else if (dir2 == DirectionPivot.BottomRight)
                    {
                        return DirectionPivot.Right;
                    }
                    break;
            }

            return null;
        }

        /// <summary>
        /// Extension; gets the couple of <see cref="DirectionPivot"/> you can go from a given direction.
        /// </summary>
        /// <param name="dir">The direction.</param>
        /// <returns>The couple of directions; result is never <c>null</c>.</returns>
        internal static Tuple<DirectionPivot, DirectionPivot> Close(this DirectionPivot dir)
        {
            switch (dir)
            {
                case DirectionPivot.Bottom:
                    return new Tuple<DirectionPivot, DirectionPivot>(DirectionPivot.BottomLeft, DirectionPivot.BottomRight);
                case DirectionPivot.BottomLeft:
                    return new Tuple<DirectionPivot, DirectionPivot>(DirectionPivot.Left, DirectionPivot.Bottom);
                case DirectionPivot.BottomRight:
                    return new Tuple<DirectionPivot, DirectionPivot>(DirectionPivot.Right, DirectionPivot.Bottom);
                case DirectionPivot.Left:
                    return new Tuple<DirectionPivot, DirectionPivot>(DirectionPivot.BottomLeft, DirectionPivot.TopLeft);
                case DirectionPivot.Right:
                    return new Tuple<DirectionPivot, DirectionPivot>(DirectionPivot.BottomRight, DirectionPivot.TopRight);
                case DirectionPivot.Top:
                    return new Tuple<DirectionPivot, DirectionPivot>(DirectionPivot.TopLeft, DirectionPivot.TopRight);
                case DirectionPivot.TopLeft:
                    return new Tuple<DirectionPivot, DirectionPivot>(DirectionPivot.Top, DirectionPivot.Left);
                case DirectionPivot.TopRight:
                default:
                    return new Tuple<DirectionPivot, DirectionPivot>(DirectionPivot.Top, DirectionPivot.Right);
            }
        }
    }
}
