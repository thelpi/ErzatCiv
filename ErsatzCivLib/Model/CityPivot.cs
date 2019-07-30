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
        private List<Tuple<MapSquarePivot, int, int>> _availableMapSquares;

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

        internal CityPivot(int row, int column, IEnumerable<Tuple<MapSquarePivot, int,int>> availableMapSquares)
        {
            Row = row;
            Column = column;
            RenderValue = CITY_RENDER_PATH;

            _availableMapSquares = new List<Tuple<MapSquarePivot, int, int>>(availableMapSquares);

            var coordinates = BestVacantSpot();
            _citizens = new List<CitizenPivot>
            {
                new CitizenPivot(coordinates.Item1, coordinates.Item2)
            };
        }

        internal void AddAvailableMapSquare(MapSquarePivot square, int row, int column)
        {
            if (!_availableMapSquares.Any(x => x.Item1 == square) && _availableMapSquares.Count < AVAILABLE_SQUARES_MAX_COUNT)
            {
                _availableMapSquares.Add(new Tuple<MapSquarePivot, int, int>(square, row, column));
            }
        }

        internal void RemoveAvailableMapSquare(int row, int column)
        {
            _availableMapSquares.RemoveAll(x => x.Item2 == row && x.Item3 == column);
        }

        private Tuple<int, int> BestVacantSpot()
        {
            return _availableMapSquares
                .Where(x => _citizens?.Any(c => c.Row == x.Item2 && c.Column == x.Item3) != true)
                .OrderByDescending(x => x.Item1.TotalValue)
                .Select(x => new Tuple<int, int>(x.Item2, x.Item3))
                .FirstOrDefault();
        }

        [Serializable]
        public class CitizenPivot
        {
            public const int FOOD_BY_TURN = 2;

            public int Row { get; private set; }
            public int Column { get; private set; }
            public MoodPivot Mood { get; private set; }
            public CitizenTypePivot Type { get; private set; }

            public CitizenPivot(int row, int column)
            {
                Row = row;
                Column = column;
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
