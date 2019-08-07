using System;

namespace ErsatzCivLib.Model.Static
{
    /// <summary>
    /// Represents a worker action.
    /// </summary>
    [Serializable]
    public class WorkerActionPivot : IEquatable<WorkerActionPivot>
    {
        internal const double ROAD_SPEED_COST_RATIO = 0.3;

        internal const int RAILROAD_PRODUCTIVITY_BONUS = 1;
        internal const int RAILROAD_COMMERCE_BONUS = 1;
        internal const int ROAD_COMMERCE_BONUS = 1;
        internal const int MINE_PRODUCTIVITY_BONUS = 1;
        internal const int IRRIGATE_FOOD_BONUS = 1;

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
        /// <summary>
        /// <see cref="AdvancePivot"/> required to access the instance.
        /// </summary>
        public AdvancePivot AdvancePrerequisite { get; private set; }

        #endregion

        private WorkerActionPivot() { }

        /// <summary>
        /// Mines.
        /// </summary>
        /// <remarks>
        /// Doubles the productivity value (or makes it 1 if 0).
        /// Not cumulable with <see cref="Irrigate"/>.
        /// </remarks>
        public static readonly WorkerActionPivot Mine = new WorkerActionPivot
        {
            Name = "Mine",
            TurnCost = 2,
            AlwaysAvailable = false
        };
        /// <summary>
        /// Irrigates.
        /// </summary>
        /// <remarks>
        /// Doubles the food value (or makes it 1 if 0).
        /// Not cumulable with <see cref="Mine"/>.
        /// </remarks>
        public static readonly WorkerActionPivot Irrigate = new WorkerActionPivot
        {
            Name = "Irrigate",
            TurnCost = 2,
            AlwaysAvailable = false
        };
        /// <summary>
        /// Builds a road.
        /// </summary>
        /// <remarks>
        /// Increase the unit speed (x3), and the commerce by one unit.
        /// </remarks>
        public static readonly WorkerActionPivot Road = new WorkerActionPivot
        {
            Name = "Road",
            TurnCost = 1,
            AlwaysAvailable = false
        };
        /// <summary>
        /// Builds a rail road.
        /// </summary>
        /// <remarks>
        /// Increase the unit speed (no cost), and the commerce and productivity by one unit.
        /// </remarks>
        public static readonly WorkerActionPivot RailRoad = new WorkerActionPivot
        {
            Name = "RailRoad",
            TurnCost = 2,
            AlwaysAvailable = false,
            AdvancePrerequisite = AdvancePivot.Railroad
        };
        /// <summary>
        /// Clears a forest, a swamp or a jungle.
        /// </summary>
        /// <remarks>
        /// Transforms the biome of a <see cref="MapSquarePivot"/> into its <see cref="BiomePivot.UnderlyingBiomes"/>.
        /// </remarks>
        public static readonly WorkerActionPivot Clear = new WorkerActionPivot
        {
            Name = "Clear",
            TurnCost = 2,
            AlwaysAvailable = false
        };
        /// <summary>
        /// Plants a forest.
        /// </summary>
        /// <remarks>
        /// Changes the <see cref="BiomePivot"/> when applied on a <see cref="MapSquarePivot"/>.
        /// </remarks>
        public static readonly WorkerActionPivot Plant = new WorkerActionPivot
        {
            Name = "Plant forest",
            TurnCost = 3,
            AlwaysAvailable = false,
            AdvancePrerequisite = AdvancePivot.Engineering
        };
        /// <summary>
        /// Clears pollution.
        /// </summary>
        public static readonly WorkerActionPivot ClearPollution = new WorkerActionPivot
        {
            Name = "Clear pollution",
            TurnCost = 3,
            AlwaysAvailable = true
        };
        /// <summary>
        /// Builds a fortress.
        /// </summary>
        /// <remarks>Fortress triple the defensive value of a map square.</remarks>
        public static readonly WorkerActionPivot BuildFortress = new WorkerActionPivot
        {
            Name = "Build fortress",
            TurnCost = 3,
            AlwaysAvailable = false,
            AdvancePrerequisite = AdvancePivot.Masonry
        };
        /// <summary>
        /// Destroys road and railroad.
        /// </summary>
        /// <remarks>
        /// Removes the railroad if applicable, then the road.
        /// </remarks>
        public static readonly WorkerActionPivot DestroyRoad = new WorkerActionPivot
        {
            Name = "Destroy road",
            TurnCost = 1,
            AlwaysAvailable = true
        };
        /// <summary>
        /// Destroys irrigation system, mine and fortress.
        /// </summary>
        /// <remarks>Fortress comes first.</remarks>
        public static readonly WorkerActionPivot DestroyImprovement = new WorkerActionPivot
        {
            Name = "Destroy improvement",
            TurnCost = 1,
            AlwaysAvailable = true
        };

        #region IEquatable implementation

        /// <summary>
        /// Checks if this instance is equal to another one.
        /// </summary>
        /// <param name="other">The other instance.</param>
        /// <returns><c>True</c> if equals; <c>False</c> otherwise.</returns>
        public bool Equals(WorkerActionPivot other)
        {
            return Name == other?.Name;
        }

        /// <summary>
        /// Operator "==" override. Checks equality between two instances.
        /// </summary>
        /// <param name="r1">The first <see cref="WorkerActionPivot"/>.</param>
        /// <param name="r2">The second <see cref="WorkerActionPivot"/>.</param>
        /// <returns><c>True</c> if equals; <c>False</c> otherwise.</returns>
        public static bool operator ==(WorkerActionPivot r1, WorkerActionPivot r2)
        {
            if (r1 is null)
            {
                return r2 is null;
            }

            return r1.Equals(r2) == true;
        }

        /// <summary>
        /// Operator "!=" override. Checks non-equality between two instances.
        /// </summary>
        /// <param name="r1">The first <see cref="WorkerActionPivot"/>.</param>
        /// <param name="r2">The second <see cref="WorkerActionPivot"/>.</param>
        /// <returns><c>False</c> if equals; <c>True</c> otherwise.</returns>
        public static bool operator !=(WorkerActionPivot r1, WorkerActionPivot r2)
        {
            return !(r1 == r2);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is WorkerActionPivot && Equals(obj as WorkerActionPivot);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        #endregion
    }
}
