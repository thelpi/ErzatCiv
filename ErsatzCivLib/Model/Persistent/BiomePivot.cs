using System;
using System.Collections.Generic;
using System.Linq;

namespace ErsatzCivLib.Model.Persistent
{
    /// <summary>
    /// Represents a type of square on the map.
    /// </summary>
    [Serializable]
    public class BiomePivot
    {
        private List<WorkerActionPivot> _actions;
        private List<MapPivot.TemperaturePivot> _temperatures;
        private Dictionary<MapPivot.TemperaturePivot, BiomePivot> _underlyingBiomes;

        #region Properties

        /// <summary>
        /// Name.
        /// </summary>
        public string Name { get; private set; }
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
        /// Biome's humidity level.
        /// </summary>
        internal MapPivot.HumidityPivot Humidity { get; private set; }
        /// <summary>
        /// Biome's flatness level.
        /// </summary>
        internal MapPivot.AgePivot Age { get; private set; }

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
            Name = "Grassland",
            Commerce = 1,
            Food = 2,
            Productivity = 1,
            Defense = 0,
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
            Size = BiomeSizePivot.Medium,
            Humidity = MapPivot.HumidityPivot.Average,
            Age = MapPivot.AgePivot.Old
        };
        public static BiomePivot Sea { get; } = new BiomePivot
        {
            Name = "Sea",
            Commerce = 1,
            Food = 1,
            Productivity = 0,
            Defense = 0,
            _actions = new List<WorkerActionPivot>(),
            _temperatures = new List<MapPivot.TemperaturePivot>(),
            _underlyingBiomes = new Dictionary<MapPivot.TemperaturePivot, BiomePivot>(),
            IsSeaType = true,
            IsCityBuildable = false,
            SpeedCost = 1,
            AppearanceRatio = 0.1,
            Size = BiomeSizePivot.Large,
            Humidity = MapPivot.HumidityPivot.Average,
            Age = MapPivot.AgePivot.Average
        };
        public static BiomePivot Ice { get; } = new BiomePivot
        {
            Name = "Ice",
            Commerce = 0,
            Food = 0,
            Productivity = 0,
            Defense = 1,
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
            Size = BiomeSizePivot.Large,
            Humidity = MapPivot.HumidityPivot.Dry,
            Age = MapPivot.AgePivot.Average
        };
        public static BiomePivot Toundra { get; } = new BiomePivot
        {
            Name = "Toundra",
            Commerce = 0,
            Food = 1,
            Productivity = 1,
            Defense = 0,
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
            Size = BiomeSizePivot.Medium,
            Humidity = MapPivot.HumidityPivot.Average,
            Age = MapPivot.AgePivot.Old
        };
        public static BiomePivot Desert { get; } = new BiomePivot
        {
            Name = "Desert",
            Commerce = 0,
            Food = 0,
            Productivity = 1,
            Defense = 1,
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
            Size = BiomeSizePivot.Large,
            Humidity = MapPivot.HumidityPivot.Dry,
            Age = MapPivot.AgePivot.Average
        };
        public static BiomePivot Jungle { get; } = new BiomePivot
        {
            Name = "Jungle",
            Commerce = 0,
            Food = 1,
            Productivity = 0,
            Defense = 2,
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
            Size = BiomeSizePivot.Medium,
            Humidity = MapPivot.HumidityPivot.Wet,
            Age = MapPivot.AgePivot.Average
        };
        public static BiomePivot Mountain { get; } = new BiomePivot
        {
            Name = "Mountain",
            Commerce = 0,
            Food = 0,
            Productivity = 2,
            Defense = 2,
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
            Size = BiomeSizePivot.Medium,
            Humidity = MapPivot.HumidityPivot.Average,
            Age = MapPivot.AgePivot.New
        };
        public static BiomePivot Hill { get; } = new BiomePivot
        {
            Name = "Hill",
            Commerce = 1,
            Food = 1,
            Productivity = 1,
            Defense = 1,
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
            Size = BiomeSizePivot.Small,
            Humidity = MapPivot.HumidityPivot.Average,
            Age = MapPivot.AgePivot.New
        };
        public static BiomePivot Swamp { get; } = new BiomePivot
        {
            Name = "Swamp",
            Commerce = 0,
            Food = 1,
            Productivity = 0,
            Defense = 1,
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
            Size = BiomeSizePivot.Small,
            Humidity = MapPivot.HumidityPivot.Wet,
            Age = MapPivot.AgePivot.Old
        };
        public static BiomePivot Forest { get; } = new BiomePivot
        {
            Name = "Forest",
            Commerce = 1,
            Food = 1,
            Productivity = 2,
            Defense = 1,
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
            Size = BiomeSizePivot.Medium,
            Humidity = MapPivot.HumidityPivot.Wet,
            Age = MapPivot.AgePivot.Average
        };
        public static BiomePivot Plain { get; } = new BiomePivot
        {
            Name = "Plain",
            Commerce = 1,
            Food = 1,
            Productivity = 1,
            Defense = 0,
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
            Size = BiomeSizePivot.Large,
            Humidity = MapPivot.HumidityPivot.Dry,
            Age = MapPivot.AgePivot.Old
        };
        public static BiomePivot Coast { get; } = new BiomePivot
        {
            Name = "Coast",
            Commerce = 1,
            Food = 1,
            Productivity = 0,
            Defense = 0,
            _actions = new List<WorkerActionPivot>(),
            _temperatures = new List<MapPivot.TemperaturePivot>(),
            _underlyingBiomes = new Dictionary<MapPivot.TemperaturePivot, BiomePivot>(),
            IsSeaType = true,
            IsCityBuildable = false,
            SpeedCost = 1,
            AppearanceRatio = 0.1,
            Size = BiomeSizePivot.Small,
            Humidity = MapPivot.HumidityPivot.Average,
            Age = MapPivot.AgePivot.Average
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
        internal int ChunkSquaresCount(int totalSquaresCount, double chunkCoeff, MapPivot.HumidityPivot humidity, MapPivot.AgePivot age)
        {
            if (age == MapPivot.AgePivot.New)
            {
                if (Age == MapPivot.AgePivot.Old)
                {
                    chunkCoeff += 1.5;
                }
                else if (Age == MapPivot.AgePivot.New)
                {
                    chunkCoeff -= 1.5;
                }
            }
            else if (age == MapPivot.AgePivot.Old)
            {
                if (Age == MapPivot.AgePivot.Old)
                {
                    chunkCoeff -= 1.5;
                }
                else if (Age == MapPivot.AgePivot.New)
                {
                    chunkCoeff += 1.5;
                }
            }

            var realRatio = AppearanceRatio;
            if (humidity == MapPivot.HumidityPivot.Dry)
            {
                if (Humidity == MapPivot.HumidityPivot.Dry)
                {
                    realRatio *= 2;
                }
                else if (Humidity == MapPivot.HumidityPivot.Wet)
                {
                    realRatio /= 2;
                }
            }
            else if (humidity == MapPivot.HumidityPivot.Wet)
            {
                if (Humidity == MapPivot.HumidityPivot.Dry)
                {
                    realRatio /= 2;
                }
                else if (Humidity == MapPivot.HumidityPivot.Wet)
                {
                    realRatio *= 2;
                }
            }

            return (int)Math.Round((totalSquaresCount * (realRatio * 3)) / ((int)Size * chunkCoeff));
        }
    }
}
