using System;
using ErsatzCivLib.Model.Enums;
using ErsatzCivLib.Model.Static;

namespace ErsatzCivLib.Model
{
    /// <summary>
    /// Represents an unit.
    /// </summary>
    /// <remarks>
    /// Each concrete implementation must implement :
    /// - A <c>static internal readonly</c> field "Default" used as a template.
    /// - A <c>static internal</c> method "CreateAtLocation" used to create instances, based on the default template. The method must have a single parameter, of type <see cref="MapSquarePivot"/>.
    /// - Every constructors must be <c>private</c>.
    /// </remarks>
    [Serializable]
    public abstract class UnitPivot : BuildablePivot
    {
        private const int CITY_SPEED_COST = 1;

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
        /// <summary>
        /// Citizens cost to produce the unit.
        /// </summary>
        public int CitizenCostToProduce { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="location">The <see cref="MapSquareLocation"/> value.</param>
        /// <param name="seaNavigate">The <see cref="SeaNavigate"/> value.</param>
        /// <param name="groundNavigate">The <see cref="GroundNavigate"/> value.</param>
        /// <param name="defensePoints">The <see cref="DefensePoints"/> value.</param>
        /// <param name="offensePoints">The <see cref="OffensePoints"/> value.</param>
        /// <param name="lifePoints">The intial <see cref="CurrentLifePoints"/> value.</param>
        /// <param name="speed">The <see cref="Speed"/> value.</param>
        /// <param name="productivityCost">The <see cref="BuildablePivot.ProductivityCost"/> value.</param>
        /// <param name="advancePrerequisite">The <see cref="BuildablePivot.AdvancePrerequisite"/> value.</param>
        /// <param name="advanceObsolescence">The <see cref="BuildablePivot.AdvanceObsolescence"/> value.</param>
        /// <param name="name">The <see cref="BuildablePivot.Name"/> value.</param>
        /// <param name="citizenCostToProduce">The <see cref="CitizenCostToProduce"/> value.</param>
        protected UnitPivot(MapSquarePivot location, bool seaNavigate, bool groundNavigate, int defensePoints, int offensePoints,
            int lifePoints, int speed, int productivityCost, AdvancePivot advancePrerequisite, AdvancePivot advanceObsolescence,
            string name = null, int citizenCostToProduce = 0) :
            base(productivityCost, advancePrerequisite, advanceObsolescence, name)
        {
            MapSquareLocation = location;
            SeaNavigate = seaNavigate;
            GroundNavigate = groundNavigate;
            DefensePoints = defensePoints;
            OffensePoints = offensePoints;
            CurrentLifePoints = lifePoints;
            Speed = speed;
            RemainingMoves = Speed;
            CitizenCostToProduce = citizenCostToProduce;
        }

        /// <summary>
        /// Sets <see cref="RemainingMoves"/> to <c>0</c> without actually moving the instance.
        /// </summary>
        internal void ForceNoMove()
        {
            if (RemainingMoves > 0)
            {
                RemainingMoves = 0;
            }
        }

        /// <summary>
        /// Tries to move the instance.
        /// </summary>
        /// <param name="direction">The <see cref="DirectionPivot"/>.</param>
        /// <param name="comeIntoCity"><c>True</c> if the unit comes into a city.</param>
        /// <param name="previousMapSquare">The previous <see cref="MapSquarePivot"/>.</param>
        /// <param name="currentMapSquare">The new <see cref="MapSquarePivot"/>.</param>
        /// <returns><c>True</c> if success; <c>False</c> otherwise.</returns>
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
                    (comeIntoCity ? CITY_SPEED_COST : currentMapSquare.Biome.SpeedCost)
                    * (previousMapSquare.Road && currentMapSquare.Road ? WorkerActionPivot.ROAD_SPEED_COST_RATIO : 1);
            }

            MapSquareLocation = currentMapSquare;

            if (RemainingMoves <= 0)
            {
                RemainingMoves = 0;
            }

            return true;
        }

        /// <summary>
        /// Overriden; Resets the <see cref="RemainingMoves"/> value.
        /// </summary>
        internal virtual void Release()
        {
            RemainingMoves = Speed;
        }
    }
}
