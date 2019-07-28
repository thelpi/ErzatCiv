using System;

namespace ErsatzCivLib.Model
{
    /// <summary>
    /// Abstraction for any element displayable on the map.
    /// </summary>
    public abstract class IOnMapPivot
    {
        /// <summary>
        /// Row on the map grid.
        /// </summary>
        public int Row { get; private set; }
        /// <summary>
        /// Column on the map grid.
        /// </summary>
        public int Column { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="row"><see cref="Row"/>.</param>
        /// <param name="column"><see cref="Column"/>.</param>
        protected IOnMapPivot(int row, int column)
        {
            Move(row, column);
        }

        /// <summary>
        /// Moves the instance.
        /// </summary>
        /// <param name="row">New <see cref="Row"/>.</param>
        /// <param name="column">New <see cref="Column"/>.</param>
        protected void Move(int row, int column)
        {
            Row = row < 0 ? throw new ArgumentException("Invalid value.", nameof(row)) : row;
            Column = column < 0 ? throw new ArgumentException("Invalid value.", nameof(column)) : column;
        }

        /// <summary>
        /// Checks if the instance is at the same point than other instance.
        /// </summary>
        /// <param name="other">The other <see cref="IOnMapPivot"/>.</param>
        /// <returns><c>True</c> if same spot; <c>False</c> otherwise.</returns>
        internal bool Same(IOnMapPivot other)
        {
            return other != null && other.Row == Row && other.Column == Column;
        }

        /// <summary>
        /// Checks if the instance is at a specified point.
        /// </summary>
        /// <param name="column">Column to check.</param>
        /// <param name="row">Row to check.</param>
        /// <returns><c>True</c> if same spot; <c>False</c> otherwise.</returns>
        internal bool Same(int row, int column)
        {
            return row == Row && column == Column;
        }

        /// <summary>
        /// Checks if the instance is at a specified point.
        /// </summary>
        /// <param name="point">Point to check.</param>
        /// <param name="firstIsColumn">Optionnal; set <c>True</c> to specify that the first value of the tuple is the column. Default value is <c>False</c>.</param>
        /// <returns><c>True</c> if same spot; <c>False</c> otherwise.</returns>
        internal bool Same(Tuple<int, int> point, bool firstIsColumn = false)
        {
            return point != null && point.Item1 == Row && point.Item2 == Column;
        }

        /// <summary>
        /// Checks if the instance is right next (or previous) to another instance.
        /// The diagonal is also checked.
        /// </summary>
        /// <param name="other">The other <see cref="IOnMapPivot"/>.</param>
        /// <param name="closeFrom">Optionnal; indicates how close the other instance must be. Default is <c>1</c>.</param>
        /// <returns><c>True</c> if close; <c>False</c> otherwise.</returns>
        internal bool IsClose(IOnMapPivot other, int closeFrom = 1)
        {
            return (Math.Abs(Row - other.Row) == closeFrom && Math.Abs(Column - other.Column) < closeFrom) ||
                (Math.Abs(Column - other.Column) == closeFrom && Math.Abs(Row - other.Row) < closeFrom) ||
                (Math.Abs(Row - other.Row) == closeFrom && Math.Abs(Column - other.Column) == closeFrom);
        }
    }
}
