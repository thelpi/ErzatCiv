using System;
using System.Collections.Generic;
using System.Linq;
using ErsatzCivLib.Model.Enums;
using ErsatzCivLib.Model.Static;

namespace ErsatzCivLib.Model
{
    [Serializable]
    public class CityPivot : IEquatable<CityPivot>
    {
        private const double MARKETPLACE_COMMERCE_INCREASE_RATIO = 1.5;
        private const double LIBRARY_SCIENCE_INCREASE_RATIO = 1.5;
        private const int PRODUCTION_INFINITE = 9999;
        private const int DEFAULT_RATIO_CITIZEN_UNHAPPY = 5;
        internal const int CITY_SPEED_COST = 1;
        private const double MAX_CITIZEN_COUNT = 40;
        private const double MIN_CC_POP = 1000;
        private const double MAX_CC_POP = 20000000;
        private static readonly double POP_GROWTH_RATIO = Math.Log(MIN_CC_POP / MAX_CC_POP) / (1 - MAX_CITIZEN_COUNT);
        private const int FOOD_RATIO_TO_NEXT_CITIZEN = 40;
        private const double PRODUCTIVITY_TO_COMMERCE_RATIO = 0.1;
        private const int MAX_POPULATION_WITHOUT_WATER_SUPPLY = 10;

        private readonly Func<CityPivot, List<MapSquarePivot>> _availableMapSquaresFunc;
        private readonly List<CitizenPivot> _citizens;
        private readonly List<CityImprovementPivot> _improvements;
        private readonly List<WonderPivot> _wonders;

        public BuildablePivot Production { get; private set; }
        public int CreationTurn { get; private set; }
        public int FoodStorage { get; private set; }
        public int FoodGranaryStorage { get; private set; }
        public int ProductivityStorage { get; private set; }
        public string Name { get; private set; }
        public MapSquarePivot MapSquareLocation { get; private set; }
        public bool HasWaterSupply { get { return MapSquareLocation.Rivers.Count > 0; } }
        public IReadOnlyCollection<CitizenPivot> Citizens { get { return _citizens; } }
        public bool InCivilTrouble
        {
            get
            {
                return _citizens.Count(c => c.Mood == MoodPivot.Unhappy) > _citizens.Count(c => c.Mood == MoodPivot.Happy);
            }
        }
        public int Population
        {
            get
            {
                return (int)Math.Round((MIN_CC_POP / Math.Exp(POP_GROWTH_RATIO)) * Math.Exp(POP_GROWTH_RATIO * _citizens.Count));
            }
        }
        public int Food
        {
            get
            {
                // TODO : include city improvements
                var foodValue = MapSquareLocation.CityFood + _citizens
                        .Where(c => !c.Type.HasValue)
                        .Sum(c => c.MapSquare.Food);

                if (InCivilTrouble && foodValue > _citizens.Count * CitizenPivot.FOOD_BY_TURN)
                {
                    foodValue = _citizens.Count * CitizenPivot.FOOD_BY_TURN;
                }

                return foodValue;
            }
        }
        public int Commerce
        {
            get
            {
                if (InCivilTrouble)
                {
                    return 0;
                }
                
                var commerceValue = MapSquareLocation.CityCommerce
                    + (Production.Is<CapitalizationPivot>() ? (int)Math.Ceiling(Productivity * PRODUCTIVITY_TO_COMMERCE_RATIO) : 0)
                    + _citizens
                        .Where(c => !c.Type.HasValue)
                        .Sum(c => c.MapSquare.Commerce);

                if (_improvements.Contains(CityImprovementPivot.Marketplace))
                {
                    commerceValue = (int)Math.Floor(MARKETPLACE_COMMERCE_INCREASE_RATIO * commerceValue);
                }

                return commerceValue;
            }
        }
        public int Tax
        {
            get
            {
                if (InCivilTrouble)
                {
                    return 0;
                }

                // TODO : include city improvements
                return _citizens.Count(c => c.Type == CitizenTypePivot.TaxCollector || !c.Type.HasValue);
            }
        }
        public int Productivity
        {
            get
            {
                if (InCivilTrouble)
                {
                    return 0;
                }

                // TODO : include city improvements
                return MapSquareLocation.CityProductivity + _citizens
                        .Where(c => !c.Type.HasValue)
                        .Sum(c => c.MapSquare.Productivity);
            }
        }
        public int Science
        {
            get
            {
                if (InCivilTrouble)
                {
                    return 0;
                }
                
                var scienceValue = _citizens.Count(c => c.Type == CitizenTypePivot.Scientist || !c.Type.HasValue);

                if (_improvements.Contains(CityImprovementPivot.Library))
                {
                    scienceValue = (int)Math.Floor(scienceValue * LIBRARY_SCIENCE_INCREASE_RATIO);
                }

                return scienceValue;
            }
        }
        public int Pollution
        {
            get
            {
                // TODO
                return 0;
            }
        }
        public IReadOnlyCollection<CityImprovementPivot> Improvements { get { return _improvements; } }
        public IReadOnlyCollection<WonderPivot> Wonders { get { return _wonders; } }
        public IReadOnlyCollection<BuildablePivot> ImprovementsAndWonders
        {
            get
            {
                return new List<BuildablePivot>(_improvements).Concat(_wonders).ToList();
            }
        }
        public int RemainingProductionCost
        {
            get
            {
                return Production.ProductivityCost - ProductivityStorage;
            }
        }
        public int RemainingProductionTurns
        {
            get
            {
                return (Productivity == 0 ? PRODUCTION_INFINITE :
                    (int)Math.Ceiling(RemainingProductionCost / (double)Productivity));
            }
        }
        public int NextCitizenFoodRequirement
        {
            get
            {
                return (FOOD_RATIO_TO_NEXT_CITIZEN * _citizens.Count) / (_improvements.Contains(CityImprovementPivot.Granary) ? 2 : 1);
            }
        }
        public int ExtraFoodByTurn
        {
            get
            {
                return Food - (CitizenPivot.FOOD_BY_TURN * _citizens.Count);
            }
        }
        public int NextCitizenTurns
        {
            get
            {
                return ExtraFoodByTurn == 0 ? 0 : (int)Math.Ceiling((NextCitizenFoodRequirement - FoodStorage) / (double)ExtraFoodByTurn);
            }
        }
        public string FoodStatus
        {
            get
            {
                return ExtraFoodByTurn > 0 ? $"Growth ({NextCitizenTurns} turns)" :
                    (ExtraFoodByTurn == 0 ? "Balanced" : "Famine");
            }
        }

