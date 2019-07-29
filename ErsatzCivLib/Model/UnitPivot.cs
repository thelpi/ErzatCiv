using System;
using ErsatzCivLib.Model.Persistent;

namespace ErsatzCivLib.Model
{
    /// <summary>
    /// Represents an unit.
    /// </summary>
    [Serializable]
    public abstract class UnitPivot : BasePivot
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
        /// <summary>
        /// Remaining moves.
        /// </summary>
        public double RemainingMoves { get; protected set; }
        /// <summary>
        /// Render value.
        /// </summary>
        /// <remarks>Path to image; hexadecimal color code.</remarks>
        public string RenderValue { get; private set; }
        /// <summary>
        /// Type of render.
        /// </summary>
        public RenderTypePivot RenderType { get; private set; }

        protected UnitPivot(Engine owner, int row, int column, bool seaNavigate, bool groundNavigate, int defensePoints, int offensePoints,
            string renderValue, RenderTypePivot renderType, int lifePoints, int speed) : base(owner)
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

        internal bool Move(DirectionPivot? direction)
        {
            if (RemainingMoves == 0)
            {
                return false;
            }

            if (!direction.HasValue)
            {
                RemainingMoves = 0;
                Owner.ToNextUnit();
                return true;
            }

            var x = direction.Value.Row(Row);
            var y = direction.Value.Column(Column);
            var square = Owner.Map[x, y];
            var prevSq = Owner.Map[Row, Column];
            bool isCity = Owner.IsCity(x, y);
            if (square == null
                || (square.Biome.IsSeaType && !SeaNavigate && !isCity)
                || (!square.Biome.IsSeaType && !GroundNavigate && !isCity))
            {
                return false;
            }

            if (!prevSq.RailRoad || !square.RailRoad)
            {
                RemainingMoves -=
                    (isCity ? CityPivot.CITY_SPEED_COST : square.Biome.SpeedCost)
                    * (prevSq.Road && square.Road ? WorkerActionPivot.ROAD_SPEED_COST_RATIO : 1);
            }

            Row = x;
            Column = y;

            if (RemainingMoves <= 0)
            {
                RemainingMoves = 0;
                Owner.ToNextUnit();
            }

            return true;
        }

        internal virtual void Release()
        {
            RemainingMoves = Speed;
        }
    }
}
