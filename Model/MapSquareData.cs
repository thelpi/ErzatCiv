using System;

namespace ErsatzCiv.Model
{
    public class MapSquareData
    {
        public MapSquareTypeData MapSquareType { get; private set; }
        public int Row { get; private set; }
        public int Column { get; private set; }

        public MapSquareData(MapSquareTypeData mapSquareType, int row, int column)
        {
            MapSquareType = mapSquareType ?? throw new ArgumentNullException(nameof(mapSquareType));
            Row = row < 0 ? throw new ArgumentException("Invalid value.", nameof(row)) : row;
            Column = column < 0 ? throw new ArgumentException("Invalid value.", nameof(column)) : column;
        }
    }
}
