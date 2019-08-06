using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ErsatzCivLib.Model.MapEnums;
using ErsatzCivLib.Model.Persistent;

namespace ErsatzCivLib.Model
{
    /// <summary>
    /// Represents a map mades of <see cref="MapSquarePivot"/>.
    /// </summary>
    /// <seealso cref="IEnumerable{T}"/>
    [Serializable]
    public class MapPivot : IEnumerable<MapSquarePivot>
    {
        #region Constants (private)

        private const int RATIO_WIDTH_HEIGHT = 2;
        private const int MINIMAL_HEIGHT = 20;
        private const int CHUNK_SIZE_RATIO = 5;
        private const double MAX_RATIO_TEMPERATURE = 0.45;
        private const double MIN_RATIO_TEMPERATURE = 0.05;
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

        #region Public properties

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
        /// <summary>
        /// Inferred; <see cref="Width"/> to <see cref="Height"/> ratio.
        /// </summary>
        public int WidthHeighRatio { get { return Width / Height; } }

        #endregion

        #region Private properties

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
            
            Height = MINIMAL_HEIGHT * (int)mapSize;
            Width = Height * RATIO_WIDTH_HEIGHT;
            GlobalTemperature = temperature;
            _mapSquareList = new MapSquarePivot[Height, Width];

            var continentInfos = new List<List<MapSquarePivot>>();
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

            // sets chunks
            var chunksByType = BiomePivot.NonSeaAndNonDefaultBiomes.ToDictionary(b => b, b => new List<List<Tuple<int, int>>>());

            // List of every rivers (on every continents).
            var riversPoints = new List<List<Tuple<int, int>>>();

            // Delegate to check if a specified point is already a river point.
            var ExistRiver = new Func<Tuple<int, int>, bool>(delegate (Tuple<int, int> coordinates)
            {
                return riversPoints.Any(riverPts =>
                    riverPts.Any(rivPt =>
                        rivPt.Item1 == coordinates.Item1 && rivPt.Item2 == coordinates.Item2));
            });

            foreach (var fullLand in continentInfos)
            {
                fullLand.ForEach(msloc => _mapSquareList[msloc.Row, msloc.Column] = msloc);
                var continentLand = fullLand.Where(ms => !ms.Biome.IsSeaType).ToList();

                var chunksCountByType =
                    BiomePivot.NonSeaAndNonDefaultBiomes
                        .ToDictionary(b => b, b => b.ChunkSquaresCount(continentLand.Count, CHUNK_SIZE_RATIO, humidity, age));
                var riversChunksCount = (int)Math.Round(continentLand.Count * RIVER_STARTER_RATIO);

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

                for (var iRiver = 0; iRiver < riversChunksCount; iRiver++)
                {
                    // Note : a river shouldn't start on coast.
                    var riverPoint = new Tuple<int, int>(
                        Tools.Randomizer.Next(topY + 1, bottomY + 1),
                        Tools.Randomizer.Next(leftX + 1, rightX + 1)
                    );

                    // No river source at a point where a river already exists.
                    if (ExistRiver(riverPoint))
                    {
                        continue;
                    }

                    var currentRiver = new List<Tuple<int, int>> { riverPoint };

                    // A river can go randomly in 3 directions, but can't go back and forth.
                    var forbiddenCardinal = (DirectionPivot)Tools.Randomizer.Next(0, 4);

                    // The process stops when the sea (or another river) is reached.
                    var reachOtherRiver = false;
                    var outOfContinentBounds = false;
                    while (!reachOtherRiver && !outOfContinentBounds)
                    {
                        // Find the next point (going back to a previous point is forbidden).
                        Tuple<int, int> tmpRiverPoint = null;
                        do
                        {
                            // Randomize a cardinal (until not forbidden).
                            DirectionPivot nextCardinal;
                            do
                            {
                                nextCardinal = (DirectionPivot)Tools.Randomizer.Next(0, 4);
                            }
                            while (nextCardinal == forbiddenCardinal);

                            switch (nextCardinal)
                            {
                                case DirectionPivot.Bottom:
                                    tmpRiverPoint = new Tuple<int, int>(riverPoint.Item1 + 1, riverPoint.Item2);
                                    break;
                                case DirectionPivot.Top:
                                    tmpRiverPoint = new Tuple<int, int>(riverPoint.Item1 - 1, riverPoint.Item2);
                                    break;
                                case DirectionPivot.Right:
                                    tmpRiverPoint = new Tuple<int, int>(riverPoint.Item1, riverPoint.Item2 + 1);
                                    break;
                                case DirectionPivot.Left:
                                    tmpRiverPoint = new Tuple<int, int>(riverPoint.Item1, riverPoint.Item2 - 1);
                                    break;
                            }
                        }
                        while (currentRiver.Any(cr => cr.Item1 == tmpRiverPoint.Item1 && cr.Item2 == tmpRiverPoint.Item2));

                        // Do NOT remove this line and use "tmpRiverPoint" directly.
                        riverPoint = tmpRiverPoint;

                        reachOtherRiver = ExistRiver(riverPoint);
                        outOfContinentBounds = !continentLand.Any(msq => msq.Row == riverPoint.Item1 && msq.Column == riverPoint.Item2);

                        currentRiver.Add(riverPoint);
                    }

                    riversPoints.Add(currentRiver);
                }
            }

