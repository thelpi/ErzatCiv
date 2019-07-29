using System;
using System.Collections.Generic;
using System.Linq;
using ErsatzCivLib.Model.Persistent;

namespace ErsatzCivLib.Model
{
    [Serializable]
    public class MapPivot : BasePivot
    {
        #region Constants (private)

        private const int RATIO_WIDTH_HEIGHT = 2;
        private const int MINIMAL_HEIGHT = 20;
        private const int CHUNK_SIZE_RATIO = 5;
        private const double MAX_RATIO_TEMPERATURE = 0.4;
        private const double MIN_RATIO_TEMPERATURE = 0.1;
        private const double AVG_RATIO_TEMPERATURE = 0.25;
        private const double RIVER_STARTER_RATIO = 0.005;
        private const int CONTINENT_COUNT_MIN = 3;
        private const int CONTINENT_COUNT_MAX = 6;
        private const int ISLAND_COUNT_MIN = 10;
        private const int ISLAND_COUNT_MAX = 16; // ALWAYS A 2^X !!!
        private const double MIN_SPLIT_RATIO = 0.35;
        private const double MAX_SPLIT_RATIO = 0.65;
        private static readonly Dictionary<LandCoveragePivot, double> LAND_COVERAGE_RATIOS = new Dictionary<LandCoveragePivot, double>
        {
            { LandCoveragePivot.VeryLow, 0.3 },
            { LandCoveragePivot.Low, 0.45 },
            { LandCoveragePivot.Medium, 0.6 },
            { LandCoveragePivot.High, 0.75 },
            { LandCoveragePivot.VeryHigh, 0.9 }
        };

        #endregion

        private MapSquarePivot[,] _mapSquareList;

        #region Properties (public)

        /// <summary>
        /// Single <see cref="MapSquarePivot"/> access.
        /// </summary>
        /// <param name="row">The row, between <c>0</c> and <see cref="Height"/>.</param>
        /// <param name="column">The column, between <c>0</c> and <see cref="Width"/>.</param>
        /// <returns>The square; <c>Null</c> if <paramref name="column"/> or <paramref name="row"/> is invalid.</returns>
        public MapSquarePivot this[int row, int column]
        {
            get
            {
                if (row < 0 || row >= Height || column < 0 || column >= Width)
                {
                    return null;
                }
                return _mapSquareList[row, column];
            }
        }
        /// <summary>
        /// Width.
        /// </summary>
        public int Width { get; private set; }
        /// <summary>
        /// Height.
        /// </summary>
        public int Height { get; private set; }
        /// <summary>
        /// Global temperature.
        /// </summary>
        public TemperaturePivot GlobalTemperature { get; private set; }
        /// <summary>
        /// Inferred; <see cref="Width"/> to <see cref="Height"/> ratio.
        /// </summary>
        public int WidthHeighRatio { get { return Width / Height; } }

        #endregion

        #region Properties (private)

