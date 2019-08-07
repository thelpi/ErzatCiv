using System;
using System.Collections.Generic;
using System.Linq;
using ErsatzCivLib.Model.Enums;
using ErsatzCivLib.Model.Events;
using ErsatzCivLib.Model.Static;
using ErsatzCivLib.Model.Units;

namespace ErsatzCivLib.Model
{
    /// <summary>
    /// Represents a map square.
    /// </summary>
    /// <seealso cref="IEquatable{T}"/>
    [Serializable]
    public class MapSquarePivot : IEquatable<MapSquarePivot>
    {
        /// <summary>
        /// Event triggered when the instance is edited.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler<SquareChangedEventArgs> SquareChangeEvent;

        private List<InProgressWorkerActionPivot> _currentActions = new List<InProgressWorkerActionPivot>();
        private readonly Dictionary<DirectionPivot, bool> _rivers =
            Enum.GetValues(typeof(DirectionPivot)).Cast<DirectionPivot>().ToDictionary(x => x, x => false);

        #region Embedded properties

        /// <summary>
        /// Row on the map.
        /// </summary>
        public int Row { get; private set; }
        /// <summary>
        /// Column on the map.
        /// </summary>
        public int Column { get; private set; }
        /// <summary>
        /// Square type.
        /// </summary>
        public BiomePivot Biome { get; private set; }
        /// <summary>
        /// Underlying type in case of clearage (forest, jungle...).
        /// </summary>
        public BiomePivot UnderlyingBiome { get; private set; }
        /// <summary>
        /// Mine built y/n.
        /// </summary>
        public bool Mine { get; private set; }
        /// <summary>
        /// Irrigation system built y/n.
        /// </summary>
        public bool Irrigate { get; private set; }
        /// <summary>
        /// Road built y/n.
        /// </summary>
        public bool Road { get; private set; }
        /// <summary>
        /// RailRoad built y/n.
        /// </summary>
        public bool RailRoad { get; private set; }
        /// <summary>
        /// Has pollution y/n.
        /// </summary>
        public bool Pollution { get; private set; }
        /// <summary>
        /// Fortress built y/n.
        /// </summary>
        public bool Fortress { get; private set; }

        #endregion

        #region Inferred properties

        /// <summary>
        /// List of <see cref="InProgressWorkerActionPivot"/> in progress for this instance.
        /// </summary>
        public IReadOnlyCollection<InProgressWorkerActionPivot> CurrentActions { get { return _currentActions; } }
        /// <summary>
        /// List of <see cref="DirectionPivot"/> around the instance where a river is set.
        /// </summary>
        public IReadOnlyCollection<DirectionPivot> Rivers { get { return _rivers.Where(r => r.Value).Select(r => r.Key).ToList(); } }
        /// <summary>
        /// Indicates if there's at least one river around the instance.
        /// </summary>
        public bool HasRiver
        {
            get
            {
                return _rivers.Any(r => r.Value);
            }
        }
        /// <summary>
        /// Food value of this instance.
        /// </summary>
        /// <remarks>Pollution and worker actions included.</remarks>
        public int Food
        {
            get
            {
                var baseValue = 0;
                if (!Pollution)
                {
                    baseValue = Biome.Food;
                    if (Irrigate)
                    {
                        baseValue += WorkerActionPivot.IRRIGATE_FOOD_BONUS;
                    }
                }
                return baseValue;
            }
        }
        /// <summary>
        /// Productivity value of this instance.
        /// </summary>
        /// <remarks>Pollution and worker actions included.</remarks>
        public int Productivity
        {
            get
            {
                var baseValue = 0;
                if (!Pollution)
                {
                    baseValue = Biome.Productivity;
                    if (Mine)
                    {
                        baseValue += WorkerActionPivot.MINE_PRODUCTIVITY_BONUS;
                    }
                    if (RailRoad && baseValue > 0)
                    {
                        baseValue += WorkerActionPivot.RAILROAD_PRODUCTIVITY_BONUS;
                    }
                }
                return baseValue;
            }
        }
        /// <summary>
        /// Commerce value of this instance.
        /// </summary>
        /// <remarks>Pollution and worker actions included.</remarks>
        public int Commerce
        {
            get
            {
                var baseValue = 0;
                if (!Pollution)
                {
                    baseValue = Biome.Commerce;
                    if (Road)
                    {
                        baseValue += WorkerActionPivot.ROAD_COMMERCE_BONUS;
                    }
                    if (RailRoad)
                    {
                        baseValue += WorkerActionPivot.RAILROAD_COMMERCE_BONUS;
                    }
                }
                return baseValue;
            }
        }
        /// <summary>
        /// Sum of food, productivity and commerce statistics.
        /// </summary>
        public int TotalValue { get { return Food + Productivity + Commerce; } }
        /// <summary>
        /// Food value if the square is a city.
        /// </summary>
        public int CityFood
        {
            get
            {
                return Biome.Food < 2 ? 2 : Biome.Food;
            }
        }
        /// <summary>
        /// Productivity value if the square is a city.
        /// </summary>
        public int CityProductivity
        {
            get
            {
                return Biome.Productivity < 1 ? 1 : Biome.Productivity;
            }
        }
        /// <summary>
        /// Commerce value if the square is a city.
        /// </summary>
        public int CityCommerce
        {
            get
            {
                return Biome.Commerce < 2 ? 2 : Biome.Commerce;
            }
        }

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="row">The <see cref="Row"/> value.</param>
        /// <param name="column">The <see cref="Column"/> value.</param>
        /// <param name="biome">The <see cref="Biome"/> value.</param>
        /// <param name="underlyingType">The underlying biome if <see cref="WorkerActionPivot.Clear"/> action possible.</param>
        internal MapSquarePivot(int row, int column, BiomePivot biome, BiomePivot underlyingType = null)
        {
            Row = row;
            Column = column;
            Biome = biome ?? throw new ArgumentNullException(nameof(biome));
            if (Biome.Actions.Contains(WorkerActionPivot.Clear))
            {
                UnderlyingBiome = underlyingType ?? throw new ArgumentNullException(nameof(underlyingType));
            }
        }

