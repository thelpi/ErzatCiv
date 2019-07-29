using System;
using System.Collections.Generic;

namespace ErsatzCivLib.Model
{
    [Serializable]
    public class CityPivot
    {
        public const double DISPLAY_RATIO = 0.8;
        internal const int CITY_SPEED_COST = 1;
        private const string CITY_RENDER_PATH = "city.png";
        private const double FIRST_CITIZEN_EULER = 9.21034;
        private const double FORTY_CITIZEN_EULER = 16.81124;
        private static readonly double NEXT_CITIZEN_EULER = (FORTY_CITIZEN_EULER - FIRST_CITIZEN_EULER) / (40 - 1);

        private List<CitizenPivot> _citizens = new List<CitizenPivot>();

        public int Row { get; private set; }
        public int Column { get; private set; }
        public string RenderValue { get; private set; }
        public IReadOnlyCollection<CitizenPivot> Citizens { get { return _citizens; } }
        public int Population
        {
            get
            {
                return (int)Math.Round(Math.Pow(Math.E, FIRST_CITIZEN_EULER + (NEXT_CITIZEN_EULER * _citizens.Count - 1)));
            }
        }

        internal CityPivot(int row, int column)
        {
            Row = row;
            Column = column;
            RenderValue = CITY_RENDER_PATH;
            //_citizens.Add(new CitizenPivot());
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
