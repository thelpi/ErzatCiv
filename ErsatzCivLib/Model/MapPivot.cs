using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ErsatzCivLib.Model.Enums;
using ErsatzCivLib.Model.Static;

namespace ErsatzCivLib.Model
{
    /// <summary>
    /// Represents a map mades of <see cref="MapSquarePivot"/>.
    /// </summary>
    /// <seealso cref="IEnumerable{T}"/>
    /// <seealso cref="MapSquarePivot"/>
    [Serializable]
    public class MapPivot : IEnumerable<MapSquarePivot>
    {
        #region Constants relative to map generation

        private const double HUT_APPEARANCE_RATE = 0.01;
        private const int RATIO_WIDTH_HEIGHT = 2;
        private const int MINIMAL_HEIGHT = 20;
        private const int CHUNK_SIZE_RATIO = 5;
        private const double MAX_RATIO_TEMPERATURE = 0.45;
        private const double MIN_RATIO_TEMPERATURE = 0.05;
        private const double AVG_RATIO_TEMPERATURE = 0.25;
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

        #region Embedded properties

        private List<HutPivot> _huts;
        /// <summary>
        /// List of <see cref="HutPivot"/>.
        /// </summary>
        public IReadOnlyCollection<HutPivot> Huts { get { return _huts; } }

        private MapSquarePivot[,] _mapSquareList;
        /// <summary>
        /// Single <see cref="MapSquarePivot"/> access.
        /// </summary>
        /// <param name="row">The row, between <c>0</c> and <see cref="Height"/>.</param>
        /// <param name="column">The column, between <c>0</c> and <see cref="Width"/>.</param>
        /// <returns>The square; <c>Null</c> if <paramref name="column"/> or <paramref name="row"/> is invalid.</returns>
        internal MapSquarePivot this[int row, int column]
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

        #endregion

        #region Inferred properties

