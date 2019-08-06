using System;
using System.Collections.Generic;
using System.Linq;
using ErsatzCivLib.Model.Enums;

namespace ErsatzCivLib.Model.Static
{
    [Serializable]
    public class AdvancePivot : IEquatable<AdvancePivot>
    {
        public const int SCIENCE_COST = 100;

        private List<AdvancePivot> _prerequisites;

        public string Name { get; private set; }
        public EraPivot Era { get; private set; }
        public IReadOnlyCollection<AdvancePivot> Prerequisites { get { return _prerequisites; } }

        private AdvancePivot() { }

        public bool Equals(AdvancePivot other)
        {
            return Name == other?.Name;
        }

        public static bool operator ==(AdvancePivot ms1, AdvancePivot ms2)
        {
            if (ms1 is null)
            {
                return ms2 is null;
            }

            return ms1.Equals(ms2) == true;
        }

        public static bool operator !=(AdvancePivot ms1, AdvancePivot ms2)
        {
            return !(ms1 == ms2);
        }

        public override bool Equals(object obj)
        {
            return obj is AdvancePivot && Equals(obj as AdvancePivot);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        #region Static instances

        public static readonly AdvancePivot Alphabet = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Alphabet",
            _prerequisites = new List<AdvancePivot>()
        };
        public static readonly AdvancePivot BronzeWorking = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Bronze Working",
            _prerequisites = new List<AdvancePivot>()
        };
        public static readonly AdvancePivot CeremonialBurial = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "CeremonialBurial",
            _prerequisites = new List<AdvancePivot>()
        };
        public static readonly AdvancePivot HorsebackRiding = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Horseback Riding",
            _prerequisites = new List<AdvancePivot>()
        };
        public static readonly AdvancePivot Masonry = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Masonry",
            _prerequisites = new List<AdvancePivot>()
        };
        public static readonly AdvancePivot Pottery = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Pottery",
            _prerequisites = new List<AdvancePivot>()
        };
        public static readonly AdvancePivot Wheel = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Wheel",
            _prerequisites = new List<AdvancePivot>()
        };
        public static readonly AdvancePivot Writing = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Writing",
            _prerequisites = new List<AdvancePivot> { Alphabet }
        };
        public static readonly AdvancePivot IronWorking = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Iron Working",
            _prerequisites = new List<AdvancePivot> { BronzeWorking }
        };
        public static readonly AdvancePivot Literacy = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Literacy",
            _prerequisites = new List<AdvancePivot> { Writing, CodeOfLaws }
        };
        public static readonly AdvancePivot CodeOfLaws = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Code of Laws",
            _prerequisites = new List<AdvancePivot> { Alphabet }
        };
        public static readonly AdvancePivot Currency = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Currency",
            _prerequisites = new List<AdvancePivot> { BronzeWorking }
        };
        public static readonly AdvancePivot Construction = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Construction",
            _prerequisites = new List<AdvancePivot> { Currency, Masonry }
        };
        public static readonly AdvancePivot Trade = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Trade",
            _prerequisites = new List<AdvancePivot> { CodeOfLaws, Currency }
        };
        public static readonly AdvancePivot Republic = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Republic",
            _prerequisites = new List<AdvancePivot> { CodeOfLaws, Literacy }
        };
        public static readonly AdvancePivot Mysticism = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Mysticism",
            _prerequisites = new List<AdvancePivot> { CeremonialBurial }
        };
        public static readonly AdvancePivot Monarchy = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Monarchy",
            _prerequisites = new List<AdvancePivot> { CodeOfLaws, CeremonialBurial }
        };
        public static readonly AdvancePivot Mathematics = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Mathematics",
            _prerequisites = new List<AdvancePivot> { Alphabet, Masonry }
        };
        public static readonly AdvancePivot MapMaking = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Map Making",
            _prerequisites = new List<AdvancePivot> { Alphabet }
        };
        public static readonly AdvancePivot BridgeBuilding = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Bridge Building",
            _prerequisites = new List<AdvancePivot> { IronWorking, Construction }
        };

        public static readonly AdvancePivot Astronomy = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "Astronomy",
            _prerequisites = new List<AdvancePivot> { Mysticism, MapMaking }
        };
        public static readonly AdvancePivot Banking = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "Banking",
            _prerequisites = new List<AdvancePivot> { Trade, Republic }
        };
        public static readonly AdvancePivot Feudalism = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "Feudalism",
            _prerequisites = new List<AdvancePivot> { Masonry, Monarchy }
        };
        public static readonly AdvancePivot Chivalry = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "Chivalry",
            _prerequisites = new List<AdvancePivot> { Feudalism, HorsebackRiding }
        };
        public static readonly AdvancePivot Democracy = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "Democracy",
            _prerequisites = new List<AdvancePivot> { Philosophy }
        };
        public static readonly AdvancePivot Philosophy = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "Philosophy",
            _prerequisites = new List<AdvancePivot> { Mysticism, Literacy }
        };
        public static readonly AdvancePivot University = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "University",
            _prerequisites = new List<AdvancePivot> { Mathematics, Philosophy }
        };
        public static readonly AdvancePivot TheoryOfGravity = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "Theory of Gravity",
            _prerequisites = new List<AdvancePivot> { Astronomy, University }
        };
        public static readonly AdvancePivot Religion = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "Religion",
            _prerequisites = new List<AdvancePivot> { Philosophy, Writing }
        };
        public static readonly AdvancePivot Navigation = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "Navigation",
            _prerequisites = new List<AdvancePivot> { MapMaking, Astronomy }
        };
        public static readonly AdvancePivot Medicine = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "Medicine",
            _prerequisites = new List<AdvancePivot> { Philosophy, Trade }
        };
        public static readonly AdvancePivot Engineering = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "Engineering",
            _prerequisites = new List<AdvancePivot> { Wheel, Construction }
        };
        public static readonly AdvancePivot Invention = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "Invention",
            _prerequisites = new List<AdvancePivot> { Literacy, Engineering }
        };
        public static readonly AdvancePivot Gunpowder = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "Gunpowder",
            _prerequisites = new List<AdvancePivot> { Invention, IronWorking }
        };
        public static readonly AdvancePivot Physics = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "Physics",
            _prerequisites = new List<AdvancePivot> { Mathematics, Navigation }
        };
        public static readonly AdvancePivot Magnetism = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "Magnetism",
            _prerequisites = new List<AdvancePivot> { Navigation, Physics }
        };
        public static readonly AdvancePivot Metallurgy = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "Metallurgy",
            _prerequisites = new List<AdvancePivot> { Gunpowder, University }
        };
        public static readonly AdvancePivot Chemistry = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "Chemistry",
            _prerequisites = new List<AdvancePivot> { University, Medicine }
        };

        public static readonly AdvancePivot AtomicTheory = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Atomic Theory",
            _prerequisites = new List<AdvancePivot> { TheoryOfGravity, Physics }
        };
        public static readonly AdvancePivot Electricity = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Electricity",
            _prerequisites = new List<AdvancePivot> { Magnetism, Metallurgy }
        };
        public static readonly AdvancePivot AdvancedFlight = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Advanced Flight",
            _prerequisites = new List<AdvancePivot> { Flight, Electricity }
        };
        public static readonly AdvancePivot Flight = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Flight",
            _prerequisites = new List<AdvancePivot> { Physics, Combustion }
        };
        public static readonly AdvancePivot Combustion = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Combustion",
            _prerequisites = new List<AdvancePivot> { Refining, Explosives }
        };
        public static readonly AdvancePivot Refining = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Refining",
            _prerequisites = new List<AdvancePivot> { Chemistry, Corporation }
        };
        public static readonly AdvancePivot Explosives = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Explosives",
            _prerequisites = new List<AdvancePivot> { Gunpowder, Chemistry }
        };
        public static readonly AdvancePivot Corporation = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Corporation",
            _prerequisites = new List<AdvancePivot> { Banking, Industrialization }
        };
        public static readonly AdvancePivot Industrialization = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Industrialization",
            _prerequisites = new List<AdvancePivot> { Banking, Railroad }
        };
        public static readonly AdvancePivot Railroad = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Railroad",
            _prerequisites = new List<AdvancePivot> { SteamEngine, BridgeBuilding }
        };
        public static readonly AdvancePivot SteamEngine = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Steam Engine",
            _prerequisites = new List<AdvancePivot> { Physics, Invention }
        };
        public static readonly AdvancePivot Superconductor = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Superconductor",
            _prerequisites = new List<AdvancePivot> { MassProduction, Plastics }
        };
        public static readonly AdvancePivot MassProduction = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "MassProduction",
            _prerequisites = new List<AdvancePivot> { Automobile, Corporation }
        };
        public static readonly AdvancePivot Plastics = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Plastics",
            _prerequisites = new List<AdvancePivot> { Refining, SpaceFlight }
        };
        public static readonly AdvancePivot SpaceFlight = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "SpaceFlight",
            _prerequisites = new List<AdvancePivot> { Rocketry, Computers }
        };
        public static readonly AdvancePivot Rocketry = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Rocketry",
            _prerequisites = new List<AdvancePivot> { AdvancedFlight, Electronics }
        };
        public static readonly AdvancePivot Computers = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Computers",
            _prerequisites = new List<AdvancePivot> { Electronics, Mathematics }
        };
        public static readonly AdvancePivot Electronics = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Electronics",
            _prerequisites = new List<AdvancePivot> { Electricity, Engineering }
        };
        public static readonly AdvancePivot Automobile = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Automobile",
            _prerequisites = new List<AdvancePivot> { Combustion, Steel }
        };
        public static readonly AdvancePivot Steel = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Steel",
            _prerequisites = new List<AdvancePivot> { Industrialization, Metallurgy }
        };
        public static readonly AdvancePivot Robotics = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Robotics",
            _prerequisites = new List<AdvancePivot> { Plastics, Computers }
        };
        public static readonly AdvancePivot Recycling = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Recycling",
            _prerequisites = new List<AdvancePivot> { Democracy, MassProduction }
        };
        public static readonly AdvancePivot NuclearFission = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Nuclear Fission",
            _prerequisites = new List<AdvancePivot> { MassProduction, AtomicTheory }
        };
        public static readonly AdvancePivot NuclearPower = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Nuclear Power",
            _prerequisites = new List<AdvancePivot> { NuclearFission, Electronics }
        };
        public static readonly AdvancePivot LaborUnion = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Labor Union",
            _prerequisites = new List<AdvancePivot> { MassProduction, Communism }
        };
        public static readonly AdvancePivot Communism = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Communism",
            _prerequisites = new List<AdvancePivot> { Philosophy, Industrialization }
        };
        public static readonly AdvancePivot Conscription = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Conscription",
            _prerequisites = new List<AdvancePivot> { Republic, Explosives }
        };
        public static readonly AdvancePivot FusionPower = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Fusion Power",
            _prerequisites = new List<AdvancePivot> { NuclearPower, Superconductor }
        };
        public static readonly AdvancePivot GeneticEngineering = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Genetic Engineering",
            _prerequisites = new List<AdvancePivot> { Medicine, Corporation }
        };
        // TODO : manage future techs.

        private static Dictionary<EraPivot, List<AdvancePivot>> _advancesByEra = null;

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

        #endregion
    }
}
