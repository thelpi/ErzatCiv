using System;
using System.Collections.Generic;
using System.Linq;
using ErsatzCivLib.Model.Enums;
using ErsatzCivLib.Model.Static;
using ErsatzCivLib.Model.Units.Land;

namespace ErsatzCivLib.Model
{
    /// <summary>
    /// Represents a city.
    /// </summary>
    /// <seealso cref="IEquatable{T}"/>
    [Serializable]
    public class CityPivot : IEquatable<CityPivot>
    {
        #region Happiness constants

        // Number of luxuries required for one happy citizen.
        private const int LUXURY_TO_HAPPY_CITIZEN_RATE = 2;
        // Multiplication rate of mysticism on temple's effect.
        private const int MYSTICISM_ADVANCE_HAPPINESS_RATE = 2;
        // Number of citizens content before becoming unhappy.
        private const int CONTENT_CITIZENS_COUNT = 5;
        // Rate of unhappy citizens preserved.
        private const int SHAKESPEARE_THEATRE_HAPPINESS_RATE = 0;
        // Temple effect on citizens (count unhappy to cnotent).
        private const int TEMPLE_HAPPINESS_EFFECT = 1;
        // Colosseum effect on citizens (count unhappy to cnotent).
        private const int COLOSSEUM_HAPPINESS_EFFECT = 3;
        // Cathedral effect on citizens (count unhappy to cnotent).
        private const int CATHEDRAL_HAPPINESS_EFFECT = 4;
        // Cathedral effect boost by wonder Michelangelo's chapel.
        private const double MICHELANGELO_CHAPEL_HAPPINESS_EFFECT = 1.5;
        // Number of unhappy citizens transforms to content by wonder JS Bachs cathedral.
        private const int JSBACHS_WONDER_HAPPINESS_EFFECT = 2;
        // Number of citizens happy with wonder Hanging gardens.
        private const int CUREFORCANCER_WONDER_HAPPINESS_EFFECT = 1;
        // Number of citizens happy with wonder Hanging gardens.
        private const int HANGING_GARDENS_WONDER_HAPPINESS_EFFECT = 1;
        // Temple effect boost by wonder Oracle.
        private const int ORACLE_WONDER_HAPPINESS_RATE = 2;

        #endregion

        #region Constants relative to citizens behavior

        private const int FOOD_BY_CITIZEN_BY_TURN = 2;
        private const int FOOD_RATIO_TO_NEXT_CITIZEN = 20;
        // Increase on [tax / luxury / science] for one specialist.
        private const int SPECIALIST_EFFECT_RATE = 2;

        #endregion

        #region Constants relative to improvements effects

        private const double LIBRARY_SCIENCE_INCREASE_RATIO = 1.5;
        private const double UNIVERSITY_SCIENCE_INCREASE_RATIO = 1.5;
        private const double HYDROPLANT_POLLUTION_INCREASE_RATIO = 0.5;
        private const double NUCLEARPLANT_POLLUTION_INCREASE_RATIO = 0.5;
        private const double RECYCLINGCENTER_POLLUTION_INCREASE_RATIO = 1 / (double)3;
        private const double GRANARY_FOOD_INCREASE_RATIO = 0.5;
        private const double MASSTRANSIT_POLLUTION_INCREASE_RATIO = 0;
        private const double PALACE_CORRUPTION_INCREASE_RATE = 0.5;
        private const double COURTHOUSE_CORRUPTION_INCREASE_RATE = 0.5;
        private const int AQUEDUC_MAX_POPULATION_WITHOUT = 10;
        private const double MARKETPLACE_COMMERCE_INCREASE_RATIO = 1.5;
        private const double BANK_COMMERCE_INCREASE_RATIO = 1.5;

        #endregion

        #region Constants relative to wonders effects

        private const double SETI_PROGRAM_SCIENCE_INCREASE_RATIO = 1.5;
        private const double COPERNICUS_OBSERVATORY_SCIENCE_INCREASE_RATIO = 2;
        private const double ISAAC_NEWTON_COLLEGE_SCIENCE_INCREASE_RATE = 2 / (double)3;

        #endregion

        #region Constants relative to real population computing

        private const double MAX_CITIZEN_COUNT = 40;
        private const double MIN_CC_POP = 1000;
        private const double MAX_CC_POP = 20000000;
        private static readonly double POP_GROWTH_RATIO = Math.Log(MIN_CC_POP / MAX_CC_POP) / (1 - MAX_CITIZEN_COUNT);

        #endregion

        #region Embedded properties

