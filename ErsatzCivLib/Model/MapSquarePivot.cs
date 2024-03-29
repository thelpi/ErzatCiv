﻿using System;
using System.Collections.Generic;
using System.Linq;
using ErsatzCivLib.Model.Enums;
using ErsatzCivLib.Model.Events;
using ErsatzCivLib.Model.Static;
using ErsatzCivLib.Model.Units.Land;

namespace ErsatzCivLib.Model
{
    /// <summary>
    /// Represents a map square.
    /// </summary>
    /// <seealso cref="IEquatable{T}"/>
    [Serializable]
    public class MapSquarePivot : IEquatable<MapSquarePivot>
    {
        private const double RAILROAD_BONUS_RATE = 1.5;
        private const int ROAD_COMMERCE_BONUS = 1;
        private const int MINE_PRODUCTIVITY_BONUS = 1;
        private const int MINE_PRODUCTIVITY_BONUS_HILLS = 3;
        private const int IRRIGATE_FOOD_BONUS = 1;

        #region Embedded properties

        /// <summary>
        /// Continent index.
        /// </summary>
        public int ContinentIndex { get; private set; }
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
        /// Indicates if the square has a biome bonus.
        /// </summary>
        public bool HasBonus { get; private set; }
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

        private List<InProgressMapSquareImprovementPivot> _currentActions = new List<InProgressMapSquareImprovementPivot>();
        /// <summary>
        /// List of <see cref="InProgressMapSquareImprovementPivot"/> in progress for this instance.
        /// </summary>
        public IReadOnlyCollection<InProgressMapSquareImprovementPivot> CurrentActions { get { return _currentActions; } }

        #endregion

        #region Inferred properties

        /// <summary>
        /// Food value of this instance.
        /// </summary>
        /// <remarks>Pollution and settler actions included.</remarks>
        public int Food
        {
            get
            {
                var baseValue = Biome.Food + (HasBonus ? Biome.BonusFood : 0);
                if (Irrigate)
                {
                    baseValue += IRRIGATE_FOOD_BONUS;
                }
                if (RailRoad && baseValue > 0)
                {
                    baseValue = (int)Math.Floor(baseValue * RAILROAD_BONUS_RATE);
                }

                return baseValue / (Pollution ? 2 : 1);
            }
        }
        /// <summary>
        /// Productivity value of this instance.
        /// </summary>
        /// <remarks>Pollution and settler actions included.</remarks>
        public int Productivity
        {
            get
            {
                var baseValue = (Biome.Productivity + (HasBonus ? Biome.BonusProductivity : 0));
                if (Mine)
                {
                    baseValue += Biome == BiomePivot.Hills ? MINE_PRODUCTIVITY_BONUS_HILLS : MINE_PRODUCTIVITY_BONUS;
                }
                if (RailRoad && baseValue > 0)
                {
                    baseValue = (int)Math.Floor(baseValue * RAILROAD_BONUS_RATE);
                }

                return baseValue / (Pollution ? 2 : 1);
            }
        }
        /// <summary>
        /// Commerce value of this instance.
        /// </summary>
        /// <remarks>Pollution and settler actions included.</remarks>
        public int Commerce
        {
            get
            {
                var baseValue = Biome.Commerce + (HasBonus ? Biome.BonusCommerce : 0);
                if (Road && (Biome == BiomePivot.Grassland || Biome == BiomePivot.Plains || Biome == BiomePivot.Desert))
                {
                    baseValue += ROAD_COMMERCE_BONUS;
                }
                if (RailRoad)
                {
                    // TODO : not applicable if democracy or republic.
                    baseValue = (int)Math.Floor(baseValue * RAILROAD_BONUS_RATE);
                }

                return baseValue / (Pollution ? 2 : 1);
            }
        }
        /// <summary>
        /// Sum of food, productivity and commerce statistics.
        /// </summary>
        public int TotalValue { get { return Food + Productivity + Commerce; } }

        #endregion

        #region Public events

        /// <summary>
        /// Event triggered when the instance is edited.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler<SquareChangedEventArgs> SquareChangeEvent;

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="row">The <see cref="Row"/> value.</param>
        /// <param name="column">The <see cref="Column"/> value.</param>
        /// <param name="biome">The <see cref="Biome"/> value.</param>
        /// <param name="continentIndex">The <see cref="ContinentIndex"/> value.</param>
        internal MapSquarePivot(int row, int column, BiomePivot biome, int continentIndex)
        {
            Row = row;
            Column = column;
            Biome = biome;
            HasBonus = Tools.Randomizer.NextDouble() < Biome.BonusApperanceRate;
            ContinentIndex = continentIndex;
        }

        #region Internal methods

        /// <summary>
        /// Gets the <see cref="DirectionPivot"/> of the current instance relatively to another instance.
        /// </summary>
        /// <param name="other">The second instance.</param>
        /// <returns>The direction; <c>Null</c> if instances have a distance above <c>1</c>, or instances are equal.</returns>
        internal DirectionPivot? GetDirectionRelativeTo(MapSquarePivot other)
        {
            if (this == other)
            {
                return null;
            }

            if (Row == other.Row && Column == other.Column + 1)
            {
                return DirectionPivot.Right;
            }
            else if (Row == other.Row && Column == other.Column - 1)
            {
                return DirectionPivot.Left;
            }
            else if(Row == other.Row + 1 && Column == other.Column)
            {
                return DirectionPivot.Bottom;
            }
            else if (Row == other.Row - 1 && Column == other.Column)
            {
                return DirectionPivot.Top;
            }
            else if (Row == other.Row - 1 && Column == other.Column - 1)
            {
                return DirectionPivot.TopLeft;
            }
            else if (Row == other.Row - 1 && Column == other.Column + 1)
            {
                return DirectionPivot.TopRight;
            }
            else if (Row == other.Row + 1 && Column == other.Column - 1)
            {
                return DirectionPivot.BottomLeft;
            }
            else if (Row == other.Row + 1 && Column == other.Column + 1)
            {
                return DirectionPivot.BottomRight;
            }

            return null;
        }

