using System;
using System.Collections.Generic;
using System.Linq;

namespace ErsatzCivLib.Model
{
    [Serializable]
    public class CityPivot
    {
        private const int DEFAULT_RATIO_CITIZEN_UNHAPPY = 5;
        internal const int CITY_SPEED_COST = 1;
        private const double MAX_CITIZEN_COUNT = 40;
        private const double MIN_CC_POP = 1000;
        private const double MAX_CC_POP = 20000000;
        private static readonly double POP_GROWTH_RATIO = Math.Log(MIN_CC_POP / MAX_CC_POP) / (1 - MAX_CITIZEN_COUNT);
        public const int FOOD_RATIO_TO_NEXT_CITIZEN = 40;
        private const double PRODUCTIVITY_TO_COMMERCE_RATIO = 0.1;

        private readonly Func<CityPivot, List<MapSquarePivot>> _availableMapSquaresFunc;
        private readonly List<CitizenPivot> _citizens;
        private readonly List<CityImprovementPivot> _improvements;
        private readonly List<WonderPivot> _wonders;

        public BuildablePivot Production { get; private set; }
        public int CreationTurn { get; private set; }
        public int FoodStorage { get; private set; }
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

                // TODO : include city improvements
                return MapSquareLocation.CityCommerce
                    + (Production.GetType() == typeof(CapitalizationPivot) ? (int)Math.Ceiling(Productivity * PRODUCTIVITY_TO_COMMERCE_RATIO) : 0)
                    + _citizens
                        .Where(c => !c.Type.HasValue)
                        .Sum(c => c.MapSquare.Commerce);
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

                // TODO : include city improvements
                return _citizens.Count(c => c.Type == CitizenTypePivot.Scientist || !c.Type.HasValue);
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

            var foodToConsume = CitizenPivot.FOOD_BY_TURN * _citizens.Count;
            FoodStorage -= foodToConsume;
            FoodStorage += Food;

            if (FoodStorage < 0)
            {
                _citizens.RemoveAt(0);
                ResetCitizens();
                FoodStorage = 0;
            }
            else if (FoodStorage >= FOOD_RATIO_TO_NEXT_CITIZEN * _citizens.Count)
            {
                FoodStorage = FoodStorage - (FOOD_RATIO_TO_NEXT_CITIZEN * _citizens.Count);
                _citizens.Add(new CitizenPivot(null));
                ResetCitizens();
            }

            ProductivityStorage += Productivity;
            if (ProductivityStorage >= Production.ProductivityCost)
            {
                ProductivityStorage -= Production.ProductivityCost;
                produced = Production;
                Production = CapitalizationPivot.CreateAtLocation(MapSquareLocation);
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
            for (int i = 0; i < entertainers; i++)
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

        [Serializable]
        public class CitizenPivot : IComparable<CitizenPivot>
        {
            public const int FOOD_BY_TURN = 2;

            public MapSquarePivot MapSquare { get; private set; }
            public MoodPivot Mood { get; internal set; }
            public CitizenTypePivot? Type { get; private set; }

            internal CitizenPivot(MapSquarePivot mapSquare)
            {
                MapSquare = mapSquare;
                Mood = MoodPivot.Content;
                Type = mapSquare == null ? CitizenTypePivot.Entertainer : (CitizenTypePivot?)null;
            }

            internal void ToSpecialist(CitizenTypePivot citizenType)
            {
                Mood = MoodPivot.Content;
                Type = citizenType;
                MapSquare = null;
            }

            internal void ToCitizen(MapSquarePivot mapSquare)
            {
                MapSquare = mapSquare ?? throw new ArgumentNullException("Argument is null !", nameof(mapSquare));
                Mood = MoodPivot.Content;
                Type = null;
            }

            public int CompareTo(CitizenPivot other)
            {
                if (other == null)
                {
                    return -1;
                }

                var compareType = Type.HasValue ?
                    (other.Type.HasValue ? ((int)Type.Value).CompareTo((int)other.Type.Value) : 1) :
                    (other.Type.HasValue ? -1 : 0);
                var compareMood = ((int)Mood).CompareTo((int)other.Mood);
                var compareMapS = (MapSquare?.TotalValue).GetValueOrDefault(0).CompareTo((other.MapSquare?.TotalValue).GetValueOrDefault(0));

                return compareType == 0 ? (compareMood == 0 ? compareMapS : compareMood) : compareType;
            }

            /// <inheritdoc />
            public override string ToString()
            {
                return (!Type.HasValue ? Mood.ToString() : Type.ToString());
            }
        }

        [Serializable]
        public enum MoodPivot
        {
            Happy,
            Content,
            Unhappy
        }

        [Serializable]
        public enum CitizenTypePivot
        {
            Scientist,
            TaxCollector,
            Entertainer
        }
    }
}
