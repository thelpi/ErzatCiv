using System;
using System.Collections.Generic;
using System.Linq;
using ErsatzCivLib.Model.Persistent;
using ErsatzCivLib.Model.Units;

namespace ErsatzCivLib.Model
{
    /// <summary>
    /// Represents a map square.
    /// </summary>
    [Serializable]
    public class MapSquarePivot
    {
        #region Properties

        [field: NonSerialized]
        public event EventHandler<SquareChangedEventArgs> SquareChangeEvent;

        private List<InProgressWorkerActionPivot> _currentActions = new List<InProgressWorkerActionPivot>();

        public int Row { get; private set; }
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
        /// List of <see cref="InProgressWorkerActionPivot"/> in progress for this instance.
        /// </summary>
        public IReadOnlyCollection<InProgressWorkerActionPivot> CurrentActions { get { return _currentActions; } }
        /// <summary>
        /// Indicates if the square is crossed by a river.
        /// </summary>
        public bool CrossedByRiver { get; private set; }
        public bool? RiverTopToBottom { get; private set; }

        public int Food
        {
            get
            {
                var baseValue = 0;
                if (!Pollution)
                {
                    baseValue = Biome.Food;
                    if (CrossedByRiver)
                    {
                        baseValue += BiomePivot.RIVER_BONUS;
                    }
                    if (Irrigate)
                    {
                        baseValue = baseValue == 0 ?
                            WorkerActionPivot.IRRIGATE_FOOD_BONUS_IF_ZERO :
                            baseValue * WorkerActionPivot.IRRIGATE_FOOD_MULTIPLE;
                    }
                }
                return baseValue;
            }
        }
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
                        baseValue = baseValue == 0 ?
                            WorkerActionPivot.MINE_PRODUCTIVITY_BONUS_IF_ZERO :
                            baseValue * WorkerActionPivot.MINE_PRODUCTIVITY_MULTIPLE;
                    }
                    if (RailRoad && baseValue > 0)
                    {
                        baseValue += WorkerActionPivot.RAILROAD_PRODUCTIVITY_BONUS;
                    }
                }
                return baseValue;
            }
        }
        public int Commerce
        {
            get
            {
                var baseValue = 0;
                if (!Pollution)
                {
                    baseValue = Biome.Commerce;
                    if (CrossedByRiver)
                    {
                        baseValue += BiomePivot.RIVER_BONUS;
                    }
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
        public int TotalValue { get { return Food + Productivity + Commerce; } }

        public int CityFood
        {
            get
            {
                var baseValue = Biome.Food;
                if (CrossedByRiver)
                {
                    baseValue += BiomePivot.RIVER_BONUS;
                }
                return baseValue < 2 ? 2 : baseValue;
            }
        }
        public int CityProductivity
        {
            get
            {
                return Biome.Productivity < 1 ? 1 : Biome.Productivity;
            }
        }
        public int CityCommerce
        {
            get
            {
                var baseValue = Biome.Commerce;
                if (CrossedByRiver)
                {
                    baseValue += BiomePivot.RIVER_BONUS;
                }
                return baseValue < 2 ? 2 : baseValue;
            }
        }

        #endregion

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
            if (worker == null)
            {
                throw new ArgumentNullException(nameof(worker));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

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

        [Serializable]
        public class SquareChangedEventArgs : EventArgs
        {
            public MapSquarePivot MapSquare { get; private set; }

            public SquareChangedEventArgs(MapSquarePivot mapSquare)
            {
                MapSquare = mapSquare;
            }
        }
    }
}
