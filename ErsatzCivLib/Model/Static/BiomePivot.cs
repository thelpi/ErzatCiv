using System;
using System.Collections.Generic;
using System.Linq;
using ErsatzCivLib.Model.Enums;

namespace ErsatzCivLib.Model.Static
{
    /// <summary>
    /// Represents a type of square on the map.
    /// </summary>
    /// <seealso cref="IEquatable{T}"/>
    [Serializable]
    public class BiomePivot : IEquatable<BiomePivot>
    {
        private List<WorkerActionPivot> _actions;
        private List<TemperaturePivot> _temperatures;
        private Dictionary<TemperaturePivot, BiomePivot> _underlyingBiomes;

        #region Embedded properties

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
        /// Appearance ratio on map (between 0 and 1).
        /// Is ignored for <see cref="IsSeaType"/> biomes and <see cref="Default"/>.
        /// </summary>
        internal double AppearanceRatio { get; private set; }
        /// <summary>
        /// Medium size of a chunk of this biome.
        /// Is ignored for <see cref="IsSeaType"/> biomes and <see cref="Default"/>.
        /// </summary>
        private BiomeSizePivot Size { get; set; }
        /// <summary>
        /// Biome's humidity level.
        /// </summary>
        internal HumidityPivot Humidity { get; private set; }
        /// <summary>
        /// Biome's flatness level.
        /// </summary>
        internal AgePivot Age { get; private set; }

        #endregion

        #region Inferred properties

        /// <summary>
        /// The <see cref="Size"/> value as <c>integer</c>.
        /// </summary>
        public int SizeInt { get { return (int)Size; } }
        /// <summary>
        /// Underlying biome by temperature (if <see cref="WorkerActionPivot.Clear"/> available).
        /// </summary>
        internal IReadOnlyDictionary<TemperaturePivot, BiomePivot> UnderlyingBiomes { get { return _underlyingBiomes; } }
        /// <summary>
        /// List of available <see cref="WorkerActionPivot"/>.
        /// Doesn't include <see cref="WorkerActionPivot.AlwaysAvailable"/>.
        /// </summary>
        public IReadOnlyCollection<WorkerActionPivot> Actions { get { return _actions; } }
        /// <summary>
        /// List of <see cref="TemperaturePivot"/> (depending on latitude) where the biome can appears.
        /// </summary>
        public IReadOnlyCollection<TemperaturePivot> Temperatures { get { return _temperatures; } }

        #endregion

        private BiomePivot() { }

        #region Static instances

