using System;
using System.Linq;

namespace ErsatzCiv.Model
{
    /// <summary>
    /// Represents an unit.
    /// </summary>
    public abstract class UnitPivot
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
        /// Current life points.
        /// </summary>
        public int CurrentLifePoints { get; private set; }
        /// <summary>
        /// Can navigate on sea y/n.
        /// </summary>
        public bool SeaNavigate { get; private set; }
        /// <summary>
        /// Can navigate on ground y/n.
        /// </summary>
        public bool GroundNavigate { get; private set; }
        /// <summary>
        /// Defensive points.
        /// </summary>
        public int DefensePoints { get; private set; }
        /// <summary>
        /// Offensive points.
        /// </summary>
        public int OffensePoints { get; private set; }
        public bool Locked { get; private set; }

        public void Release()
        {
            Locked = false;
        }

        /// <summary>
        /// Render value.
        /// </summary>
        /// <remarks>Path to image; hexadecimal color code.</remarks>
        public string RenderValue { get; private set; }
        /// <summary>
        /// Type of render.
        /// </summary>
        public RenderTypeEnum RenderType { get; private set; }

        protected UnitPivot(int row, int column, bool seaNavigate, bool groundNavigate, int defensePoints, int offensePoints,
            string renderValue, RenderTypeEnum renderType, int lifePoints)
        {
            Row = row;
            Column = column;
            SeaNavigate = seaNavigate;
            GroundNavigate = groundNavigate;
            DefensePoints = defensePoints;
            OffensePoints = offensePoints;
            RenderValue = renderValue;
            RenderType = renderType;
            CurrentLifePoints = lifePoints;
        }

        public bool Move(Engine engine, DirectionEnumPivot direction)
        {
            var x = direction.Row(Row);
            var y = direction.Column(Column);
            var square = engine.Map.MapSquareList.FirstOrDefault(ms => ms.Row == x && ms.Column == y);
            if (square == null
                || (square.MapSquareType.IsSeaType && !SeaNavigate)
                || (!square.MapSquareType.IsSeaType && !GroundNavigate))
            {
                return false;
            }
            Row = x;
            Column = y;
            Locked = true;
            return true;
        }
    }
}