        /// <summary>
        /// Inferred; <see cref="Width"/> to <see cref="Height"/> ratio.
        /// </summary>
        public int WidthHeighRatio { get { return Width / Height; } }
        /// <summary>
        /// Distance between a corder and the center of the map.
        /// </summary>
        public double DiagonalRadius { get { return Tools.DistanceBetweenTwoPoints(this[0, 0], this[(Height / 2) - 1, (Width / 2) - 1]); } }

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
        /// <param name="age"><see cref="AgePivot"/></param>
        /// <param name="humidity"><see cref="HumidityPivot"/></param>
        internal MapPivot(SizePivot mapSize, LandShapePivot mapShape, LandCoveragePivot landCoverage,
            TemperaturePivot temperature, AgePivot age, HumidityPivot humidity)
        {
            var continentCount = mapShape == LandShapePivot.Pangaea ? 1 : (
                mapShape == LandShapePivot.Continent ? Tools.Randomizer.Next(CONTINENT_COUNT_MIN, CONTINENT_COUNT_MAX + 1) :
                    Tools.Randomizer.Next(ISLAND_COUNT_MIN, ISLAND_COUNT_MAX + 1)
            );
            var landRatio = LAND_COVERAGE_RATIOS[landCoverage];

            _huts = new List<HutPivot>();
            Height = MINIMAL_HEIGHT * (int)mapSize;
            Width = Height * RATIO_WIDTH_HEIGHT;
            GlobalTemperature = temperature;
            _mapSquareList = new MapSquarePivot[Height, Width];

            var continentInfos = new List<List<MapSquarePivot>>();
            
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

            var cIndex = 0;
            foreach (var boundary in boundaries)
            {
                continentInfos.Add(ConvertContinentBlueprintToMapSquares(landRatio, boundary, cIndex));
                cIndex++;
            }

            // sets chunks
            var chunksByType = BiomePivot.ChunkAppearanceBiomes.ToDictionary(b => b, b => new List<List<Tuple<int, int>>>());
            var rivers = new List<List<Tuple<int, int>>>();
            var huts = new List<Tuple<int, int>>();

            foreach (var fullLand in continentInfos)
            {
                fullLand.ForEach(msloc => _mapSquareList[msloc.Row, msloc.Column] = msloc);
                var continentLand = fullLand.Where(ms => !ms.Biome.IsSeaType).ToList();

                var chunksCountByType =
                    BiomePivot.ChunkAppearanceBiomes
                        .ToDictionary(b => b, b => b.ChunkSquaresCount(continentLand.Count, CHUNK_SIZE_RATIO, humidity, age));
                
                var topY = continentLand.Min(x => x.Row);
                var leftX = continentLand.Min(x => x.Column);
                var bottomY = continentLand.Max(x => x.Row);
                var rightX = continentLand.Max(x => x.Column);
                // Squares count in height for one square in width
                var ratioHeightWidth = (int)Math.Round((bottomY - topY + 1) / (double)(rightX - leftX + 1));

                foreach (var chunkType in chunksByType.Keys)
                {
                    chunksByType[chunkType].AddRange(FiilContinentBlueprintWithBiomeChunks(
                        chunksCountByType[chunkType],
                        chunkType.SizeInt * CHUNK_SIZE_RATIO,
                        topY, leftX, bottomY, rightX, ratioHeightWidth, chunkType.Temperatures));
                }

                // Rivers count on the continent.
                var riversCount = (int)Math.Round(continentLand.Count * BiomePivot.River.AppearanceRate);

                for (int i = 0; i < riversCount; i++)
                {
                    var river = new List<Tuple<int, int>>();

                    // Random river starting point.
                    var riverPt = new Tuple<int, int>(
                        Tools.Randomizer.Next(topY, bottomY),
                        Tools.Randomizer.Next(leftX, rightX)
                    );

                    // The river can go in two directions [(bottom or top) and (left or right)]
                    var forbiddenDirection1 = (DirectionPivot)Tools.Randomizer.Next(2, 4);
                    var forbiddenDirection2 = (DirectionPivot)Tools.Randomizer.Next(0, 2);

                    // Loops until the river reachs the coast, or another river.
                    while (riverPt.Item1 >= topY
                        && riverPt.Item1 <= bottomY
                        && riverPt.Item2 >= leftX
                        && riverPt.Item2 <= rightX
                        && !rivers.Any(r => r.Any(rsquare => rsquare.Item1 == riverPt.Item1 && rsquare.Item2 == riverPt.Item2)))
                    {
                        river.Add(riverPt);

                        DirectionPivot currentDirection;
                        do
                        {
                            // Picks a random direction, which can't be a forbidden one.
                            currentDirection = (DirectionPivot)Tools.Randomizer.Next(0, 4);
                        }
                        while (currentDirection == forbiddenDirection1 || currentDirection == forbiddenDirection2);

                        riverPt = new Tuple<int, int>(
                            riverPt.Item1 + (currentDirection == DirectionPivot.Bottom ? 1 : (currentDirection == DirectionPivot.Top ? -1 : 0)),
                            riverPt.Item2 + (currentDirection == DirectionPivot.Right ? 1 : (currentDirection == DirectionPivot.Left ? -1 : 0))
                        );
                    }

                    rivers.Add(river);
                }

                // Huts count on the continent.
                var hutsCount = (int)Math.Round(continentLand.Count * HUT_APPEARANCE_RATE);

                // Creates hut locations.
                for (int i = 0; i < hutsCount; i++)
                {
                    Tuple<int, int> hutPoint;
                    do
                    {
                        hutPoint = new Tuple<int, int>(
                            Tools.Randomizer.Next(topY, bottomY),
                            Tools.Randomizer.Next(leftX, rightX)
                        );
                    }
                    while (huts.Any(h => h.Item1 == hutPoint.Item1 && h.Item2 == hutPoint.Item2));

                    huts.Add(hutPoint);
                }
            }

            foreach (var type in chunksByType.Keys)
            {
                foreach (var chunkOfType in chunksByType[type])
                {
                    foreach (var ofType in chunkOfType)
                    {
                        var currSq = _mapSquareList[ofType.Item1, ofType.Item2];
                        currSq.ChangeBiome(type);
                    }
                }
            }

            // Overrides chunks with river squares.
            rivers.ForEach(r =>
                r.ForEach(rSquare =>
                    _mapSquareList[rSquare.Item1, rSquare.Item2].ChangeBiome(BiomePivot.River)));

            // Add huts.
            foreach (var hSquare in huts)
            {
                HutPivot hut = null;
                var location = _mapSquareList[hSquare.Item1, hSquare.Item2];
                var typeHut = Tools.Randomizer.Next(0, 6);
                switch (typeHut)
                {
                    case 0:
                        hut = HutPivot.EmptyHut(location);
                        break;
                    case 1:
                        hut = HutPivot.AdvanceHut(location);
                        break;
                    case 2:
                        hut = HutPivot.BarbariansHut(location);
                        break;
                    case 3:
                        hut = HutPivot.SettlerHut(location);
                        break;
                    case 4:
                        hut = HutPivot.FriendlyCavalryUnitHut(location);
                        break;
                    case 5:
                        hut = HutPivot.GoldHut(location);
                        break;
                }
                if (hut != null)
                {
                    _huts.Add(hut);
                }
            }
        }

        #region Internal methods

