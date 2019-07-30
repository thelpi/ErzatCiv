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
        private static readonly double FIRST_CITIZEN_LN = Math.Log(10000);
        private static readonly double FORTY_CITIZEN_LN = Math.Log(20000000);
        private static readonly double NEXT_CITIZEN_LN = (FORTY_CITIZEN_LN - FIRST_CITIZEN_LN) / (40 - 1);

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
                return (int)Math.Round(Math.Pow(Math.E, FIRST_CITIZEN_LN + (NEXT_CITIZEN_LN * (_citizens.Count - 1))));
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