        /// <summary>
        /// The <see cref="PlayerPivot"/> which owns the instance.
        /// </summary>
        public PlayerPivot Player { get; private set; }
        /// <summary>
        /// Current production.
        /// </summary>
        public BuildablePivot Production { get; private set; }
        /// <summary>
        /// Creation turn.
        /// </summary>
        public int CreationTurn { get; private set; }
        /// <summary>
        /// Food in store (outside granary).
        /// </summary>
        public int FoodStorage { get; private set; }
        /// <summary>
        /// Food in store (inside granary).
        /// </summary>
        public int FoodGranaryStorage { get; private set; }
        /// <summary>
        /// Productivity in store.
        /// </summary>
        public int ProductivityStorage { get; private set; }
        /// <summary>
        /// Name.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Location on the map.
        /// </summary>
        public MapSquarePivot MapSquareLocation { get; private set; }

        private List<UnitPivot> _garrison = new List<UnitPivot>();
        /// <summary>
        /// List of units in garrison.
        /// </summary>
        public IReadOnlyCollection<UnitPivot> Garrison { get { return _garrison; } }

        private readonly List<CitizenPivot> _specialistCitizens;
        /// <summary>
        /// List of specialists <see cref="CitizenPivot"/>.
        /// </summary>
        public IReadOnlyCollection<CitizenPivot> SpecialistCitizens { get { return _specialistCitizens; } }

        private readonly List<CityImprovementPivot> _improvements;
        /// <summary>
        /// List of <see cref="CityImprovementPivot"/>
        /// </summary>
        public IReadOnlyCollection<CityImprovementPivot> Improvements { get { return _improvements; } }

        private readonly List<WonderPivot> _wonders;
        /// <summary>
        /// List of <see cref="WonderPivot"/>.
        /// </summary>
        public IReadOnlyCollection<WonderPivot> Wonders { get { return _wonders; } }

        private readonly List<CityAreaMapSquarePivot> _areaMapSquares;
        /// <summary>
        /// List of <see cref="CityAreaMapSquarePivot"/>; includes city center.
        /// </summary>
        public IReadOnlyCollection<CityAreaMapSquarePivot> AreaMapSquares { get { return _areaMapSquares; } }

        #endregion

        #region Inferred properties

