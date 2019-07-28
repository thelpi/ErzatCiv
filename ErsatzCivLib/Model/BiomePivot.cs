using System;
using System.Collections.Generic;
using System.Linq;

namespace ErsatzCivLib.Model
{
    /// <summary>
    /// Represents a type of square on the map.
    /// </summary>
    public class BiomePivot
    {
        /// <summary>
        /// Productivity, food and commerce bonus when the square is through by a river.
        /// </summary>
        public const int RIVER_BONUS = 1;

        #region Properties

        private List<WorkerActionPivot> _actions;
        private List<MapPivot.TemperaturePivot> _temperatures;
        private Dictionary<MapPivot.TemperaturePivot, BiomePivot> _underlyingBiomes;

        /// <summary>
        /// Default productivity points.
        /// </summary>
        public int Productivity { get; private set; }
        /// <summary>
        /// Default commerce points.
        /// </summary>
        public int Commerce { get; private set; }
        /// <summary>
        /// Default food points.
        /// </summary>
        public int Food { get; private set; }
        /// <summary>
        /// Default defensive points.
        /// </summary>
        public int Defense { get; private set; }
        /// <summary>
        /// Render value.
        /// </summary>
        /// <remarks>Path to image; hexadecimal color code.</remarks>
        public string RenderValue { get; private set; }
        /// <summary>
        /// Type of render.
        /// </summary>
        public RenderTypePivot RenderType { get; private set; }
        /// <summary>
        /// Indicates if the square map can be crossed by a river.
        /// </summary>
        public bool RiverCrossable { get; private set; }
        /// <summary>
        /// Indicates of the type is sea navigable (otherwise, it's ground navigable).
        /// </summary>
        public bool IsSeaType { get; private set; }
        /// <summary>
        /// Indicates of a city can be built on it.
        /// </summary>
        public bool IsCityBuildable { get; private set; }
        /// <summary>
        /// Indicates the speed cost when a unit walks through it.
        /// </summary>
        public int SpeedCost { get; private set; }

        /// <summary>
        /// Available <see cref="WorkerActionPivot"/>.
        /// Doesn't include <see cref="WorkerActionPivot.AlwaysAvailable"/>.
        /// </summary>
        public IReadOnlyCollection<WorkerActionPivot> Actions
        {
            get { return _actions; }
        }
        /// <summary>
        /// List of <see cref="TemperaturePivot"/> (depending on latitude) where the biome can appears.
        /// </summary>
        public IReadOnlyCollection<MapPivot.TemperaturePivot> Temperatures
        {
            get { return _temperatures; }
        }

        /// <summary>
        /// Appearance ratio on map (between 0 and 1).
        /// Is ignored for <see cref="IsSeaType"/> biomes and <see cref="Default"/>.
        /// </summary>
        internal double AppearanceRatio { get; private set; }
        /// <summary>
        /// Medium size of a chunk of this biome.
        /// Is ignored for <see cref="IsSeaType"/> biomes and <see cref="Default"/>.
        /// </summary>
        internal BiomeSizePivot Size { get; private set; }

        /// <summary>
        /// Underlying biome by temperature (if <see cref="WorkerActionPivot.Clear"/> available).
        /// </summary>
        internal IReadOnlyDictionary<MapPivot.TemperaturePivot, BiomePivot> UnderlyingBiomes
        {
            get { return _underlyingBiomes; }
        }

        #endregion

        private BiomePivot() { }

        #region Direct access to instances

