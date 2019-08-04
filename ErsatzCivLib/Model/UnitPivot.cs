using System;
using ErsatzCivLib.Model.Persistent;

namespace ErsatzCivLib.Model
{
    /// <summary>
    /// Represents an unit.
    /// </summary>
    [Serializable]
    public abstract class UnitPivot : BuildablePivot
    {
        /// <summary>
        /// Location on map.
        /// </summary>
        public MapSquarePivot MapSquareLocation { get; private set; }
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

        protected UnitPivot(MapSquarePivot location, bool seaNavigate, bool groundNavigate, int defensePoints, int offensePoints,
            int lifePoints, int speed, int productivityCost, string name = null) : base(productivityCost, name)
        {
            MapSquareLocation = location;
            SeaNavigate = seaNavigate;
            GroundNavigate = groundNavigate;
            DefensePoints = defensePoints;
            OffensePoints = offensePoints;
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

        internal bool Move(DirectionPivot direction, bool comeIntoCity, MapSquarePivot previousMapSquare, MapSquarePivot currentMapSquare)
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

            MapSquareLocation = currentMapSquare;

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