        private bool? _isOnCoast = null;
        /// <summary>
        /// Indicates if the city is near the coast.
        /// </summary>
        public bool IsOnCoast
        {
            get
            {
                if (!_isOnCoast.HasValue)
                {
                    _isOnCoast = Player.GetCityIsCoast(this);
                }
                return _isOnCoast.Value;
            }
        }
        /// <summary>
        /// List of <see cref="CityAreaMapSquarePivot"/> without the city center.
        /// </summary>
        public IReadOnlyCollection<CityAreaMapSquarePivot> AreaWithoutCityMapSquares
        {
            get
            {
                return AreaMapSquares.Where(ams => !ams.IsCityCenter).ToList();
            }
        }
        /// <summary>
        /// Number of citizens.
        /// </summary>
        public int CitizensCount
        {
            get
            {
                return Citizens.Count;
            }
        }
        /// <summary>
        /// List of every <see cref="CitizenPivot"/> (specialist or not).
        /// </summary>
        public IReadOnlyCollection<CitizenPivot> Citizens
        {
            get
            {
                return AreaWithoutCityMapSquares.Select(ams => ams.Citizen).Concat(_specialistCitizens).OrderBy(c => c).ToList();
            }
        }
        /// <summary>
        /// Indicates if the city is in civil trouble (too much citizens in <see cref="HappinessPivot.Unhappy"/> happiness).
        /// </summary>
        public bool InCivilTrouble
        {
            get
            {
                return AreaWithoutCityMapSquares.Count(ams => ams.Citizen.Happiness == HappinessPivot.Unhappy) >
                    AreaWithoutCityMapSquares.Count(ams => ams.Citizen.Happiness == HappinessPivot.Happy);
            }
        }
        /// <summary>
        /// Indicates if the city is <see cref="InCivilTrouble"/> or the current regime is <see cref="RegimePivot.Anarchy"/>.
        /// </summary>
        public bool InCivilTroubleOrAnarchy
        {
            get
            {
                return InCivilTrouble || Player.Regime == RegimePivot.Anarchy;
            }
        }
        /// <summary>
        /// Real population count.
        /// </summary>
        public int Population
        {
            get
            {
                return (int)Math.Round((MIN_CC_POP / Math.Exp(POP_GROWTH_RATIO)) * Math.Exp(POP_GROWTH_RATIO * CitizensCount));
            }
        }
        /// <summary>
        /// Commerce by turn.
        /// </summary>
        public int Commerce
        {
            get
            {
                if (InCivilTroubleOrAnarchy)
                {
                    return 0;
                }

                var baseValue = AreaMapSquares.Sum(ams => ams.Commerce);

                double corruptionRate = (Player.Regime.CorruptionRate * Player.GetDistanceToCapitalRate(this));

                if (_improvements.Contains(CityImprovementPivot.Palace))
                {
                    corruptionRate *= PALACE_CORRUPTION_INCREASE_RATE;
                }
                else if (_improvements.Contains(CityImprovementPivot.Courthouse))
                {
                    corruptionRate *= COURTHOUSE_CORRUPTION_INCREASE_RATE;
                }

                return (int)Math.Round(baseValue * (1 - corruptionRate));
            }
        }
        /// <summary>
        /// Food consumption by turn.
        /// </summary>
        public int FoodConsumption
        {
            get
            {
                return (CitizensCount * FOOD_BY_CITIZEN_BY_TURN) + (Player.Units.Count(u => u.Is<SettlerPivot>() && u.City == this) * Player.Regime.SettlerFoodConsumption);
            }
        }
        /// <summary>
        /// Food consumption by turn without settlers.
        /// </summary>
        public int FoodConsumptionWithoutSettlers
        {
            get
            {
                return CitizensCount * FOOD_BY_CITIZEN_BY_TURN;
            }
        }
        /// <summary>
        /// Food production by turn.
        /// </summary>
        public int Food
        {
            get
            {
                return AreaMapSquares.Sum(ams => Player.Regime.ProductionMalus && ams.Food > 2 ? (ams.Food - 1) : ams.Food);
            }
        }
        /// <summary>
        /// Productivity by turn.
        /// </summary>
        public int Productivity
        {
            get
            {
                if (InCivilTrouble)
                {
                    return 0;
                }

                var baseValue = AreaMapSquares.Sum(ams =>
                    Player.Regime.ProductionMalus && ams.Food > 2 ? (ams.Productivity - 1) : ams.Productivity);

                baseValue -= Player.Units.Where(u => u.City == this).Sum(u => u.MaintenanceCost);

                return baseValue;
            }
        }
        /// <summary>
        /// Science production by turn.
        /// </summary>
        public int Science
        {
            get
            {
                bool hasSetiProgram = Player.WonderIsActive(WonderPivot.SetiProgram);

                var scienceValue = (Commerce * Player.ScienceRate) + (Citizens.Count(c => c.Type == CitizenTypePivot.Scientist) * SPECIALIST_EFFECT_RATE);

                if (_improvements.Contains(CityImprovementPivot.Library))
                {
                    var scienceCoeff = LIBRARY_SCIENCE_INCREASE_RATIO;
                    if (Player.WonderIsActive(WonderPivot.IsaacNewtonCollege) && !hasSetiProgram)
                    {
                        scienceCoeff += ISAAC_NEWTON_COLLEGE_SCIENCE_INCREASE_RATE * scienceCoeff;
                    }
                    scienceValue *= scienceCoeff;
                }

                if (_improvements.Contains(CityImprovementPivot.University))
                {
                    var scienceCoeff = UNIVERSITY_SCIENCE_INCREASE_RATIO;
                    if (Player.WonderIsActive(WonderPivot.IsaacNewtonCollege) && !hasSetiProgram)
                    {
                        scienceCoeff += ISAAC_NEWTON_COLLEGE_SCIENCE_INCREASE_RATE * scienceCoeff;
                    }
                    scienceValue *= scienceCoeff;
                }

                if (Player.WonderIsActive(WonderPivot.CopernicusObservatory) && _wonders.Contains(WonderPivot.CopernicusObservatory))
                {
                    scienceValue *= COPERNICUS_OBSERVATORY_SCIENCE_INCREASE_RATIO;
                }

                if (hasSetiProgram)
                {
                    scienceValue *= SETI_PROGRAM_SCIENCE_INCREASE_RATIO;
                }

                return (int)Math.Floor(scienceValue);
            }
        }
        /// <summary>
        /// Luxury production by turn.
        /// </summary>
        public int Luxury
        {
            get
            {
                var baseValue = (Commerce * Player.LuxuryRate) + (Citizens.Count(c => c.Type == CitizenTypePivot.Entertainer) * SPECIALIST_EFFECT_RATE);

                if (Improvements.Contains(CityImprovementPivot.Marketplace))
                {
                    baseValue *= MARKETPLACE_COMMERCE_INCREASE_RATIO;
                }

                if (Improvements.Contains(CityImprovementPivot.Bank))
                {
                    baseValue *= BANK_COMMERCE_INCREASE_RATIO;
                }

                return (int)Math.Floor(baseValue);
            }
        }
        /// <summary>
        /// Tax production by turn.
        /// </summary>
        public int Tax
        {
            get
            {
                var baseValue = (Commerce * Player.TaxRate) + (Citizens.Count(c => c.Type == CitizenTypePivot.TaxCollector) * SPECIALIST_EFFECT_RATE);

                if (Improvements.Contains(CityImprovementPivot.Marketplace))
                {
                    baseValue *= MARKETPLACE_COMMERCE_INCREASE_RATIO;
                }

                if (Improvements.Contains(CityImprovementPivot.Bank))
                {
                    baseValue *= BANK_COMMERCE_INCREASE_RATIO;
                }

                var valueInt = (int)Math.Round(baseValue);

                valueInt -= _improvements.Sum(i => i.MaintenanceCost);

                return valueInt;
            }
        }
        /// <summary>
        /// Pollution value relative to industry.
        /// </summary>
        public int IndustrialPollution
        {
            get
            {
                var baseValue = 0; // TODO (based on current productivity)

                if (_improvements.Contains(CityImprovementPivot.HydroPlant))
                {
                    baseValue = (int)Math.Floor(HYDROPLANT_POLLUTION_INCREASE_RATIO * baseValue);
                }
                else if (_improvements.Contains(CityImprovementPivot.NuclearPlant))
                {
                    baseValue = (int)Math.Floor(NUCLEARPLANT_POLLUTION_INCREASE_RATIO * baseValue);
                }

                if (_improvements.Contains(CityImprovementPivot.RecyclingCenter))
                {
                    baseValue = (int)Math.Floor(RECYCLINGCENTER_POLLUTION_INCREASE_RATIO * baseValue);
                }

                return baseValue;
            }
        }
        /// <summary>
        /// Pollution value relative to citizens.
        /// </summary>
        public int CitizenPollution
        {
            get
            {
                var baseValue = 0; // TODO (based on current population)

                if (_improvements.Contains(CityImprovementPivot.MassTransit))
                {
                    baseValue = (int)Math.Floor(MASSTRANSIT_POLLUTION_INCREASE_RATIO * baseValue);
                }

                return baseValue;
            }
        }
        /// <summary>
        /// Pollution value overall (citizens + industry).
        /// </summary>
        public int Pollution
        {
            get
            {
                return IndustrialPollution + CitizenPollution;
            }
        }
        /// <summary>
        /// Productivity cost remaining for the current production.
        /// </summary>
        public int RemainingProductionCost
        {
            get
            {
                return Production == null ? 0 : Production.ProductivityCost - ProductivityStorage;
            }
        }
        /// <summary>
        /// Turns count before the current production is done.
        /// </summary>
        public int RemainingProductionTurns
        {
            get
            {
                return (Productivity == 0 ? 9999 :
                    (int)Math.Ceiling(RemainingProductionCost / (double)Productivity));
            }
        }
        /// <summary>
        /// Total food required to produce a new citizen.
        /// </summary>
        public int NextCitizenFoodRequirement
        {
            get
            {
                var baseValue = FOOD_RATIO_TO_NEXT_CITIZEN * CitizensCount;

                if (_improvements.Contains(CityImprovementPivot.Granary))
                {
                    baseValue = (int)Math.Ceiling(baseValue * GRANARY_FOOD_INCREASE_RATIO);
                }

                return baseValue;
            }
        }
        /// <summary>
        /// Extra food stored by turn.
        /// </summary>
        public int ExtraFoodByTurn
        {
            get
            {
                return Food - FoodConsumption;
            }
        }
        /// <summary>
        /// Turns count before the next citizen.
        /// </summary>
        public int NextCitizenTurns
        {
            get
            {
                return ExtraFoodByTurn == 0 ? 0 : (int)Math.Ceiling((NextCitizenFoodRequirement - FoodStorage) / (double)ExtraFoodByTurn);
            }
        }
        /// <summary>
        /// Text indicating the food status [growth / balanced / famine].
        /// </summary>
        public string FoodStatus
        {
            get
            {
                return ExtraFoodByTurn > 0 ? $"Growth ({NextCitizenTurns} turns)" :
                    (ExtraFoodByTurn == 0 ? "Balanced" : "Famine");
            }
        }

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> value.</param>
        /// <param name="currentTurn">The <see cref="CreationTurn"/> value.</param>
        /// <param name="name">The <see cref="Name"/> value.</param>
        /// <param name="location">The <see cref="MapSquareLocation"/> value.</param>
        /// <param name="production">The <see cref="Production"/> value.</param>
        internal CityPivot(PlayerPivot player, int currentTurn, string name, MapSquarePivot location, BuildablePivot production)
        {
            Player = player;
            Name = name;
            MapSquareLocation = location;
            CreationTurn = currentTurn;
            Production = production;

            _improvements = new List<CityImprovementPivot>();
            _wonders = new List<WonderPivot>();

            _specialistCitizens = new List<CitizenPivot>();
            _areaMapSquares = new List<CityAreaMapSquarePivot>
            {
                new CityAreaMapSquarePivot(this, MapSquareLocation)
            };
            _areaMapSquares.Add(new CityAreaMapSquarePivot(this, BestVacantMapSquareLocation()));

            location.ApplyCityActions();
        }

