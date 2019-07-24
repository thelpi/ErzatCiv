using System;
using System.Collections.Generic;
using System.Linq;

namespace ErsatzCiv.Model
{
    /// <summary>
    /// Represents a map square.
    /// </summary>
    public class MapSquareData
    {
        #region Properties

        public event EventHandler NextUnitEvent;

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
        public MapSquareTypeData MapSquareType { get; private set; }
        /// <summary>
        /// Underlying type in case of clearage (forest, jungle...).
        /// </summary>
        public MapSquareTypeData UnderlyingMapSquareType { get; private set; }
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
        /// <summary>
        /// <c>True</c> if render must be recomputed.
        /// </summary>
        public bool Redraw { get; private set; }

        #endregion

        public MapSquareData(MapSquareTypeData mapSquareType, int row, int column,
            MapSquareTypeData underlyingType = null, bool crossedByRiver = false)
        {
            MapSquareType = mapSquareType ?? throw new ArgumentNullException(nameof(mapSquareType));
            Row = row < 0 ? throw new ArgumentException("Invalid value.", nameof(row)) : row;
            Column = column < 0 ? throw new ArgumentException("Invalid value.", nameof(column)) : column;
            if (MapSquareType.Actions.Contains(MapSquareActionPivot.Clear))
            {
                UnderlyingMapSquareType = underlyingType ?? throw new ArgumentNullException(nameof(underlyingType));
            }
            if (MapSquareType.RiverCrossable)
            {
                CrossedByRiver = crossedByRiver;
            }
        }

        /// <summary>
        /// Tries to apply a <see cref="MapSquareActionPivot"/> on the current instance.
        /// </summary>
        /// <param name="worker">The worker</param>
        /// <param name="action">The action to apply.</param>
        /// <returns>
        /// <c>True</c> if the worker actually starts the action; <c>False</c> otherwise.
        /// </returns>
        public bool ApplyAction(Engine engine, WorkerPivot worker, MapSquareActionPivot action)
        {
            if (worker == null)
            {
                throw new ArgumentNullException(nameof(worker));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if ((!action.AlwaysAvailable && !MapSquareType.Actions.Contains(action)) || engine.IsCity(worker.Row, worker.Column))
            {
                return false;
            }

            if (action == MapSquareActionPivot.Irrigate)
            {
                return ApplyActionInternal(worker, action, Irrigate);
            }

            if (action == MapSquareActionPivot.Mine)
            {
                return ApplyActionInternal(worker, action, Mine);
            }

            if (action == MapSquareActionPivot.Road)
            {
                return ApplyActionInternal(worker, action, Road);
            }

            if (action == MapSquareActionPivot.DestroyImprovement)
            {
                return ApplyActionInternal(worker, action, !Irrigate && !Mine && !Fortress);
            }

            if (action == MapSquareActionPivot.DestroyRoad)
            {
                return ApplyActionInternal(worker, action, !Road && !RailRoad);
            }

            if (action == MapSquareActionPivot.BuildFortress)
            {
                return ApplyActionInternal(worker, action, Fortress);
            }

            if (action == MapSquareActionPivot.Clear)
            {
                return ApplyActionInternal(worker, action, false);
            }

            if (action == MapSquareActionPivot.ClearPollution)
            {
                return ApplyActionInternal(worker, action, Pollution);
            }

            if (action == MapSquareActionPivot.Plant)
            {
                return ApplyActionInternal(worker, action, false);
            }

            if (action == MapSquareActionPivot.RailRoad)
            {
                return ApplyActionInternal(worker, action, RailRoad);
            }

            return false;
        }

        private bool ApplyActionInternal(WorkerPivot worker, MapSquareActionPivot action, bool currentApplianceValue)
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
        public void UpdateActionsProgress()
        {
            var removableActions = new List<CurrentActionPivot>();
            foreach (var action in _currentActions)
            {
                if (action.IsDone)
                {
                    removableActions.Add(action);
                    Redraw = true;
                    if (action.Action == MapSquareActionPivot.BuildFortress)
                    {
                        Fortress = true;
                    }
                    if (action.Action == MapSquareActionPivot.Clear)
                    {
                        MapSquareType = UnderlyingMapSquareType;
                        Mine = false;
                        Irrigate = false;
                    }
                    if (action.Action == MapSquareActionPivot.ClearPollution)
                    {
                        Pollution = false;
                    }
                    if (action.Action == MapSquareActionPivot.DestroyImprovement)
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
                    if (action.Action == MapSquareActionPivot.DestroyRoad)
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
                    if (action.Action == MapSquareActionPivot.Irrigate)
                    {
                        Mine = false;
                        Irrigate = true;
                    }
                    if (action.Action == MapSquareActionPivot.Mine)
                    {
                        Mine = true;
                        Irrigate = false;
                    }
                    if (action.Action == MapSquareActionPivot.Plant)
                    {
                        MapSquareType = MapSquareTypeData.Forest;
                        Mine = false;
                        Irrigate = false;
                    }
                    if (action.Action == MapSquareActionPivot.RailRoad)
                    {
                        RailRoad = true;
                    }
                    if (action.Action == MapSquareActionPivot.Road)
                    {
                        Road = true;
                    }
                }
                else if (!action.HasWorkers)
                {
                    removableActions.Add(action);
                }
                else
                {
                    action.ForwardProgression();
                }
            }

            foreach (var action in _currentActions)
            {
                _currentActions.Remove(action);
                action.Dispose();
            }
        }

        /// <summary>
        /// Clears the <see cref="Redraw"/> property.
        /// </summary>
        public void ResetRedraw()
        {
            Redraw = false;
        }

        /// <summary>
        /// Represents a <see cref="MapSquareActionPivot"/> in progress.
        /// </summary>
        public class CurrentActionPivot : IDisposable
        {
            private static List<CurrentActionPivot> _globalActions = new List<CurrentActionPivot>();

            private List<WorkerPivot> _workers;

            /// <summary>
            /// Related <see cref="MapSquareActionPivot"/>.
            /// </summary>
            public MapSquareActionPivot Action { get; private set; }
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

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <remarks>The task has no worker by default.</remarks>
            /// <param name="guid">Caller key.</param>
            /// <param name="action">The action to start.</param>
            public CurrentActionPivot(MapSquareActionPivot action)
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
            public bool AddWorker(WorkerPivot worker)
            {
                if (!_globalActions.Any(a => a._workers.Contains(worker)))
                {
                    _workers.Add(worker);
                    return true;
                }
                return false;
            }

            /// <summary>
            /// Removes a worker from the action.
            /// </summary>
            /// <param name="worker">The worker.</param>
            public static void RemoveWorker(WorkerPivot worker)
            {
                _globalActions.ForEach(a => a._workers.Remove(worker));
            }

            /// <summary>
            /// Recomputes <see cref="TurnsCount"/>.
            /// </summary>
            public void ForwardProgression()
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
