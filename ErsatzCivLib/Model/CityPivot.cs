using System;
using System.Collections.Generic;
using System.Linq;
using ErsatzCivLib.Model.Enums;
using ErsatzCivLib.Model.Static;

namespace ErsatzCivLib.Model
{
    /// <summary>
    /// Represents a city.
    /// </summary>
    /// <seealso cref="IEquatable{T}"/>
    [Serializable]
    public class CityPivot : IEquatable<CityPivot>
    {
        #region Constants relative to citizens behavior

        private const int FOOD_BY_CITIZEN_BY_TURN = 2;
        private const int DEFAULT_RATIO_CITIZEN_UNHAPPY = 5;
        private const int FOOD_RATIO_TO_NEXT_CITIZEN = 40;

        #endregion

        #region Constants relative to improvements effects

        private const double MARKETPLACE_COMMERCE_INCREASE_RATIO = 1.5;
        private const double BANK_COMMERCE_INCREASE_RATIO = 1.5;
        private const double LIBRARY_SCIENCE_INCREASE_RATIO = 1.5;
        private const double UNIVERSITY_SCIENCE_INCREASE_RATIO = 1.5;
        private const double FACTORY_PRODUCTIVITY_INCREASE_RATIO = 1.5;
        private const double MFGPLANT_PRODUCTIVITY_INCREASE_RATIO = 2;
        private const double HYDROPLANT_PRODUCTIVITY_INCREASE_RATIO = 1.5;
        private const double NUCLEARPLANT_PRODUCTIVITY_INCREASE_RATIO = 1.5;
        private const double HYDROPLANT_POLLUTION_INCREASE_RATIO = 0.5;
        private const double NUCLEARPLANT_POLLUTION_INCREASE_RATIO = 0.5;
        private const double RECYCLINGCENTER_POLLUTION_INCREASE_RATIO = 1 / (double)3;
        private const double POWERPLANT_PRODUCTIVITY_INCREASE_RATIO = 1.5;
        private const double GRANARY_FOOD_INCREASE_RATIO = 0.5;
        private const double MASSTRANSIT_POLLUTION_INCREASE_RATIO = 0;
        private const double PALACE_CORRUPTION_INCREASE_RATE = 0.5;
        private const double COURTHOUSE_CORRUPTION_INCREASE_RATE = 0.5;
        private const int AQUEDUC_MAX_POPULATION_WITHOUT = 10;
        private const double CAPITALIZATION_PRODUCTIVITY_TO_COMMERCE_RATIO = 0.1;
        private const int TEMPLE_HAPPINESS_EFFECT = 1;
        private const int COLOSSEUM_HAPPINESS_EFFECT = 3;
        private const int CATHEDRAL_HAPPINESS_EFFECT = 4;

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

        private List<Units.SettlerPivot> _settlers = new List<Units.SettlerPivot>();
        /// <summary>
        /// Settlers on the map who belong to this city.
        /// </summary>
        public IReadOnlyCollection<Units.SettlerPivot> Settlers { get { return _settlers; } }

        private readonly List<CitizenPivot> _citizens;
        /// <summary>
        /// List of <see cref="CitizenPivot"/>.
        /// </summary>
        public IReadOnlyCollection<CitizenPivot> Citizens { get { return _citizens; } }

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

        #endregion

        #region Inferred properties