        #region Private methods

        private void RemovePreviousPlant(BuildablePivot produced)
        {
            if (CityImprovementPivot.HydroPlant == produced)
            {
                _improvements.Remove(CityImprovementPivot.NuclearPlant);
                _improvements.Remove(CityImprovementPivot.PowerPlant);
            }
            else if (CityImprovementPivot.NuclearPlant == produced)
            {
                _improvements.Remove(CityImprovementPivot.PowerPlant);
                _improvements.Remove(CityImprovementPivot.HydroPlant);
            }
            else if (CityImprovementPivot.PowerPlant == produced)
            {
                _improvements.Remove(CityImprovementPivot.NuclearPlant);
                _improvements.Remove(CityImprovementPivot.HydroPlant);
            }
        }

        private void ChangeCitizenToSpecialist(CitizenPivot citizenSource, CitizenTypePivot citizenType)
        {
            bool wasSpecialistAlready = _specialistCitizens.Contains(citizenSource);
            citizenSource.ToSpecialist(citizenType);
            if (!wasSpecialistAlready)
            {
                _areaMapSquares.Remove(_areaMapSquares.SingleOrDefault(ams => ams.Citizen == citizenSource));
                _specialistCitizens.Add(citizenSource);
            }
        }

        private bool ChangeCitizenToRegularAtTheBestVacantMapSquare(CitizenPivot citizenSource)
        {
            var mapSquare = BestVacantMapSquareLocation();
            if (mapSquare != null)
            {
                AddRegularCitizen(citizenSource, mapSquare);
                return true;
            }
            return false;
        }

