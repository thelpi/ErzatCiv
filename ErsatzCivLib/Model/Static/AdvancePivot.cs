using System;
using System.Collections.Generic;
using System.Linq;
using ErsatzCivLib.Model.Enums;

namespace ErsatzCivLib.Model.Static
{
    /// <summary>
    /// Represents a scientific advance.
    /// </summary>
    /// <seealso cref="IEquatable{T}"/>
    [Serializable]
    public class AdvancePivot : IEquatable<AdvancePivot>
    {
        #region Embedded properties

        /// <summary>
        /// Name.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Associated <see cref="EraPivot"/>.
        /// Its imposible to discover advances for a new era before knowning every advances of the previous era.
        /// </summary>
        public EraPivot Era { get; private set; }

        private List<AdvancePivot> _prerequisites;
        /// <summary>
        /// List of <see cref="AdvancePivot"/> required to discover this one.
        /// </summary>
        public IReadOnlyCollection<AdvancePivot> Prerequisites { get { return _prerequisites; } }

        #endregion

        private AdvancePivot() { }

        #region IEquatable implementation

        /// <summary>
        /// Checks if this instance is equal to another one.
        /// </summary>
        /// <param name="other">The other instance.</param>
        /// <returns><c>True</c> if equals; <c>False</c> otherwise.</returns>
        public bool Equals(AdvancePivot other)
        {
            return Name == other?.Name;
        }

