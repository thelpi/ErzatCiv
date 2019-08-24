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
        /// Indicates the speed cost when a unit walks through it.
        /// </summary>
        public int SpeedCost { get; private set; }
        /// <summary>
        /// Appearance rate on map (between 0 and 1); for <see cref="ChunkAppearanceBiomes"/> only.
        /// </summary>
        internal double AppearanceRate { get; private set; }
        /// <summary>
        /// Medium size of a chunk of this biome; for <see cref="ChunkAppearanceBiomes"/> only.
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
        internal Func<IEnumerable<BiomePivot>, BiomePivot> UnderlyingBiome { get; private set; }

        private List<MapSquareImprovementPivot> _actions;
        /// <summary>
        /// List of available <see cref="MapSquareImprovementPivot"/>.
        /// Doesn't include <see cref="MapSquareImprovementPivot.AlwaysAvailable"/>.
        /// </summary>
        public IReadOnlyCollection<MapSquareImprovementPivot> Actions { get { return _actions; } }

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
        /// <summary>
        /// Indicates of the type is sea navigable (otherwise, it's ground navigable).
        /// </summary>
        public bool IsSeaType { get { return this == Ocean; } }

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

            var realRatio = AppearanceRate;
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
            _actions = new List<MapSquareImprovementPivot>
            {
                MapSquareImprovementPivot.Irrigate,
                MapSquareImprovementPivot.Plant,
                MapSquareImprovementPivot.RailRoad,
                MapSquareImprovementPivot.Road,
                MapSquareImprovementPivot.BuildFortress
            },
            _temperatures = new List<TemperaturePivot>
            {
                TemperaturePivot.Hot,
                TemperaturePivot.Temperate
            },
            UnderlyingBiome = null,
            SpeedCost = 1,
            AppearanceRate = 0.1,
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
            _actions = new List<MapSquareImprovementPivot>(),
            _temperatures = new List<TemperaturePivot>(),
            UnderlyingBiome = null,
            SpeedCost = 1,
            AppearanceRate = 0.1,
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
            _actions = new List<MapSquareImprovementPivot>
            {
                MapSquareImprovementPivot.RailRoad,
                MapSquareImprovementPivot.Road,
                MapSquareImprovementPivot.BuildFortress
            },
            _temperatures = new List<TemperaturePivot>
            {
                TemperaturePivot.Cold
            },
            UnderlyingBiome = null,
            SpeedCost = 2,
            AppearanceRate = 0.1,
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
            _actions = new List<MapSquareImprovementPivot>
            {
                MapSquareImprovementPivot.RailRoad,
                MapSquareImprovementPivot.Road,
                MapSquareImprovementPivot.BuildFortress
            },
            _temperatures = new List<TemperaturePivot>
            {
                TemperaturePivot.Cold
            },
            UnderlyingBiome = null,
            SpeedCost = 1,
            AppearanceRate = 0.1,
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
            _actions = new List<MapSquareImprovementPivot>
            {
                MapSquareImprovementPivot.Irrigate,
                MapSquareImprovementPivot.Mine,
                MapSquareImprovementPivot.RailRoad,
                MapSquareImprovementPivot.Road,
                MapSquareImprovementPivot.BuildFortress
            },
            _temperatures = new List<TemperaturePivot>
            {
                TemperaturePivot.Hot
            },
            UnderlyingBiome = null,
            SpeedCost = 2,
            AppearanceRate = 0.1,
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
            _actions = new List<MapSquareImprovementPivot>
            {
                MapSquareImprovementPivot.Clear,
                MapSquareImprovementPivot.RailRoad,
                MapSquareImprovementPivot.Road,
                MapSquareImprovementPivot.BuildFortress
            },
            _temperatures = new List<TemperaturePivot>
            {
                TemperaturePivot.Hot
            },
            UnderlyingBiome = delegate(IEnumerable<BiomePivot> biomes) { return biomes?.SingleOrDefault(b => b == Plains); },
            SpeedCost = 2,
            AppearanceRate = 0.1,
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
            _actions = new List<MapSquareImprovementPivot>
            {
                MapSquareImprovementPivot.Mine,
                MapSquareImprovementPivot.RailRoad,
                MapSquareImprovementPivot.Road,
                MapSquareImprovementPivot.BuildFortress
            },
            _temperatures = new List<TemperaturePivot>
            {
                TemperaturePivot.Cold,
                TemperaturePivot.Hot,
                TemperaturePivot.Temperate
            },
            UnderlyingBiome = null,
            SpeedCost = 3,
            AppearanceRate = 0.1,
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
            _actions = new List<MapSquareImprovementPivot>
            {
                MapSquareImprovementPivot.Mine,
                MapSquareImprovementPivot.Irrigate,
                MapSquareImprovementPivot.RailRoad,
                MapSquareImprovementPivot.Road,
                MapSquareImprovementPivot.BuildFortress
            },
            _temperatures = new List<TemperaturePivot>
            {
                TemperaturePivot.Cold,
                TemperaturePivot.Hot,
                TemperaturePivot.Temperate
            },
            UnderlyingBiome = null,
            SpeedCost = 2,
            AppearanceRate = 0.1,
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
            _actions = new List<MapSquareImprovementPivot>
            {
                MapSquareImprovementPivot.RailRoad,
                MapSquareImprovementPivot.Road,
                MapSquareImprovementPivot.BuildFortress,
                MapSquareImprovementPivot.Clear
            },
            _temperatures = new List<TemperaturePivot>
            {
                TemperaturePivot.Cold,
                TemperaturePivot.Hot,
                TemperaturePivot.Temperate
            },
            UnderlyingBiome = delegate (IEnumerable<BiomePivot> biomes) { return biomes?.SingleOrDefault(b => b == Grassland); },
            SpeedCost = 2,
            AppearanceRate = 0.02,
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
            _actions = new List<MapSquareImprovementPivot>
            {
                MapSquareImprovementPivot.Clear,
                MapSquareImprovementPivot.RailRoad,
                MapSquareImprovementPivot.Road,
                MapSquareImprovementPivot.BuildFortress
            },
            _temperatures = new List<TemperaturePivot>
            {
                TemperaturePivot.Cold,
                TemperaturePivot.Temperate
            },
            UnderlyingBiome = delegate (IEnumerable<BiomePivot> biomes) { return biomes?.SingleOrDefault(b => b == Plains); },
            SpeedCost = 2,
            AppearanceRate = 0.1,
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
            DefenseBonusRate = 1,
            _actions = new List<MapSquareImprovementPivot>
            {
                MapSquareImprovementPivot.Irrigate,
                MapSquareImprovementPivot.Plant,
                MapSquareImprovementPivot.RailRoad,
                MapSquareImprovementPivot.Road,
                MapSquareImprovementPivot.BuildFortress
            },
            _temperatures = new List<TemperaturePivot>
            {
                TemperaturePivot.Cold,
                TemperaturePivot.Hot,
                TemperaturePivot.Temperate
            },
            UnderlyingBiome = null,
            SpeedCost = 1,
            AppearanceRate = 0.1,
            Size = BiomeSizePivot.Large,
            Humidity = HumidityPivot.Dry,
            Age = AgePivot.Old,
            BonusApperanceRate = 0.05,
            BonusCommerce = 0,
            BonusFood = 0,
            BonusProductivity = 2,
            BonusName = "Horses"
        };
        /// <summary>
        /// River.
        /// </summary>
        public static readonly BiomePivot River = new BiomePivot
        {
            Name = "River",
            Commerce = 1,
            Food = 2,
            Productivity = 0,
            DefenseBonusRate = 1.5,
            _actions = new List<MapSquareImprovementPivot>
            {
                MapSquareImprovementPivot.Irrigate,
                MapSquareImprovementPivot.RailRoad,
                MapSquareImprovementPivot.Road,
                MapSquareImprovementPivot.BuildFortress
            },
            _temperatures = new List<TemperaturePivot>
            {
                TemperaturePivot.Cold,
                TemperaturePivot.Hot,
                TemperaturePivot.Temperate
            },
            UnderlyingBiome = null,
            SpeedCost = 1,
            AppearanceRate = 0.005, // Not used in the same way than other biomes.
            Size = BiomeSizePivot.Small, // Irrelevant.
            Humidity = HumidityPivot.Wet,
            Age = AgePivot.Old,
            BonusApperanceRate = 0.5,
            BonusCommerce = 0,
            BonusFood = 0,
            BonusProductivity = 1,
            BonusName = "Resource"
        };

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
        /// List of biomes which appears on the map by chunk.
        /// </summary>
        internal static IReadOnlyCollection<BiomePivot> ChunkAppearanceBiomes
        {
            get
            {
                return Biomes.Where(b => b != Ocean && b != Default && b != River).ToList();
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
