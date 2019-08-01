using System;
using System.Collections.Generic;
using System.Linq;

namespace ErsatzCivLib.Model
{
    [Serializable]
    public class CityPivot
    {
        private static readonly int AVAILABLE_SQUARES_MAX_COUNT = (5 * 5) - 4 - 1;
        public const double DISPLAY_RATIO = 0.8;
        internal const int CITY_SPEED_COST = 1;
        private const string CITY_RENDER_PATH = "city.png";
        private const double MAX_CITIZEN_COUNT = 40;
        private const double MIN_CC_POP = 1000;
        private const double MAX_CC_POP = 20000000;
        private static readonly double POP_GROWTH_RATIO = Math.Log(MIN_CC_POP / MAX_CC_POP) / (1 - MAX_CITIZEN_COUNT);
        private const int FOOD_RATIO_TO_NEXT_CITIZEN = 50;
        private const int PRODUCTIVITY_TO_COMMERCE_RATIO = 10;

        private List<CitizenPivot> _citizens;
        private List<MapSquarePivot> _availableMapSquares;

        public BuildablePivot Production { get; private set; }
        public int CreationTurn { get; private set; }
        public int FoodStorage { get; private set; }
        public int ProductivityStorage { get; private set; }
        public string Name { get; private set; }
        public MapSquarePivot MapSquareLocation { get; private set; }
        public string RenderValue { get; private set; }
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
                return MapSquareLocation.CityFood + _citizens
                        .Where(c => c.Type == CitizenTypePivot.Regular)
                        .Sum(c => c.MapSquare.Food);
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
                    + (Production == null ? Productivity / PRODUCTIVITY_TO_COMMERCE_RATIO : 0)
                    + _citizens
                        .Where(c => c.Type == CitizenTypePivot.Regular)
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
                return _citizens.Count(c => c.Type == CitizenTypePivot.TaxCollector || c.Type == CitizenTypePivot.Regular);
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
                        .Where(c => c.Type == CitizenTypePivot.Regular)
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
                return _citizens.Count(c => c.Type == CitizenTypePivot.Scientist || c.Type == CitizenTypePivot.Regular);
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

        internal CityPivot(int currentTurn, string name, MapSquarePivot location, IEnumerable<MapSquarePivot> availableMapSquares, BuildablePivot production)
        {
            Name = name;
            MapSquareLocation = location;
            RenderValue = CITY_RENDER_PATH;
            CreationTurn = currentTurn;
            Production = production;

            _availableMapSquares = new List<MapSquarePivot>(availableMapSquares);
            
            _citizens = new List<CitizenPivot>
            {
                new CitizenPivot(BestVacantSpot())
            };
        }

        internal void AddAvailableMapSquare(MapSquarePivot square)
        {
            if (!_availableMapSquares.Any(x => x == square) && _availableMapSquares.Count < AVAILABLE_SQUARES_MAX_COUNT)
            {
                _availableMapSquares.Add(square);
            }
        }

        internal void RemoveAvailableMapSquare(MapSquarePivot square)
        {
            _availableMapSquares.RemoveAll(x => x == square);
        }

        private MapSquarePivot BestVacantSpot()
        {
            return _availableMapSquares
                .Where(x => _citizens?.Any(c => c.MapSquare == x) != true)
                .OrderByDescending(x => x.TotalValue)
                .FirstOrDefault();
        }

        internal BuildablePivot UpdateStatus()
        {
            BuildablePivot produced = null;

            var foodToConsume = CitizenPivot.FOOD_BY_TURN * _citizens.Count;
            FoodStorage -= foodToConsume;
            FoodStorage += Food;

            if (FoodStorage < 0)
            {
                _citizens.Remove(_citizens.OrderByDescending(c => (int)c.Type).ThenBy(c => c.MapSquare?.Food).First());
                FoodStorage = 0;
            }
            else if (FoodStorage > FOOD_RATIO_TO_NEXT_CITIZEN * _citizens.Count)
            {
                FoodStorage = FoodStorage - (FOOD_RATIO_TO_NEXT_CITIZEN * _citizens.Count);
                _citizens.Add(new CitizenPivot(BestVacantSpot()));
            }

            if (Production == null)
            {
                ProductivityStorage = 0;
            }
            else
            {
                ProductivityStorage += Productivity;
                if (ProductivityStorage >= Production.ProductivityCost)
                {
                    ProductivityStorage -= Production.ProductivityCost;
                    produced = Production;
                    Production = null;
                }
            }

            return produced;
        }

        [Serializable]
        public class CitizenPivot
        {
            public const int FOOD_BY_TURN = 2;

            public MapSquarePivot MapSquare { get; private set; }
            public MoodPivot Mood { get; private set; }
            public CitizenTypePivot Type { get; private set; }

            public CitizenPivot(MapSquarePivot mapSquare)
            {
                MapSquare = mapSquare;
                Mood = MoodPivot.Content;
                Type = mapSquare == null ? CitizenTypePivot.Entertaining : CitizenTypePivot.Regular;
            }

            public void ToSpecialist(CitizenTypePivot citizenType)
            {
                if (citizenType != CitizenTypePivot.Regular)
                {
                    Mood = MoodPivot.Content;
                    Type = citizenType;
                    MapSquare = null;
                }
            }

            public void ToCitizen(MapSquarePivot mapSquare)
            {
                MapSquare = mapSquare ?? throw new ArgumentNullException("Argument is null !", nameof(mapSquare));
                Mood = MoodPivot.Content;
                Type = CitizenTypePivot.Regular;
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
            Regular,
            Scientist,
            TaxCollector,
            Entertaining
        }
    }
}
