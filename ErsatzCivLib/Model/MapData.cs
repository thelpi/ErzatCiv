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
        private const double MOUNTAIN_COVER_RATIO = 0.2;
        private const double FOREST_COVER_RATIO = 0.3;
        // THIS VALUE MUST BE DIVISABLE BY 3 !
        private const int MOUNTAIN_CHUNK_COUNT_SQUARE = 6; // 3, 6, 9
        // THIS VALUE MUST BE DIVISABLE BY 3 !
        private const int FOREST_CHUNK_COUNT_SQUARE = 9; // 6, 9, 12

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
                            var sq = new MapSquareData(MapSquareTypeData.Grassland, j, i, MapSquareTypeData.Grassland);
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

            var mountainChunksCount = (int)Math.Round((landSquaresByContinent * MOUNTAIN_COVER_RATIO) / MOUNTAIN_CHUNK_COUNT_SQUARE);
            var forestChunksCount = (int)Math.Round((landSquaresByContinent * FOREST_COVER_RATIO) / FOREST_CHUNK_COUNT_SQUARE);
            var riversChunksCount = (int)Math.Round(landSquaresByContinent * RIVER_COUNT_RATIO);

            // sets moutains and forests
            var mountainChunks = new List<List<Tuple<int, int>>>();
            var forestChunks = new List<List<Tuple<int, int>>>();
            var riverChunks = new Dictionary<List<Tuple<int, int>>, bool>();

            foreach (var continentLand in continentInfos)
            {
                var topY = continentLand.Min(x => x.Row);
                var leftX = continentLand.Min(x => x.Column);
                var bottomY = continentLand.Max(x => x.Row);
                var rightX = continentLand.Max(x => x.Column);
                // Squares count in height for one square in width
                var ratioHeightWidth = (int)Math.Round((bottomY - topY + 1) / (double)(rightX - leftX + 1));

                mountainChunks.AddRange(CreateContinentChunksOFType(mountainChunksCount, MOUNTAIN_CHUNK_COUNT_SQUARE, topY, leftX, bottomY, rightX, ratioHeightWidth));
                forestChunks.AddRange(CreateContinentChunksOFType(forestChunksCount, FOREST_CHUNK_COUNT_SQUARE, topY, leftX, bottomY, rightX, ratioHeightWidth));

                for (int i = 0; i < riversChunksCount; i++)
                {
                    var riverChunk = new List<Tuple<int, int>>();
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
                                riverChunk.Add(new Tuple<int, int>(curY, x));
                            }
                        }
                        else
                        {
                            // to the bottom
                            for (int curY = y; curY <= bottomY; curY++)
                            {
                                riverChunk.Add(new Tuple<int, int>(curY, x));
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
                                riverChunk.Add(new Tuple<int, int>(y, curX));
                            }
                        }
                        else
                        {
                            // to the right
                            for (int curX = x; curX <= rightX; curX++)
                            {
                                riverChunk.Add(new Tuple<int, int>(y, curX));
                            }
                        }
                    }
                    riverChunks.Add(riverChunk, vertical);
                }
            }

            foreach (var riverChunk in riverChunks.Keys)
            {
                foreach (var river in riverChunk)
                {
                    MapSquareList.Single(sq => sq.Column == river.Item2 && sq.Row == river.Item1).SetRiver(riverChunks[riverChunk]);
                }
            }

            foreach (var mountainChunk in mountainChunks)
            {
                foreach (var mountain in mountainChunk)
                {
                    MapSquareList.Single(sq => sq.Column == mountain.Item2 && sq.Row == mountain.Item1).ChangeMapSquareType(MapSquareTypeData.Mountain);
                }
            }

            foreach (var forestChunk in forestChunks)
            {
                foreach (var forest in forestChunk)
                {
                    MapSquareList.Single(sq => sq.Column == forest.Item2 && sq.Row == forest.Item1).ChangeMapSquareType(MapSquareTypeData.Forest, MapSquareTypeData.Grassland);
                }
            }

            var grdList = MapSquareList.Where(ms => !ms.MapSquareType.IsSeaType).ToList();
            var coastList = MapSquareList.Where(ms => ms.MapSquareType == MapSquareTypeData.Sea && grdList.Any(grd => grd.IsClose(ms))).ToList();
            foreach (var coastSq in coastList)
            {
                coastSq.ChangeMapSquareType(MapSquareTypeData.Coast);
            }
        }

        private static List<List<Tuple<int, int>>> CreateContinentChunksOFType(int chunksCount, int chunkCountSquare, int topY, int leftX, int bottomY, int rightX, int ratioHeightWidth)
        {
            var chunksList = new List<List<Tuple<int, int>>>();

            for (int i = 0; i < chunksCount; i++)
            {
                var chunk = new List<Tuple<int, int>>();
                var x = Tools.Randomizer.Next(leftX, rightX + 1);
                var y = Tools.Randomizer.Next(topY, bottomY + 1);
                var vertical = Tools.Randomizer.Next(0, (1 * (ratioHeightWidth)) + 1) >= ratioHeightWidth;
                var chunkSizeSeed = Tools.Randomizer.Next(-1, 2);
                var chunkSize = chunkCountSquare + (chunkSizeSeed * 3);
                var chunkHeight = chunkSize / 3;
                var chunkWidth = chunkSize / chunkHeight;
                if (vertical || (!vertical && chunkWidth < chunkHeight))
                {
                    var tmpWidth = chunkWidth;
                    chunkWidth = chunkHeight;
                    chunkHeight = tmpWidth;
                }
                for (int xI = x; xI < (chunkWidth + x > rightX ? rightX : chunkWidth + x); xI++)
                {
                    for (int yI = y; yI < (chunkHeight + y > bottomY ? bottomY : chunkHeight + y); yI++)
                    {
                        chunk.Add(new Tuple<int, int>(yI, xI));
                    }
                }
                chunksList.Add(chunk);
            }

            return chunksList;
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