        /// <summary>
        /// Changes the <see cref="BiomePivot"/> of this instance.
        /// </summary>
        /// <param name="biome">The biome.</param>
        /// <param name="underlyingType">The underlying biome if <see cref="WorkerActionPivot.Clear"/> action possible.</param>
        internal void ChangeBiome(BiomePivot biome, BiomePivot underlyingType = null)
        {
            Biome = biome ?? throw new ArgumentNullException(nameof(biome));
            if (Biome.Actions.Contains(WorkerActionPivot.Clear))
            {
                UnderlyingBiome = underlyingType ?? throw new ArgumentNullException(nameof(underlyingType));
            }
        }

        /// <summary>
        /// Applies default actions of the instance when a <see cref="CitizenPivot"/> is built on it.
        /// </summary>
        /// <param name="city">The city.</param>
        internal void ApplyCityActions(CityPivot city)
        {
            Road = true;
            RailRoad = true;
            SquareChangeEvent?.Invoke(this, new SquareChangedEventArgs(this));
        }

        /// <summary>
        /// Tries to apply a <see cref="WorkerActionPivot"/> on the current instance.
        /// </summary>
        /// <param name="worker">The worker</param>
        /// <param name="action">The action to apply.</param>
        /// <returns>
        /// <c>True</c> if the worker actually starts the action; <c>False</c> otherwise.
        /// </returns>
        internal bool ApplyAction(WorkerPivot worker, WorkerActionPivot action)
        {
            if (!action.AlwaysAvailable && !Biome.Actions.Contains(action))
            {
                return false;
            }

            if (action == WorkerActionPivot.Irrigate)
            {
                return ApplyActionInternal(worker, action, Irrigate);
            }

            if (action == WorkerActionPivot.Mine)
            {
                return ApplyActionInternal(worker, action, Mine);
            }

            if (action == WorkerActionPivot.Road)
            {
                return ApplyActionInternal(worker, action, Road);
            }

            if (action == WorkerActionPivot.DestroyImprovement)
            {
                return ApplyActionInternal(worker, action, !Irrigate && !Mine && !Fortress);
            }

            if (action == WorkerActionPivot.DestroyRoad)
            {
                return ApplyActionInternal(worker, action, !Road && !RailRoad);
            }

            if (action == WorkerActionPivot.BuildFortress)
            {
                return ApplyActionInternal(worker, action, Fortress);
            }

            if (action == WorkerActionPivot.Clear)
            {
                return ApplyActionInternal(worker, action, false);
            }

            if (action == WorkerActionPivot.ClearPollution)
            {
                return ApplyActionInternal(worker, action, Pollution);
            }

            if (action == WorkerActionPivot.Plant)
            {
                return ApplyActionInternal(worker, action, false);
            }

            if (action == WorkerActionPivot.RailRoad)
            {
                return ApplyActionInternal(worker, action, RailRoad);
            }

            return false;
        }