        private void AddRegularCitizen(CitizenPivot citizenSource, MapSquarePivot location, bool delayHappinessCheck = false)
        {
            var alreadyInArea = _areaMapSquares.SingleOrDefault(ams => ams.Citizen == citizenSource);
            if (alreadyInArea != null)
            {
                _areaMapSquares.Remove(alreadyInArea);
            }
            else
            {
                _specialistCitizens.Remove(citizenSource);
            }
            citizenSource.ToRegular();
            _areaMapSquares.Add(new CityAreaMapSquarePivot(this, location));

            if (!delayHappinessCheck)
            {
                CheckCitizensHappiness();
            }
        }

        #endregion

        #region Internal methods

        /// <summary>
        /// Removes any citizen, starting with specialists if any.
        /// </summary>
        /// <param name="recheck">Optionnal; proceeds to <see cref="ResetCitizens()"/> after removing.</param>
        internal void RemoveAnyCitizen(bool recheck = false)
        {
            if (_specialistCitizens.Count > 0)
            {
                _specialistCitizens.RemoveAt(0);
            }
            else
            {
                _areaMapSquares.Remove(AreaWithoutCityMapSquares.First());
            }
            if (recheck)
            {
                ResetCitizens();
            }
        }

        /// <summary>
        /// Removes as specified <see cref="UnitPivot"/> from garrison, then check the citizen happiness.
        /// </summary>
        /// <param name="unit">The unit to remove.</param>
        internal void RemoveFromGarrison(UnitPivot unit)
        {
            _garrison.Remove(unit);
            CheckCitizensHappiness();
        }

        /// <summary>
        /// Adds as specified <see cref="UnitPivot"/> in garrison, then check the citizen happiness.
        /// </summary>
        /// <param name="unit">The unit to add.</param>
        internal void AddInGarrison(UnitPivot unit)
        {
            _garrison.Add(unit);
            CheckCitizensHappiness();
        }

        /// <summary>
        /// Tries to get the most productive [productivity / commerce / food] vacant location on the city radius;
        /// doesn't include the city itself.
        /// </summary>
        /// <returns>The <see cref="MapSquarePivot"/> location; <c>Null</c> if no location available.</returns>
        internal MapSquarePivot BestVacantMapSquareLocation()
        {
            return Player.ComputeCityAvailableMapSquares(this)
                .Where(x => !AreaWithoutCityMapSquares.Any(ams => ams.MapSquare == x))
                .OrderByDescending(x => x.TotalValue)
                .FirstOrDefault();
        }

        /// <summary>
        /// Changes the city current production.
        /// </summary>
        /// <param name="buildable">The new production.</param>
        internal void ChangeProduction(BuildablePivot buildable)
        {
            Production = buildable;
        }