            // We don't care about one-point rivers.
            riversPoints.RemoveAll(r => r.Count < 2);

            foreach (var riverPoints in riversPoints)
            {
                // Starts at the second point (the first is "inert" as it gives no indication on direction)
                for (int i = 1; i < riverPoints.Count; i++)
                {
                    SetMapSquareRiversAroundPoint(riverPoints, i);
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

        #region Public and internal methods

        /// <summary>
        /// Gets the <c>8</c> adjacent squares of a <see cref="MapSquarePivot"/>.
        /// Invalid values are cleaned, so the final list might not contains <c>8</c> elements.
        /// </summary>
        /// <param name="mapSquare">The location.</param>
        /// <returns>List of <see cref="MapSquarePivot"/>, one by <see cref="DirectionPivot"/>.</returns>
        internal IReadOnlyDictionary<DirectionPivot, MapSquarePivot> GetAdjacentMapSquares(MapSquarePivot mapSquare)
        {
            var msList = new Dictionary<DirectionPivot, MapSquarePivot>
            {
                { DirectionPivot.TopLeft, this[mapSquare.Row - 1, mapSquare.Column - 1] },
                { DirectionPivot.Left, this[mapSquare.Row, mapSquare.Column - 1] },
                { DirectionPivot.BottomLeft, this[mapSquare.Row + 1, mapSquare.Column - 1] },
                { DirectionPivot.Top, this[mapSquare.Row - 1, mapSquare.Column] },
                { DirectionPivot.Bottom, this[mapSquare.Row + 1, mapSquare.Column] },
                { DirectionPivot.TopRight, this[mapSquare.Row - 1, mapSquare.Column + 1] },
                { DirectionPivot.Right, this[mapSquare.Row, mapSquare.Column + 1] },
                { DirectionPivot.BottomRight, this[mapSquare.Row + 1, mapSquare.Column + 1] },
            };

            return msList.Where(x => x.Value != null).ToDictionary(x => x.Key, x => x.Value);
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

        #endregion

        #region Private methods

        private List<MapSquarePivot> ConvertContinentBlueprintToMapSquares(double landRatio, ContinentBlueprint contBound, out List<MapSquarePivot> costSquares)
        {
            costSquares = new List<MapSquarePivot>();

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
                    var newSquare = new MapSquarePivot(y, x, biome, null);
                    continentSquares.Add(newSquare);
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

        private void SetMapSquareRiversAroundPoint(List<Tuple<int, int>> riverPoints, int i)
        {
            // "MapSquarePivot" around the current point.
            var topRightSquare = this[riverPoints[i].Item1 - 1, riverPoints[i].Item2];
            var topLeftSquare = this[riverPoints[i].Item1 - 1, riverPoints[i].Item2 - 1];
            var bottomRightSquare = this[riverPoints[i].Item1, riverPoints[i].Item2];
            var bottomLeftSquare = this[riverPoints[i].Item1, riverPoints[i].Item2 - 1];

            var deltaRow = riverPoints[i].Item1 - riverPoints[i - 1].Item1;
            var deltaCol = riverPoints[i].Item2 - riverPoints[i - 1].Item2;

            if (deltaRow > 0)
            {
                topRightSquare?.SetRiver(DirectionPivot.Left, true);
                topLeftSquare?.SetRiver(DirectionPivot.Right, true);
            }
            else if (deltaRow < 0)
            {
                bottomRightSquare?.SetRiver(DirectionPivot.Left, true);
                bottomLeftSquare?.SetRiver(DirectionPivot.Right, true);
            }

            if (deltaCol < 0)
            {
                topRightSquare?.SetRiver(DirectionPivot.Bottom, true);
                bottomRightSquare?.SetRiver(DirectionPivot.Top, true);
            }
            else if (deltaCol > 0)
            {
                topLeftSquare?.SetRiver(DirectionPivot.Bottom, true);
                bottomLeftSquare?.SetRiver(DirectionPivot.Top, true);
            }
        }

        #endregion

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
