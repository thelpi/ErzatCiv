using System;
using System.Collections.Generic;
using System.Linq;
using ErsatzCivLib.Model.Enums;
using ErsatzCivLib.Model.Events;
using ErsatzCivLib.Model.Static;
using ErsatzCivLib.Model.Units.Air;
using ErsatzCivLib.Model.Units.Land;

namespace ErsatzCivLib.Model
{
    /// <summary>
    /// Represents a player in-game.
    /// </summary>
    /// <seealso cref="IEquatable{T}"/>
    [Serializable]
    public class PlayerPivot : IEquatable<PlayerPivot>
    {
        private const double REVOLUTION_TURNS_BY_CITY = 0.5;
        private const int TREASURE_START = 50;
        private const int SCIENCE_COST = 100;
        private const double DEFAULT_LUXURY_RATE = 0;
        private const double DEFAULT_SCIENCE_RATE = 0.5;
        private const int MAX_TURNS = 9999;

        #region Embedded properties

        private int _anarchyTurnsCount;
        private readonly EnginePivot _engine;
        private bool _pendingOrderAttack;
        private bool _orderAttack;

        private int _currentUnitIndex;
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

        private int _previousUnitIndex;
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
        /// The current <see cref="RegimePivot"/>.
        /// </summary>
        public RegimePivot Regime { get; private set; }
        /// <summary>
        /// The player civilization.
        /// </summary>
        public CivilizationPivot Civilization { get; private set; }
        /// <summary>
        /// Indicates if the city's name should be randomly generated.
        /// </summary>
        public bool RandomCityNames { get; private set; }
        /// <summary>
        /// Indicates the player gender. <c>True</c> for man, <c>False</c> for woman.
        /// </summary>
        public bool Gender { get; private set; }
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
        /// Current gold amount in the treasure.
        /// </summary>
        public int Treasure { get; private set; }
        /// <summary>
        /// Current capital.
        /// </summary>
        public CityPivot Capital { get; private set; }
        /// <summary>
        /// Science rate; between <c>0</c> and <c>1</c>.
        /// </summary>
        /// <remarks>This value plus <see cref="LuxuryRate"/> can't exceed <c>1</c>.</remarks>
        public double ScienceRate { get; private set; }
        /// <summary>
        /// Luxury rate; between <c>0</c> and <c>1</c>.
        /// </summary>
        /// <remarks>This value plus <see cref="ScienceRate"/> can't exceed <c>1</c>.</remarks>
        public double LuxuryRate { get; private set; }

        private readonly List<MapSquarePivot> _knownMapSquares = new List<MapSquarePivot>();
        /// <summary>
        /// Collection of <see cref="MapSquarePivot"/> discovered by the player / civilization.
        /// </summary>
        public IReadOnlyCollection<MapSquarePivot> KnownMapSquares { get { return _knownMapSquares; } }

        private readonly List<UnitPivot> _units = new List<UnitPivot>();
        /// <summary>
        /// Collection of <see cref="UnitPivot"/>.
        /// </summary>
        public IReadOnlyCollection<UnitPivot> Units { get { return _units; } }

        private readonly List<CityPivot> _cities = new List<CityPivot>();
        /// <summary>
        /// List of <see cref="CityPivot"/> built by the player.
        /// </summary>
        public IReadOnlyCollection<CityPivot> Cities { get { return _cities; } }

        private readonly List<AdvancePivot> _advances = new List<AdvancePivot>();
        /// <summary>
        /// List of discovered <see cref="AdvancePivot"/>.
        /// </summary>
        public IReadOnlyCollection<AdvancePivot> Advances { get { return _advances; } }

        private readonly List<PlayerPivot> _enemies = new List<PlayerPivot>();
        /// <summary>
        /// List of enemies <see cref="PlayerPivot"/>.
        /// </summary>
        public IReadOnlyCollection<PlayerPivot> Enemies { get { return _enemies; } }

        #endregion

        #region Inferred properties

        /// <summary>
        /// Indicates the name of the leader (you).
        /// </summary>
        public string LeaderName { get { return Gender ? Civilization.ManLeaderName : Civilization.WomanLeaderName; } }
        /// <summary>
        /// Tax rate; between <c>0</c> and <c>1</c>.
        /// </summary>
        public double TaxRate { get { return 1 - (ScienceRate + LuxuryRate); } }
        /// <summary>
        /// List of every <see cref="WonderPivot"/> for this player / civilization.
        /// </summary>
        public IReadOnlyCollection<WonderPivot> Wonders
        {
            get
            {
                return _cities.SelectMany(c => c.Wonders).ToList();
            }
        }
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
                if (Civilization.IsBarbarian)
                {
                    return MAX_TURNS;
                }