        /// <summary>
        /// Recomputes everything before passing to the next turn.
        /// </summary>
        /// <returns>A tuple of values [finished production / settler to disband].</returns>
        internal Tuple<BuildablePivot, bool> NextTurn()
        {
            bool disbandSettler = false;

            BuildablePivot produced = null;
            bool resetCitizensRequired = false;
            bool checkCitizensHappiness = false;

            if (ExtraFoodByTurn > 0)
            {
                // Fills the FoodGranaryStorage, then the FoodStorage.
                var tmpExtraFoodByTurn = ExtraFoodByTurn;
                if (_improvements.Contains(CityImprovementPivot.Granary)
                    && FoodGranaryStorage < NextCitizenFoodRequirement)
                {
                    FoodGranaryStorage += tmpExtraFoodByTurn;
                    tmpExtraFoodByTurn = 0;
                    if (FoodGranaryStorage > NextCitizenFoodRequirement)
                    {
                        tmpExtraFoodByTurn = FoodGranaryStorage - NextCitizenFoodRequirement;
                        FoodGranaryStorage = NextCitizenFoodRequirement;
                    }
                }
                FoodStorage += tmpExtraFoodByTurn;
            }
            else if (ExtraFoodByTurn < 0)
            {
                // Empty the FoodStorage, then the FoodGranaryStorage.
                FoodStorage += ExtraFoodByTurn;
                if (FoodStorage < 0 && _improvements.Contains(CityImprovementPivot.Granary))
                {
                    FoodGranaryStorage += FoodStorage;
                    FoodStorage = 0;
                }
            }

            if (FoodStorage < 0 && FoodGranaryStorage < 0)
            {
                if (Player.Units.Any(u => u.Is<SettlerPivot>() && u.City == this))
                {
                    disbandSettler = true;
                }
                else
                {
                    RemoveAnyCitizen();
                    resetCitizensRequired = true;
                }
                FoodStorage = 0;
                FoodGranaryStorage = 0;
            }
            else if (FoodStorage >= NextCitizenFoodRequirement)
            {
                if (CitizensCount < AQUEDUC_MAX_POPULATION_WITHOUT || _improvements.Contains(CityImprovementPivot.Aqueduc))
                {
                    FoodStorage = 0; // Note : excess is not keeped.
                    _areaMapSquares.Add(new CityAreaMapSquarePivot(this, BestVacantMapSquareLocation()));
                    resetCitizensRequired = true;
                }
                else
                {
                    FoodStorage = NextCitizenFoodRequirement;
                }
            }

            ProductivityStorage += Productivity;
            if (Production != null && ProductivityStorage >= Production.ProductivityCost)
            {
                bool produce = true;
                ProductivityStorage = Production.ProductivityCost;
                if (Production.Is<UnitPivot>())
                {
                    produce = false;
                    var citizensCost = ((UnitPivot)Production).CitizenCostToProduce;
                    if (citizensCost < CitizensCount)
                    {
                        for (int i = 0; i < citizensCost; i++)
                        {
                            RemoveAnyCitizen();
                        }
                        produce = true;
                    }
                }
                if (produce)
                {
                    ProductivityStorage = 0;
                    produced = Production;
                    Production = null;
                    if (produced.Is<CityImprovementPivot>())
                    {
                        _improvements.Add((CityImprovementPivot)produced);
                        if (CityImprovementPivot.Granary == produced)
                        {
                            // Note : at this point, "NextCitizenFoodRequirement" takes the granary in consideration.
                            if (FoodStorage > NextCitizenFoodRequirement)
                            {
                                FoodGranaryStorage = NextCitizenFoodRequirement;
                                FoodStorage -= NextCitizenFoodRequirement;
                            }
                            else
                            {
                                FoodGranaryStorage = FoodStorage;
                                FoodStorage = 0;
                            }
                        }
                    }
                    else if (produced.Is<WonderPivot>())
                    {
                        _wonders.Add((WonderPivot)produced);
                    }
                    if (produced.HasCitizenHappinessEffect)
                    {
                        checkCitizensHappiness = true;
                    }
                }
            }

            if (resetCitizensRequired)
            {
                ResetCitizens();
            }
            else if (checkCitizensHappiness)
            {
                CheckCitizensHappiness();
            }

            if (produced != null)
            {
                RemovePreviousPlant(produced);
            }

            return new Tuple<BuildablePivot, bool>(produced, disbandSettler);
        }