        /// <summary>
        /// Indicates if the city is in civil trouble (too much citizens in <see cref="MoodPivot.Unhappy"/> mood).
        /// </summary>
        public bool InCivilTrouble
        {
            get
            {
                return _citizens.Count(c => c.Mood == MoodPivot.Unhappy) > _citizens.Count(c => c.Mood == MoodPivot.Happy);
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
                return (int)Math.Round((MIN_CC_POP / Math.Exp(POP_GROWTH_RATIO)) * Math.Exp(POP_GROWTH_RATIO * _citizens.Count));
            }
        }
        /// <summary>
        /// Treasure by turn.
        /// </summary>
        public int Treasure
        {
            get
            {
                if (InCivilTroubleOrAnarchy)
                {
                    return 0;
                }

                var capitalizationValue = Production.Is<CapitalizationPivot>() ?
                    (int)Math.Ceiling(Productivity * CAPITALIZATION_PRODUCTIVITY_TO_COMMERCE_RATIO) : 0;
                var cityCommerceValue = MapSquareLocation.CityCommerce + (
                    MapSquareLocation.CityCommerce > 0 ? Player.Regime.CommerceBonus : 0
                );
                var aroundCityCommerceValue = _citizens
                    .Where(c => !c.Type.HasValue)
                    .Sum(c => c.MapSquare.Commerce + (c.MapSquare.Commerce > 0 ? Player.Regime.CommerceBonus : 0));

                var commerceValue = cityCommerceValue + capitalizationValue + aroundCityCommerceValue;

                if (_improvements.Contains(CityImprovementPivot.Marketplace))
                {
                    commerceValue = (int)Math.Floor(MARKETPLACE_COMMERCE_INCREASE_RATIO * commerceValue);
                }

                if (_improvements.Contains(CityImprovementPivot.Bank))
                {
                    commerceValue = (int)Math.Floor(BANK_COMMERCE_INCREASE_RATIO * commerceValue);
                }

                var taxValue = _citizens.Count(c => c.Type == CitizenTypePivot.TaxCollector || !c.Type.HasValue);

                var totalValue = taxValue + commerceValue;

                double corruptionRate = (Player.Regime.CorruptionRate * Player.GetDistanceToCapitalRate(this));

                if (_improvements.Contains(CityImprovementPivot.Palace))
                {
                    corruptionRate *= PALACE_CORRUPTION_INCREASE_RATE;
                }
                else if (_improvements.Contains(CityImprovementPivot.Courthouse))
                {
                    corruptionRate *= COURTHOUSE_CORRUPTION_INCREASE_RATE;
                }

                return (int)Math.Round(totalValue * corruptionRate);
            }
        }
        /// <summary>
        /// Food consumption by turn.
        /// </summary>
        public int FoodConsumption
        {
            get
            {
                return (_citizens.Count + _settlers.Count) * FOOD_BY_CITIZEN_BY_TURN;
            }
        }
        /// <summary>
        /// Food consumption by turn without settlers.
        /// </summary>
        public int FoodConsumptionWithoutSettlers
        {
            get
            {
                return _citizens.Count * FOOD_BY_CITIZEN_BY_TURN;
            }
        }
        /// <summary>
        /// Food production by turn.
        /// </summary>
        public int Food
        {
            get
            {
                var foodValue = MapSquareLocation.CityFood + _citizens
                        .Where(c => !c.Type.HasValue)
                        .Sum(c => c.MapSquare.Food);

                if (InCivilTroubleOrAnarchy && foodValue > FoodConsumptionWithoutSettlers)
                {
                    foodValue = FoodConsumptionWithoutSettlers;
                }

                return foodValue;
            }
        }
        /// <summary>
        /// Productivity by turn.
        /// </summary>
        public int Productivity
        {
            get
            {
                if (InCivilTroubleOrAnarchy)
                {
                    return 0;
                }

                var baseValue = MapSquareLocation.CityProductivity + _citizens
                    .Where(c => !c.Type.HasValue)
                    .Sum(c => c.MapSquare.Productivity);

                bool hasProdImprovement = false;
                if (_improvements.Contains(CityImprovementPivot.MfgPlant))
                {
                    baseValue = (int)Math.Floor(MFGPLANT_PRODUCTIVITY_INCREASE_RATIO * baseValue);
                    hasProdImprovement = true;
                }
                else if (_improvements.Contains(CityImprovementPivot.Factory))
                {
                    baseValue = (int)Math.Floor(FACTORY_PRODUCTIVITY_INCREASE_RATIO * baseValue);
                    hasProdImprovement = true;
                }

                if (hasProdImprovement)
                {
                    if (_improvements.Contains(CityImprovementPivot.HydroPlant))
                    {
                        baseValue = (int)Math.Floor(HYDROPLANT_PRODUCTIVITY_INCREASE_RATIO * baseValue);
                    }
                    else if (_improvements.Contains(CityImprovementPivot.NuclearPlant))
                    {
                        baseValue = (int)Math.Floor(NUCLEARPLANT_PRODUCTIVITY_INCREASE_RATIO * baseValue);
                    }
                    else if (_improvements.Contains(CityImprovementPivot.PowerPlant))
                    {
                        baseValue = (int)Math.Floor(POWERPLANT_PRODUCTIVITY_INCREASE_RATIO * baseValue);
                    }
                }

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
                if (InCivilTroubleOrAnarchy)
                {
                    return 0;
                }
                
                var scienceValue = _citizens.Count(c => c.Type == CitizenTypePivot.Scientist || !c.Type.HasValue);

                if (_improvements.Contains(CityImprovementPivot.Library))
                {
                    scienceValue = (int)Math.Floor(scienceValue * LIBRARY_SCIENCE_INCREASE_RATIO);
                }

                if (_improvements.Contains(CityImprovementPivot.University))
                {
                    scienceValue = (int)Math.Floor(scienceValue * UNIVERSITY_SCIENCE_INCREASE_RATIO);
                }

                return (int)Math.Ceiling(scienceValue * Player.Regime.ScienceRate);
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
                return Production.ProductivityCost - ProductivityStorage;
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
                var baseValue = FOOD_RATIO_TO_NEXT_CITIZEN * _citizens.Count;

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

            _citizens = new List<CitizenPivot>
            {
                new CitizenPivot(BestVacantMapSquareLocation(), this)
            };
        }

        #region Internal methods

        /// <summary>
        /// Tries to get the most productive [productivity / commerce / food] vacant location on the city radius;
        /// doesn't include the city itself.
        /// </summary>
        /// <returns>The <see cref="MapSquarePivot"/> location; <c>Null</c> if no location available.</returns>
        internal MapSquarePivot BestVacantMapSquareLocation()
        {
            return Player.ComputeCityAvailableMapSquares(this)
                .Where(x => _citizens?.Any(c => c.MapSquare == x) != true)
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
        /// <returns>A tuple of values [finished production / settler disbanded].</returns>
        internal Tuple<BuildablePivot, Units.SettlerPivot> NextTurn()
        {
            Units.SettlerPivot disbandedSettler = null;

            BuildablePivot produced = null;
            bool resetCitizensRequired = false;
            bool checkCitizensMood = false;

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
                if (_settlers.Count > 0)
                {
                    disbandedSettler = _settlers[0];
                    _settlers.RemoveAt(0);
                }
                else
                {
                    _citizens.RemoveAt(0);
                    resetCitizensRequired = true;
                }
                FoodStorage = 0;
                FoodGranaryStorage = 0;
            }
            else if (FoodStorage >= NextCitizenFoodRequirement)
            {
                if (_citizens.Count < AQUEDUC_MAX_POPULATION_WITHOUT || _improvements.Contains(CityImprovementPivot.Aqueduc))
                {
                    FoodStorage = 0; // Note : excess is not keeped.
                    _citizens.Add(new CitizenPivot(null, this));
                    resetCitizensRequired = true;
                }
                else
                {
                    FoodStorage = NextCitizenFoodRequirement;
                }
            }

            ProductivityStorage += Productivity;
            if (ProductivityStorage >= Production.ProductivityCost)
            {
                bool produce = true;
                ProductivityStorage = Production.ProductivityCost;
                if (Production.Is<UnitPivot>())
                {
                    produce = false;
                    var citizensCost = ((UnitPivot)Production).CitizenCostToProduce;
                    if (citizensCost < _citizens.Count)
                    {
                        _citizens.RemoveRange(0, citizensCost);
                        produce = true;
                    }
                }
                if (produce)
                {
                    ProductivityStorage = 0;
                    produced = Production;
                    Production = CapitalizationPivot.Default;
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
                    if (produced.HasCitizenMoodEffect)
                    {
                        checkCitizensMood = true;
                    }
                }
            }

            if (resetCitizensRequired)
            {
                ResetCitizens();
            }
            else if (checkCitizensMood)
            {
                CheckCitizensMood();
            }

            if (produced != null)
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
                else if (produced.Is<Units.SettlerPivot>())
                {
                    _settlers.Add(produced as Units.SettlerPivot);
                }
            }

            return new Tuple<BuildablePivot, Units.SettlerPivot>(produced, disbandedSettler);
        }

