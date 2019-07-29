using System;

namespace ErsatzCivLib.Model.Persistent
{
    /// <summary>
    /// Represents a worker action.
    /// </summary>
    [Serializable]
    public class WorkerActionPivot
    {
        internal const double ROAD_SPEED_COST_RATIO = 0.3;

        internal const int RAILROAD_PRODUCTIVITY_BONUS = 1;
        internal const int RAILROAD_COMMERCE_BONUS = 1;
        internal const int ROAD_COMMERCE_BONUS = 1;
        internal const int MINE_PRODUCTIVITY_MULTIPLE = 2;
        internal const int MINE_PRODUCTIVITY_BONUS_IF_ZERO = 1;
        internal const int IRRIGATE_FOOD_MULTIPLE = 2;
        internal const int IRRIGATE_FOOD_BONUS_IF_ZERO = 1;

        #region Properties

        /// <summary>
        /// Name of the action.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Minimal number of turns to perform the action.
        /// </summary>
        public int TurnCost { get; private set; }
        /// <summary>
        /// Indicates if the action is always available for a worker.
        /// </summary>
        public bool AlwaysAvailable { get; private set; }

        #endregion

        private WorkerActionPivot() { }

        private static WorkerActionPivot _mine = null;
        private static WorkerActionPivot _irrigate = null;
        private static WorkerActionPivot _road = null;
        private static WorkerActionPivot _railRoad = null;
        private static WorkerActionPivot _clear = null;
        private static WorkerActionPivot _plant = null;
        private static WorkerActionPivot _clearPollution = null;
        private static WorkerActionPivot _buildFortress = null;
        private static WorkerActionPivot _destroyRoad = null;
        private static WorkerActionPivot _destroyImprovement = null;

        /// <summary>
        /// Mines.
        /// </summary>
        /// <remarks>
        /// Doubles the productivity value (or makes it 1 if 0).
        /// Not cumulable with <see cref="Irrigate"/>.
        /// </remarks>
        public static WorkerActionPivot Mine
        {
            get
            {
                if (_mine == null)
                {
                    _mine = new WorkerActionPivot
                    {
                        Name = "Mine",
                        TurnCost = 2,
                        AlwaysAvailable = false
                    };
                }
                return _mine;
            }
        }
        /// <summary>
        /// Irrigates.
        /// </summary>
        /// <remarks>
        /// Doubles the food value (or makes it 1 if 0).
        /// Not cumulable with <see cref="Mine"/>.
        /// </remarks>
        public static WorkerActionPivot Irrigate
        {
            get
            {
                if (_irrigate == null)
                {
                    _irrigate = new WorkerActionPivot
                    {
                        Name = "Irrigate",
                        TurnCost = 2,
                        AlwaysAvailable = false
                    };
                }
                return _irrigate;
            }
        }
        /// <summary>
        /// Builds a road.
        /// </summary>
        /// <remarks>
        /// Increase the unit speed (x3), and the commerce by one unit.
        /// </remarks>
        public static WorkerActionPivot Road
        {
            get
            {
                if (_road == null)
                {
                    _road = new WorkerActionPivot
                    {
                        Name = "Road",
                        TurnCost = 1,
                        AlwaysAvailable = false
                    };
                }
                return _road;
            }
        }
        /// <summary>
        /// Builds a rail road.
        /// </summary>
        /// <remarks>
        /// Increase the unit speed (no cost), and the commerce and productivity by one unit.
        /// </remarks>
        public static WorkerActionPivot RailRoad
        {
            get
            {
                if (_railRoad == null)
                {
                    _railRoad = new WorkerActionPivot
                    {
                        Name = "RailRoad",
                        TurnCost = 2,
                        AlwaysAvailable = false
                    };
                }
                return _railRoad;
            }
        }
        /// <summary>
        /// Clears a forest, a swamp or a jungle.
        /// </summary>
        /// <remarks>
        /// Transforms a <see cref="MapSquarePivot"/> into its <see cref="MapSquarePivot._underlyingType"/>.
        /// </remarks>
        public static WorkerActionPivot Clear
        {
            get
            {
                if (_clear == null)
                {
                    _clear = new WorkerActionPivot
                    {
                        Name = "Clear",
                        TurnCost = 2,
                        AlwaysAvailable = false
                    };
                }
                return _clear;
            }
        }
        /// <summary>
        /// Plants a forest.
        /// </summary>
        /// <remarks>
        /// Changes the <see cref="BiomePivot"/> when applied on a <see cref="MapSquarePivot"/>.
        /// </remarks>
        public static WorkerActionPivot Plant
        {
            get
            {
                if (_plant == null)
                {
                    _plant = new WorkerActionPivot
                    {
                        Name = "Plant forest",
                        TurnCost = 3,
                        AlwaysAvailable = false
                    };
                }
                return _plant;
            }
        }
        /// <summary>
        /// Clears pollution.
        /// </summary>
        public static WorkerActionPivot ClearPollution
        {
            get
            {
                if (_clearPollution == null)
                {
                    _clearPollution = new WorkerActionPivot
                    {
                        Name = "Clear pollution",
                        TurnCost = 3,
                        AlwaysAvailable = true
                    };
}
                return _clearPollution;
            }
        }
        /// <summary>
        /// Builds a fortress.
        /// </summary>
        /// <remarks>Fortress triple the defensive value of a map square.</remarks>
        public static WorkerActionPivot BuildFortress
        {
            get
            {
                if (_buildFortress == null)
                {
                    _buildFortress = new WorkerActionPivot
                    {
                        Name = "Build fortress",
                        TurnCost = 3,
                        AlwaysAvailable = false
                    };
                }
                return _buildFortress;
            }
        }
        /// <summary>
        /// Destroys road and railroad.
        /// </summary>
        /// <remarks>
        /// Removes the railroad if applicable, then the road.
        /// </remarks>
        public static WorkerActionPivot DestroyRoad
        {
            get
            {
                if (_destroyRoad == null)
                {
                    _destroyRoad = new WorkerActionPivot
                    {
                        Name = "Destroy road",
                        TurnCost = 1,
                        AlwaysAvailable = true
                    };
                }
                return _destroyRoad;
            }
        }
        /// <summary>
        /// Destroys irrigation system, mine and fortress.
        /// </summary>
        /// <remarks>Fortress comes first.</remarks>
        public static WorkerActionPivot DestroyImprovement
        {
            get
            {
                if (_destroyImprovement == null)
                {
                    _destroyImprovement = new WorkerActionPivot
                    {
                        Name = "Destroy improvement",
                        TurnCost = 1,
                        AlwaysAvailable = true
                    };
                }
                return _destroyImprovement;
            }
        }
    }
}