        public static BiomePivot Grassland { get; } = new BiomePivot
        {
            Commerce = 1,
            Food = 2,
            Productivity = 1,
            Defense = 0,
            RenderValue = "#32CD32",
            RenderType = RenderTypePivot.PlainBrush,
            RiverCrossable = true,
            _actions = new List<WorkerActionPivot>
                {
                    WorkerActionPivot.Irrigate,
                    WorkerActionPivot.Mine,
                    WorkerActionPivot.Plant,
                    WorkerActionPivot.RailRoad,
                    WorkerActionPivot.Road,
                    WorkerActionPivot.BuildFortress
                },
            _temperatures = new List<MapPivot.TemperaturePivot>
            {
                MapPivot.TemperaturePivot.Hot,
                MapPivot.TemperaturePivot.Temperate
            },
            _underlyingBiomes = new Dictionary<MapPivot.TemperaturePivot, BiomePivot>(),
            IsSeaType = false,
            IsCityBuildable = true,
            SpeedCost = 1,
            AppearanceRatio = 0.1,
            Size = BiomeSizePivot.Medium
        };
        public static BiomePivot Sea { get; } = new BiomePivot
        {
            Commerce = 1,
            Food = 1,
            Productivity = 0,
            Defense = 0,
            RenderValue = "#1E90FF",
            RenderType = RenderTypePivot.PlainBrush,
            RiverCrossable = false,
            _actions = new List<WorkerActionPivot>(),
            _temperatures = new List<MapPivot.TemperaturePivot>(),
            _underlyingBiomes = new Dictionary<MapPivot.TemperaturePivot, BiomePivot>(),
            IsSeaType = true,
            IsCityBuildable = false,
            SpeedCost = 1,
            AppearanceRatio = 0.1,
            Size = BiomeSizePivot.Large
        };
        public static BiomePivot Ice { get; } = new BiomePivot
        {
            Commerce = 0,
            Food = 0,
            Productivity = 0,
            Defense = 1,
            RenderValue = "#FFFAF0",
            RenderType = RenderTypePivot.PlainBrush,
            RiverCrossable = true,
            _actions = new List<WorkerActionPivot>
                {
                    WorkerActionPivot.Mine,
                    WorkerActionPivot.RailRoad,
                    WorkerActionPivot.Road,
                    WorkerActionPivot.BuildFortress
                },
            _temperatures = new List<MapPivot.TemperaturePivot>
            {
                MapPivot.TemperaturePivot.Cold
            },
            _underlyingBiomes = new Dictionary<MapPivot.TemperaturePivot, BiomePivot>(),
            IsSeaType = false,
            IsCityBuildable = false,
            SpeedCost = 2,
            AppearanceRatio = 0.1,
            Size = BiomeSizePivot.Large
        };
        public static BiomePivot Toundra { get; } = new BiomePivot
        {
            Commerce = 0,
            Food = 1,
            Productivity = 1,
            Defense = 0,
            RenderValue = "#2F4F4F",
            RenderType = RenderTypePivot.PlainBrush,
            RiverCrossable = true,
            _actions = new List<WorkerActionPivot>
                {
                    WorkerActionPivot.Irrigate,
                    WorkerActionPivot.Mine,
                    WorkerActionPivot.Plant,
                    WorkerActionPivot.RailRoad,
                    WorkerActionPivot.Road,
                    WorkerActionPivot.BuildFortress
                },
            _temperatures = new List<MapPivot.TemperaturePivot>
            {
                MapPivot.TemperaturePivot.Cold
            },
            _underlyingBiomes = new Dictionary<MapPivot.TemperaturePivot, BiomePivot>(),
            IsSeaType = false,
            IsCityBuildable = true,
            SpeedCost = 1,
            AppearanceRatio = 0.1,
            Size = BiomeSizePivot.Medium
        };
        public static BiomePivot Desert { get; } = new BiomePivot
        {
            Commerce = 0,
            Food = 0,
            Productivity = 1,
            Defense = 1,
            RenderValue = "#FF7F50",
            RenderType = RenderTypePivot.PlainBrush,
            RiverCrossable = true,
            _actions = new List<WorkerActionPivot>
                {
                    WorkerActionPivot.Irrigate,
                    WorkerActionPivot.Mine,
                    WorkerActionPivot.RailRoad,
                    WorkerActionPivot.Road,
                    WorkerActionPivot.BuildFortress
                },
            _temperatures = new List<MapPivot.TemperaturePivot>
            {
                MapPivot.TemperaturePivot.Hot
            },
            _underlyingBiomes = new Dictionary<MapPivot.TemperaturePivot, BiomePivot>(),
            IsSeaType = false,
            IsCityBuildable = true,
            SpeedCost = 2,
            AppearanceRatio = 0.1,
            Size = BiomeSizePivot.Large
        };
        public static BiomePivot Jungle { get; } = new BiomePivot
        {
            Commerce = 0,
            Food = 1,
            Productivity = 0,
            Defense = 2,
            RenderValue = "#9ACD32",
            RenderType = RenderTypePivot.PlainBrush,
            RiverCrossable = true,
            _actions = new List<WorkerActionPivot>
                {
                    WorkerActionPivot.Clear,
                    WorkerActionPivot.RailRoad,
                    WorkerActionPivot.Road,
                    WorkerActionPivot.BuildFortress
                },
            _temperatures = new List<MapPivot.TemperaturePivot>
            {
                MapPivot.TemperaturePivot.Hot
            },
            _underlyingBiomes = new Dictionary<MapPivot.TemperaturePivot, BiomePivot>
            {
                { MapPivot.TemperaturePivot.Hot, Plain }
            },
            IsSeaType = false,
            IsCityBuildable = false,
            SpeedCost = 3,
            AppearanceRatio = 0.1,
            Size = BiomeSizePivot.Medium
        };
        public static BiomePivot Mountain { get; } = new BiomePivot
        {
            Commerce = 0,
            Food = 0,
            Productivity = 2,
            Defense = 2,
            RenderValue = "#A52A2A",
            RenderType = RenderTypePivot.PlainBrush,
            RiverCrossable = true,
            _actions = new List<WorkerActionPivot>
                {
                    WorkerActionPivot.Mine,
                    WorkerActionPivot.RailRoad,
                    WorkerActionPivot.Road,
                    WorkerActionPivot.BuildFortress
                },
            _temperatures = new List<MapPivot.TemperaturePivot>
            {
                MapPivot.TemperaturePivot.Cold,
                MapPivot.TemperaturePivot.Hot,
                MapPivot.TemperaturePivot.Temperate
            },
            _underlyingBiomes = new Dictionary<MapPivot.TemperaturePivot, BiomePivot>(),
            IsSeaType = false,
            IsCityBuildable = false,
            SpeedCost = 3,
            AppearanceRatio = 0.1,
            Size = BiomeSizePivot.Medium
        };
        public static BiomePivot Hill { get; } = new BiomePivot
        {
            Commerce = 1,
            Food = 1,
            Productivity = 1,
            Defense = 1,
            RenderValue = "#556B2F",
            RenderType = RenderTypePivot.PlainBrush,
            RiverCrossable = true,
            _actions = new List<WorkerActionPivot>
                {
                    WorkerActionPivot.Mine,
                    WorkerActionPivot.RailRoad,
                    WorkerActionPivot.Road,
                    WorkerActionPivot.BuildFortress
                },
            _temperatures = new List<MapPivot.TemperaturePivot>
            {
                MapPivot.TemperaturePivot.Cold,
                MapPivot.TemperaturePivot.Hot,
                MapPivot.TemperaturePivot.Temperate
            },
            _underlyingBiomes = new Dictionary<MapPivot.TemperaturePivot, BiomePivot>(),
            IsSeaType = false,
            IsCityBuildable = true,
            SpeedCost = 2,
            AppearanceRatio = 0.1,
            Size = BiomeSizePivot.Small
        };
        public static BiomePivot Swamp { get; } = new BiomePivot
        {
            Commerce = 0,
            Food = 1,
            Productivity = 0,
            Defense = 1,
            RenderValue = "#3CB371",
            RenderType = RenderTypePivot.PlainBrush,
            RiverCrossable = true,
            _actions = new List<WorkerActionPivot>
                {
                    WorkerActionPivot.RailRoad,
                    WorkerActionPivot.Road,
                    WorkerActionPivot.BuildFortress,
                    WorkerActionPivot.Clear
                },
            _temperatures = new List<MapPivot.TemperaturePivot>
            {
                MapPivot.TemperaturePivot.Cold,
                MapPivot.TemperaturePivot.Hot,
                MapPivot.TemperaturePivot.Temperate
            },
            _underlyingBiomes = new Dictionary<MapPivot.TemperaturePivot, BiomePivot>
            {
                { MapPivot.TemperaturePivot.Cold, Toundra },
                { MapPivot.TemperaturePivot.Temperate, Grassland },
                { MapPivot.TemperaturePivot.Hot, Plain }
            },
            IsSeaType = false,
            IsCityBuildable = false,
            SpeedCost = 2,
            AppearanceRatio = 0.02,
            Size = BiomeSizePivot.Small
        };
        public static BiomePivot Forest { get; } = new BiomePivot
        {
            Commerce = 1,
            Food = 1,
            Productivity = 2,
            Defense = 1,
            RenderValue = "#006400",
            RenderType = RenderTypePivot.PlainBrush,
            RiverCrossable = true,
            _actions = new List<WorkerActionPivot>
                {
                    WorkerActionPivot.Clear,
                    WorkerActionPivot.RailRoad,
                    WorkerActionPivot.Road,
                    WorkerActionPivot.BuildFortress
                },
            _temperatures = new List<MapPivot.TemperaturePivot>
            {
                MapPivot.TemperaturePivot.Cold,
                MapPivot.TemperaturePivot.Temperate
            },
            _underlyingBiomes = new Dictionary<MapPivot.TemperaturePivot, BiomePivot>
            {
                { MapPivot.TemperaturePivot.Cold, Toundra },
                { MapPivot.TemperaturePivot.Temperate, Grassland }
            },
            IsSeaType = false,
            IsCityBuildable = true,
            SpeedCost = 2,
            AppearanceRatio = 0.1,
            Size = BiomeSizePivot.Medium
        };
        public static BiomePivot Plain { get; } = new BiomePivot
        {
            Commerce = 1,
            Food = 1,
            Productivity = 1,
            Defense = 0,
            RenderValue = "#EEE8AA",
            RenderType = RenderTypePivot.PlainBrush,
            RiverCrossable = true,
            _actions = new List<WorkerActionPivot>
                {
                    WorkerActionPivot.Irrigate,
                    WorkerActionPivot.Mine,
                    WorkerActionPivot.Plant,
                    WorkerActionPivot.RailRoad,
                    WorkerActionPivot.Road,
                    WorkerActionPivot.BuildFortress
                },
            _temperatures = new List<MapPivot.TemperaturePivot>
            {
                MapPivot.TemperaturePivot.Cold,
                MapPivot.TemperaturePivot.Hot,
                MapPivot.TemperaturePivot.Temperate
            },
            _underlyingBiomes = new Dictionary<MapPivot.TemperaturePivot, BiomePivot>(),
            IsSeaType = false,
            IsCityBuildable = true,
            SpeedCost = 1,
            AppearanceRatio = 0.1,
            Size = BiomeSizePivot.Large
        };
        public static BiomePivot Coast { get; } = new BiomePivot
        {
            Commerce = 1,
            Food = 1,
            Productivity = 0,
            Defense = 0,
            RenderValue = "#00BFFF",
            RenderType = RenderTypePivot.PlainBrush,
            RiverCrossable = false,
            _actions = new List<WorkerActionPivot>(),
            _temperatures = new List<MapPivot.TemperaturePivot>(),
            _underlyingBiomes = new Dictionary<MapPivot.TemperaturePivot, BiomePivot>(),
            IsSeaType = true,
            IsCityBuildable = false,
            SpeedCost = 1,
            AppearanceRatio = 0.1,
            Size = BiomeSizePivot.Small
        };

