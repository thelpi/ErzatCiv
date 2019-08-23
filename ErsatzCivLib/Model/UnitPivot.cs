using System;
using System.Reflection;
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
    /// - A <c>static internal</c> method "CreateAtLocation" used to create instances, based on the default template.
    /// The method must have three parameters, of type <see cref="CityPivot"/>, <see cref="MapSquarePivot"/> and <see cref="PlayerPivot"/> respectively.
    /// - Every constructors must be <c>private</c>.
    /// </remarks>
    /// <seealso cref="BuildablePivot"/>
    [Serializable]
    public abstract class UnitPivot : BuildablePivot
    {
        private const double ROAD_SPEED_COST_RATIO = 0.3;

        #region Embedded properties

        /// <summary>
        /// The <see cref="PlayerPivot"/> which owns this instance.
        /// </summary>
        public PlayerPivot Player { get; private set; }
        /// <summary>
        /// The <see cref="CityPivot"/> which maintains this instance.
        /// </summary>
        public CityPivot City { get; private set; }
        /// <summary>
        /// Location on map.
        /// </summary>
        public MapSquarePivot MapSquareLocation { get; private set; }
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
        /// If <c>True</c>, ignore opponent control's zone.
        /// </summary>
        public bool IgnoreControlZone { get; private set; }
        /// <summary>
        /// If <c>True</c>, opponent <see cref="CityImprovementPivot.CityWalls"/> has no effect.
        /// </summary>
        public bool IgnoreCityWalls { get; private set; }
        /// <summary>
        /// Number of squares in sight.
        /// </summary>
        public int SquareSight { get; private set; }
        /// <summary>
        /// The maintenance cost, in productivity by turn.
        /// </summary>
        public int MaintenanceCost { get; private set; }

        #endregion

        #region Inferred properties

        /// <summary>
        /// Indicates if the unit is a military one; peaceful units doesn't cause unhappiness in republic or democracy.
        /// </summary>
        public bool IsMilitary { get { return OffensePoints > 0; } }

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="city">The <see cref="City"/> value.</param>
        /// <param name="defensePoints">The <see cref="DefensePoints"/> value.</param>
        /// <param name="offensePoints">The <see cref="OffensePoints"/> value.</param>
        /// <param name="speed">The <see cref="Speed"/> value.</param>
        /// <param name="productivityCost">The <see cref="BuildablePivot.ProductivityCost"/> value.</param>
        /// <param name="advancePrerequisite">The <see cref="BuildablePivot.AdvancePrerequisite"/> value.</param>
        /// <param name="advanceObsolescence">The <see cref="BuildablePivot.AdvanceObsolescence"/> value.</param>
        /// <param name="purchasePrice">The <see cref="BuildablePivot.PurchasePrice"/> value.</param>
        /// <param name="name">The <see cref="BuildablePivot.Name"/> value.</param>
        /// <param name="citizenCostToProduce">The <see cref="CitizenCostToProduce"/> value.</param>
        /// <param name="location">The <see cref="MapSquareLocation"/> value, if <paramref name="city"/> is <c>Null</c>.</param>
        /// <param name="player">The <see cref="Player"/> value, if <paramref name="city"/> is <c>Null</c>.</param>
        /// <param name="ignoreControlZone">The <see cref="IgnoreControlZone"/> value.</param>
        /// <param name="ignoreCityWalls">The <see cref="IgnoreCityWalls"/> value.</param>
        /// <param name="squareSight">The <see cref="SquareSight"/> value.</param>
        /// <param name="maintenanceCost">The <see cref="MaintenanceCost"/> value.</param>
        protected UnitPivot(CityPivot city, int offensePoints, int defensePoints, int speed, int productivityCost, AdvancePivot advancePrerequisite,
            AdvancePivot advanceObsolescence, int purchasePrice, string name, int citizenCostToProduce, MapSquarePivot location, PlayerPivot player,
            bool ignoreControlZone, bool ignoreCityWalls, int squareSight, int maintenanceCost) :
            base(productivityCost, advancePrerequisite, advanceObsolescence, purchasePrice, name, false)
        {
            Player = city?.Player ?? player;
            City = city;
            MapSquareLocation = city?.MapSquareLocation ?? location;
            DefensePoints = defensePoints;
            OffensePoints = offensePoints;
            Speed = speed;
            RemainingMoves = ComputeRealSpeed();
            CitizenCostToProduce = citizenCostToProduce;
            IgnoreControlZone = ignoreControlZone;
            IgnoreCityWalls = ignoreCityWalls;
            SquareSight = squareSight;
            MaintenanceCost = maintenanceCost;
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
        /// Moves the instance.
        /// </summary>
        /// <param name="direction">The <see cref="DirectionPivot"/>.</param>
        /// <param name="previousMapSquare">The previous <see cref="MapSquarePivot"/>.</param>
        /// <param name="currentMapSquare">The new <see cref="MapSquarePivot"/>.</param>
        internal void Move(DirectionPivot direction, MapSquarePivot previousMapSquare, MapSquarePivot currentMapSquare)
        {
            if (RemainingMoves == 0)
            {
                // Just in case !
                return;
            }

            if (!previousMapSquare.RailRoad || !currentMapSquare.RailRoad)
            {
                RemainingMoves -= currentMapSquare.Biome.SpeedCost *
                    (previousMapSquare.Road && currentMapSquare.Road ? ROAD_SPEED_COST_RATIO : 1);
            }

            MapSquareLocation = currentMapSquare;

            if (RemainingMoves < 0)
            {
                RemainingMoves = 0;
            }
        }

        /// <summary>
        /// Overriden; Resets the <see cref="RemainingMoves"/> value.
        /// </summary>
        internal virtual void Release()
        {
            RemainingMoves = ComputeRealSpeed();
        }

        /// <summary>
        /// Overriden; computes the real <see cref="Speed"/>.
        /// </summary>
        /// <returns>The speed.</returns>
        protected virtual int ComputeRealSpeed()
        {
            return Speed;
        }

        /// <summary>
        /// Creates a new instance from the current one.
        /// </summary>
        /// <param name="city">The <see cref="City"/> value.</param>
        /// <param name="location">The <see cref="MapSquareLocation"/> value; pertinent only if <paramref name="city"/> is <c>Null</c>.</param>
        /// <param name="player">The <see cref="Player"/> value; pertinent only if <paramref name="city"/> is <c>Null</c>.</param>
        /// <returns>A new <see cref="UnitPivot"/> from the current one.</returns>
        internal UnitPivot CreateInstance(CityPivot city, MapSquarePivot location, PlayerPivot player)
        {
            var method = GetType().GetMethod(
                "CreateAtLocation",
                BindingFlags.Static | BindingFlags.NonPublic,
                null,
                new[] { typeof(CityPivot), typeof(MapSquarePivot), typeof(PlayerPivot) },
                null);

            return (UnitPivot)method?.Invoke(null, new object[] { city, location, player });
        }
    }
}