        /// <summary>
        /// Operator "==" override. Checks equality between two instances.
        /// </summary>
        /// <param name="r1">The first <see cref="AdvancePivot"/>.</param>
        /// <param name="r2">The second <see cref="AdvancePivot"/>.</param>
        /// <returns><c>True</c> if equals; <c>False</c> otherwise.</returns>
        public static bool operator ==(AdvancePivot r1, AdvancePivot r2)
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
        /// <param name="r1">The first <see cref="AdvancePivot"/>.</param>
        /// <param name="r2">The second <see cref="AdvancePivot"/>.</param>
        /// <returns><c>False</c> if equals; <c>True</c> otherwise.</returns>
        public static bool operator !=(AdvancePivot r1, AdvancePivot r2)
        {
            return !(r1 == r2);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is AdvancePivot && Equals(obj as AdvancePivot);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        #endregion

        #region Static instances

        /// <summary>
        /// Alphabet.
        /// </summary>
        public static readonly AdvancePivot Alphabet = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Alphabet",
            _prerequisites = new List<AdvancePivot>()
        };
        /// <summary>
        /// Bronze Working.
        /// </summary>
        public static readonly AdvancePivot BronzeWorking = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Bronze Working",
            _prerequisites = new List<AdvancePivot>()
        };
        /// <summary>
        /// Ceremonial Burial.
        /// </summary>
        public static readonly AdvancePivot CeremonialBurial = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Ceremonial Burial",
            _prerequisites = new List<AdvancePivot>()
        };
        /// <summary>
        /// Horseback Riding.
        /// </summary>
        public static readonly AdvancePivot HorsebackRiding = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Horseback Riding",
            _prerequisites = new List<AdvancePivot>()
        };
        /// <summary>
        /// Masonry.
        /// </summary>
        public static readonly AdvancePivot Masonry = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Masonry",
            _prerequisites = new List<AdvancePivot>()
        };
        /// <summary>
        /// Pottery.
        /// </summary>
        public static readonly AdvancePivot Pottery = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Pottery",
            _prerequisites = new List<AdvancePivot>()
        };
        /// <summary>
        /// Wheel.
        /// </summary>
        public static readonly AdvancePivot Wheel = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Wheel",
            _prerequisites = new List<AdvancePivot>()
        };
        /// <summary>
        /// Writing.
        /// </summary>
        public static readonly AdvancePivot Writing = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Writing",
            _prerequisites = new List<AdvancePivot> { Alphabet }
        };
        /// <summary>
        /// Iron Working.
        /// </summary>
        public static readonly AdvancePivot IronWorking = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Iron Working",
            _prerequisites = new List<AdvancePivot> { BronzeWorking }
        };
        /// <summary>
        /// Literacy.
        /// </summary>
        public static readonly AdvancePivot Literacy = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Literacy",
            _prerequisites = new List<AdvancePivot> { Writing, CodeOfLaws }
        };
        /// <summary>
        /// Code of Laws.
        /// </summary>
        public static readonly AdvancePivot CodeOfLaws = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Code of Laws",
            _prerequisites = new List<AdvancePivot> { Alphabet }
        };
        /// <summary>
        /// Currency.
        /// </summary>
        public static readonly AdvancePivot Currency = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Currency",
            _prerequisites = new List<AdvancePivot> { BronzeWorking }
        };
        /// <summary>
        /// Construction.
        /// </summary>
        public static readonly AdvancePivot Construction = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Construction",
            _prerequisites = new List<AdvancePivot> { Currency, Masonry }
        };
        /// <summary>
        /// Trade.
        /// </summary>
        public static readonly AdvancePivot Trade = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Trade",
            _prerequisites = new List<AdvancePivot> { CodeOfLaws, Currency }
        };
        /// <summary>
        /// Republic.
        /// </summary>
        public static readonly AdvancePivot Republic = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Republic",
            _prerequisites = new List<AdvancePivot> { CodeOfLaws, Literacy }
        };
        /// <summary>
        /// Mysticism.
        /// </summary>
        public static readonly AdvancePivot Mysticism = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Mysticism",
            _prerequisites = new List<AdvancePivot> { CeremonialBurial }
        };
        /// <summary>
        /// Monarchy.
        /// </summary>
        public static readonly AdvancePivot Monarchy = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Monarchy",
            _prerequisites = new List<AdvancePivot> { CodeOfLaws, CeremonialBurial }
        };
        /// <summary>
        /// Mathematics.
        /// </summary>
        public static readonly AdvancePivot Mathematics = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Mathematics",
            _prerequisites = new List<AdvancePivot> { Alphabet, Masonry }
        };
        /// <summary>
        /// Map Making.
        /// </summary>
        public static readonly AdvancePivot MapMaking = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Map Making",
            _prerequisites = new List<AdvancePivot> { Alphabet }
        };
        /// <summary>
        /// Bridge Building.
        /// </summary>
        public static readonly AdvancePivot BridgeBuilding = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Bridge Building",
            _prerequisites = new List<AdvancePivot> { IronWorking, Construction }
        };

        /// <summary>
        /// Astronomy.
        /// </summary>
        public static readonly AdvancePivot Astronomy = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "Astronomy",
            _prerequisites = new List<AdvancePivot> { Mysticism, MapMaking }
        };
        /// <summary>
        /// Banking.
        /// </summary>
        public static readonly AdvancePivot Banking = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "Banking",
            _prerequisites = new List<AdvancePivot> { Trade, Republic }
        };
        /// <summary>
        /// Feudalism.
        /// </summary>
        public static readonly AdvancePivot Feudalism = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "Feudalism",
            _prerequisites = new List<AdvancePivot> { Masonry, Monarchy }
        };
        /// <summary>
        /// Chivalry.
        /// </summary>
        public static readonly AdvancePivot Chivalry = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "Chivalry",
            _prerequisites = new List<AdvancePivot> { Feudalism, HorsebackRiding }
        };
        /// <summary>
        /// Democracy.
        /// </summary>
        public static readonly AdvancePivot Democracy = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "Democracy",
            _prerequisites = new List<AdvancePivot> { Philosophy }
        };
        /// <summary>
        /// Philosophy.
        /// </summary>
        public static readonly AdvancePivot Philosophy = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "Philosophy",
            _prerequisites = new List<AdvancePivot> { Mysticism, Literacy }
        };
        /// <summary>
        /// University.
        /// </summary>
        public static readonly AdvancePivot University = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "University",
            _prerequisites = new List<AdvancePivot> { Mathematics, Philosophy }
        };
        /// <summary>
        /// Theory of Gravity.
        /// </summary>
        public static readonly AdvancePivot TheoryOfGravity = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "Theory of Gravity",
            _prerequisites = new List<AdvancePivot> { Astronomy, University }
        };
        /// <summary>
        /// Religion.
        /// </summary>
        public static readonly AdvancePivot Religion = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "Religion",
            _prerequisites = new List<AdvancePivot> { Philosophy, Writing }
        };
        /// <summary>
        /// Navigation.
        /// </summary>
        public static readonly AdvancePivot Navigation = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "Navigation",
            _prerequisites = new List<AdvancePivot> { MapMaking, Astronomy }
        };
        /// <summary>
        /// Medicine.
        /// </summary>
        public static readonly AdvancePivot Medicine = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "Medicine",
            _prerequisites = new List<AdvancePivot> { Philosophy, Trade }
        };
        /// <summary>
        /// Engineering.
        /// </summary>
        public static readonly AdvancePivot Engineering = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "Engineering",
            _prerequisites = new List<AdvancePivot> { Wheel, Construction }
        };
        /// <summary>
        /// Invention.
        /// </summary>
        public static readonly AdvancePivot Invention = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "Invention",
            _prerequisites = new List<AdvancePivot> { Literacy, Engineering }
        };
        /// <summary>
        /// Gunpowder.
        /// </summary>
        public static readonly AdvancePivot Gunpowder = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "Gunpowder",
            _prerequisites = new List<AdvancePivot> { Invention, IronWorking }
        };
        /// <summary>
        /// Physics.
        /// </summary>
        public static readonly AdvancePivot Physics = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "Physics",
            _prerequisites = new List<AdvancePivot> { Mathematics, Navigation }
        };
        /// <summary>
        /// Magnetism.
        /// </summary>
        public static readonly AdvancePivot Magnetism = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "Magnetism",
            _prerequisites = new List<AdvancePivot> { Navigation, Physics }
        };
        /// <summary>
        /// Metallurgy.
        /// </summary>
        public static readonly AdvancePivot Metallurgy = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "Metallurgy",
            _prerequisites = new List<AdvancePivot> { Gunpowder, University }
        };
        /// <summary>
        /// Chemistry.
        /// </summary>
        public static readonly AdvancePivot Chemistry = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "Chemistry",
            _prerequisites = new List<AdvancePivot> { University, Medicine }
        };

        /// <summary>
        /// Atomic Theory.
        /// </summary>
        public static readonly AdvancePivot AtomicTheory = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Atomic Theory",
            _prerequisites = new List<AdvancePivot> { TheoryOfGravity, Physics }
        };
        /// <summary>
        /// Electricity.
        /// </summary>
        public static readonly AdvancePivot Electricity = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Electricity",
            _prerequisites = new List<AdvancePivot> { Magnetism, Metallurgy }
        };
        /// <summary>
        /// Advanced Flight.
        /// </summary>
        public static readonly AdvancePivot AdvancedFlight = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Advanced Flight",
            _prerequisites = new List<AdvancePivot> { Flight, Electricity }
        };
        /// <summary>
        /// Flight.
        /// </summary>
        public static readonly AdvancePivot Flight = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Flight",
            _prerequisites = new List<AdvancePivot> { Physics, Combustion }
        };
        /// <summary>
        /// Combustion.
        /// </summary>
        public static readonly AdvancePivot Combustion = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Combustion",
            _prerequisites = new List<AdvancePivot> { Refining, Explosives }
        };
        /// <summary>
        /// Refining.
        /// </summary>
        public static readonly AdvancePivot Refining = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Refining",
            _prerequisites = new List<AdvancePivot> { Chemistry, Corporation }
        };
        /// <summary>
        /// Explosives.
        /// </summary>
        public static readonly AdvancePivot Explosives = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Explosives",
            _prerequisites = new List<AdvancePivot> { Gunpowder, Chemistry }
        };
        /// <summary>
        /// Corporation.
        /// </summary>
        public static readonly AdvancePivot Corporation = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Corporation",
            _prerequisites = new List<AdvancePivot> { Banking, Industrialization }
        };
        /// <summary>
        /// Industrialization.
        /// </summary>
        public static readonly AdvancePivot Industrialization = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Industrialization",
            _prerequisites = new List<AdvancePivot> { Banking, Railroad }
        };
        /// <summary>
        /// Railroad.
        /// </summary>
        public static readonly AdvancePivot Railroad = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Railroad",
            _prerequisites = new List<AdvancePivot> { SteamEngine, BridgeBuilding }
        };
        /// <summary>
        /// Steam Engine.
        /// </summary>
        public static readonly AdvancePivot SteamEngine = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Steam Engine",
            _prerequisites = new List<AdvancePivot> { Physics, Invention }
        };
        /// <summary>
        /// Superconductor.
        /// </summary>
        public static readonly AdvancePivot Superconductor = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Superconductor",
            _prerequisites = new List<AdvancePivot> { MassProduction, Plastics }
        };
        /// <summary>
        /// MassProduction.
        /// </summary>
        public static readonly AdvancePivot MassProduction = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "MassProduction",
            _prerequisites = new List<AdvancePivot> { Automobile, Corporation }
        };
        /// <summary>
        /// Plastics.
        /// </summary>
        public static readonly AdvancePivot Plastics = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Plastics",
            _prerequisites = new List<AdvancePivot> { Refining, SpaceFlight }
        };
        /// <summary>
        /// SpaceFlight.
        /// </summary>
        public static readonly AdvancePivot SpaceFlight = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "SpaceFlight",
            _prerequisites = new List<AdvancePivot> { Rocketry, Computers }
        };
        /// <summary>
        /// Rocketry.
        /// </summary>
        public static readonly AdvancePivot Rocketry = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Rocketry",
            _prerequisites = new List<AdvancePivot> { AdvancedFlight, Electronics }
        };
        /// <summary>
        /// Computers.
        /// </summary>
        public static readonly AdvancePivot Computers = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Computers",
            _prerequisites = new List<AdvancePivot> { Electronics, Mathematics }
        };
        /// <summary>
        /// Electronics.
        /// </summary>
        public static readonly AdvancePivot Electronics = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Electronics",
            _prerequisites = new List<AdvancePivot> { Electricity, Engineering }
        };
        /// <summary>
        /// Automobile.
        /// </summary>
        public static readonly AdvancePivot Automobile = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Automobile",
            _prerequisites = new List<AdvancePivot> { Combustion, Steel }
        };
        /// <summary>
        /// Steel.
        /// </summary>
        public static readonly AdvancePivot Steel = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Steel",
            _prerequisites = new List<AdvancePivot> { Industrialization, Metallurgy }
        };
        /// <summary>
        /// Robotics.
        /// </summary>
        public static readonly AdvancePivot Robotics = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Robotics",
            _prerequisites = new List<AdvancePivot> { Plastics, Computers }
        };
        /// <summary>
        /// Recycling.
        /// </summary>
        public static readonly AdvancePivot Recycling = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Recycling",
            _prerequisites = new List<AdvancePivot> { Democracy, MassProduction }
        };
        /// <summary>
        /// Nuclear Fission.
        /// </summary>
        public static readonly AdvancePivot NuclearFission = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Nuclear Fission",
            _prerequisites = new List<AdvancePivot> { MassProduction, AtomicTheory }
        };
        /// <summary>
        /// Nuclear Power.
        /// </summary>
        public static readonly AdvancePivot NuclearPower = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Nuclear Power",
            _prerequisites = new List<AdvancePivot> { NuclearFission, Electronics }
        };
        /// <summary>
        /// Labor Union.
        /// </summary>
        public static readonly AdvancePivot LaborUnion = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Labor Union",
            _prerequisites = new List<AdvancePivot> { MassProduction, Communism }
        };
        /// <summary>
        /// Communism.
        /// </summary>
        public static readonly AdvancePivot Communism = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Communism",
            _prerequisites = new List<AdvancePivot> { Philosophy, Industrialization }
        };
        /// <summary>
        /// Conscription.
        /// </summary>
        public static readonly AdvancePivot Conscription = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Conscription",
            _prerequisites = new List<AdvancePivot> { Republic, Explosives }
        };
        /// <summary>
        /// Fusion Power.
        /// </summary>
        public static readonly AdvancePivot FusionPower = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Fusion Power",
            _prerequisites = new List<AdvancePivot> { NuclearPower, Superconductor }
        };
        /// <summary>
        /// Genetic Engineering.
        /// </summary>
        public static readonly AdvancePivot GeneticEngineering = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Genetic Engineering",
            _prerequisites = new List<AdvancePivot> { Medicine, Corporation }
        };
        // TODO : manage future techs.

        #endregion

        private static Dictionary<EraPivot, List<AdvancePivot>> _advancesByEra = null;
        /// <summary>
        /// Dictionnary of every <see cref="AdvancePivot"/> by <see cref="EraPivot"/>.
        /// </summary>
        public static IReadOnlyDictionary<EraPivot, List<AdvancePivot>> AdvancesByEra
        {
            get
            {
                if (_advancesByEra == null)
                {
                    _advancesByEra =
                        Tools.GetInstancesOfTypeFromStaticFields<AdvancePivot>()
                            .GroupBy(a => a.Era)
                            .ToDictionary(a => a.Key, a => a.ToList());
                }
                return _advancesByEra;
            }
        }
    }
}