        /// <summary>
        /// Recomputes the happiness of every citizens.
        /// </summary>
        internal void CheckCitizensHappiness()
        {
            var nonSpecialistFaces = CitizensCount - _specialistCitizens.Count;
            var unhappyFaces = nonSpecialistFaces - CONTENT_CITIZENS_COUNT;

            if (Player.WonderIsActive(WonderPivot.ShakespeareTheatre) && _wonders.Contains(WonderPivot.ShakespeareTheatre))
            {
                unhappyFaces *= SHAKESPEARE_THEATRE_HAPPINESS_RATE;
            }

            if (Player.WonderIsActive(WonderPivot.JsBachsCathedral, MapSquareLocation))
            {
                unhappyFaces -= JSBACHS_WONDER_HAPPINESS_EFFECT;
            }

            var cathedralEffect = _improvements.Contains(CityImprovementPivot.Cathedral) ? CATHEDRAL_HAPPINESS_EFFECT : 0;
            if (Player.WonderIsActive(WonderPivot.MichelangeloChapel))
            {
                cathedralEffect = (int)(cathedralEffect * MICHELANGELO_CHAPEL_HAPPINESS_EFFECT);
            }

            var templeEffect = _improvements.Contains(CityImprovementPivot.Temple) ? TEMPLE_HAPPINESS_EFFECT : 0;
            if (Player.Advances.Contains(AdvancePivot.Mysticism))
            {
                templeEffect *= MYSTICISM_ADVANCE_HAPPINESS_RATE;
            }
            if (Player.WonderIsActive(WonderPivot.Oracle))
            {
                templeEffect *= ORACLE_WONDER_HAPPINESS_RATE;
            }

            // transforms unhappy in content.
            var contentEffects = new List<int>
            {
                templeEffect,
                cathedralEffect,
                _improvements.Contains(CityImprovementPivot.Colosseum) ? COLOSSEUM_HAPPINESS_EFFECT : 0,
                _garrison.Where(u => u.IsMilitary).Take(Player.Regime.MartialLawUnitCount).Count()
            };

            // transforms content into happy.
            var happinessEffects = new List<int>
            {
                Luxury * LUXURY_TO_HAPPY_CITIZEN_RATE,
                Player.WonderIsActive(WonderPivot.CureForCancer) ? CUREFORCANCER_WONDER_HAPPINESS_EFFECT : 0,
                Player.WonderIsActive(WonderPivot.HangingGardens) ? HANGING_GARDENS_WONDER_HAPPINESS_EFFECT : 0
            };

            var happyFaces = happinessEffects.Sum();
            if (happyFaces < nonSpecialistFaces)
            {
                // Not full happy : count of content OR unhappy.
                var remaniningFaces = nonSpecialistFaces - happyFaces;

                // Count of unhappy after content effects.
                unhappyFaces -= contentEffects.Sum();
                if (unhappyFaces < 0)
                {
                    // Overflow of content effect.
                    unhappyFaces = 0;
                }
                else if (unhappyFaces > remaniningFaces)
                {
                    // Just in case...
                    unhappyFaces = remaniningFaces;
                }
            }
            else
            {
                // Full happy (0 unhappy, 0 content).
                unhappyFaces = 0;
            }

            var citizensToCheck = AreaWithoutCityMapSquares.Select(ams => ams.Citizen);
            foreach (var citizen in citizensToCheck)
            {
                if (happyFaces > 0)
                {
                    citizen.Happiness = HappinessPivot.Happy;
                    happyFaces--;
                }
                else if (unhappyFaces > 0)
                {
                    citizen.Happiness = HappinessPivot.Unhappy;
                    unhappyFaces--;
                }
                else
                {
                    citizen.Happiness = HappinessPivot.Content;
                }
            }
        }

        /// <summary>
        /// Reset the status and the type of every citizens to optimize [productivity / commerce / food].
        /// </summary>
        internal void ResetCitizens()
        {
            var availableMapSquares = Player.ComputeCityAvailableMapSquares(this);

            var citizensToReset = Citizens.ToList(); // Fix the list !
            int i = 0;
            foreach (var citizen in citizensToReset)
            {
                if (i > availableMapSquares.Count)
                {
                    // From this point, only specialists
                    if (!citizen.Type.HasValue)
                    {
                        ChangeCitizenToSpecialist(citizen, CitizenTypePivot.Entertainer);
                    }
                }
                else
                {
                    AddRegularCitizen(citizen, BestVacantMapSquareLocation(), true);
                }
                i++;
            }

            CheckCitizensHappiness();
        }

        /// <summary>
        /// Changes the capital (adds / removes <see cref="CityImprovementPivot.Palace"/> and <see cref="CityImprovementPivot.Courthouse"/>).
        /// </summary>
        /// <param name="oldCapital">The previous capital.</param>
        internal void SetAsNewCapital(CityPivot oldCapital)
        {
            if (oldCapital != null)
            {
                oldCapital._improvements.Remove(CityImprovementPivot.Palace);
            }
            //_improvements.Remove(CityImprovementPivot.Courthouse);
            _improvements.Add(CityImprovementPivot.Palace);
        }

        /// <summary>
        /// Forces the remove of a specified <see cref="CityImprovementPivot"/> of the city.
        /// </summary>
        /// <param name="cityImprovement">The city improvement.</param>
        internal void ForceRemoveCityImprovement(CityImprovementPivot cityImprovement)
        {
            if (Improvements.Contains(cityImprovement))
            {
                _improvements.Remove(cityImprovement);
                if (cityImprovement.HasCitizenHappinessEffect)
                {
                    CheckCitizensHappiness();
                }
            }
        }

        /// <summary>
        /// Forces the addition of a specified <see cref="CityImprovementPivot"/> to the city.
        /// </summary>
        /// <param name="cityImprovement">The city improvement.</param>
        internal void ForceCityImprovement(CityImprovementPivot cityImprovement)
        {
            if (!Improvements.Contains(cityImprovement))
            {
                _improvements.Add(cityImprovement);
                RemovePreviousPlant(cityImprovement);
            }
        }