        private double ColdRatio { get { return GlobalTemperature == TemperaturePivot.Temperate ? AVG_RATIO_TEMPERATURE : (GlobalTemperature == TemperaturePivot.Cold ? MAX_RATIO_TEMPERATURE : MIN_RATIO_TEMPERATURE); } }
        private double HotRatio { get { return GlobalTemperature == TemperaturePivot.Temperate ? AVG_RATIO_TEMPERATURE : (GlobalTemperature == TemperaturePivot.Hot ? MAX_RATIO_TEMPERATURE : MIN_RATIO_TEMPERATURE); } }
        private double TemperateRatio { get { return 1 - (ColdRatio + HotRatio); } }
        private int TemperateNorthTopBorder { get { return (int)Math.Round(Height * (ColdRatio / 2)); } }
        private int HotTopBorder { get { return (int)Math.Round(Height * (TemperateRatio / 2)) + TemperateNorthTopBorder; } }
        private int TemperateSouthTopBorder { get { return (int)Math.Round(Height * HotRatio) + HotTopBorder; } }
        private int ColdSouthTopBorder { get { return (int)Math.Round(Height * (TemperateRatio / 2)) + TemperateSouthTopBorder; } }

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="mapSize"><see cref="SizePivot"/></param>
        /// <param name="mapShape"><see cref="LandShapePivot"/></param>
        /// <param name="landCoverage"><see cref="LandCoveragePivot"/></param>
        /// <param name="temperature"><see cref="TemperaturePivot"/></param>
        internal MapPivot(Engine owner, SizePivot mapSize, LandShapePivot mapShape, LandCoveragePivot landCoverage, TemperaturePivot temperature) : base(owner)
        {
            var continentCount = mapShape == LandShapePivot.Pangaea ? 1 : (
                mapShape == LandShapePivot.Continent ? Tools.Randomizer.Next(CONTINENT_COUNT_MIN, CONTINENT_COUNT_MAX + 1) :
                    Tools.Randomizer.Next(ISLAND_COUNT_MIN, ISLAND_COUNT_MAX + 1)
            );
            var landRatio = LAND_COVERAGE_RATIOS[landCoverage];
            
            Height = MINIMAL_HEIGHT * (int)mapSize;
            Width = Height * RATIO_WIDTH_HEIGHT;
            GlobalTemperature = temperature;
            _mapSquareList = new MapSquarePivot[Height, Width];

            var continentInfos = new List<List<Tuple<MapSquarePivot, int, int>>>();
            var coastSquares = new List<MapSquarePivot>();
            
            var boundaries = new List<ContinentBlueprint>();
            Action<ContinentBlueprint> SplitX = delegate (ContinentBlueprint contPick)
            {
                var splitX = Tools.Randomizer.Next((int)Math.Floor(MIN_SPLIT_RATIO * contPick.Width), (int)Math.Floor(MAX_SPLIT_RATIO * contPick.Width));
                boundaries.Add(new ContinentBlueprint(splitX, contPick.Height, contPick.StartX, contPick.StartY));
                boundaries.Add(new ContinentBlueprint(contPick.Width - splitX, contPick.Height, contPick.StartX + splitX, contPick.StartY));
            };
            Action<ContinentBlueprint> SplitY = delegate (ContinentBlueprint contPick)
            {
                var splitY = Tools.Randomizer.Next((int)Math.Floor(MIN_SPLIT_RATIO * contPick.Height), (int)Math.Floor(MAX_SPLIT_RATIO * contPick.Height));
                boundaries.Add(new ContinentBlueprint(contPick.Width, splitY, contPick.StartX, contPick.StartY));
                boundaries.Add(new ContinentBlueprint(contPick.Width, contPick.Height - splitY, contPick.StartX, contPick.StartY + splitY));
            };
            Action<int, bool> Split = delegate (int pickIndex, bool inY)
            {
                var contPick = boundaries[pickIndex];
                if (inY)
                {
                    SplitY(contPick);
                }
                else
                {
                    SplitX(contPick);
                }
                boundaries.Remove(contPick);
            };

            List<int> ranges = new List<int>();
            var cpt = ISLAND_COUNT_MAX;
            while (cpt > 1)
            {
                ranges.Add(cpt);
                cpt /= 2;
            }

            for (int i = 1; i <= continentCount; i++)
            {
                if (boundaries.Count == 0)
                {
                    boundaries.Add(new ContinentBlueprint(Width, Height, 0, 0));
                }
                else
                {
                    var nextTypeI = ranges.Where(x => x > boundaries.Count).Min();
                    Split(
                        Tools.Randomizer.Next(0, nextTypeI - boundaries.Count),
                        ranges.Any(x => x == Math.Sqrt(nextTypeI))
                    );
                }
            }

            foreach (var boundary in boundaries)
            {
                var tmpCoastSquares = new List<MapSquarePivot>();
                continentInfos.Add(ConvertContinentBlueprintToMapSquares(landRatio, boundary, out tmpCoastSquares));
                coastSquares.AddRange(tmpCoastSquares);
            }

            // sets chunks and rivers
            var chunksByType = BiomePivot.NonSeaAndNonDefaultBiomes.ToDictionary(b => b, b => new List<List<Tuple<int, int>>>());
            var riverChunks = new Dictionary<List<Tuple<int, int>>, bool>();

            foreach (var fullLand in continentInfos)
            {
                fullLand.ForEach(msloc => _mapSquareList[msloc.Item2, msloc.Item3] = msloc.Item1);
                var continentLand = fullLand.Where(ms => !ms.Item1.Biome.IsSeaType).ToList();

                var chunksCountByType = BiomePivot.NonSeaAndNonDefaultBiomes
                                            .ToDictionary(b => b, b => b.ChunkSquaresCount(continentLand.Count, CHUNK_SIZE_RATIO));
                var riversChunksCount = (int)Math.Round(continentLand.Count * RIVER_STARTER_RATIO);

                var topY = continentLand.Min(x => x.Item2);
                var leftX = continentLand.Min(x => x.Item3);
                var bottomY = continentLand.Max(x => x.Item2);
                var rightX = continentLand.Max(x => x.Item3);
                // Squares count in height for one square in width
                var ratioHeightWidth = (int)Math.Round((bottomY - topY + 1) / (double)(rightX - leftX + 1));

                foreach (var chunkType in chunksByType.Keys)
                {
                    chunksByType[chunkType].AddRange(FiilContinentBlueprintWithBiomeChunks(
                        chunksCountByType[chunkType],
                        (int)chunkType.Size * CHUNK_SIZE_RATIO,
                        topY, leftX, bottomY, rightX, ratioHeightWidth, chunkType.Temperatures));
                }

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
                    _mapSquareList[river.Item1, river.Item2].SetRiver(riverChunks[riverChunk]);
                }
            }