        /// <summary>
        /// Gets the adjacent squares of a <see cref="MapSquarePivot"/>.
        /// </summary>
        /// <param name="mapSquare">The location.</param>
        /// <param name="radius">The radius.</param>
        /// <returns>List of <see cref="MapSquarePivot"/>.</returns>
        internal IReadOnlyCollection<MapSquarePivot> GetAdjacentMapSquares(MapSquarePivot mapSquare, int radius = 1)
        {
            var squares = new List<MapSquarePivot>();
            for (int i = mapSquare.Row - radius; i <= mapSquare.Row + radius; i++)
            {
                for (int j = mapSquare.Column - radius; j <= mapSquare.Column + radius; j++)
                {
                    var sq = this[i, j];
                    if (sq != null)
                    {
                        squares.Add(sq);
                    }
                }
            }

            return squares.Where(sq => sq != null).ToList();
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

        /// <summary>
        /// Removes an <see cref="HutPivot"/> after discovery.
        /// </summary>
        /// <param name="hut">The hut to remove.</param>
        internal void RemoveHut(HutPivot hut)
        {
            _huts.Remove(hut);
        }

        #endregion

        #region Private methods

        private List<MapSquarePivot> ConvertContinentBlueprintToMapSquares(double landRatio, ContinentBlueprint contBound, int continentIndex)
        {
            var continentWidth = (int)Math.Floor(contBound.Width * landRatio);
            var continentHeight = (int)Math.Floor(contBound.Height * landRatio);
            var maxStartChunkX = (int)Math.Floor((1 - landRatio) * contBound.Width);
            var maxStartChunkY = (int)Math.Floor((1 - landRatio) * contBound.Height);

            var startChunkX = 1;
            do
            {
                startChunkX = Tools.Randomizer.Next(1, maxStartChunkX);
            }
            while (startChunkX + continentWidth > (contBound.Width - 1));

            var startChunkY = 1;
            do
            {
                startChunkY = Tools.Randomizer.Next(1, maxStartChunkY);
            }
            while (startChunkY + continentHeight > (contBound.Height - 1));

            Func<int, bool> IsGroundXFunc = delegate (int x) { return x >= startChunkX + (contBound.StartX - 1) && x <= (startChunkX + (contBound.StartX - 1) + continentWidth); };
            Func<int, bool> IsGroundYFunc = delegate (int y) { return y >= startChunkY + (contBound.StartY - 1) && y <= (startChunkY + (contBound.StartY - 1) + continentHeight); };
            Func<int, int, bool> IsGroundFunc = delegate (int x, int y) { return IsGroundXFunc(x) && IsGroundYFunc(y); };

            var continentSquares = new List<MapSquarePivot>();
            for (var x = contBound.StartX; x <= contBound.LastX; x++)
            {
                for (var y = contBound.StartY; y <= contBound.LastY; y++)
                {
                    var biome = BiomePivot.Ocean;
                    if (IsGroundFunc(x, y))
                    {
                        biome = BiomePivot.Default;
                    }
                    var newSquare = new MapSquarePivot(y, x, biome, continentIndex);
                    continentSquares.Add(newSquare);
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

        #endregion

        /// <inheritdoc />
        public IEnumerator<MapSquarePivot> GetEnumerator()
        {
            return new MapSquareEnumerator(_mapSquareList);
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new MapSquareEnumerator(_mapSquareList);
        }

        // Enumerator for the list of squares inside the map.
        private class MapSquareEnumerator : IEnumerator<MapSquarePivot>
        {
            private MapSquarePivot[,] _mapSquares;

            private int _rowPosition = -1;
            private int _columnPosition = 0;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="mapSquares">Collection of <see cref="MapSquarePivot"/>.</param>
            public MapSquareEnumerator(MapSquarePivot[,] mapSquares)
            {
                _mapSquares = mapSquares;
            }

            /// <inheritdoc />
            public bool MoveNext()
            {
                if (_rowPosition < _mapSquares.GetLength(0) - 1)
                {
                    _rowPosition++;
                }
                else
                {
                    _columnPosition++;
                    _rowPosition = 0;
                }
                return _columnPosition < _mapSquares.GetLength(1);
            }

            /// <inheritdoc />
            public void Reset()
            {
                _rowPosition = -1;
                _columnPosition = 0;
            }

            /// <inheritdoc />
            public void Dispose() { }

            /// <inheritdoc />
            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            /// <inheritdoc />
            public MapSquarePivot Current
            {
                get
                {
                    return _mapSquares[_rowPosition, _columnPosition];
                }
            }
        }

        // Represents a blueprint to create a continent of land squares inside the map.
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
