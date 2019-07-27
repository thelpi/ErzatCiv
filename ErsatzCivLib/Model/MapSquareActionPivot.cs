namespace ErsatzCivLib.Model
{
    /// <summary>
    /// Represents a worker action.
    /// </summary>
    public class MapSquareActionPivot
    {
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

        private MapSquareActionPivot() { }

        private static MapSquareActionPivot _mine = null;
        private static MapSquareActionPivot _irrigate = null;
        private static MapSquareActionPivot _road = null;
        private static MapSquareActionPivot _railRoad = null;
        private static MapSquareActionPivot _clear = null;
        private static MapSquareActionPivot _plant = null;
        private static MapSquareActionPivot _clearPollution = null;
        private static MapSquareActionPivot _buildFortress = null;
        private static MapSquareActionPivot _destroyRoad = null;
        private static MapSquareActionPivot _destroyImprovement = null;

        /// <summary>
        /// Mines.
        /// </summary>
        /// <remarks>
        /// Doubles the productivity value (or makes it 1 if 0).
        /// Not cumulable with <see cref="Irrigate"/>.
        /// </remarks>
        public static MapSquareActionPivot Mine
        {
            get
            {
                if (_mine == null)
                {
                    _mine = new MapSquareActionPivot
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
        public static MapSquareActionPivot Irrigate
        {
            get
            {
                if (_irrigate == null)
                {
                    _irrigate = new MapSquareActionPivot
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
        /// Increase the unit speed (x2), and the commerce by one unit.
        /// </remarks>
        public static MapSquareActionPivot Road
        {
            get
            {
                if (_road == null)
                {
                    _road = new MapSquareActionPivot
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
        public static MapSquareActionPivot RailRoad
        {
            get
            {
                if (_railRoad == null)
                {
                    _railRoad = new MapSquareActionPivot
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
        /// Transforms a <see cref="MapSquareData"/> into its <see cref="MapSquareData._underlyingType"/>.
        /// </remarks>
        public static MapSquareActionPivot Clear
        {
            get
            {
                if (_clear == null)
                {
                    _clear = new MapSquareActionPivot
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
        /// Changes the <see cref="MapSquareTypeData"/> when applied on a <see cref="MapSquareData"/>.
        /// </remarks>
        public static MapSquareActionPivot Plant
        {
            get
            {
                if (_plant == null)
                {
                    _plant = new MapSquareActionPivot
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
        public static MapSquareActionPivot ClearPollution
        {
            get
            {
                if (_clearPollution == null)
                {
                    _clearPollution = new MapSquareActionPivot
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
        public static MapSquareActionPivot BuildFortress
        {
            get
            {
                if (_buildFortress == null)
                {
                    _buildFortress = new MapSquareActionPivot
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
        public static MapSquareActionPivot DestroyRoad
        {
            get
            {
                if (_destroyRoad == null)
                {
                    _destroyRoad = new MapSquareActionPivot
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
        public static MapSquareActionPivot DestroyImprovement
        {
            get
            {
                if (_destroyImprovement == null)
                {
                    _destroyImprovement = new MapSquareActionPivot
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