        /// <summary>
        /// Moves forward every actions currently in progress on the instance.
        /// This method need to be called at the end of each turn.
        /// If two opposed actions ends on the same turn, latest added is applied.
        /// </summary>
        internal void UpdateActionsProgress()
        {
            var removableActions = new List<InProgressWorkerActionPivot>();
            foreach (var action in _currentActions)
            {
                if (!action.HasWorkers)
                {
                    removableActions.Add(action);
                }
                else
                {
                    action.ForwardProgression();
                }

                if (action.IsDone)
                {
                    removableActions.Add(action);
                    if (action.Action == WorkerActionPivot.BuildFortress)
                    {
                        Fortress = true;
                    }
                    if (action.Action == WorkerActionPivot.Clear)
                    {
                        ChangeBiome(UnderlyingBiome, null);
                        Mine = false;
                        Irrigate = false;
                    }
                    if (action.Action == WorkerActionPivot.ClearPollution)
                    {
                        Pollution = false;
                    }
                    if (action.Action == WorkerActionPivot.DestroyImprovement)
                    {
                        if (Fortress)
                        {
                            Fortress = false;
                        }
                        else
                        {
                            // It looks like both are destroyed, but it's not possible to have both anyway.
                            Mine = false;
                            Irrigate = false;
                        }
                    }
                    if (action.Action == WorkerActionPivot.DestroyRoad)
                    {
                        if (RailRoad)
                        {
                            RailRoad = false;
                        }
                        else
                        {
                            Road = false;
                        }
                    }
                    if (action.Action == WorkerActionPivot.Irrigate)
                    {
                        Mine = false;
                        Irrigate = true;
                    }
                    if (action.Action == WorkerActionPivot.Mine)
                    {
                        Mine = true;
                        Irrigate = false;
                    }
                    if (action.Action == WorkerActionPivot.Plant)
                    {
                        Biome = BiomePivot.Forest;
                        Mine = false;
                        Irrigate = false;
                    }
                    if (action.Action == WorkerActionPivot.RailRoad)
                    {
                        RailRoad = true;
                    }
                    if (action.Action == WorkerActionPivot.Road)
                    {
                        Road = true;
                    }
                    SquareChangeEvent?.Invoke(this, new SquareChangedEventArgs(this));
                }
            }

            foreach (var action in removableActions.Distinct())
            {
                _currentActions.Remove(action);
                action.RemoveWorkers();
            }
        }

        /// <summary>
        /// Sets a river at a specified cardinal around the instance.
        /// </summary>
        /// <param name="cardinal">The <see cref="DirectionPivot"/>.</param>
        /// <param name="isRiver"><c>True</c> to set a river; <c>False</c> otherwise.</param>
        internal void SetRiver(DirectionPivot cardinal, bool isRiver)
        {
            _rivers[cardinal] = isRiver ? !Biome.IsSeaType : isRiver;
        }

        private bool ApplyActionInternal(WorkerPivot worker, WorkerActionPivot action, bool currentApplianceValue)
        {
            if (currentApplianceValue)
            {
                return false;
            }

            var actionInProgress = CurrentActions.SingleOrDefault(a => a.Action == action);
            if (actionInProgress == null)
            {
                actionInProgress = new InProgressWorkerActionPivot(action);
                _currentActions.Add(actionInProgress);
            }

            return actionInProgress.AddWorker(worker);
        }

        #region IEquatable implementation

        /// <summary>
        /// Checks if this instance is equal to another one.
        /// </summary>
        /// <param name="other">The other instance.</param>
        /// <returns><c>True</c> if equals; <c>False</c> otherwise.</returns>
        public bool Equals(MapSquarePivot other)
        {
            return Row == other?.Row && Column == other?.Commerce;
        }

        /// <summary>
        /// Operator "==" override. Checks equality between two instances.
        /// </summary>
        /// <param name="p1">The first <see cref="MapSquarePivot"/>.</param>
        /// <param name="p2">The second <see cref="MapSquarePivot"/>.</param>
        /// <returns><c>True</c> if equals; <c>False</c> otherwise.</returns>
        public static bool operator ==(MapSquarePivot p1, MapSquarePivot p2)
        {
            if (p1 is null)
            {
                return p2 is null;
            }

            return p1.Equals(p2) == true;
        }

        /// <summary>
        /// Operator "!=" override. Checks non-equality between two instances.
        /// </summary>
        /// <param name="p1">The first <see cref="MapSquarePivot"/>.</param>
        /// <param name="p2">The second <see cref="MapSquarePivot"/>.</param>
        /// <returns><c>False</c> if equals; <c>True</c> otherwise.</returns>
        public static bool operator !=(MapSquarePivot p1, MapSquarePivot p2)
        {
            return !(p1 == p2);
        }

        /// <inhrritdoc />
        public override bool Equals(object obj)
        {
            return obj is MapSquarePivot && Equals(obj as MapSquarePivot);
        }

        /// <inhrritdoc />
        public override int GetHashCode()
        {
            return Row.GetHashCode() ^ Column.GetHashCode();
        }

        #endregion
    }
}