        /// <summary>
        /// Recomputes the mood of every citizens.
        /// </summary>
        internal void CheckCitizensMood()
        {
            // Default behavior.
            var specialistFaces = _citizens.Where(c => c.Type.HasValue).Count();
            var nonSpecialistFaces = _citizens.Count - specialistFaces;
            var unhappyFaces = nonSpecialistFaces / DEFAULT_RATIO_CITIZEN_UNHAPPY;
            var happyFaces = 0;

            // Entertaining effects.
            var entertainers = _citizens.Where(c => c.Type == CitizenTypePivot.Entertainer).Count();
            var templeEffect = _improvements.Contains(CityImprovementPivot.Temple) ? TEMPLE_HAPPINESS_EFFECT : 0;
            var colosseumEffect = _improvements.Contains(CityImprovementPivot.Colosseum) ? COLOSSEUM_HAPPINESS_EFFECT : 0;
            var cathedralEffet = _improvements.Contains(CityImprovementPivot.Cathedral) ? CATHEDRAL_HAPPINESS_EFFECT : 0;
            for (int i = 0; i < (entertainers + templeEffect + colosseumEffect + cathedralEffet); i++)
            {
                if (unhappyFaces > 0)
                {
                    unhappyFaces--;
                }
                else if (happyFaces < nonSpecialistFaces)
                {
                    happyFaces++;
                }
            }

            foreach (var citizen in _citizens.Where(ci => !ci.Type.HasValue))
            {
                if (happyFaces > 0)
                {
                    citizen.Mood = MoodPivot.Happy;
                    happyFaces--;
                }
                else if (unhappyFaces > 0)
                {
                    citizen.Mood = MoodPivot.Unhappy;
                    unhappyFaces--;
                }
                else
                {
                    citizen.Mood = MoodPivot.Content;
                }
            }
            _citizens.Sort();
        }

