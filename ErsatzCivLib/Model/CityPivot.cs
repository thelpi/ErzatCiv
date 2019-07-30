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

        private List<CitizenPivot> _citizens;
        private List<MapSquarePivot> _availableMapSquares;

        public int Row { get; private set; }
        public int Column { get; private set; }
        public string RenderValue { get; private set; }
        public IReadOnlyCollection<CitizenPivot> Citizens { get { return _citizens; } }
        public int Population
        {
            get
            {
                return (int)Math.Round((MIN_CC_POP / Math.Exp(POP_GROWTH_RATIO)) * Math.Exp(POP_GROWTH_RATIO * _citizens.Count));
            }
        }

        internal CityPivot(Units.SettlerPivot settler, IEnumerable<MapSquarePivot> availableMapSquares)
        {
            Row = settler.Row;
            Column = settler.Column;
            RenderValue = CITY_RENDER_PATH;

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