            foreach (var type in chunksByType.Keys)
            {
                foreach (var chunkOfType in chunksByType[type])
                {
                    foreach (var ofType in chunkOfType)
                    {
                        var currSq = _mapSquareList[ofType.Item1, ofType.Item2];
                        type.UnderlyingBiomes.TryGetValue(TemperatureAt(ofType.Item1), out BiomePivot underlyingBiome);
                        currSq.ChangeBiome(type, underlyingBiome ?? BiomePivot.Default);
                    }
                }
            }

            coastSquares.ForEach(ms => ms.ChangeBiome(BiomePivot.Coast));
        }

        /// <summary>
        /// Computes the <see cref="TemperaturePivot"/> at a specified height.
        /// </summary>
        /// <param name="y">The height.</param>
        /// <returns>The level of temperature.</returns>
        internal TemperaturePivot TemperatureAt(int y)
        {
            if (y >= HotTopBorder && y < TemperateSouthTopBorder)
            {
                return TemperaturePivot.Hot;
            }
            else if (y < TemperateNorthTopBorder || y >= ColdSouthTopBorder)
            {
                return TemperaturePivot.Cold;
            }
            else
            {
                return TemperaturePivot.Temperate;
            }
        }

        private List<Tuple<MapSquarePivot, int, int>> ConvertContinentBlueprintToMapSquares(double landRatio, ContinentBlueprint contBound, out List<MapSquarePivot> costSquares)
        {
            costSquares = new List<MapSquarePivot>();
            var startChunkX = 0;
            var startChunkY = 0;
            var continentWidth = 0;
            var continentHeight = 0;
            do
            {
                var maxStartChunkX = (int)Math.Floor((1 - landRatio) * contBound.Width);
                var maxStartChunkY = (int)Math.Floor((1 - landRatio) * contBound.Height);
                startChunkX = Tools.Randomizer.Next(0, maxStartChunkX);
                startChunkY = Tools.Randomizer.Next(0, maxStartChunkY);
                
                continentWidth = (int)Math.Floor(contBound.Width * landRatio);
                continentHeight = (int)Math.Floor(contBound.Height * landRatio);
            }
            while (startChunkX + continentWidth > contBound.Width || startChunkY + continentHeight > contBound.Height);

            Func<int, bool> IsGroundXFunc = delegate(int x) { return x >= startChunkX + (contBound.StartX - 1) && x <= (startChunkX + (contBound.StartX - 1) + continentWidth); };
            Func<int, bool> IsGroundYFunc = delegate(int y) { return y >= startChunkY + (contBound.StartY - 1) && y <= (startChunkY + (contBound.StartY - 1) + continentHeight); };
            Func<int, int, bool> IsGroundFunc = delegate (int x, int y) { return IsGroundXFunc(x) && IsGroundYFunc(y); };

            var continentSquares = new List<Tuple<MapSquarePivot, int, int>>();
            for (var x = contBound.StartX; x <= contBound.LastX; x++)
            {
                for (var y = contBound.StartY; y <= contBound.LastY; y++)
                {
                    var biome = BiomePivot.Sea;
                    bool isCoast = false;
                    if (IsGroundFunc(x, y))
                    {
                        biome = BiomePivot.Default;
                    }
                    else if (IsGroundFunc(x + 1, y) || IsGroundFunc(x - 1, y) || IsGroundFunc(x, y + 1) || IsGroundFunc(x, y - 1)
                        || IsGroundFunc(x - 1, y - 1) || IsGroundFunc(x + 1, y + 1) || IsGroundFunc(x - 1, y + 1) || IsGroundFunc(x + 1, y - 1))
                    {
                        isCoast = true;
                    }
                    var newSquare = new MapSquarePivot(Owner, biome, null);
                    continentSquares.Add(new Tuple<MapSquarePivot, int, int>(newSquare, y, x));
                    if (isCoast)
                    {
                        costSquares.Add(newSquare);
                    }
                }
            }

            return continentSquares;
        }

