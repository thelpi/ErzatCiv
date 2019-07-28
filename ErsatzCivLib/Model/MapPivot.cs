using System;
using System.Collections.Generic;
using System.Linq;

namespace ErsatzCivLib.Model
{
    public class MapPivot
    {
        public const int MAX_CONTINENT_COUNT = 16;

        public const int RATIO_WIDTH_HEIGHT = 2;
        private const int MINIMAL_HEIGHT = 20;
        private const int CHUNK_SIZE_RATIO = 5;

        private const double RIVER_COUNT_RATIO = 0.005; // Multiply the continent squares count by this number to obtain the rivers count

        private readonly List<MapSquarePivot> _mapSquareList = new List<MapSquarePivot>();

        public IReadOnlyCollection<MapSquarePivot> MapSquareList
        {
            get
            {
                return _mapSquareList;
            }
        }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public TemperaturePivot GlobalTemperature { get; private set; }

        public double ColdRatio { get { return GlobalTemperature == TemperaturePivot.Temperate ? 0.25 : (GlobalTemperature == TemperaturePivot.Cold ? 0.4 : 0.1); } }
        public double HotRatio { get { return GlobalTemperature == TemperaturePivot.Temperate ? 0.25 : (GlobalTemperature == TemperaturePivot.Hot ? 0.4 : 0.1); } }
        public double TemperateRatio { get { return 1 - (ColdRatio + HotRatio); } }

        public int TemperateNorthTopBorder { get { return (int)Math.Round(Height * (ColdRatio / 2)); } }
        public int HotTopBorder { get { return (int)Math.Round(Height * (TemperateRatio / 2)) + TemperateNorthTopBorder; } }
        public int TemperateSouthTopBorder { get { return (int)Math.Round(Height * HotRatio) + HotTopBorder; } }
        public int ColdSouthTopBorder { get { return (int)Math.Round(Height * (TemperateRatio / 2)) + TemperateSouthTopBorder; } }