        /// <summary>
        /// The biome by default when the map is built.
        /// </summary>
        public static BiomePivot Default = Plain;

        #endregion

        private static List<BiomePivot> _biomes = null;
        /// <summary>
        /// List of every biomes.
        /// </summary>
        public static IReadOnlyCollection<BiomePivot> Biomes
        {
            get
            {
                if (_biomes == null)
                {
                    _biomes = new List<BiomePivot>
                    {
                        Sea,
                        Coast,
                        Jungle,
                        Desert,
                        Swamp,
                        Forest,
                        Toundra,
                        Ice,
                        Mountain,
                        Hill,
                        Grassland,
                        Plain
                    };
                }
                return _biomes;
            }
        }
        /// <summary>
        /// List of every biomes except <see cref="IsSeaType"/> ones and <see cref="Default"/> one.
        /// </summary>
        internal static IReadOnlyCollection<BiomePivot> NonSeaAndNonDefaultBiomes
        {
            get
            {
                return Biomes.Where(b => !b.IsSeaType && b != Default).ToList();
            }
        }

        /// <summary>
        /// Number of 
        /// </summary>
        /// <param name="totalSquaresCount">Total count of squares in the continent.</param>
        /// <param name="chunkCoeff">Number real of squares in a <see cref="BiomeSizePivot.Small"/> chunk.</param>
        /// <returns>Number of squares </returns>
        internal int ChunkSquaresCount(int totalSquaresCount, int chunkCoeff)
        {
            return (int)Math.Round((totalSquaresCount * (AppearanceRatio * 3)) / ((int)Size * chunkCoeff));
        }
    }
}
