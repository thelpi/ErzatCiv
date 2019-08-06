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
    /// Represents a player in-game.
    /// </summary>
    /// <seealso cref="IEquatable{T}"/>
    [Serializable]
    public class PlayerPivot : IEquatable<PlayerPivot>
    {
        private const int TREASURE_START = 100;

        private List<CityPivot> _cities = new List<CityPivot>();
        private List<AdvancePivot> _advances = new List<AdvancePivot>();
        private List<UnitPivot> _units = new List<UnitPivot>();
        private int _currentUnitIndex;
        private int _previousUnitIndex;
        private int _anarchyTurnsCount;

        #region Embedded properties

        /// <summary>
        /// The player civilization.
        /// </summary>
        public CivilizationPivot Civilization { get; private set; }
        /// <summary>
        /// IA player yes/no.
        /// </summary>
        public bool IsIA { get; private set; }
        /// <summary>
        /// Science points stacked.
        /// </summary>
        public int ScienceStack { get; private set; }
        /// <summary>
        /// <see cref="AdvancePivot"/> currently in discovery.
        /// </summary>
        public AdvancePivot CurrentAdvance { get; private set; }
        /// <summary>
        /// Current <see cref="RegimePivot"/>.
        /// </summary>
        public RegimePivot CurrentRegime { get; private set; }
        /// <summary>
        /// Current gold amount in the treasure.
        /// </summary>
        public int Treasure { get; private set; }

        #endregion

        #region Inferred properties

        /// <summary>
        /// Collection of <see cref="UnitPivot"/>.
        /// </summary>
        public IReadOnlyCollection<UnitPivot> Units { get { return _units; } }
        /// <summary>
        /// The current <see cref="UnitPivot"/>. Might be <c>null</c>.
        /// </summary>
        public UnitPivot CurrentUnit
        {
            get
            {
                if (_currentUnitIndex == -1)
                {
                    return null;
                }

                if (_currentUnitIndex < -1 || _currentUnitIndex >= _units.Count)
                {
                    // Anormal behavior.
                    return null;
                }

                return _units[_currentUnitIndex];
            }
        }
        /// <summary>
        /// The previous <see cref="UnitPivot"/>. Might be <c>null</c>.
        /// </summary>
        public UnitPivot PreviousUnit
        {
            get
            {
                if (_previousUnitIndex == -1 || _previousUnitIndex == _currentUnitIndex)
                {
                    return null;
                }

                if (_previousUnitIndex < -1 || _previousUnitIndex >= _units.Count)
                {
                    // Anormal behavior.
                    return null;
                }

                return _units[_previousUnitIndex];
            }
        }
        /// <summary>
        /// List of <see cref="CityPivot"/> built by the player.
        /// </summary>
        public IReadOnlyCollection<CityPivot> Cities { get { return _cities; } }
        /// <summary>
        /// List of discovered <see cref="AdvancePivot"/>.
        /// </summary>
        public IReadOnlyCollection<AdvancePivot> Advances { get { return _advances; } }
        /// <summary>
        /// Science points at each turn.
        /// </summary>
        public int ScienceByTurn
        {
            get
            {
                return _cities.Sum(c => c.Science);
            }
        }
        /// <summary>
        /// Number of turns before current advance discovered.
        /// </summary>
        public int TurnsBeforeNewAdvance
        {
            get
            {
                return CurrentAdvance == null ? 0 : (
                    ScienceByTurn == 0 ? 9999 : (int)Math.Ceiling((AdvancePivot.SCIENCE_COST - ScienceStack) / (double)ScienceByTurn)
                );
            }
        }
        /// <summary>
        /// Current player <see cref="EraPivot"/>.
        /// </summary>
        public EraPivot CurrentEra
        {
            get
            {
                return (EraPivot)(_advances.Count == 0 ? 0 : _advances.Max(a => (int)a.Era));
            }
        }
        /// <summary>
        /// Gold added (or substract) to the treasure at each turn.
        /// </summary>
        public int TreasureByTurn
        {
            get
            {
                // TODO
                return _cities.Sum(c => c.Commerce + c.Tax);
            }
        }
        /// <summary>
        /// Number of turns left before the end of a revolution (if any).
        /// </summary>
        public int RevolutionTurnsCount
        {
            get
            {
                var turnsLeft = ((_cities.Count * RegimePivot.REVOLUTION_TURNS_BY_CITY) + 1) - _anarchyTurnsCount;
                return (int)Math.Ceiling(turnsLeft < 0 ? 0 : turnsLeft);
            }
        }
        /// <summary>
        /// Gets if a revolution was in progress and now done.
        /// </summary>
        public bool RevolutionIsDone
        {
            get
            {
                return CurrentRegime == RegimePivot.Anarchy && RevolutionTurnsCount == 0;
            }
        }
        /// <summary>
        /// Gets the name of the next city to build.
        /// </summary>
        /// <returns>Name of the next city.</returns>
        public string GetNextCityName
        {
            get
            {
                return Civilization.NextCityName(_cities);
            }
        }

        #endregion

        #region Public events

        /// <summary>
        /// Triggered when a <see cref="UnitPivot"/> is moved.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler<NextUnitEventArgs> NextUnitEvent;
        /// <summary>
        /// Triggered when <see cref="CurrentRegime"/> changes.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler<EventArgs> NewRegimeEvent;
        /// <summary>
        /// Triggered when <see cref="CurrentAdvance"/> changes.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler<EventArgs> NewAdvanceEvent;

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="civilization">The <see cref="Civilization"/> value.</param>
        /// <param name="isIa">The <see cref="IsIA"/> value.</param>
        /// <param name="beginLocation">Units position at the beginning.</param>
        internal PlayerPivot(CivilizationPivot civilization, bool isIa, MapSquarePivot beginLocation)
        {
            Civilization = civilization;
            IsIA = isIa;
            _advances.AddRange(civilization.Advances);

            CurrentRegime = RegimePivot.Despotism;
            Treasure = TREASURE_START;

            _units.Add(SettlerPivot.CreateAtLocation(beginLocation));
            _units.Add(WorkerPivot.CreateAtLocation(beginLocation));

            SetUnitIndex(false, true);
        }

        #region Public methods

        /// <summary>
        /// Get the list of <see cref="AdvancePivot"/> the player can discover at this point.
        /// </summary>
        /// <returns>List of <see cref="AdvancePivot"/>.</returns>
        public IReadOnlyCollection<AdvancePivot> GetAvailableAdvances()
        {
            var currentEraList = AdvancePivot.AdvancesByEra[CurrentEra]
                .Where(a => !_advances.Contains(a) && a.Prerequisites.All(_advances.Contains))
                .ToList();

            if (!currentEraList.Any())
            {
                if (CurrentEra != EraPivot.ModernAge)
                {
                    // TODO : "futures advances"
                }
                else
                {
                    currentEraList = AdvancePivot.AdvancesByEra[(EraPivot)(((int)CurrentEra) + 1)]
                        .Where(a => !_advances.Contains(a) && a.Prerequisites.All(_advances.Contains))
                        .ToList();
                }
            }

            return currentEraList;
        }

        /// <summary>
        /// Gets every <see cref="RegimePivot"/> instances the player can access to.
        /// </summary>
        /// <returns>List of <see cref="RegimePivot"/>.</returns>
        public IReadOnlyCollection<RegimePivot> GetAvailableRegimes()
        {
            return RegimePivot
                    .Instances
                    .Where(r => r.AdvancePrerequisite is null || _advances.Contains(r.AdvancePrerequisite))
                    .ToList();
        }

        #endregion

        #region Internal methods

        /// <summary>
        /// Triggrs a revolution; sets <see cref="CurrentRegime"/> to <see cref="RegimePivot.Anarchy"/> for a while.
        /// </summary>
        internal void TriggerRevolution()
        {
            if (CurrentRegime != RegimePivot.Anarchy)
            {
                CurrentRegime = RegimePivot.Anarchy;
                _anarchyTurnsCount = 0;
                NewRegimeEvent?.Invoke(this, new EventArgs());
            }
        }

        /// <summary>
        /// Tries to modify the <see cref="AdvancePivot"/> currently in discovery.
        /// </summary>
        /// <remarks>
        /// If an <see cref="AdvancePivot"/> was already in progress,
        /// calling this method divide <see cref="ScienceStack"/> in half.
        /// </remarks>
        /// <param name="advance">The <see cref="AdvancePivot"/>.</param>
        /// <returns><c>True</c> if the current advance has been changed; <c>False</c> otherwise.</returns>
        internal bool ChangeCurrentAdvance(AdvancePivot advance)
        {
            // Conditions to search :
            // - not already known
            // - prerequisites are known
            // - same era as the current one, or next one if the current era is done
            if (_advances.Contains(advance)
                || (advance.Prerequisites.Any() && !advance.Prerequisites.All(_advances.Contains))
                || ((int)advance.Era > (int)CurrentEra && GetAvailableAdvances().Any()))
            {
                return false;
            }

            if (CurrentAdvance != null && CurrentAdvance != advance)
            {
                ScienceStack /= 2;
            }
            CurrentAdvance = advance;
            NewAdvanceEvent?.Invoke(this, new EventArgs());
            return true;
        }

        /// <summary>
        /// Tries to create a city at the location of the <see cref="CurrentUnit"/>.
        /// </summary>
        /// <param name="currentTurn">The <see cref="Engine.CurrentTurn"/> value.</param>
        /// <param name="name">Name of the city.</param>
        /// <param name="notUniqueNameError">Out; indicates a failure caused by a non-unique city name.</param>
        /// <param name="isCityCallback">Callback method to check if a city, on any civilization, already exists at this location.</param>
        /// <param name="computeCityAvailableMapSquaresCallback">Callback method to compute available <see cref="MapSquarePivot"/> around the city to build.</param>
        /// <returns>The <see cref="CityPivot"/> built; <c>Null</c> if failure.</returns>
        internal CityPivot BuildCity(int currentTurn, string name, out bool notUniqueNameError,
            Func<MapSquarePivot, bool> isCityCallback,
            Func<CityPivot, List<MapSquarePivot>> computeCityAvailableMapSquaresCallback)
        {
            notUniqueNameError = false;

            if (!CanBuildCity(isCityCallback))
            {
                return null;
            }

            if (_cities.Any(c => c.Name.Equals(name.ToLower(), StringComparison.InvariantCultureIgnoreCase)))
            {
                notUniqueNameError = true;
                return null;
            }

            var settler = CurrentUnit as SettlerPivot;
            var sq = CurrentUnit.MapSquareLocation;

            var city = new CityPivot(currentTurn, name, sq, computeCityAvailableMapSquaresCallback, CapitalizationPivot.Default);
            sq.ApplyCityActions(city);

            _cities.Add(city);
            _units.Remove(settler);
            SetUnitIndex(true, false);

            return city;
        }

        /// <summary>
        /// Checks if a <see cref="CityPivot"/> can be built at the <see cref="CurrentUnit"/> location.
        /// <see cref="CurrentUnit"/> must be a <see cref="SettlerPivot"/>.
        /// </summary>
        /// <param name="isCityCallback">Callback method to check if a city, on any civilization, already exists at this location.</param>
        /// <returns><c>True</c> if a city can be build; <c>False</c> otherwise.</returns>
        internal bool CanBuildCity(Func<MapSquarePivot, bool> isCityCallback)
        {
            if (CurrentUnit?.Is<SettlerPivot>() != true)
            {
                return false;
            }

            var sq = CurrentUnit.MapSquareLocation;

            return sq?.Biome?.IsCityBuildable == true
                && !isCityCallback(sq)
                && sq.Pollution != true;
        }

        /// <summary>
        /// Sets the <see cref="_currentUnitIndex"/> to next moveable unit.
        /// Also changes <see cref="_previousUnitIndex"/> accordingly.
        /// </summary>
        /// <param name="currentJustBeenRemoved">
        /// <c>True</c> if the <see cref="UnitPivot"/> at the current index has been removed; <c>False</c> otherwise.
        /// </param>
        /// <param name="reset"><c>True</c> to reset the value at the first available unit.</param>
        internal void SetUnitIndex(bool currentJustBeenRemoved, bool reset)
        {
            if (reset)
            {
                _currentUnitIndex = _units.IndexOf(_units.FirstOrDefault(u => u.RemainingMoves > 0));
                _previousUnitIndex = -1;
            }
            else
            {
                _previousUnitIndex = _currentUnitIndex;

                for (int i = _currentUnitIndex + 1; i < _units.Count; i++)
                {
                    if (_units[i].RemainingMoves > 0)
                    {
                        _currentUnitIndex = i;
                        NextUnitEvent?.Invoke(this, new NextUnitEventArgs(true));
                        return;
                    }
                }
                for (int i = 0; i < _currentUnitIndex + (currentJustBeenRemoved ? 1 : 0); i++)
                {
                    if (_units.Count > i && _units[i].RemainingMoves > 0)
                    {
                        _currentUnitIndex = i;
                        NextUnitEvent?.Invoke(this, new NextUnitEventArgs(true));
                        return;
                    }
                }
                _currentUnitIndex = -1;
            }
            NextUnitEvent?.Invoke(this, new NextUnitEventArgs(_currentUnitIndex >= 0));
        }

        /// <summary>
        /// Proceeds to do every actions required by a move to the next turn.
        /// </summary>
        /// <param name="getAdjacentSquaresCallback">Callback method to get adjacent squares of a single <see cref="MapSquarePivot"/>.</param>
        /// <returns>A <see cref="TurnConsequencesPivot"/>.</returns>
        internal TurnConsequencesPivot NextTurn(Func<MapSquarePivot, IReadOnlyDictionary<DirectionPivot, MapSquarePivot>> getAdjacentSquaresCallback)
        {
            var citiesWithDoneProduction = new Dictionary<CityPivot, BuildablePivot>();

            foreach (var city in _cities)
            {
                var produced = city.UpdateStatus(getAdjacentSquaresCallback);
                if (produced != null)
                {
                    if (produced.Is<UnitPivot>())
                    {
                        _units.Add(produced as UnitPivot);
                        SetUnitIndex(false, false);
                    }
                    else if (!produced.Is<CapitalizationPivot>())
                    {
                        citiesWithDoneProduction.Add(city, produced);
                    }
                }
            }
            foreach (var u in _units)
            {
                u.Release();
            }

            CheckScienceAtNextTurn();
            CheckTreasureAtNextTurn();

            SetUnitIndex(false, true);

            _anarchyTurnsCount++;

            return new TurnConsequencesPivot
            {
                EndOfProduction = citiesWithDoneProduction,
                EndOfRevolution = RevolutionIsDone,
                EndOfAdvance = CurrentAdvance is null
            };
        }

        /// <summary>
        /// Tries to move the current unit.
        /// </summary>
        /// <param name="direction">The <see cref="DirectionPivot"/>; <c>Null</c> to skip unit turn without moving.</param>
        /// <param name="getMapSquareCallback">Callback method to get the <see cref="MapSquarePivot"/> at the new coordinates.</param>
        /// <returns><c>True</c> if success; <c>False</c> otherwise.</returns>
        internal bool MoveCurrentUnit(DirectionPivot? direction, Func<int, int, MapSquarePivot> getMapSquareCallback)
        {
            if (CurrentUnit == null)
            {
                return false;
            }

            if (direction == null)
            {
                CurrentUnit.ForceNoMove();
                SetUnitIndex(false, false);
                return true;
            }

            var prevSq = CurrentUnit.MapSquareLocation;

            var x = direction.Value.Row(prevSq.Row);
            var y = direction.Value.Column(prevSq.Column);

            var square = getMapSquareCallback(x, y);
            if (square == null)
            {
                return false;
            }

            bool isCity = _cities.Any(c => c.MapSquareLocation == square);

            var res = CurrentUnit.Move(direction.Value, isCity, prevSq, square);
            if (res && CurrentUnit.RemainingMoves == 0)
            {
                SetUnitIndex(false, false);
            }
            return res;
        }

        /// <summary>
        /// Sets a new <see cref="RegimePivot"/>.
        /// </summary>
        /// <param name="regimePivot">The new <see cref="RegimePivot"/>.</param>
        internal void ChangeCurrentRegime(RegimePivot regimePivot)
        {
            CurrentRegime = regimePivot;
            _anarchyTurnsCount = 0;
            NewRegimeEvent?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Tries to trigger a <see cref="WorkerActionPivot"/> for the <see cref="CurrentUnit"/>.
        /// <see cref="CurrentUnit"/> must be a worker.
        /// </summary>
        /// <param name="actionPivot">The <see cref="WorkerActionPivot"/>.</param>
        /// <param name="isCityCallback">Callback method to check if a city, on any civilization, already exists at this location.</param>
        /// <param name="getAdjacentSquaresCallback">Callback method to get adjacent squares of a single <see cref="MapSquarePivot"/>.</param>
        /// <returns><c>True</c> if success; <c>False</c> otherwise.</returns>
        internal bool WorkerAction(WorkerActionPivot actionPivot, Func<MapSquarePivot, bool> isCityCallback,
            Func<MapSquarePivot, IReadOnlyDictionary<DirectionPivot, MapSquarePivot>> getAdjacentSquaresCallback)
        {
            if (CurrentUnit == null || !CurrentUnit.Is<WorkerPivot>())
            {
                return false;
            }

            var worker = CurrentUnit as WorkerPivot;
            var sq = worker.MapSquareLocation;
            if (sq == null || isCityCallback(sq))
            {
                return false;
            }

            if (actionPivot == WorkerActionPivot.RailRoad && !sq.Road)
            {
                actionPivot = WorkerActionPivot.Road;
            }

            if (actionPivot.AdvancePrerequisite != null && !_advances.Contains(actionPivot.AdvancePrerequisite))
            {
                return false;
            }
            
            if (actionPivot == WorkerActionPivot.Road
                && !_advances.Contains(AdvancePivot.BridgeBuilding)
                && sq.HasRiver(getAdjacentSquaresCallback(sq)))
            {
                return false;
            }
            
            if (actionPivot == WorkerActionPivot.Irrigate
                && !_advances.Contains(AdvancePivot.Electricity)
                && !sq.IsIrrigable(getAdjacentSquaresCallback(sq)))
            {
                return false;
            }

            var result = sq.ApplyAction(worker, actionPivot);
            if (result)
            {
                worker.ForceNoMove();
                SetUnitIndex(false, false);
            }
            return result;
        }

        /// <summary>
        /// Gets, for a specified <see cref="CityPivot"/>, the list of <see cref="BuildablePivot"/> which can be built.
        /// </summary>
        /// <param name="city">The <see cref="CityPivot"/>.</param>
        /// <param name="indexOfDefault">Out; the index, in the result list, of the city current production.</param>
        /// <param name="getAdjacentSquaresCallback">Callback method to get adjacent squares of a single <see cref="MapSquarePivot"/>.</param>
        /// <returns>List of <see cref="BuildablePivot"/> the city can build.</returns>
        internal IReadOnlyCollection<BuildablePivot> GetBuildableItemsForCity(CityPivot city, out int indexOfDefault,
            Func<MapSquarePivot, IReadOnlyDictionary<DirectionPivot, MapSquarePivot>> getAdjacentSquaresCallback)
        {
            indexOfDefault = -1;

            var buildableDefaultInstances = BuildablePivot.GetEveryDefaultInstances.ToList();
            if (buildableDefaultInstances == null)
            {
                return null;
            }

            // Scientific requirement not achieved yet.
            buildableDefaultInstances.RemoveAll(b => b.AdvancePrerequisite != null && !_advances.Contains(b.AdvancePrerequisite));

            // Already built for the current city.
            buildableDefaultInstances.RemoveAll(b => city.Improvements.Contains(b));

            // No aqueduc required if a river is close to the city.
            if (city.MapSquareLocation.HasRiver(getAdjacentSquaresCallback(city.MapSquareLocation)))
            {
                buildableDefaultInstances.RemoveAll(b => CityImprovementPivot.Aqueduc == b);
            }

            indexOfDefault = buildableDefaultInstances.FindIndex(b =>
                b.GetType() == city.Production.GetType() && b.Name == city.Production.Name);
            return buildableDefaultInstances;
        }

        #endregion

        #region Private methods

        private void CheckScienceAtNextTurn()
        {
            if (CurrentAdvance != null)
            {
                ScienceStack += ScienceByTurn;
                if (ScienceStack >= AdvancePivot.SCIENCE_COST)
                {
                    _advances.Add(CurrentAdvance);
                    ScienceStack = 0;
                    CurrentAdvance = null;
                    NewAdvanceEvent?.Invoke(this, new EventArgs());
                }
            }
        }

        private void CheckTreasureAtNextTurn()
        {
            Treasure += TreasureByTurn;
            if (Treasure < 0)
            {
                // TODO
                throw new NotImplementedException();
            }
        }

        #endregion

        #region IEquatable implementation

        /// <summary>
        /// Checks if this instance is equal to another one.
        /// </summary>
        /// <param name="other">The other instance.</param>
        /// <returns><c>True</c> if equals; <c>False</c> otherwise.</returns>
        public bool Equals(PlayerPivot other)
        {
            return Civilization == other?.Civilization;
        }

        /// <summary>
        /// Operator "==" override. Checks equality between two instances.
        /// </summary>
        /// <param name="p1">The first <see cref="PlayerPivot"/>.</param>
        /// <param name="p2">The second <see cref="PlayerPivot"/>.</param>
        /// <returns><c>True</c> if equals; <c>False</c> otherwise.</returns>
        public static bool operator ==(PlayerPivot p1, PlayerPivot p2)
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
        /// <param name="p1">The first <see cref="PlayerPivot"/>.</param>
        /// <param name="p2">The second <see cref="PlayerPivot"/>.</param>
        /// <returns><c>False</c> if equals; <c>True</c> otherwise.</returns>
        public static bool operator !=(PlayerPivot p1, PlayerPivot p2)
        {
            return !(p1 == p2);
        }

        /// <inhrritdoc />
        public override bool Equals(object obj)
        {
            return obj is PlayerPivot && Equals(obj as PlayerPivot);
        }

        /// <inhrritdoc />
        public override int GetHashCode()
        {
            return Civilization.GetHashCode();
        }

        #endregion
    }
}