        private List<List<Tuple<int, int>>> FiilContinentBlueprintWithBiomeChunks(int chunksCount, int chunkCountSquare,
            int topY, int leftX, int bottomY, int rightX, int ratioHeightWidth, IReadOnlyCollection<TemperaturePivot> temperatures)
        {
            var chunksList = new List<List<Tuple<int, int>>>();

            for (int i = 0; i < chunksCount; i++)
            {
                var chunk = new List<Tuple<int, int>>();
                var x = Tools.Randomizer.Next(leftX, rightX + 1);
                var y = Tools.Randomizer.Next(topY, bottomY + 1);
                var vertical = Tools.Randomizer.Next(0, (1 * (ratioHeightWidth)) + 1) >= ratioHeightWidth;
                var chunkSizeSeed = Tools.Randomizer.Next(-1, 2);
                var chunkSize = chunkCountSquare + (chunkSizeSeed * CHUNK_SIZE_RATIO);
                var chunkHeight = 1;
                var chunkWidth = chunkSize;
                if (chunkSize >= CHUNK_SIZE_RATIO)
                {
                    chunkHeight = chunkSize / CHUNK_SIZE_RATIO;
                    chunkWidth = chunkSize / chunkHeight;
                }
                if (vertical || (!vertical && chunkWidth < chunkHeight))
                {
                    var tmpWidth = chunkWidth;
                    chunkWidth = chunkHeight;
                    chunkHeight = tmpWidth;
                }

                var endX = chunkWidth + x > rightX ? rightX : chunkWidth + x;
                var endY = chunkHeight + y > bottomY ? bottomY : chunkHeight + y;
                if (!temperatures.Contains(TemperatureAt(endY)) && !temperatures.Contains(TemperatureAt(y)))
                {
                    continue;
                }

                for (int xI = x; xI < endX; xI++)
                {
                    for (int yI = y; yI < endY; yI++)
                    {
                        chunk.Add(new Tuple<int, int>(yI, xI));
                    }
                }
                chunksList.Add(chunk);
            }

            return chunksList;
        }

        /// <summary>
        /// Represents the map size.
        /// </summary>
        [Serializable]
        public enum SizePivot
        {
            /// <summary>
            /// Very small.
            /// </summary>
            VerySmall = 1, // do not change the indice !
            /// <summary>
            /// Small.
            /// </summary>
            Small,
            /// <summary>
            /// Medium.
            /// </summary>
            Medium,
            /// <summary>
            /// Large.
            /// </summary>
            Large,
            /// <summary>
            /// Very large.
            /// </summary>
            VeryLarge
        }

        /// <summary>
        /// Represents the land coverage of the map.
        /// </summary>
        [Serializable]
        public enum LandCoveragePivot
        {
            /// <summary>
            /// Very low.
            /// </summary>
            VeryLow,
            /// <summary>
            /// Low.
            /// </summary>
            Low,
            /// <summary>
            /// Medium.
            /// </summary>
            Medium,
            /// <summary>
            /// High.
            /// </summary>
            High,
            /// <summary>
            /// Very high.
            /// </summary>
            VeryHigh
        }

        /// <summary>
        /// Represents the land organization inside the map.
        /// </summary>
        [Serializable]
        public enum LandShapePivot
        {
            /// <summary>
            /// Single pangaea.
            /// </summary>
            Pangaea,
            /// <summary>
            /// Few continents.
            /// </summary>
            Continent,
            /// <summary>
            /// Several islands.
            /// </summary>
            Island
        }

        /// <summary>
        /// Levels of temperature.
        /// </summary>
        [Serializable]
        public enum TemperaturePivot
        {
            /// <summary>
            /// Cold.
            /// </summary>
            Cold,
            /// <summary>
            /// Temperate.
            /// </summary>
            Temperate,
            /// <summary>
            /// Hot.
            /// </summary>
            Hot
        }

        // Tool to represent a continent square.
        private class ContinentBlueprint
        {
            public int Width { get; private set; }
            public int Height { get; private set; }
            public int StartX { get; private set; }
            public int StartY { get; private set; }
            public int LastX { get { return StartX + Width - 1; } }
            public int LastY { get { return StartY + Height - 1; } }

            public ContinentBlueprint(int width, int height, int startX, int startY)
            {
                Width = width;
                Height = height;
                StartX = startX;
                StartY = startY;
            }
        }
    }
}
