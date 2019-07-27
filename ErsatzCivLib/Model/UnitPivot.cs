using System.Linq;

namespace ErsatzCivLib.Model
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
        /// <summary>
        /// Square movement by turn.
        /// </summary>
        public int Speed { get; private set; }
        public double RemainingMoves { get; protected set; }

        internal virtual void Release()
        {
            RemainingMoves = Speed;
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
            string renderValue, RenderTypeEnum renderType, int lifePoints, int speed)
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
            Speed = speed;
            RemainingMoves = Speed;
        }

        internal bool Move(Engine engine, DirectionEnumPivot? direction)
        {
            if (RemainingMoves == 0)
            {
                return false;
            }

            if (!direction.HasValue)
            {
                RemainingMoves = 0;
                engine.ToNextUnit();
                return true;
            }

            var x = direction.Value.Row(Row);
            var y = direction.Value.Column(Column);
            var square = engine.Map.MapSquareList.FirstOrDefault(ms => ms.Row == x && ms.Column == y);
            var prevSq = engine.Map.MapSquareList.FirstOrDefault(ms => ms.Row == Row && ms.Column == Column);
            bool isCity = engine.IsCity(x, y);
            if (square == null
                || (square.MapSquareType.IsSeaType && !SeaNavigate && !isCity)
                || (!square.MapSquareType.IsSeaType && !GroundNavigate && !isCity))
            {
                return false;
            }

            if (!prevSq.RailRoad || !square.RailRoad)
            {
                RemainingMoves -=
                    (isCity ? Constants.CITY_SPEED_COST : square.MapSquareType.SpeedCost)
                    * (prevSq.Road && square.Road ? Constants.ROAD_RATIO : 1);
            }

            Row = x;
            Column = y;

            if (RemainingMoves <= 0)
            {
                RemainingMoves = 0;
                engine.ToNextUnit();
            }

            return true;
        }
    }
}