        /// <summary>
        /// Changes the <see cref="BiomePivot"/> of this instance.
        /// </summary>
        /// <param name="biome">The biome.</param>
        internal void ChangeBiome(BiomePivot biome)
        {
            Biome = biome;
            HasBonus = Tools.Randomizer.NextDouble() < Biome.BonusApperanceRate;
        }

        /// <summary>
        /// Applies default actions of the instance when a <see cref="CitizenPivot"/> is built on it.
        /// </summary>
        internal void ApplyCityActions()
        {
            Road = true;
            RailRoad = false; // Game bug !
            SquareChangeEvent?.Invoke(this, new SquareChangedEventArgs(this));
        }

        /// <summary>
        /// Tries to apply a <see cref="MapSquareImprovementPivot"/> on the current instance.
        /// </summary>
        /// <param name="settler">The settler.</param>
        /// <param name="action">The action to apply.</param>
        /// <returns>
        /// <c>True</c> if the settler actually starts the action; <c>False</c> otherwise.
        /// </returns>
        internal bool ApplyAction(SettlerPivot settler, MapSquareImprovementPivot action)
        {
            if (!action.AlwaysAvailable && !Biome.Actions.Contains(action))
            {
                return false;
            }

            if (action == MapSquareImprovementPivot.Irrigate)
            {
                return ApplyActionInternal(settler, action, Irrigate);
            }

            if (action == MapSquareImprovementPivot.Mine)
            {
                return ApplyActionInternal(settler, action, Mine);
            }

            if (action == MapSquareImprovementPivot.Road)
            {
                return ApplyActionInternal(settler, action, Road);
            }

            if (action == MapSquareImprovementPivot.DestroyImprovement)
            {
                return ApplyActionInternal(settler, action, !Irrigate && !Mine && !Fortress);
            }

            if (action == MapSquareImprovementPivot.DestroyRoad)
            {
                return ApplyActionInternal(settler, action, !Road && !RailRoad);
            }

            if (action == MapSquareImprovementPivot.BuildFortress)
            {
                return ApplyActionInternal(settler, action, Fortress);
            }

            if (action == MapSquareImprovementPivot.Clear)
            {
                return ApplyActionInternal(settler, action, false);
            }

            if (action == MapSquareImprovementPivot.ClearPollution)
            {
                return ApplyActionInternal(settler, action, Pollution);
            }

            if (action == MapSquareImprovementPivot.Plant)
            {
                return ApplyActionInternal(settler, action, false);
            }

            if (action == MapSquareImprovementPivot.RailRoad)
            {
                return ApplyActionInternal(settler, action, RailRoad);
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
            var removableActions = new List<InProgressMapSquareImprovementPivot>();
            foreach (var action in _currentActions)
            {
                if (!action.HasSettlers)
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
                    if (action.Action == MapSquareImprovementPivot.BuildFortress)
                    {
                        Fortress = true;
                    }
                    if (action.Action == MapSquareImprovementPivot.Clear)
                    {
                        ChangeBiome(Biome.UnderlyingBiome(BiomePivot.Biomes));
                        Mine = false;
                        Irrigate = false;
                    }
                    if (action.Action == MapSquareImprovementPivot.ClearPollution)
                    {
                        Pollution = false;
                    }
                    if (action.Action == MapSquareImprovementPivot.DestroyImprovement)
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
                    if (action.Action == MapSquareImprovementPivot.DestroyRoad)
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
                    if (action.Action == MapSquareImprovementPivot.Irrigate)
                    {
                        Mine = false;
                        Irrigate = true;
                    }
                    if (action.Action == MapSquareImprovementPivot.Mine)
                    {
                        Mine = true;
                        Irrigate = false;
                    }
                    if (action.Action == MapSquareImprovementPivot.Plant)
                    {
                        Biome = BiomePivot.Forest;
                        Mine = false;
                        Irrigate = false;
                    }
                    if (action.Action == MapSquareImprovementPivot.RailRoad)
                    {
                        RailRoad = true;
                    }
                    if (action.Action == MapSquareImprovementPivot.Road)
                    {
                        Road = true;
                    }
                    SquareChangeEvent?.Invoke(this, new SquareChangedEventArgs(this));
                }
            }

            foreach (var action in removableActions.Distinct())
            {
                _currentActions.Remove(action);
                action.RemoveSettlers();
            }
        }

        #endregion

        #region Private methods

        private bool ApplyActionInternal(SettlerPivot settler, MapSquareImprovementPivot action, bool currentApplianceValue)
        {
            if (currentApplianceValue)
            {
                return false;
            }

            var actionInProgress = CurrentActions.SingleOrDefault(a => a.Action == action);
            if (actionInProgress == null)
            {
                actionInProgress = new InProgressMapSquareImprovementPivot(action);
                _currentActions.Add(actionInProgress);
            }

            return actionInProgress.AddSettler(settler);
        }

        #endregion

        #region IEquatable implementation

        /// <summary>
        /// Checks if this instance is equal to another one.
        /// </summary>
        /// <param name="other">The other instance.</param>
        /// <returns><c>True</c> if equals; <c>False</c> otherwise.</returns>
        public bool Equals(MapSquarePivot other)
        {
            return Row == other?.Row && Column == other?.Column;
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
