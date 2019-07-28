using System;
using System.Collections.Generic;
using System.Linq;
using ErsatzCivLib.Model.Units;

namespace ErsatzCivLib.Model
{
    /// <summary>
    /// Represents a map square.
    /// </summary>
    public class MapSquarePivot
    {
        #region Properties

        public event EventHandler SquareChangeEvent;

        private List<CurrentActionPivot> _currentActions = new List<CurrentActionPivot>();

        /// <summary>
        /// Row on the map grid.
        /// </summary>
        public int Row { get; private set; }
        /// <summary>
        /// Column on the map grid.
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
        /// <summary>
        /// List of <see cref="CurrentActionPivot"/> in progress for this instance.
        /// </summary>
        public IReadOnlyCollection<CurrentActionPivot> CurrentActions { get { return _currentActions; } }
        /// <summary>
        /// Indicates if the square is crossed by a river.
        /// </summary>
        public bool CrossedByRiver { get; private set; }
        public bool? RiverTopToBottom { get; private set; }

        #endregion

        internal MapSquarePivot(BiomePivot biome, int row, int column, BiomePivot underlyingType = null)
        {
            Biome = biome ?? throw new ArgumentNullException(nameof(biome));
            Row = row < 0 ? throw new ArgumentException("Invalid value.", nameof(row)) : row;
            Column = column < 0 ? throw new ArgumentException("Invalid value.", nameof(column)) : column;
            if (Biome.Actions.Contains(WorkerActionPivot.Clear))
            {
                UnderlyingBiome = underlyingType ?? throw new ArgumentNullException(nameof(underlyingType));
            }
        }

        internal void ChangeBiome(BiomePivot biome, BiomePivot underlyingType = null)
        {
            Biome = biome ?? throw new ArgumentNullException(nameof(biome));
            if (Biome.Actions.Contains(WorkerActionPivot.Clear))
            {
                UnderlyingBiome = underlyingType ?? throw new ArgumentNullException(nameof(underlyingType));
            }
        }

        internal void SetRiver(bool riverTopToBottom)
        {
            if (Biome.RiverCrossable)
            {
                RiverTopToBottom = CrossedByRiver && (RiverTopToBottom != riverTopToBottom || !RiverTopToBottom.HasValue) ? (bool?)null : riverTopToBottom;
                CrossedByRiver = true;
            }
        }

        internal void ApplyCityActions(CityPivot city)
        {
            if (city.Row != Row || city.Column != Column)
            {
                return;
            }

            Road = true;
            RailRoad = true;
            SquareChangeEvent?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Tries to apply a <see cref="WorkerActionPivot"/> on the current instance.
        /// </summary>
        /// <param name="worker">The worker</param>
        /// <param name="action">The action to apply.</param>
        /// <returns>
        /// <c>True</c> if the worker actually starts the action; <c>False</c> otherwise.
        /// </returns>
        internal bool ApplyAction(Engine engine, WorkerPivot worker, WorkerActionPivot action)
        {
            if (worker == null)
            {
                throw new ArgumentNullException(nameof(worker));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if ((!action.AlwaysAvailable && !Biome.Actions.Contains(action)) || engine.IsCity(worker.Row, worker.Column))
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

        private bool ApplyActionInternal(WorkerPivot worker, WorkerActionPivot action, bool currentApplianceValue)
        {
            if (currentApplianceValue)
            {
                return false;
            }

            var actionInProgress = CurrentActions.SingleOrDefault(a => a.Action == action);
            if (actionInProgress == null)
            {
                actionInProgress = new CurrentActionPivot(action);
                _currentActions.Add(actionInProgress);
            }

            return actionInProgress.AddWorker(worker);
        }

        /// <summary>
        /// Moves forward every actions currently in progress on the instance.
        /// This method need to be called at the end of each turn.
        /// If two opposed actions ends on the same turn, latest added is applied.
        /// </summary>
        internal void UpdateActionsProgress()
        {
            var removableActions = new List<CurrentActionPivot>();
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
                    SquareChangeEvent?.Invoke(this, new EventArgs());
                }
            }

            foreach (var action in removableActions.Distinct())
            {
                _currentActions.Remove(action);
                action.Dispose();
            }
        }

        internal bool IsClose(MapSquarePivot other)
        {
            return (Math.Abs(Row - other.Row) == 1 && Column == other.Column) ||
                (Math.Abs(Column - other.Column) == 1 && Row == other.Row) ||
                (Math.Abs(Row - other.Row) == 1 && Math.Abs(Column - other.Column) == 1);
        }

        /// <summary>
        /// Represents a <see cref="WorkerActionPivot"/> in progress.
        /// </summary>
        public class CurrentActionPivot : IDisposable
        {
            private static List<CurrentActionPivot> _globalActions = new List<CurrentActionPivot>();

            private List<WorkerPivot> _workers;

            /// <summary>
            /// Related <see cref="WorkerActionPivot"/>.
            /// </summary>
            public WorkerActionPivot Action { get; private set; }
            /// <summary>
            /// Number of turns already spent.
            /// </summary>
            public int TurnsCount { get; private set; }

            /// <summary>
            /// Inferred; indicates if the action has at least one worker.
            /// </summary>
            public bool HasWorkers { get { return _workers.Count > 0; } }
            /// <summary>
            /// Inferred; indicates if the action is done.
            /// </summary>
            public bool IsDone { get { return TurnsCount >= Action.TurnCost; } }

            internal static bool WorkerIsBusy(WorkerPivot worker)
            {
                return _globalActions.Any(a => a._workers.Contains(worker));
            }

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <remarks>The task has no worker by default.</remarks>
            /// <param name="guid">Caller key.</param>
            /// <param name="action">The action to start.</param>
            internal CurrentActionPivot(WorkerActionPivot action)
            {
                Action = action ?? throw new ArgumentNullException(nameof(action));
                _workers = new List<WorkerPivot>();
                TurnsCount = 0;
                _globalActions.Add(this);
            }

            /// <summary>
            /// Adds a worker to the action.
            /// </summary>
            /// <param name="worker">The worker.</param>
            /// <returns><c>True</c> if success; <c>False</c> otherwise.</returns>
            internal bool AddWorker(WorkerPivot worker)
            {
                bool canWork = !WorkerIsBusy(worker);
                if (canWork)
                {
                    _workers.Add(worker);
                }
                return canWork;
            }

            /// <summary>
            /// Recomputes <see cref="TurnsCount"/>.
            /// </summary>
            internal void ForwardProgression()
            {
                TurnsCount += _workers.Count;
            }

            /// <summary>
            /// Disposes the instance.
            /// </summary>
            public void Dispose()
            {
                _globalActions.Remove(this);
            }
        }
    }
}
