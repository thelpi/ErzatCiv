using System;
using ErsatzCivLib.Model.Persistent;

namespace ErsatzCivLib.Model
{
    /// <summary>
    /// Represents an unit.
    /// </summary>
    [Serializable]
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

        protected UnitPivot(int row, int column, bool seaNavigate, bool groundNavigate, int defensePoints, int offensePoints,
            string renderValue, RenderTypePivot renderType, int lifePoints, int speed)
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

        internal void ForceNoMove()
        {
            if (RemainingMoves > 0)
            {
                RemainingMoves = 0;
            }
        }

        internal bool Move(DirectionPivot direction, int newRow, int newColumn, bool comeIntoCity,
            MapSquarePivot previousMapSquare, MapSquarePivot currentMapSquare)
        {
            if (RemainingMoves == 0)
            {
                return false;
            }

            if ((currentMapSquare.Biome.IsSeaType && !SeaNavigate && !comeIntoCity)
                || (!currentMapSquare.Biome.IsSeaType && !GroundNavigate && !comeIntoCity))
            {
                return false;
            }

            if (!previousMapSquare.RailRoad || !currentMapSquare.RailRoad)
            {
                RemainingMoves -=
                    (comeIntoCity ? CityPivot.CITY_SPEED_COST : currentMapSquare.Biome.SpeedCost)
                    * (previousMapSquare.Road && currentMapSquare.Road ? WorkerActionPivot.ROAD_SPEED_COST_RATIO : 1);
            }

            Row = newRow;
            Column = newColumn;

            if (RemainingMoves <= 0)
            {
                RemainingMoves = 0;
            }

            return true;
        }

        internal virtual void Release()
        {
            RemainingMoves = Speed;
        }
    }
}