        /// <summary>
        /// Grassland.
        /// </summary>
        public static readonly BiomePivot Grassland = new BiomePivot
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
            _temperatures = new List<TemperaturePivot>
            {
                TemperaturePivot.Hot,
                TemperaturePivot.Temperate
            },
            _underlyingBiomes = new Dictionary<TemperaturePivot, BiomePivot>(),
            IsSeaType = false,
            IsCityBuildable = true,
            SpeedCost = 1,
            AppearanceRatio = 0.1,
            Size = BiomeSizePivot.Medium,
            Humidity = HumidityPivot.Average,
            Age = AgePivot.Old
        };
        /// <summary>
        /// Sea.
        /// </summary>
        public static readonly BiomePivot Sea = new BiomePivot
        {
            Name = "Sea",
            Commerce = 1,
            Food = 1,
            Productivity = 0,
            Defense = 0,
            _actions = new List<WorkerActionPivot>(),
            _temperatures = new List<TemperaturePivot>(),
            _underlyingBiomes = new Dictionary<TemperaturePivot, BiomePivot>(),
            IsSeaType = true,
            IsCityBuildable = false,
            SpeedCost = 1,
            AppearanceRatio = 0.1,
            Size = BiomeSizePivot.Large,
            Humidity = HumidityPivot.Average,
            Age = AgePivot.Average
        };
        /// <summary>
        /// Ice.
        /// </summary>
        public static readonly BiomePivot Ice = new BiomePivot
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
            _temperatures = new List<TemperaturePivot>
            {
                TemperaturePivot.Cold
            },
            _underlyingBiomes = new Dictionary<TemperaturePivot, BiomePivot>(),
            IsSeaType = false,
            IsCityBuildable = false,
            SpeedCost = 2,
            AppearanceRatio = 0.1,
            Size = BiomeSizePivot.Large,
            Humidity = HumidityPivot.Dry,
            Age = AgePivot.Average
        };
        /// <summary>
        /// Toundra.
        /// </summary>
        public static readonly BiomePivot Toundra = new BiomePivot
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
            _temperatures = new List<TemperaturePivot>
            {
                TemperaturePivot.Cold
            },
            _underlyingBiomes = new Dictionary<TemperaturePivot, BiomePivot>(),
            IsSeaType = false,
            IsCityBuildable = true,
            SpeedCost = 1,
            AppearanceRatio = 0.1,
            Size = BiomeSizePivot.Medium,
            Humidity = HumidityPivot.Average,
            Age = AgePivot.Old
        };
        /// <summary>
        /// Desert.
        /// </summary>
        public static readonly BiomePivot Desert = new BiomePivot
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
            _temperatures = new List<TemperaturePivot>
            {
                TemperaturePivot.Hot
            },
            _underlyingBiomes = new Dictionary<TemperaturePivot, BiomePivot>(),
            IsSeaType = false,
            IsCityBuildable = true,
            SpeedCost = 2,
            AppearanceRatio = 0.1,
            Size = BiomeSizePivot.Large,
            Humidity = HumidityPivot.Dry,
            Age = AgePivot.Average
        };
        /// <summary>
        /// Jungle.
        /// </summary>
        public static readonly BiomePivot Jungle = new BiomePivot
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
            _temperatures = new List<TemperaturePivot>
            {
                TemperaturePivot.Hot
            },
            _underlyingBiomes = new Dictionary<TemperaturePivot, BiomePivot>
            {
                { TemperaturePivot.Hot, Plain }
            },
            IsSeaType = false,
            IsCityBuildable = false,
            SpeedCost = 3,
            AppearanceRatio = 0.1,
            Size = BiomeSizePivot.Medium,
            Humidity = HumidityPivot.Wet,
            Age = AgePivot.Average
        };
        /// <summary>
        /// Mountain.
        /// </summary>
        public static readonly BiomePivot Mountain = new BiomePivot
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
            _temperatures = new List<TemperaturePivot>
            {
                TemperaturePivot.Cold,
                TemperaturePivot.Hot,
                TemperaturePivot.Temperate
            },
            _underlyingBiomes = new Dictionary<TemperaturePivot, BiomePivot>(),
            IsSeaType = false,
            IsCityBuildable = false,
            SpeedCost = 3,
            AppearanceRatio = 0.1,
            Size = BiomeSizePivot.Medium,
            Humidity = HumidityPivot.Average,
            Age = AgePivot.New
        };
        /// <summary>
        /// Hill.
        /// </summary>
        public static readonly BiomePivot Hill = new BiomePivot
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
            _temperatures = new List<TemperaturePivot>
            {
                TemperaturePivot.Cold,
                TemperaturePivot.Hot,
                TemperaturePivot.Temperate
            },
            _underlyingBiomes = new Dictionary<TemperaturePivot, BiomePivot>(),
            IsSeaType = false,
            IsCityBuildable = true,
            SpeedCost = 2,
            AppearanceRatio = 0.1,
            Size = BiomeSizePivot.Small,
            Humidity = HumidityPivot.Average,
            Age = AgePivot.New
        };
        /// <summary>
        /// Swamp.
        /// </summary>
        public static readonly BiomePivot Swamp = new BiomePivot
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
            _temperatures = new List<TemperaturePivot>
            {
                TemperaturePivot.Cold,
                TemperaturePivot.Hot,
                TemperaturePivot.Temperate
            },
            _underlyingBiomes = new Dictionary<TemperaturePivot, BiomePivot>
            {
                { TemperaturePivot.Cold, Toundra },
                { TemperaturePivot.Temperate, Grassland },
                { TemperaturePivot.Hot, Plain }
            },
            IsSeaType = false,
            IsCityBuildable = false,
            SpeedCost = 2,
            AppearanceRatio = 0.02,
            Size = BiomeSizePivot.Small,
            Humidity = HumidityPivot.Wet,
            Age = AgePivot.Old
        };
        /// <summary>
        /// Forest.
        /// </summary>
        public static readonly BiomePivot Forest = new BiomePivot
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
            _temperatures = new List<TemperaturePivot>
            {
                TemperaturePivot.Cold,
                TemperaturePivot.Temperate
            },
            _underlyingBiomes = new Dictionary<TemperaturePivot, BiomePivot>
            {
                { TemperaturePivot.Cold, Toundra },
                { TemperaturePivot.Temperate, Grassland }
            },
            IsSeaType = false,
            IsCityBuildable = true,
            SpeedCost = 2,
            AppearanceRatio = 0.1,
            Size = BiomeSizePivot.Medium,
            Humidity = HumidityPivot.Wet,
            Age = AgePivot.Average
        };
        /// <summary>
        /// Plain.
        /// </summary>
        public static readonly BiomePivot Plain = new BiomePivot
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
            _temperatures = new List<TemperaturePivot>
            {
                TemperaturePivot.Cold,
                TemperaturePivot.Hot,
                TemperaturePivot.Temperate
            },
            _underlyingBiomes = new Dictionary<TemperaturePivot, BiomePivot>(),
            IsSeaType = false,
            IsCityBuildable = true,
            SpeedCost = 1,
            AppearanceRatio = 0.1,
            Size = BiomeSizePivot.Large,
            Humidity = HumidityPivot.Dry,
            Age = AgePivot.Old
        };
        /// <summary>
        /// Coast.
        /// </summary>
        public static readonly BiomePivot Coast = new BiomePivot
        {
            Name = "Coast",
            Commerce = 1,
            Food = 1,
            Productivity = 0,
            Defense = 0,
            _actions = new List<WorkerActionPivot>(),
            _temperatures = new List<TemperaturePivot>(),
            _underlyingBiomes = new Dictionary<TemperaturePivot, BiomePivot>(),
            IsSeaType = true,
            IsCityBuildable = false,
            SpeedCost = 1,
            AppearanceRatio = 0.1,
            Size = BiomeSizePivot.Small,
            Humidity = HumidityPivot.Average,
            Age = AgePivot.Average
        };

        /// <summary>
        /// The biome by default when the map is built.
        /// </summary>
        public static BiomePivot Default { get; } = Plain;

        #endregion

        private static List<BiomePivot> _instances = null;

        /// <summary>
        /// List of every instances.
        /// </summary>
        public static IReadOnlyCollection<BiomePivot> Biomes
        {
            get
            {
                if (_instances == null)
                {
                    _instances = Tools.GetInstancesOfTypeFromStaticFields<BiomePivot>();
                }
                return _instances;
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
        /// Computes the number of <see cref="MapSquarePivot"/> for a chunk of the current biome, in the specified context.
        /// </summary>
        /// <param name="totalSquaresCount">Total count of squares in the continent.</param>
        /// <param name="chunkCoeff">Number real of squares in a <see cref="BiomeSizePivot.Small"/> chunk.</param>
        /// <param name="humidity">Level of <see cref="HumidityPivot"/> at the current latitude.</param>
        /// <param name="age">Age of the map.</param>
        /// <returns>Number of squares </returns>
        internal int ChunkSquaresCount(int totalSquaresCount, double chunkCoeff, HumidityPivot humidity, AgePivot age)
        {
            if (age == AgePivot.New)
            {
                if (Age == AgePivot.Old)
                {
                    chunkCoeff += 1.5;
                }
                else if (Age == AgePivot.New)
                {
                    chunkCoeff -= 1.5;
                }
            }
            else if (age == AgePivot.Old)
            {
                if (Age == AgePivot.Old)
                {
                    chunkCoeff -= 1.5;
                }
                else if (Age == AgePivot.New)
                {
                    chunkCoeff += 1.5;
                }
            }

            var realRatio = AppearanceRatio;
            if (humidity == HumidityPivot.Dry)
            {
                if (Humidity == HumidityPivot.Dry)
                {
                    realRatio *= 2;
                }
                else if (Humidity == HumidityPivot.Wet)
                {
                    realRatio /= 2;
                }
            }
            else if (humidity == HumidityPivot.Wet)
            {
                if (Humidity == HumidityPivot.Dry)
                {
                    realRatio /= 2;
                }
                else if (Humidity == HumidityPivot.Wet)
                {
                    realRatio *= 2;
                }
            }

            return (int)Math.Round((totalSquaresCount * (realRatio * 3)) / ((int)Size * chunkCoeff));
        }

        #region IEquatable implementation

        /// <summary>
        /// Checks if this instance is equal to another one.
        /// </summary>
        /// <param name="other">The other instance.</param>
        /// <returns><c>True</c> if equals; <c>False</c> otherwise.</returns>
        public bool Equals(BiomePivot other)
        {
            return Name == other?.Name;
        }

        /// <summary>
        /// Operator "==" override. Checks equality between two instances.
        /// </summary>
        /// <param name="r1">The first <see cref="BiomePivot"/>.</param>
        /// <param name="r2">The second <see cref="BiomePivot"/>.</param>
        /// <returns><c>True</c> if equals; <c>False</c> otherwise.</returns>
        public static bool operator ==(BiomePivot r1, BiomePivot r2)
        {
            if (r1 is null)
            {
                return r2 is null;
            }

            return r1.Equals(r2) == true;
        }

        /// <summary>
        /// Operator "!=" override. Checks non-equality between two instances.
        /// </summary>
        /// <param name="r1">The first <see cref="BiomePivot"/>.</param>
        /// <param name="r2">The second <see cref="BiomePivot"/>.</param>
        /// <returns><c>False</c> if equals; <c>True</c> otherwise.</returns>
        public static bool operator !=(BiomePivot r1, BiomePivot r2)
        {
            return !(r1 == r2);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is RegimePivot && Equals(obj as RegimePivot);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        #endregion

        // sizes of biome.
        [Serializable]
        private enum BiomeSizePivot
        {
            Small = 1,
            Medium,
            Large
        }
    }
}