        internal MapPivot(MapSizeEnum mapSize, MapLandShape mapShape, MapLandCoverage landCoverage, TemperaturePivot temperature)
        {
            // Values can be changed, but not over MAX_CONTINENT_COUNT
            var continentCount = mapShape == MapLandShape.Pangaea ? 1 : (
                mapShape == MapLandShape.Continent ? Tools.Randomizer.Next(3, 6) : Tools.Randomizer.Next(10, 15)
            );
            var landRatio = landCoverage == MapLandCoverage.VeryLow ? 0.3 : (
                landCoverage == MapLandCoverage.Low ? 0.45 : (
                    landCoverage == MapLandCoverage.Medium ? 0.6 : (
                        landCoverage == MapLandCoverage.High ? 0.75 : 0.9)));
            
            Height = MINIMAL_HEIGHT * (int)mapSize;
            Width = Height * RATIO_WIDTH_HEIGHT;
            GlobalTemperature = temperature;

            var continentInfos = new List<List<MapSquarePivot>>();
            var coastSquares = new List<MapSquarePivot>();
            
            var boundaries = new List<ContSquare>();
            Action<ContSquare> SplitX = delegate (ContSquare contPick)
            {
                var splitX = Tools.Randomizer.Next((int)Math.Floor(0.35 * contPick.Width), (int)Math.Floor(0.65 * contPick.Width));
                boundaries.Add(new ContSquare(splitX, contPick.Height, contPick.StartX, contPick.StartY));
                boundaries.Add(new ContSquare(contPick.Width - splitX, contPick.Height, contPick.StartX + splitX, contPick.StartY));
            };
            Action<ContSquare> SplitY = delegate (ContSquare contPick)
            {
                var splitY = Tools.Randomizer.Next((int)Math.Floor(0.35 * contPick.Height), (int)Math.Floor(0.65 * contPick.Height));
                boundaries.Add(new ContSquare(contPick.Width, splitY, contPick.StartX, contPick.StartY));
                boundaries.Add(new ContSquare(contPick.Width, contPick.Height - splitY, contPick.StartX, contPick.StartY + splitY));
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
            var cpt = MAX_CONTINENT_COUNT;
            while (cpt > 1)
            {
                ranges.Add(cpt);
                cpt /= 2;
            }

            for (int i = 1; i <= continentCount; i++)
            {
                if (boundaries.Count == 0)
                {
                    boundaries.Add(new ContSquare(Width, Height, 0, 0));
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
                continentInfos.Add(CreateContinentChunks(landRatio, boundary, out tmpCoastSquares));
                coastSquares.AddRange(tmpCoastSquares);
            }

            // sets chunks and rivers
            var chunksByType = BiomePivot.NonSeaAndNonDefaultBiomes.ToDictionary(b => b, b => new List<List<Tuple<int, int>>>());
            var riverChunks = new Dictionary<List<Tuple<int, int>>, bool>();

            foreach (var fullLand in continentInfos)
            {
                _mapSquareList.AddRange(fullLand);
                var continentLand = fullLand.Where(ms => !ms.Biome.IsSeaType).ToList();

                var chunksCountByType = BiomePivot.NonSeaAndNonDefaultBiomes
                                            .ToDictionary(b => b, b => b.ChunkSquaresCount(continentLand.Count, CHUNK_SIZE_RATIO));
                var riversChunksCount = (int)Math.Round(continentLand.Count * RIVER_COUNT_RATIO);

                var topY = continentLand.Min(x => x.Row);
                var leftX = continentLand.Min(x => x.Column);
                var bottomY = continentLand.Max(x => x.Row);
                var rightX = continentLand.Max(x => x.Column);
                // Squares count in height for one square in width
                var ratioHeightWidth = (int)Math.Round((bottomY - topY + 1) / (double)(rightX - leftX + 1));

                foreach (var chunkType in chunksByType.Keys)
                {
                    chunksByType[chunkType].AddRange(CreateContinentChunksOFType(
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
                    MapSquareList.Single(sq => sq.Same(river)).SetRiver(riverChunks[riverChunk]);
                }
            }

            foreach (var type in chunksByType.Keys)
            {
                foreach (var chunkOfType in chunksByType[type])
                {
                    foreach (var ofType in chunkOfType)
                    {
                        var currSq = MapSquareList.Single(sq => sq.Same(ofType));
                        type.UnderlyingBiomes.TryGetValue(TemperatureAt(currSq.Row), out BiomePivot underlyingBiome);
                        currSq.ChangeBiome(type, underlyingBiome ?? BiomePivot.Default);
                    }
                }
            }

            coastSquares.ForEach(ms => ms.ChangeBiome(BiomePivot.Coast));
        }

        public TemperaturePivot TemperatureAt(int y)
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

        private static List<MapSquarePivot> CreateContinentChunks(double landRatio, ContSquare contBound, out List<MapSquarePivot> costSquares)
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

            var continentSquares = new List<MapSquarePivot>();
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
                    var newSquare = new MapSquarePivot(biome, y, x, null);
                    continentSquares.Add(newSquare);
                    if (isCoast)
                    {
                        costSquares.Add(newSquare);
                    }
                }
            }

            return continentSquares;
        }

        private List<List<Tuple<int, int>>> CreateContinentChunksOFType(int chunksCount, int chunkCountSquare,
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

        public enum MapSizeEnum
        {
            VerySmall = 1,
            Small,
            Medium,
            Large,
            VeryLarge
        }

        private class ContSquare
        {
            public int Width { get; private set; }
            public int Height { get; private set; }
            public int StartX { get; private set; }
            public int StartY { get; private set; }
            public int LastX { get { return StartX + Width - 1; } }
            public int LastY { get { return StartY + Height - 1; } }

            public ContSquare(int width, int height, int startX, int startY)
            {
                Width = width;
                Height = height;
                StartX = startX;
                StartY = startY;
            }

            public override string ToString()
            {
                return string.Concat(Width, " - ", Height);
            }
        }

        public enum MapLandCoverage
        {
            VeryLow = 1,
            Low,
            Medium,
            High,
            VeryHigh
        }

        public enum MapLandShape
        {
            Pangaea = 1,
            Continent,
            Island
        }
    }
}
