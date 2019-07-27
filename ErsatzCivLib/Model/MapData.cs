using System;
using System.Collections.Generic;
using System.Linq;

namespace ErsatzCivLib.Model
{
    public class MapData
    {
        public const int RATIO_WIDTH_HEIGHT = 2;
        private const int MINIMAL_HEIGHT = 20;
        // Multiply the continent squares count by this number to obtain the rivers count
        private const double RIVER_COUNT_RATIO = 0.005;

        private readonly List<MapSquareData> _mapSquareList = new List<MapSquareData>();

        public IReadOnlyCollection<MapSquareData> MapSquareList
        {
            get
            {
                return _mapSquareList;
            }
        }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Aire { get { return Width * Height; } }
        public MapSquareData this[int i, int j]
        {
            get { return _mapSquareList.Single(x => x.Column == j && x.Row == i); }
        }

        internal MapData(MapSizeEnum mapSize, int continentCount, int landRatio1To10)
        {
            // Ensures correct values.
            continentCount = CapProperty(continentCount, maxExcluded: 11);
            landRatio1To10 = CapProperty(landRatio1To10);

            // General map properties.
            Height = MINIMAL_HEIGHT * (int)mapSize;
            Width = Height * RATIO_WIDTH_HEIGHT;

            // Squares count for land overall.
            var landSquares = Convert.ToInt32(Math.Floor((landRatio1To10 / (double)10) * Aire));

            // Squares count by continent. Must be a rectangle. Every continents have the same count.
            var restOfSquares = landSquares % continentCount;
            var landSquaresByContinent = (restOfSquares == 0 ? landSquares : landSquares - restOfSquares) / continentCount;
            landSquaresByContinent = landSquaresByContinent % 2 == 0 ? landSquaresByContinent : landSquaresByContinent - 1;

            // Divides the map into columns.
            var mapRectanglesDim = new Tuple<int, int>[continentCount];
            var restOfWidth = Width % continentCount;
            double minimalContainerHeightWidthRatio = double.NaN;
            for (int i = 0; i < continentCount; i++)
            {
                bool extraWidth = restOfWidth > 0;
                var currentWidth = (Width / continentCount) + (extraWidth ? 1 : 0);
                mapRectanglesDim[i] = new Tuple<int, int>(currentWidth, Height);
                if (!extraWidth || minimalContainerHeightWidthRatio == double.NaN)
                {
                    minimalContainerHeightWidthRatio = currentWidth / (double)Height;
                }
                restOfWidth--;
            }

            // Sets continent dimensions on the same ratio width/height as the minimal container.
            var squareRatioAire = Math.Sqrt(minimalContainerHeightWidthRatio * landSquaresByContinent);
            var continentWidth = Convert.ToInt32(squareRatioAire);
            var continentHeight = Convert.ToInt32(squareRatioAire / minimalContainerHeightWidthRatio);

            var continentInfos = new List<List<MapSquareData>>();

            // Puts each continent inside each column of the map.
            int iRect = 0;
            while (iRect < mapRectanglesDim.Length)
            {
                continentInfos.Add(new List<MapSquareData>());

                // Number of sea squares before the continent appears (horizontal).
                var seaWidth = (mapRectanglesDim[iRect].Item1 - continentWidth) / 2;
                if (((mapRectanglesDim[iRect].Item1 - continentWidth) % 2) > 0)
                {
                    seaWidth++;
                }

                // Number of sea squares before the continent appears (vertical).
                // Note : this number is the same for each rectangle.
                var seaHeight = (mapRectanglesDim[iRect].Item2 - continentHeight) / 2;
                if (((mapRectanglesDim[iRect].Item2 - continentHeight) % 2) > 0)
                {
                    seaHeight++;
                }

                // Fills square list.
                var currentContinentWidth = continentWidth;
                var widthFromPreviousRect = mapRectanglesDim.Take(iRect).Sum(x => x.Item1);
                for (int i = widthFromPreviousRect; i < (mapRectanglesDim[iRect].Item1 + widthFromPreviousRect); i++)
                {
                    var currentSeaHeight = seaHeight;
                    var currentContinentHeight = continentHeight;
                    for (int j = 0; j < Height; j++)
                    {
                        if (seaWidth > 0 || currentSeaHeight > 0)
                        {
                            _mapSquareList.Add(new MapSquareData(MapSquareTypeData.Sea, j, i));
                        }
                        else if (currentContinentWidth <= 0 || currentContinentHeight <= 0)
                        {
                            _mapSquareList.Add(new MapSquareData(MapSquareTypeData.Sea, j, i));
                        }
                        else
                        {
                            // Picks a random type of square.
                            var rdmTypeSeed = Tools.Randomizer.Next(0, 20);
                            var squareType = rdmTypeSeed < 12 ? MapSquareTypeData.Grassland : (rdmTypeSeed < 18 ? MapSquareTypeData.Forest : MapSquareTypeData.Mountain);
                            var sq = new MapSquareData(squareType, j, i, MapSquareTypeData.Grassland);
                            _mapSquareList.Add(sq);
                            continentInfos.Last().Add(sq);
                        }
                        if (currentSeaHeight > 0)
                        {
                            currentSeaHeight--;
                        }
                        else if (currentContinentHeight > 0)
                        {
                            currentContinentHeight--;
                        }
                    }
                    if (seaWidth > 0)
                    {
                        seaWidth--;
                    }
                    else if (currentContinentWidth > 0)
                    {
                        currentContinentWidth--;
                    }
                }

                iRect++;
            }

            // Rivers count by continent
            var rivers = (int)Math.Round(landSquaresByContinent * RIVER_COUNT_RATIO);

            // sets rivers
            var riverSquares = new Dictionary<List<Tuple<int, int>>, bool>();
            foreach (var continentLand in continentInfos)
            {
                var topY = continentLand.Min(x => x.Row);
                var leftX = continentLand.Min(x => x.Column);
                var bottomY = continentLand.Max(x => x.Row);
                var rightX = continentLand.Max(x => x.Column);
                // Squares count in height for one square in width
                var ratioHeightWidth = (int)Math.Round((bottomY - topY + 1) / (double)(rightX - leftX + 1));

                for (int i = 0; i < rivers; i++)
                {
                    var river = new List<Tuple<int, int>>();
                    var x = Tools.Randomizer.Next(leftX, rightX + 1);
                    var y = Tools.Randomizer.Next(topY, bottomY + 1);
                    // 0 : left to right / top to bottom, 1 : opposite
                    var reverseDir = Tools.Randomizer.Next(0, 2) == 0;
                    var vertical = Tools.Randomizer.Next(0, (1 * (ratioHeightWidth)) + 1) >= ratioHeightWidth;
                    if (vertical)
                    {
                        if (reverseDir)
                        {
                            // to the top
                            for (int curY = y; curY >= topY; curY--)
                            {
                                river.Add(new Tuple<int, int>(curY, x));
                            }
                        }
                        else
                        {
                            // to the bottom
                            for (int curY = y; curY <= bottomY; curY++)
                            {
                                river.Add(new Tuple<int, int>(curY, x));
                            }
                        }
                    }
                    else
                    {
                        if (reverseDir)
                        {
                            // to the left
                            for (int curX = x; curX >= leftX; curX--)
                            {
                                river.Add(new Tuple<int, int>(y, curX));
                            }
                        }
                        else
                        {
                            // to the right
                            for (int curX = x; curX <= rightX; curX++)
                            {
                                river.Add(new Tuple<int, int>(y, curX));
                            }
                        }
                    }
                    riverSquares.Add(river, vertical);
                }
            }

            foreach (var river in riverSquares.Keys)
            {
                foreach (var riverSq in river)
                {
                    MapSquareList.Single(sq => sq.Column == riverSq.Item2 && sq.Row == riverSq.Item1).SetRiver(riverSquares[river]);
                }
            }
        }

        private int CapProperty(int value, int minIncluded = 1, int maxExcluded = 6)
        {
            if (minIncluded > maxExcluded)
            {
                var tempSwitch = minIncluded;
                minIncluded = maxExcluded;
                maxExcluded = tempSwitch;
            }

            return value < 1 ? 1 : (value >= maxExcluded ? maxExcluded - 1 : value);
        }

        public enum MapSizeEnum
        {
            VerySmall = 1,
            Small,
            Medium,
            Large,
            VeryLarge
        }
    }
}