        /// <summary>
        /// Takes any specialist citizen of the city and makes it a regular citizen.
        /// </summary>
        /// <param name="mapSquare">The location.</param>
        internal void ChangeAnySpecialistToRegular(MapSquarePivot mapSquare)
        {
            var citizenSource = _specialistCitizens.FirstOrDefault();
            if (citizenSource != null)
            {
                AddRegularCitizen(citizenSource, mapSquare);
            }
        }

        /// <summary>
        /// Changes the <see cref="CitizenTypePivot"/> of the specified <see cref="CitizenPivot"/>;
        /// using a static sequence of type [Regular -> Entertainer -> Scientist -> TaxCollector -> Regular].
        /// </summary>
        /// <param name="citizenSource">The citizen.</param>
        internal void SwitchCitizenType(CitizenPivot citizenSource)
        {
            if (!citizenSource.Type.HasValue)
            {
                ChangeCitizenToSpecialist(citizenSource, CitizenTypePivot.Entertainer);
                CheckCitizensHappiness();
            }
            else
            {
                switch (citizenSource.Type.Value)
                {
                    case CitizenTypePivot.Entertainer:
                        ChangeCitizenToSpecialist(citizenSource, CitizenTypePivot.Scientist);
                        CheckCitizensHappiness();
                        break;
                    case CitizenTypePivot.Scientist:
                        ChangeCitizenToSpecialist(citizenSource, CitizenTypePivot.TaxCollector);
                        CheckCitizensHappiness();
                        break;
                    case CitizenTypePivot.TaxCollector:
                        ChangeCitizenToRegularAtTheBestVacantMapSquare(citizenSource);
                        break;
                }
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Checks if the specified coordinates are in the city radius.
        /// </summary>
        /// <param name="row">Row coordinate.</param>
        /// <param name="column">Column coordinate.</param>
        /// <returns><c>True</c> if in the city radius; <c>False</c> otherwise.</returns>
        public bool CoordinatesAreCityRadius(int row, int column)
        {
            return (row >= MapSquareLocation.Row - 2)
                && (row <= MapSquareLocation.Row + 2)
                && (column >= MapSquareLocation.Column - 2)
                && (column <= MapSquareLocation.Column + 2)
                && (row != MapSquareLocation.Row - 2 || column != MapSquareLocation.Column - 2)
                && (row != MapSquareLocation.Row - 2 || column != MapSquareLocation.Column + 2)
                && (row != MapSquareLocation.Row + 2 || column != MapSquareLocation.Column - 2)
                && (row != MapSquareLocation.Row + 2 || column != MapSquareLocation.Column + 2);
        }

        /// <summary>
        /// Checks if the specified coordinates are the city location.
        /// </summary>
        /// <param name="row">Row coordinate.</param>
        /// <param name="column">Column coordinate.</param>
        /// <returns><c>True</c> if it's the city location; <c>False</c> otherwise.</returns>
        public bool CoordinatesAreCityCenter(int row, int column)
        {
            return row == MapSquareLocation.Row && column == MapSquareLocation.Column;
        }

        /// <summary>
        /// Checks if the city is at the left border of the map.
        /// </summary>
        /// <returns><c>True</c> if it is; <c>False</c> otherwise.</returns>
        public bool OnLeftBorder()
        {
            return MapSquareLocation.Row == 0;
        }

        /// <summary>
        /// Checks if the city is at the right border of the map.
        /// </summary>
        /// <param name="mapWidth">Map width.</param>
        /// <returns><c>True</c> if it is; <c>False</c> otherwise.</returns>
        public bool OnRightBorder(int mapWidth)
        {
            return MapSquareLocation.Row == mapWidth - 1;
        }

        #endregion

        #region IEquatable implementation

        /// <summary>
        /// Checks if this instance is equal to another one.
        /// </summary>
        /// <param name="other">The other instance.</param>
        /// <returns><c>True</c> if equals; <c>False</c> otherwise.</returns>
        public bool Equals(CityPivot other)
        {
            return Name == other?.Name;
        }

        /// <summary>
        /// Operator "==" override. Checks equality between two instances.
        /// </summary>
        /// <param name="r1">The first <see cref="CityPivot"/>.</param>
        /// <param name="r2">The second <see cref="CityPivot"/>.</param>
        /// <returns><c>True</c> if equals; <c>False</c> otherwise.</returns>
        public static bool operator ==(CityPivot r1, CityPivot r2)
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
        /// <param name="r1">The first <see cref="CityPivot"/>.</param>
        /// <param name="r2">The second <see cref="CityPivot"/>.</param>
        /// <returns><c>False</c> if equals; <c>True</c> otherwise.</returns>
        public static bool operator !=(CityPivot r1, CityPivot r2)
        {
            return !(r1 == r2);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is CityPivot && Equals(obj as CityPivot);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        #endregion
    }
}