        /// <summary>
        /// Reset the status and the type of every citizens to optimize [productivity / commerce / food].
        /// </summary>
        internal void ResetCitizens()
        {
            var availableMapSquaresCount = Player.ComputeCityAvailableMapSquares(this).Count;

            _citizens.Sort();
            for (int i = 0; i < _citizens.Count; i++)
            {
                if (i > availableMapSquaresCount)
                {
                    if (!_citizens[i].Type.HasValue)
                    {
                        _citizens[i].ToSpecialist(CitizenTypePivot.Entertainer);
                    }
                }
                else
                {
                    var newBestSpot = BestVacantMapSquareLocation();
                    if (_citizens[i].MapSquare == null
                        || _citizens[i].MapSquare.TotalValue < newBestSpot.TotalValue)
                    {
                        _citizens[i].ToCitizen(newBestSpot);
                    }
                }
            }

            CheckCitizensMood();
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
        /// Gets a random specialist citizen, if any.
        /// </summary>
        /// <returns>The citizen; or <c>Null</c>.</returns>
        internal CitizenPivot GetAnySpecialistCitizen()
        {
            return _citizens.FirstOrDefault(c => c.Type.HasValue);
        }

        /// <summary>
        /// Proceeds to unlink a <see cref="Units.SettlerPivot"/> from the city.
        /// </summary>
        /// <param name="settler">The settler.</param>
        internal void UnlinkSettler(Units.SettlerPivot settler)
        {
            _settlers.Remove(settler);
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