                return CurrentAdvance == null ? 0 : (
                    ScienceByTurn == 0 ? MAX_TURNS : (int)Math.Ceiling((SCIENCE_COST - ScienceStack) / (double)ScienceByTurn)
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
                return _cities.Sum(c => c.Tax);
            }
        }
        /// <summary>
        /// Number of turns left before the end of a revolution (if any).
        /// </summary>
        public int RevolutionTurnsCount
        {
            get
            {
                if (Civilization.IsBarbarian)
                {
                    return MAX_TURNS;
                }

                var anarchyTurns = ((_cities.Count * REVOLUTION_TURNS_BY_CITY) + 1);
                if (WonderIsActive(WonderPivot.Pyramids))
                {
                    anarchyTurns = 1;
                }
                var turnsLeft = anarchyTurns - _anarchyTurnsCount;
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
                if (Civilization.IsBarbarian)
                {
                    return false;
                }

                return Regime == RegimePivot.Anarchy && RevolutionTurnsCount == 0;
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
                return Civilization.NextCityName(RandomCityNames, _cities);
            }
        }
        /// <summary>
        /// Indicates the player is dead (no <see cref="CityPivot"/>, no <see cref="SettlerPivot"/>); barbarians can't die.
        /// </summary>
        public bool IsDead
        {
            get
            {
                return !Civilization.IsBarbarian && _cities.Count == 0 && _units.Count(u => u.Is<SettlerPivot>()) == 0;
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
        /// Triggered when <see cref="Regime"/> changes.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler<EventArgs> NewRegimeEvent;
        /// <summary>
        /// Triggered when <see cref="CurrentAdvance"/> changes.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler<EventArgs> NewAdvanceEvent;
        /// <summary>
        /// Triggered when new <see cref="MapSquarePivot"/> is discovered.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler<DiscoverNewSquareEventArgs> DiscoverNewSquareEvent;
        /// <summary>
        /// Triggered when <see cref="WonderPivot.GreatLibrary"/> finds an advance or when <see cref="WonderPivot.DarwinVoyage"/> is built.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler<ForcedAdvanceEventArgs> ForcedAdvanceEvent;
        /// <summary>
        /// Triggered when an <see cref="HutPivot"/> is discovered.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler<DiscoverHutEventArgs> DiscoverHutEvent;
        /// <summary>
        /// Triggered when an peaceful <see cref="PlayerPivot"/> is attacked.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler<AttackInPeaceEventArgs> AttackInPeaceEvent;
        /// <summary>
        /// Triggered when an opponent <see cref="PlayerPivot"/> is defeated.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler<DeadPlayerEventArgs> DeadPlayerEvent;
        
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="owner">The <see cref="EnginePivot"/> related to this instance.</param>
        /// <param name="civilization">The <see cref="Civilization"/> value.</param>
        /// <param name="isIa">The <see cref="IsIA"/> value.</param>
        /// <param name="beginLocation">Units position at the beginning.</param>
        /// <param name="gender">The <see cref="Gender"/> value.</param>
        /// <param name="randomCityNames">The <see cref="RandomCityNames"/> value.</param>
        internal PlayerPivot(EnginePivot owner, CivilizationPivot civilization, bool isIa, MapSquarePivot beginLocation, bool gender, bool randomCityNames)
        {
            _engine = owner;

            Gender = gender;
            RandomCityNames = randomCityNames;
            Civilization = civilization;
            IsIA = isIa;

            if (!Civilization.IsBarbarian)
            {
                foreach (var advance in civilization.Advances)
                {
                    AddAdvance(advance);
                }

                Regime = RegimePivot.Despotism;
                Treasure = TREASURE_START;

                ScienceRate = DEFAULT_SCIENCE_RATE;
                LuxuryRate = DEFAULT_LUXURY_RATE;

                _units.Add(SettlerPivot.CreateAtLocation(null, beginLocation, this));
                _units.Add(SettlerPivot.CreateAtLocation(null, beginLocation, this));

                MapSquareDiscoveryInvokator(beginLocation, _engine.Map.GetAdjacentMapSquares(beginLocation, 2));
            }
            else
            {
                Regime = RegimePivot.Anarchy;
                Treasure = 0;

                ScienceRate = 0;
                LuxuryRate = 1;

                // TODO :add random units ?
            }

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
                    .Where(r => r.AdvancePrerequisite is null || _advances.Contains(r.AdvancePrerequisite) || WonderIsActive(WonderPivot.Pyramids))
                    .ToList();
        }

        #endregion

        #region Internal methods

        /// <summary>
        /// Sets the pending <see cref="_orderAttack"/> value.
        /// </summary>
        /// <param name="orderAttack"><c>True</c> to attack; <c>False</c> to cancel.</param>
        internal void SetPendingAttackResponse(bool orderAttack)
        {
            _orderAttack = orderAttack;
            _pendingOrderAttack = false;
        }

        /// <summary>
        /// Switches the peace status with another <see cref="PlayerPivot"/>.
        /// Also switches the status for <paramref name="opponent"/>.
        /// If status on both sides are inconsistent, only the current instance is switched.
        /// Status with <see cref="CivilizationPivot.Barbarian"/> is never peace (no switch from war).
        /// </summary>
        /// <param name="opponent">The opponent.</param>
        internal void SwitchPeaceStatusWithOpponent(PlayerPivot opponent)
        {
            if (Civilization.IsBarbarian || opponent.Civilization.IsBarbarian)
            {
                // Forces war.
                if (!_enemies.Contains(opponent))
                {
                    _enemies.Add(opponent);
                }
                if (!opponent._enemies.Contains(this))
                {
                    opponent._enemies.Add(this);
                }
            }
            else
            {
                if (!_enemies.Contains(opponent))
                {
                    _enemies.Add(opponent);
                    if (!opponent._enemies.Contains(this))
                    {
                        opponent._enemies.Add(this);
                    }
                }
                else
                {
                    _enemies.Remove(opponent);
                    if (opponent._enemies.Contains(this))
                    {
                        opponent._enemies.Remove(this);
                    }
                }
            }
        }

        /// <summary>
        /// Generates an horde of barbarians on the map, if the current instance is <see cref="CivilizationPivot.Barbarian"/>.
        /// </summary>
        /// <param name="squares">List of <see cref="MapSquarePivot"/> where units can land.</param>
        internal void CreateHordeOfBarbarians(List<MapSquarePivot> squares)
        {
            if (!Civilization.IsBarbarian)
            {
                return;
            }

            var countUnit = Tools.Randomizer.Next(1, HutPivot.MAX_BARBARIANS_COUNT + 1);

            var barbarians = new List<UnitPivot>();
            for (var i = 0; i < countUnit; i++)
            {
                var unitTemplate = HutPivot.POSSIBLE_UNIT_TYPES.ElementAt(Tools.Randomizer.Next(0, HutPivot.POSSIBLE_UNIT_TYPES.Count));
                barbarians.Add(unitTemplate.CreateInstance(null, squares[Tools.Randomizer.Next(0, squares.Count)], this));
            }

            // Always adds a diplomat.
            barbarians.Add(DiplomatPivot.CreateAtLocation(null, squares[Tools.Randomizer.Next(0, squares.Count)], this));

            _units.AddRange(barbarians);
        }

        /// <summary>
        /// Changes the <see cref="LuxuryRate"/> and <see cref="ScienceRate"/> values.
        /// </summary>
        /// <param name="luxuryRate">The <see cref="LuxuryRate"/> value.</param>
        /// <param name="scienceRate">The <see cref="ScienceRate"/> value.</param>
        internal void ChangeRates(double luxuryRate, double scienceRate)
        {
            if (Civilization.IsBarbarian)
            {
                return;
            }

            LuxuryRate = luxuryRate;
            ScienceRate = scienceRate;
        }

        /// <summary>
        /// Triggrs a revolution; sets <see cref="Regime"/> to <see cref="RegimePivot.Anarchy"/> for a while.
        /// </summary>
        internal void TriggerRevolution()
        {
            if (Civilization.IsBarbarian)
            {
                return;
            }

            if (Regime != RegimePivot.Anarchy)
            {
                Regime = RegimePivot.Anarchy;
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
            if (Civilization.IsBarbarian)
            {
                return false;
            }

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
        /// <param name="currentTurn">The <see cref="EnginePivot.CurrentTurn"/> value.</param>
        /// <param name="name">Name of the city.</param>
        /// <param name="notUniqueNameError">Out; indicates a failure caused by a non-unique city name.</param>
        /// <returns>The <see cref="CityPivot"/> built; <c>Null</c> if failure.</returns>
        internal CityPivot BuildCity(int currentTurn, string name, out bool notUniqueNameError)
        {
            notUniqueNameError = false;

            if (Civilization.IsBarbarian)
            {
                return null;
            }

            if (!CanBuildCity())
            {
                return null;
            }

            if (_engine.NotBarbarianPlayers.Any(p => p._cities.Any(c => c.Name.Equals(name.ToLower(), StringComparison.InvariantCultureIgnoreCase))))
            {
                notUniqueNameError = true;
                return null;
            }

            var settler = CurrentUnit as SettlerPivot;
            var sq = CurrentUnit.MapSquareLocation;

            // The city is built on a "CityAreaMapSquarePivot" of another city
            var cityToReset = _cities.SingleOrDefault(c => c.AreaWithoutCityMapSquares.Any(ams => ams.MapSquare == sq));

            var city = new CityPivot(this, currentTurn, name, sq, null);

            if (Capital is null)
            {
                city.SetAsNewCapital(Capital);
                Capital = city;
            }

            if (cityToReset != null)
            {
                cityToReset.ResetCitizens();
            }

            MapSquareDiscoveryInvokator(city.MapSquareLocation, _engine.GetMapSquaresAroundCity(city).Keys);

            _cities.Add(city);
            _units.Remove(settler);
            SetUnitIndex(true, false);

            return city;
        }

        /// <summary>
        /// Gets, for a specified <see cref="CityPivot"/>, the list of available <see cref="MapSquarePivot"/>.
        /// </summary>
        /// <param name="city">The city.</param>
        /// <returns>List of <see cref="MapSquarePivot"/>.</returns>
        internal IReadOnlyCollection<MapSquarePivot> ComputeCityAvailableMapSquares(CityPivot city)
        {
            return _engine.ComputeCityAvailableMapSquares(city);
        }

        /// <summary>
        /// Checks of a <see cref="CityPivot"/> is near the coast.
        /// </summary>
        /// <param name="city">The city to check.</param>
        /// <returns><c>True</c> if near the coast; <c>False otherwise.</c></returns>
        internal bool GetCityIsCoast(CityPivot city)
        {
            return _engine.GetCityIsCoast(city);
        }

        /// <summary>
        /// Computes a rate which represents hte distance between a city and the capital.
        /// <c>0</c> is the closest (the capital itself) and <c>1</c> the furthest (distance between the corner of the map and the center; beyond that distance the value stays <c>1</c>).
        /// </summary>
        /// <param name="city">The city.</param>
        /// <returns>A distance rate.</returns>
        internal double GetDistanceToCapitalRate(CityPivot city)
        {
            var distanceBetweenCityAndCapital = Tools.DistanceBetweenTwoPoints(city.MapSquareLocation, Capital.MapSquareLocation);

            return distanceBetweenCityAndCapital > _engine.Map.DiagonalRadius ? 1 : (distanceBetweenCityAndCapital / _engine.Map.DiagonalRadius);
        }

        /// <summary>
        /// Checks if a <see cref="CityPivot"/> can be built at the <see cref="CurrentUnit"/> location.
        /// <see cref="CurrentUnit"/> must be a <see cref="SettlerPivot"/>.
        /// </summary>
        /// <returns><c>True</c> if a city can be build; <c>False</c> otherwise.</returns>
        internal bool CanBuildCity()
        {
            if (Civilization.IsBarbarian || CurrentUnit?.Is<SettlerPivot>() != true)
            {
                return false;
            }

            var sq = CurrentUnit.MapSquareLocation;

            return !sq.Biome.IsSeaType
                && !_engine.IsCity(sq)
                && !sq.Pollution;
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
        /// <returns>A <see cref="TurnConsequencesPivot"/>.</returns>
        internal TurnConsequencesPivot NextTurn()
        {
            var citiesWithDoneProduction = new Dictionary<CityPivot, BuildablePivot>();

            foreach (var u in _units)
            {
                u.Release();
            }

            if (!Civilization.IsBarbarian)
            {
                foreach (var city in _cities)
                {
                    var production = TreatCityAtTheEndOfTurn(city);
                    if (production != null)
                    {
                        citiesWithDoneProduction.Add(city, production);
                    }
                }

                CheckScienceAtNextTurn(SCIENCE_COST);
                CheckTreasureAtNextTurn();
            }

            SetUnitIndex(false, true);

            _anarchyTurnsCount++;

            if (WonderIsActive(WonderPivot.GreatLibrary))
            {
                var advancesMoreThanOne =
                    _engine.NotBarbarianPlayers
                        .Where(p => p != this)
                        .SelectMany(p => p.Advances)
                        .GroupBy(a => a)
                        .Where(a => a.Count() > 1)
                        .Select(a => a.Key)
                        .Where(a => !_advances.Contains(a))
                        .ToList();
                foreach (var a in advancesMoreThanOne)
                {
                    FinishForcedAdvance(a);
                }
            }

            return new TurnConsequencesPivot(RevolutionIsDone, citiesWithDoneProduction,
                CurrentAdvance is null ? _advances.LastOrDefault() : null, _advances.Count > Civilization.Advances.Count);
        }

        /// <summary>
        /// Tries to move the current unit.
        /// </summary>
        /// <param name="direction">The <see cref="DirectionPivot"/>; <c>Null</c> to skip unit turn without moving.</param>
        /// <returns><c>True</c> if success; <c>False</c> otherwise.</returns>
        internal bool MoveCurrentUnit(DirectionPivot? direction)
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

            if (CurrentUnit.RemainingMoves == 0)
            {
                return false;
            }

            var sourceSquare = CurrentUnit.MapSquareLocation;

            var targetSquare = _engine.Map[direction.Value.Row(sourceSquare.Row), direction.Value.Column(sourceSquare.Column)];
            if (targetSquare == null)
            {
                return false;
            }

            // TODO : the code below makes impossible attacks on coast by sea units !

            // A sea unit can't navigate on land.
            // A land unit can't navigate on sea.
            // An air unit can navigate on both.
            // Each unit can navigate on city.
            bool canNavigateToTarget = _cities.Any(c => c.MapSquareLocation == targetSquare)
                || (targetSquare.Biome.IsSeaType && CurrentUnit.Is<SeaUnitPivot>())
                || (!targetSquare.Biome.IsSeaType && CurrentUnit.Is<LandUnitPivot>());

            if (!canNavigateToTarget)
            {
                return false;
            }

            // TODO :
            // Sea units attack others sea units, or coast if "CanAttackCoastUnit" is true (in that case, even if successful attack, the unit can't move to the new location)
            // Nobody except fighters can attack air units
            // Squares exploited by opponent should be impossible to cross without breaking the peace.

            // One player only.
            var unitsAttacked = _engine.Players.Where(p => p != this).SelectMany(p => p.Units).Where(u => u.MapSquareLocation == targetSquare).ToList();
            // One city only.
            var cityAttacked = _engine.Players.Where(p => p != this).SelectMany(p => p.Cities).Where(c => c.MapSquareLocation == targetSquare).SingleOrDefault();

            // True if the unit is killed in the process.
            var isWalkingDead = false;
            // True if the unit attacks a city but doesn't take it (but doesn't die either).
            var noRealMove = false;

            if (cityAttacked != null)
            {
                // Non-military units can attack city without any unit inside
                if (unitsAttacked.Any() && !CurrentUnit.IsMilitary)
                {
                    return false;
                }

                var opponent = cityAttacked.Player;
                if (_enemies.Contains(opponent))
                {
                    if (unitsAttacked.Any())
                    {
                        AttackUnitMove(unitsAttacked, cityAttacked, out isWalkingDead);
                        if (!isWalkingDead)
                        {
                            noRealMove = true;
                        }
                    }
                    else
                    {
                        CaptureCity(cityAttacked);
                    }
                }
                else
                {
                    _pendingOrderAttack = true;
                    AttackInPeaceEvent?.Invoke(this, new AttackInPeaceEventArgs(opponent));
                    while (_pendingOrderAttack) { }
                    if (_orderAttack)
                    {
                        SwitchPeaceStatusWithOpponent(opponent);
                        if (unitsAttacked.Any())
                        {
                            AttackUnitMove(unitsAttacked, cityAttacked, out isWalkingDead);
                            if (!isWalkingDead)
                            {
                                noRealMove = true;
                            }
                        }
                        else
                        {
                            CaptureCity(cityAttacked);
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else if (unitsAttacked.Any())
            {
                if (!CurrentUnit.IsMilitary)
                {
                    return false;
                }

                var opponent = unitsAttacked.First().Player;
                if (_enemies.Contains(opponent))
                {
                    AttackUnitMove(unitsAttacked, null, out isWalkingDead);
                }
                else
                {
                    _pendingOrderAttack = true;
                    AttackInPeaceEvent?.Invoke(this, new AttackInPeaceEventArgs(opponent));
                    while (_pendingOrderAttack) { }
                    if (_orderAttack)
                    {
                        SwitchPeaceStatusWithOpponent(opponent);
                        AttackUnitMove(unitsAttacked, null, out isWalkingDead);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else if (!CurrentUnit.IgnoreControlZone && IsOpponentControlZone(sourceSquare, targetSquare))
            {
                return false;
            }

            if (!isWalkingDead && !noRealMove)
            {
                CurrentUnit.Move(direction.Value, sourceSquare, targetSquare);

                MapSquareDiscoveryInvokator(targetSquare, _engine.Map.GetAdjacentMapSquares(targetSquare, CurrentUnit.SquareSight));

                var hut = _engine.Map.Huts.SingleOrDefault(h => h.MapSquareLocation == targetSquare);
                if (hut != null && !Civilization.IsBarbarian)
                {
                    DiscoverHut(targetSquare, hut);
                }
            }

            // The last thing to do.
            if (CurrentUnit.RemainingMoves == 0 || isWalkingDead)
            {
                if (isWalkingDead)
                {
                    _units.Remove(CurrentUnit);
                }
                SetUnitIndex(isWalkingDead, false);
            }

            return true;
        }

        /// <summary>
        /// Sets a new <see cref="RegimePivot"/>.
        /// </summary>
        /// <param name="regimePivot">The new <see cref="RegimePivot"/>.</param>
        internal void ChangeCurrentRegime(RegimePivot regimePivot)
        {
            if (Civilization.IsBarbarian)
            {
                return;
            }

            Regime = regimePivot;
            _anarchyTurnsCount = 0;
            CheckEveryCitiesHappiness();
            NewRegimeEvent?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Tries to trigger a <see cref="MapSquareImprovementPivot"/> for the <see cref="CurrentUnit"/>.
        /// <see cref="CurrentUnit"/> must be a settler.
        /// </summary>
        /// <param name="actionPivot">The <see cref="MapSquareImprovementPivot"/>.</param>
        /// <returns><c>True</c> if success; <c>False</c> otherwise.</returns>
        internal bool SettlerAction(MapSquareImprovementPivot actionPivot)
        {
            if (CurrentUnit == null || !CurrentUnit.Is<SettlerPivot>() || Civilization.IsBarbarian)
            {
                return false;
            }

            var settler = CurrentUnit as SettlerPivot;
            var sq = settler.MapSquareLocation;
            if (sq == null || _engine.IsCity(sq))
            {
                return false;
            }

            if (actionPivot == MapSquareImprovementPivot.RailRoad && !sq.Road)
            {
                actionPivot = MapSquareImprovementPivot.Road;
            }

            if (actionPivot.AdvancePrerequisite != null && !_advances.Contains(actionPivot.AdvancePrerequisite))
            {
                return false;
            }
            
            if (actionPivot == MapSquareImprovementPivot.Road
                && !_advances.Contains(AdvancePivot.BridgeBuilding)
                && sq.Biome == BiomePivot.River)
            {
                return false;
            }
            
            if (actionPivot == MapSquareImprovementPivot.Irrigate
                && sq.Biome != BiomePivot.River
                && !_engine.Map.GetAdjacentMapSquares(sq).Any(asq => asq.Irrigate || asq.Biome.IsSeaType || asq.Biome == BiomePivot.River))
            {
                return false;
            }

            var result = sq.ApplyAction(settler, actionPivot);
            if (result)
            {
                settler.ForceNoMove();
                SetUnitIndex(false, false);
            }
            return result;
        }

        /// <summary>
        /// Gets, for a specified <see cref="CityPivot"/>, the list of <see cref="BuildablePivot"/> which can be built.
        /// </summary>
        /// <param name="city">The <see cref="CityPivot"/>.</param>
        /// <param name="indexOfDefault">Out; the index, in the result list, of the city current production.</param>
        /// <returns>List of <see cref="BuildablePivot"/> the city can build.</returns>
        internal IReadOnlyCollection<BuildablePivot> GetBuildableItemsForCity(CityPivot city, out int indexOfDefault)
        {
            indexOfDefault = -1;

            if (Civilization.IsBarbarian)
            {
                return null;
            }

            var buildableDefaultInstances = BuildablePivot.GetEveryDefaultInstances.ToList();
            if (buildableDefaultInstances == null)
            {
                return null;
            }

            // Scientific requirement not achieved yet.
            buildableDefaultInstances.RemoveAll(b => b.AdvancePrerequisite != null && !_advances.Contains(b.AdvancePrerequisite));

            // Already built for the current city.
            buildableDefaultInstances.RemoveAll(b => city.Improvements.Contains(b));

            // Removes wonders already built globally.
            buildableDefaultInstances.RemoveAll(b => b.Is<WonderPivot>() && _engine.GetEveryWonders().Contains(b as WonderPivot));

            // Removes wonders in progress in another city.
            buildableDefaultInstances.RemoveAll(b => b.Is<WonderPivot>() && _cities.Where(c => c != city).Select(c => c.Production).Contains(b as WonderPivot));

            // Another improvement is required in the first place.
            buildableDefaultInstances.RemoveAll(b =>
                b.Is<CityImprovementPivot>()
                && (b as CityImprovementPivot).ImprovementPrerequisite != null
                && !city.Improvements.Contains((b as CityImprovementPivot).ImprovementPrerequisite));

            // Obsolete units.
            buildableDefaultInstances.RemoveAll(b =>
                b.Is<UnitPivot>()
                && b.AdvanceObsolescence != null
                && _advances.Contains(b.AdvanceObsolescence));

            // Impossible to build sea units if not close to the sea.
            if (!city.IsOnCoast)
            {
                buildableDefaultInstances.RemoveAll(b => b.Is<SeaUnitPivot>());
            }

            #region Special rules

            // No aqueduc required if a river is close to the city.
            /*if (city.MapSquareLocation.HasRiver)
            {
                buildableDefaultInstances.RemoveAll(b => CityImprovementPivot.Aqueduc == b);
            }*/

            // Courthouse not required for capital.
            if (city == Capital)
            {
                buildableDefaultInstances.Remove(CityImprovementPivot.Courthouse);
            }

            // Can't build factory if manufacturing plant exists.
            if (city.Improvements.Contains(CityImprovementPivot.MfgPlant))
            {
                buildableDefaultInstances.Remove(CityImprovementPivot.Factory);
            }

            // No hydroplant if no water.
            if (city.MapSquareLocation.Biome != BiomePivot.River
                && !_engine.Map.GetAdjacentMapSquares(city.MapSquareLocation).Any(msq => msq.Biome.IsSeaType || msq.Biome == BiomePivot.River))
            {
                buildableDefaultInstances.Remove(CityImprovementPivot.HydroPlant);
            }

            // Allows nuclear weapons globally.
            if (!_engine.GetEveryWonders().Contains(WonderPivot.ManhattanProject))
            {
                buildableDefaultInstances.Remove(NuclearPivot.Default);
            }

            // Allows spaceship items.
            if (!WonderIsActive(WonderPivot.ApolloProgram))
            {
                buildableDefaultInstances.RemoveAll(b => b.Is<SpaceShipPivot>());
            }

            #endregion
            
            if (city.Production != null)
            {
                indexOfDefault = buildableDefaultInstances.FindIndex(b =>
                    b.GetType() == city.Production.GetType() && b.Name == city.Production.Name);
            }

            return buildableDefaultInstances;
        }

        /// <summary>
        /// Checks if a <see cref="WonderPivot"/> is built and not obsolete; can also check the continent.
        /// </summary>
        /// <param name="wonder">The wonder to check.</param>
        /// <param name="location">Optionnal; the location to check (continent must be the same).</param>
        /// <returns><c>True</c> if active; <c>False</c> if obsolete.</returns>
        internal bool WonderIsActive(WonderPivot wonder, MapSquarePivot location = null)
        {
            var wonderCity = Cities.SingleOrDefault(c => c.Wonders.Contains(wonder));
            if (wonderCity == null)
            {
                return false;
            }
            if (wonder.AdvanceObsolescence != null && _advances.Contains(wonder.AdvanceObsolescence))
            {
                return false;
            }
            if (location != null && location.ContinentIndex != wonderCity.MapSquareLocation.ContinentIndex)
            {
                return false;
            }
            return true;
        }

        #endregion

        #region Private methods

        private void CaptureCity(CityPivot cityAttacked)
        {
            var formerPlayer = cityAttacked.Player;

            // TODO : fake compute.
            var treasureTransfert = formerPlayer.Treasure / formerPlayer.Cities.Count;
            Treasure += treasureTransfert;
            formerPlayer.Treasure -= treasureTransfert;

            formerPlayer._cities.Remove(cityAttacked);

            // Barbarians can't capture cities.
            if (cityAttacked.CitizensCount > 1 && !Civilization.IsBarbarian)
            {
                _cities.Add(cityAttacked);
                cityAttacked.CheckCitizensHappiness();
            }

            if (formerPlayer.IsDead)
            {
                _engine.RemoveOpponent(formerPlayer);
                DeadPlayerEvent?.Invoke(this, new DeadPlayerEventArgs(formerPlayer, this));
            }
        }

        private void AttackUnitMove(IReadOnlyCollection<UnitPivot> units, CityPivot city, out bool dieInProcess)
        {
            dieInProcess = false;

            var unitsWithRealDefense = new List<Tuple<UnitPivot, double, bool>>();
            foreach (var unit in units)
            {
                unitsWithRealDefense.Add(new Tuple<UnitPivot, double, bool>(
                    unit,
                    unit.GetUnitDefense(city),
                    unit.City?.Improvements?.Contains(CityImprovementPivot.Barracks) == true
                ));
            }

            var defenseUnitConfiguration = unitsWithRealDefense.OrderByDescending(u => u.Item2).ThenByDescending(u => u.Item3 ? 1 : 0).First();
            var offenseUnitConfiguration = new Tuple<UnitPivot, double, bool>(
                CurrentUnit,
                CurrentUnit.OffensePoints,
                CurrentUnit.City?.Improvements?.Contains(CityImprovementPivot.Barracks) == true
            );

            if (offenseUnitConfiguration.Item3 && !defenseUnitConfiguration.Item3)
            {
                offenseUnitConfiguration = new Tuple<UnitPivot, double, bool>(
                    offenseUnitConfiguration.Item1,
                    offenseUnitConfiguration.Item2 + 1,
                    offenseUnitConfiguration.Item3
                );
            }
            else if (!offenseUnitConfiguration.Item3 && defenseUnitConfiguration.Item3)
            {
                defenseUnitConfiguration = new Tuple<UnitPivot, double, bool>(
                    defenseUnitConfiguration.Item1,
                    defenseUnitConfiguration.Item2 + 1,
                    defenseUnitConfiguration.Item3
                );
            }

            var randomAttackValue = Tools.Randomizer.NextDouble() * (defenseUnitConfiguration.Item2 + offenseUnitConfiguration.Item2);
            if (randomAttackValue > offenseUnitConfiguration.Item2)
            {
                // defense win
                dieInProcess = true;
            }
            else
            {
                // offense win

                var unit = defenseUnitConfiguration.Item1;
                var formerPlayer = unit.Player;

                formerPlayer._units.Remove(unit);
                unit.City?.CheckCitizensHappiness();

                // If inside the city, only one unit is killed; otherwise, every units of the square are killed.s
                if (city != null)
                {
                    city.RemoveFromGarrison(unit);
                }
                else
                {
                    foreach (var u in units)
                    {
                        if (u != unit)
                        {
                            formerPlayer._units.Remove(u);
                            u.City?.CheckCitizensHappiness();
                        }
                    }

                    // It might be the death of the opponent player.
                    if (formerPlayer.IsDead)
                    {
                        _engine.RemoveOpponent(formerPlayer);
                        DeadPlayerEvent?.Invoke(this, new DeadPlayerEventArgs(formerPlayer, this));
                    }
                }
            }

            // Note that the population is reduced even if the defense wins.
            if (city != null && city.CitizensCount > 1 && !city.Improvements.Contains(CityImprovementPivot.CityWalls))
            {
                city.RemoveAnyCitizen(true);
            }
        }

        private bool IsOpponentControlZone(MapSquarePivot sourceSquare, MapSquarePivot destinationSquare)
        {
            // This section assumes to things :
            // Cities don't matter per se : they have the control zone of unit(s) inside, if any.
            // Control zone applies only for opponent units of the same type [air / land / sea].

            var unitsOnControlZone = new List<UnitPivot>();
            foreach (var sq in _engine.Map.GetAdjacentMapSquares(sourceSquare))
            {
                var unitOnThisSquare = _engine.Players.Where(p => p != this).SelectMany(p => p.Units).Where(u => u.MapSquareLocation == sq && u.IsMilitary && u.IsSameType(CurrentUnit)).FirstOrDefault();
                if (unitOnThisSquare != null)
                {
                    unitsOnControlZone.Add(unitOnThisSquare);
                }
            }

            foreach (var currentUnitOnControlZone in unitsOnControlZone)
            {
                if (_engine.Map.GetAdjacentMapSquares(currentUnitOnControlZone.MapSquareLocation).Contains(destinationSquare))
                {
                    return true;
                }
            }

            return false;
        }

        private BuildablePivot TreatCityAtTheEndOfTurn(CityPivot city)
        {
            BuildablePivot prudctionReturned = null;

            var turnInfo = city.NextTurn();
            var produced = turnInfo.Item1;
            if (produced != null)
            {
                if (produced.Is<UnitPivot>())
                {
                    _units.Add(produced as UnitPivot);
                    SetUnitIndex(false, false);
                }
                else
                {
                    prudctionReturned = produced;
                }

                if (CityImprovementPivot.Palace == produced)
                {
                    city.SetAsNewCapital(Capital);
                    Capital = city;
                }
                else if (WonderPivot.HooverDam == produced)
                {
                    foreach (var contCity in Cities.Where(c =>
                        c.MapSquareLocation.ContinentIndex == city.MapSquareLocation.ContinentIndex))
                    {
                        // TODO : the HydroPlant maintenance cost should be zero in this case.
                        contCity.ForceCityImprovement(CityImprovementPivot.HydroPlant);
                    }
                }
                else if (WonderPivot.ApolloProgram == produced)
                {
                    foreach (var cityToShow in _engine.NotBarbarianPlayers.SelectMany(p => p.Cities))
                    {
                        MapSquareDiscoveryInvokator(cityToShow.MapSquareLocation, new List<MapSquarePivot>());
                    }
                }
                else if (WonderPivot.DarwinVoyage == produced)
                {
                    bool oneAdvanceWasReady = true;
                    if (CurrentAdvance != null)
                    {
                        CheckScienceAtNextTurn(0);
                        oneAdvanceWasReady = false;
                    }
                    ForcedAdvanceEvent?.Invoke(this, new ForcedAdvanceEventArgs(_advances.Last(), true));
                    while (CurrentAdvance == null) { }
                    CheckScienceAtNextTurn(0);
                    ForcedAdvanceEvent?.Invoke(this, new ForcedAdvanceEventArgs(_advances.Last(), true));
                    if (!oneAdvanceWasReady)
                    {
                        while (CurrentAdvance == null) { }
                        CheckScienceAtNextTurn(0);
                        ForcedAdvanceEvent?.Invoke(this, new ForcedAdvanceEventArgs(_advances.Last(), true));
                    }
                }
            }
            if (turnInfo.Item2)
            {
                _units.Remove(_units.First(u => u.Is<SettlerPivot>() && u.City == city));
            }

            return prudctionReturned;
        }

        private void CheckEveryCitiesHappiness()
        {
            foreach (var city in _cities)
            {
                city.CheckCitizensHappiness();
            }
        }

        private void AddAdvance(AdvancePivot advance)
        {
            _advances.Add(advance);
            if (advance == AdvancePivot.Mysticism || Wonders.Any(w => w.AdvanceObsolescence == advance && w.HasCitizenHappinessEffect))
            {
                CheckEveryCitiesHappiness();
            }
            foreach (var city in _cities.Where(c => c.Improvements.Any(i => i.HasCitizenHappinessEffect && i.AdvanceObsolescence == advance)))
            {
                city.CheckCitizensHappiness();
            }
            if (advance == AdvancePivot.Combustion || advance == AdvancePivot.Gunpowder)
            {
                foreach (var city in _cities)
                {
                    city.ForceRemoveCityImprovement(CityImprovementPivot.Barracks);
                }
            }
        }

        private void CheckScienceAtNextTurn(int scienceCost)
        {
            if (CurrentAdvance != null)
            {
                ScienceStack += ScienceByTurn;
                if (ScienceStack >= scienceCost)
                {
                    AddAdvance(CurrentAdvance);
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

        private void MapSquareDiscoveryInvokator(MapSquarePivot location, IEnumerable<MapSquarePivot> aroundLocation)
        {
            var squares = aroundLocation.Concat(new[] { location }).Where(msq => !_knownMapSquares.Contains(msq)).ToList();
            if (squares.Any())
            {
                _knownMapSquares.AddRange(squares);
                DiscoverNewSquareEvent?.Invoke(this, new DiscoverNewSquareEventArgs(squares));
            }
        }

        private void FinishForcedAdvance(AdvancePivot a)
        {
            AddAdvance(a);
            bool inProgress = false;
            if (CurrentAdvance == a)
            {
                ScienceStack = 0;
                CurrentAdvance = null;
                inProgress = true;
            }
            ForcedAdvanceEvent?.Invoke(this, new ForcedAdvanceEventArgs(a, inProgress));
        }

        private void DiscoverHut(MapSquarePivot square, HutPivot hut)
        {
            if (hut.IsGold)
            {
                Treasure += HutPivot.HUT_GOLD;
            }
            else if (hut.IsSettlerUnit)
            {
                if (_engine.CurrentYear < HutPivot.MAX_AGE_WITH_SETTLER)
                {
                    _units.Add(SettlerPivot.CreateAtLocation(null, square, this));
                }
                else
                {
                    hut.WasEmpty = true;
                }
            }
            else if (hut.IsBarbarians)
            {
                // Note that hut can becomes empty inside this function !
                _engine.AddBarbariansFromHut(hut);
            }
            else if (hut.IsAdvance)
            {
                // Takes the first from antique era.
                var advance = AdvancePivot
                    .AdvancesByEra[EraPivot.Antiquity]
                    .Where(a => !_advances.Contains(a) && (a.Prerequisites == null || a.Prerequisites.All(ap => _advances.Contains(ap))))
                    .OrderBy(a => Tools.Randomizer.NextDouble())
                    .FirstOrDefault();
                if (advance != null)
                {
                    FinishForcedAdvance(advance);
                }
                else
                {
                    hut.WasEmpty = true;
                }
            }
            else if (hut.IsFriendlyCavalryUnit)
            {
                _units.Add(CavalryPivot.CreateAtLocation(null, square, this));
            }
            else
            {
                hut.WasEmpty = true;
            }

            _engine.Map.RemoveHut(hut);
            DiscoverHutEvent(this, new DiscoverHutEventArgs(hut));
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
