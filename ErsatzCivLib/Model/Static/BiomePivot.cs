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
        /// Bonus productivity points.
        /// </summary>
        public int BonusProductivity { get; private set; }
        /// <summary>
        /// Bonus commerce points.
        /// </summary>
        public int BonusCommerce { get; private set; }
        /// <summary>
        /// Bonus food points.
        /// </summary>
        public int BonusFood { get; private set; }
        /// <summary>
        /// Bonus appearance's rate
        /// </summary>
        public double BonusApperanceRate { get; private set; }
        /// <summary>
        /// Name of the bonus ressource.
        /// </summary>
        public string BonusName { get; private set; }
        /// <summary>
        /// Defense bonus rate.
        /// </summary>
        public double DefenseBonusRate { get; private set; }
        /// <summary>
        /// Indicates of the type is sea navigable (otherwise, it's ground navigable).
        /// </summary>
        public bool IsSeaType { get; private set; }
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
        /// <summary>
        /// Underlying <see cref="BiomePivot"/> after clearance, if applicable.
        /// </summary>
        internal BiomePivot UnderlyingBiome { get; private set; }

        private List<WorkerActionPivot> _actions;
        /// <summary>
        /// List of available <see cref="WorkerActionPivot"/>.
        /// Doesn't include <see cref="WorkerActionPivot.AlwaysAvailable"/>.
        /// </summary>
        public IReadOnlyCollection<WorkerActionPivot> Actions { get { return _actions; } }

        private List<TemperaturePivot> _temperatures;
        /// <summary>
        /// List of <see cref="TemperaturePivot"/> (depending on latitude) where the biome can appears.
        /// </summary>
        public IReadOnlyCollection<TemperaturePivot> Temperatures { get { return _temperatures; } }

        #endregion

        #region Inferred properties

        /// <summary>
        /// The <see cref="Size"/> value as <c>integer</c>.
        /// </summary>
        public int SizeInt { get { return (int)Size; } }

        #endregion

        private BiomePivot() { }

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
            return obj is BiomePivot && Equals(obj as BiomePivot);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        #endregion

        #region Static instances

        /// <summary>
        /// Grassland.
        /// </summary>
        public static readonly BiomePivot Grassland = new BiomePivot
        {
            Name = "Grassland",
            Commerce = 0,
            Food = 2,
            Productivity = 0,
            DefenseBonusRate = 1,
            _actions = new List<WorkerActionPivot>
            {
                WorkerActionPivot.Irrigate,
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
            UnderlyingBiome = null,
            IsSeaType = false,
            SpeedCost = 1,
            AppearanceRatio = 0.1,
            Size = BiomeSizePivot.Medium,
            Humidity = HumidityPivot.Average,
            Age = AgePivot.Old,
            BonusApperanceRate = 0.5,
            BonusCommerce = 0,
            BonusFood = 0,
            BonusProductivity = 1,
            BonusName = "Resource"
        };
        /// <summary>
        /// Sea.
        /// </summary>
        public static readonly BiomePivot Ocean = new BiomePivot
        {
            Name = "Ocean",
            Commerce = 1,
            Food = 1,
            Productivity = 0,
            DefenseBonusRate = 1,
            _actions = new List<WorkerActionPivot>(),
            _temperatures = new List<TemperaturePivot>(),
            UnderlyingBiome = null,
            IsSeaType = true,
            SpeedCost = 1,
            AppearanceRatio = 0.1,
            Size = BiomeSizePivot.Large,
            Humidity = HumidityPivot.Average,
            Age = AgePivot.Average,
            BonusApperanceRate = 0.05,
            BonusCommerce = 0,
            BonusFood = 2,
            BonusProductivity = 0,
            BonusName = "Fish"
        };
        /// <summary>
        /// Ice.
        /// </summary>
        public static readonly BiomePivot Arctic = new BiomePivot
        {
            Name = "Arctic",
            Commerce = 0,
            Food = 0,
            Productivity = 0,
            DefenseBonusRate = 1,
            _actions = new List<WorkerActionPivot>
            {
                WorkerActionPivot.RailRoad,
                WorkerActionPivot.Road,
                WorkerActionPivot.BuildFortress
            },
            _temperatures = new List<TemperaturePivot>
            {
                TemperaturePivot.Cold
            },
            UnderlyingBiome = null,
            IsSeaType = false,
            SpeedCost = 2,
            AppearanceRatio = 0.1,
            Size = BiomeSizePivot.Large,
            Humidity = HumidityPivot.Dry,
            Age = AgePivot.Average,
            BonusApperanceRate = 0.05,
            BonusCommerce = 0,
            BonusFood = 2,
            BonusProductivity = 0,
            BonusName = "Seals"
        };
        /// <summary>
        /// Toundra.
        /// </summary>
        public static readonly BiomePivot Tundra = new BiomePivot
        {
            Name = "Tundra",
            Commerce = 0,
            Food = 1,
            Productivity = 0,
            DefenseBonusRate = 1,
            _actions = new List<WorkerActionPivot>
            {
                WorkerActionPivot.RailRoad,
                WorkerActionPivot.Road,
                WorkerActionPivot.BuildFortress
            },
            _temperatures = new List<TemperaturePivot>
            {
                TemperaturePivot.Cold
            },
            UnderlyingBiome = null,
            IsSeaType = false,
            SpeedCost = 1,
            AppearanceRatio = 0.1,
            Size = BiomeSizePivot.Medium,
            Humidity = HumidityPivot.Average,
            Age = AgePivot.Old,
            BonusApperanceRate = 0.05,
            BonusCommerce = 0,
            BonusFood = 2,
            BonusProductivity = 0,
            BonusName = "Game"
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
            DefenseBonusRate = 1,
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
            UnderlyingBiome = null,
            IsSeaType = false,
            SpeedCost = 2,
            AppearanceRatio = 0.1,
            Size = BiomeSizePivot.Large,
            Humidity = HumidityPivot.Dry,
            Age = AgePivot.Average,
            BonusApperanceRate = 0.05,
            BonusCommerce = 0,
            BonusFood = 3,
            BonusProductivity = 0,
            BonusName = "Oases"
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
            DefenseBonusRate = 1.5,
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
            UnderlyingBiome = Plains,
            IsSeaType = false,
            SpeedCost = 2,
            AppearanceRatio = 0.1,
            Size = BiomeSizePivot.Medium,
            Humidity = HumidityPivot.Wet,
            Age = AgePivot.Average,
            BonusApperanceRate = 0.05,
            BonusCommerce = 4,
            BonusFood = 0,
            BonusProductivity = 0,
            BonusName = "Gems"
        };
        /// <summary>
        /// Mountain.
        /// </summary>
        public static readonly BiomePivot Mountain = new BiomePivot
        {
            Name = "Mountain",
            Commerce = 0,
            Food = 0,
            Productivity = 1,
            DefenseBonusRate = 3,
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
            UnderlyingBiome = null,
            IsSeaType = false,
            SpeedCost = 3,
            AppearanceRatio = 0.1,
            Size = BiomeSizePivot.Medium,
            Humidity = HumidityPivot.Average,
            Age = AgePivot.New,
            BonusApperanceRate = 0.05,
            BonusCommerce = 6,
            BonusFood = 0,
            BonusProductivity = 0,
            BonusName = "Gold"
        };
        /// <summary>
        /// Hill.
        /// </summary>
        public static readonly BiomePivot Hills = new BiomePivot
        {
            Name = "Hills",
            Commerce = 0,
            Food = 1,
            Productivity = 0,
            DefenseBonusRate = 2,
            _actions = new List<WorkerActionPivot>
            {
                WorkerActionPivot.Mine,
                WorkerActionPivot.Irrigate,
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
            UnderlyingBiome = null,
            IsSeaType = false,
            SpeedCost = 2,
            AppearanceRatio = 0.1,
            Size = BiomeSizePivot.Small,
            Humidity = HumidityPivot.Average,
            Age = AgePivot.New,
            BonusApperanceRate = 0.05,
            BonusCommerce = 0,
            BonusFood = 0,
            BonusProductivity = 2,
            BonusName = "Coal"
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
            DefenseBonusRate = 1.5,
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
            UnderlyingBiome = Grassland,
            IsSeaType = false,
            SpeedCost = 2,
            AppearanceRatio = 0.02,
            Size = BiomeSizePivot.Small,
            Humidity = HumidityPivot.Wet,
            Age = AgePivot.Old,
            BonusApperanceRate = 0.05,
            BonusCommerce = 0,
            BonusFood = 0,
            BonusProductivity = 4,
            BonusName = "Oil"
        };
        /// <summary>
        /// Forest.
        /// </summary>
        public static readonly BiomePivot Forest = new BiomePivot
        {
            Name = "Forest",
            Commerce = 0,
            Food = 1,
            Productivity = 2,
            DefenseBonusRate = 1.5,
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
            UnderlyingBiome = Plains,
            IsSeaType = false,
            SpeedCost = 2,
            AppearanceRatio = 0.1,
            Size = BiomeSizePivot.Medium,
            Humidity = HumidityPivot.Wet,
            Age = AgePivot.Average,
            BonusApperanceRate = 0.05,
            BonusCommerce = 0,
            BonusFood = 2,
            BonusProductivity = 0,
            BonusName = "Game"
        };
        /// <summary>
        /// Plain.
        /// </summary>
        public static readonly BiomePivot Plains = new BiomePivot
        {
            Name = "Plains",
            Commerce = 0,
            Food = 1,
            Productivity = 1,
            DefenseBonusRate = 0,
            _actions = new List<WorkerActionPivot>
            {
                WorkerActionPivot.Irrigate,
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
            UnderlyingBiome = null,
            IsSeaType = false,
            SpeedCost = 1,
            AppearanceRatio = 0.1,
            Size = BiomeSizePivot.Large,
            Humidity = HumidityPivot.Dry,
            Age = AgePivot.Old,
            BonusApperanceRate = 0.05,
            BonusCommerce = 0,
            BonusFood = 0,
            BonusProductivity = 2,
            BonusName = "Horses"
        };
        /*public static readonly BiomePivot River = new BiomePivot
        {
            Name = "River",
            Commerce = 1,
            Food = 2,
            Productivity = 0,
            DefenseBonusRate = 1.5,
            _actions = new List<WorkerActionPivot>
            {
                WorkerActionPivot.Irrigate,
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
            SpeedCost = 1,
            AppearanceRatio = 0.1, // non pertinent
            Size = BiomeSizePivot.Small, // non pertinent
            Humidity = HumidityPivot.Wet,
            Age = AgePivot.Old,
            BonusApperanceRate = 0.5,
            BonusCommerce = 0,
            BonusFood = 0,
            BonusProductivity = 1,
            BonusName = "Resource"
        };*/

        /// <summary>
        /// The biome by default when the map is built.
        /// </summary>
        public static BiomePivot Default { get; } = Plains;

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