        internal CityPivot(int currentTurn, string name, MapSquarePivot location,
            Func<CityPivot, List<MapSquarePivot>> availableMapSquaresFunc, BuildablePivot production)
        {
            Name = name;
            MapSquareLocation = location;
            CreationTurn = currentTurn;
            Production = production;

            _availableMapSquaresFunc = availableMapSquaresFunc;
            _improvements = new List<CityImprovementPivot>();
            _wonders = new List<WonderPivot>();

            _citizens = new List<CitizenPivot>
            {
                new CitizenPivot(BestVacantSpot())
            };
        }

        internal MapSquarePivot BestVacantSpot()
        {
            return _availableMapSquaresFunc(this)
                .Where(x => _citizens?.Any(c => c.MapSquare == x) != true)
                .OrderByDescending(x => x.TotalValue)
                .FirstOrDefault();
        }

        internal void ChangeProduction(BuildablePivot buildable)
        {
            Production = buildable;
        }

        internal BuildablePivot UpdateStatus()
        {
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
                _citizens.RemoveAt(0);
                resetCitizensRequired = true;
                FoodStorage = 0;
                FoodGranaryStorage = 0;
            }
            else if (FoodStorage >= NextCitizenFoodRequirement)
            {
                if (_citizens.Count < MAX_POPULATION_WITHOUT_WATER_SUPPLY
                    || MapSquareLocation.HasRiver
                    || _improvements.Contains(CityImprovementPivot.Aqueduc))
                {
                    FoodStorage = 0; // Note : excess is not keeped.
                    _citizens.Add(new CitizenPivot(null));
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

            return produced;
        }

        internal void CheckCitizensMood()
        {
            // Default behavior.
            var specialistFaces = _citizens.Where(c => c.Type.HasValue).Count();
            var nonSpecialistFaces = _citizens.Count - specialistFaces;
            var unhappyFaces = nonSpecialistFaces / DEFAULT_RATIO_CITIZEN_UNHAPPY;
            var happyFaces = 0;

            // Entertaining effects.
            var entertainers = _citizens.Where(c => c.Type == CitizenTypePivot.Entertainer).Count();
            var templeEffect = _improvements.Contains(CityImprovementPivot.Temple) ? 1 : 0;
            var colosseumEffect = _improvements.Contains(CityImprovementPivot.Colosseum) ? 3 : 0;
            var cathedralEffet = _improvements.Contains(CityImprovementPivot.Cathedral) ? 4 : 0;
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

        internal void ResetCitizens()
        {
            var availableMapSquaresCount = _availableMapSquaresFunc(this).Count;

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
                    var newBestSpot = BestVacantSpot();
                    if (_citizens[i].MapSquare == null
                        || _citizens[i].MapSquare.TotalValue < newBestSpot.TotalValue)
                    {
                        _citizens[i].ToCitizen(newBestSpot);
                    }
                }
            }

            CheckCitizensMood();
        }

        internal void SetAsNewCapital(CityPivot oldCapital)
        {
            if (oldCapital != null)
            {
                oldCapital._improvements.Remove(CityImprovementPivot.Palace);
            }
            _improvements.Remove(CityImprovementPivot.Courthouse);
            _improvements.Add(CityImprovementPivot.Palace);
        }

        public CitizenPivot GetAnySpecialistCitizen()
        {
            return _citizens.FirstOrDefault(c => c.Type.HasValue);
        }

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

        public bool CoordinatesAreCityCenter(int row, int column)
        {
            return row == MapSquareLocation.Row && column == MapSquareLocation.Column;
        }

        public bool OnLeftBorder()
        {
            return MapSquareLocation.Row == 0;
        }

        public bool OnRightBorder(int mapWidth)
        {
            return MapSquareLocation.Row == mapWidth - 1;
        }

        #region IEquatable implementation

        public bool Equals(CityPivot other)
        {
            return Name == other?.Name;
        }

        public static bool operator ==(CityPivot ms1, CityPivot ms2)
        {
            if (ms1 is null)
            {
                return ms2 is null;
            }

            return ms1.Equals(ms2) == true;
        }

        public static bool operator !=(CityPivot ms1, CityPivot ms2)
        {
            return !(ms1 == ms2);
        }

        public override bool Equals(object obj)
        {
            return obj is CityPivot && Equals(obj as CityPivot);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        #endregion
    }
}
