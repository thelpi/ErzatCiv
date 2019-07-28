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
        // Multiply the continent squares count by this number to obtain the rivers count
        private const double RIVER_COUNT_RATIO = 0.005;

        // for each type of square, except grassland, sea and cost :
        // - ratio (0,1) of appearance
        // - medium chunk squares count (must be divisible by 3)
        // - true if "hot", false if "cold", null otherwise
        // - underlying type, if "Clear" action available
        private static readonly Dictionary<BiomePivot, Tuple<double, int, bool?, BiomePivot>> CHUNK_TYPE_PROPERTIES =
            new Dictionary<BiomePivot, Tuple<double, int, bool?, BiomePivot>>
            {
                {
                    BiomePivot.Mountain, new Tuple<double, int, bool?, BiomePivot>(0.1, 6, null, null)
                },
                {
                    BiomePivot.Forest, new Tuple<double, int, bool?, BiomePivot>(0.1, 6, null, BiomePivot.Grassland)
                },
                {
                    BiomePivot.Desert, new Tuple<double, int, bool?, BiomePivot>(0.1, 9, null, null)
                },
                {
                    BiomePivot.Jungle, new Tuple<double, int, bool?, BiomePivot>(0.1, 6, null, BiomePivot.Plain)
                },
                {
                    BiomePivot.Swamp, new Tuple<double, int, bool?, BiomePivot>(0.02, 3, null, BiomePivot.Grassland)
                },
                {
                    BiomePivot.Ice, new Tuple<double, int, bool?, BiomePivot>(0.1, 9, null, null)
                },
                {
                    BiomePivot.Toundra, new Tuple<double, int, bool?, BiomePivot>(0.1, 6, null, null)
                },
                {
                    BiomePivot.Hill, new Tuple<double, int, bool?, BiomePivot>(0.1, 3, null, null)
                },
                {
                    BiomePivot.Plain, new Tuple<double, int, bool?, BiomePivot>(0.1, 6, null, null)
                }
            };

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

        internal MapPivot(MapSizeEnum mapSize, int continentCount, double landRatio, TemperaturePivot temperature)
        {
            continentCount = continentCount > 0 && continentCount <= MAX_CONTINENT_COUNT ?
                continentCount : throw new ArgumentException($"Continents count should be between 1 and {MAX_CONTINENT_COUNT} !", nameof(continentCount));
            landRatio = landRatio >= 0.2 && landRatio <= 0.8 ?
                landRatio : throw new ArgumentException("Should between 0.2 et 0.8 !", nameof(landRatio));
            
            Height = MINIMAL_HEIGHT * (int)mapSize;
            Width = Height * RATIO_WIDTH_HEIGHT;

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
            var chunksByType = CHUNK_TYPE_PROPERTIES.ToDictionary(x => x.Key, x => new List<List<Tuple<int, int>>>());
            var riverChunks = new Dictionary<List<Tuple<int, int>>, bool>();

            foreach (var fullLand in continentInfos)
            {
                _mapSquareList.AddRange(fullLand);
                var continentLand = fullLand.Where(ms => !ms.Biome.IsSeaType).ToList();

                var chunksCountByType = CHUNK_TYPE_PROPERTIES.ToDictionary(x => x.Key, x =>
                    (int)Math.Round((continentLand.Count * x.Value.Item1) / x.Value.Item2));
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
                        CHUNK_TYPE_PROPERTIES[chunkType].Item2,
                        topY, leftX, bottomY, rightX, ratioHeightWidth));
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
                        MapSquareList
                            .Single(sq => sq.Same(ofType))
                            .ChangeBiome(type, CHUNK_TYPE_PROPERTIES[type].Item4);
                    }
                }
            }

            coastSquares.ForEach(ms => ms.ChangeBiome(BiomePivot.Coast));
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
                        biome = BiomePivot.Grassland;
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
                var chunkHeight = 1;
                var chunkWidth = chunkSize;
                if (chunkSize >= 3)
                {
                    chunkHeight = chunkSize / 3;
                    chunkWidth = chunkSize / chunkHeight;
                }
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
    }
}
