﻿using System;
using System.Collections.Generic;

namespace ErsatzCivLib.Model
{
    [Serializable]
    public class CityPivot : BasePivot
    {
        public const double DISPLAY_RATIO = 0.8;
        internal const int CITY_SPEED_COST = 1;
        private const string CITY_RENDER_PATH = "city.png";
        private static readonly double FIRST_CITIZEN_LN = Math.Log(10000);
        private static readonly double FORTY_CITIZEN_LN = Math.Log(20000000);
        private static readonly double NEXT_CITIZEN_LN = (FORTY_CITIZEN_LN - FIRST_CITIZEN_LN) / (40 - 1);

        private List<CitizenPivot> _citizens = new List<CitizenPivot>();

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

        internal CityPivot(Engine owner, int row, int column) : base(owner)
        {
            Row = row;
            Column = column;
            RenderValue = CITY_RENDER_PATH;

            var coordinates = BestCoordinates();
            _citizens.Add(new CitizenPivot(coordinates.Item1, coordinates.Item2));
        }

        private Tuple<int, int> BestCoordinates()
        {
            var bestCoord = new Tuple<int, int>(-1, -1);
            var currentBestValue = -1;

            for (int x = Row - 2; x <= Row + 2; x++)
            {
                for (int y = Column - 2; y <= Column + 2; y++)
                {
                    var currentSquare = Owner.Map[x, y];
                    if (currentSquare != null
                        && !(x == Row && y == Column)
                        && !(x == Row - 2 && y == Column - 2)
                        && !(x == Row + 2 && y == Column - 2)
                        && !(x == Row - 2 && y == Column + 2)
                        && !(x == Row + 2 && y == Column + 2)
                        && !Owner.OccupiedByAnotherCity(this, x, y))
                    {
                        if (currentBestValue < currentSquare.TotalValue)
                        {
                            currentBestValue = currentSquare.TotalValue;
                            bestCoord = new Tuple<int, int>(x, y);
                        }
                    }
                }
            }

            return bestCoord;
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
