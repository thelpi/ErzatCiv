using System;
using System.Collections.Generic;

namespace ErsatzCivLib.Model.Persistent
{
    [Serializable]
    public class AdvancePivot : IEquatable<AdvancePivot>
    {
        private List<AdvancePivot> _prerequisite;

        public string Name { get; private set; }
        public EraPivot Era { get; private set; }
        public IReadOnlyCollection<AdvancePivot> Prerequisite { get { return _prerequisite; } }

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
            _prerequisite = new List<AdvancePivot>()
        };
        public static readonly AdvancePivot BronzeWorking = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Bronze Working",
            _prerequisite = new List<AdvancePivot>()
        };
        public static readonly AdvancePivot CeremonialBurial = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "CeremonialBurial",
            _prerequisite = new List<AdvancePivot>()
        };
        public static readonly AdvancePivot HorsebackRiding = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Horseback Riding",
            _prerequisite = new List<AdvancePivot>()
        };
        public static readonly AdvancePivot Masonry = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Masonry",
            _prerequisite = new List<AdvancePivot>()
        };
        public static readonly AdvancePivot Pottery = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Pottery",
            _prerequisite = new List<AdvancePivot>()
        };
        public static readonly AdvancePivot Wheel = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Wheel",
            _prerequisite = new List<AdvancePivot>()
        };
        public static readonly AdvancePivot Writing = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Writing",
            _prerequisite = new List<AdvancePivot> { Alphabet }
        };
        public static readonly AdvancePivot IronWorking = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Iron Working",
            _prerequisite = new List<AdvancePivot> { BronzeWorking }
        };
        public static readonly AdvancePivot Literacy = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Literacy",
            _prerequisite = new List<AdvancePivot> { Writing, CodeOfLaws }
        };
        public static readonly AdvancePivot CodeOfLaws = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Code of Laws",
            _prerequisite = new List<AdvancePivot> { Alphabet }
        };
        public static readonly AdvancePivot Currency = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Currency",
            _prerequisite = new List<AdvancePivot> { BronzeWorking }
        };
        public static readonly AdvancePivot Construction = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Construction",
            _prerequisite = new List<AdvancePivot> { Currency, Masonry }
        };
        public static readonly AdvancePivot Trade = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Trade",
            _prerequisite = new List<AdvancePivot> { CodeOfLaws, Currency }
        };
        public static readonly AdvancePivot Republic = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Republic",
            _prerequisite = new List<AdvancePivot> { CodeOfLaws, Literacy }
        };
        public static readonly AdvancePivot Mysticism = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Mysticism",
            _prerequisite = new List<AdvancePivot> { CeremonialBurial }
        };
        public static readonly AdvancePivot Monarchy = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Monarchy",
            _prerequisite = new List<AdvancePivot> { CodeOfLaws, CeremonialBurial }
        };
        public static readonly AdvancePivot Mathematics = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Mathematics",
            _prerequisite = new List<AdvancePivot> { Alphabet, Masonry }
        };
        public static readonly AdvancePivot MapMaking = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Map Making",
            _prerequisite = new List<AdvancePivot> { Alphabet }
        };
        public static readonly AdvancePivot BridgeBuilding = new AdvancePivot
        {
            Era = EraPivot.Antiquity,
            Name = "Bridge Building",
            _prerequisite = new List<AdvancePivot> { IronWorking, Construction }
        };

        public static readonly AdvancePivot Astronomy = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "Astronomy",
            _prerequisite = new List<AdvancePivot> { Mysticism, MapMaking }
        };
        public static readonly AdvancePivot Banking = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "Banking",
            _prerequisite = new List<AdvancePivot> { Trade, Republic }
        };
        public static readonly AdvancePivot Feudalism = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "Feudalism",
            _prerequisite = new List<AdvancePivot> { Masonry, Monarchy }
        };
        public static readonly AdvancePivot Chivalry = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "Chivalry",
            _prerequisite = new List<AdvancePivot> { Feudalism, HorsebackRiding }
        };
        public static readonly AdvancePivot Democracy = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "Democracy",
            _prerequisite = new List<AdvancePivot> { Philosophy }
        };
        public static readonly AdvancePivot Philosophy = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "Philosophy",
            _prerequisite = new List<AdvancePivot> { Mysticism, Literacy }
        };
        public static readonly AdvancePivot University = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "University",
            _prerequisite = new List<AdvancePivot> { Mathematics, Philosophy }
        };
        public static readonly AdvancePivot TheoryOfGravity = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "Theory of Gravity",
            _prerequisite = new List<AdvancePivot> { Astronomy, University }
        };
        public static readonly AdvancePivot Religion = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "Religion",
            _prerequisite = new List<AdvancePivot> { Philosophy, Writing }
        };
        public static readonly AdvancePivot Navigation = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "Navigation",
            _prerequisite = new List<AdvancePivot> { MapMaking, Astronomy }
        };
        public static readonly AdvancePivot Medicine = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "Medicine",
            _prerequisite = new List<AdvancePivot> { Philosophy, Trade }
        };
        public static readonly AdvancePivot Engineering = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "Engineering",
            _prerequisite = new List<AdvancePivot> { Wheel, Construction }
        };
        public static readonly AdvancePivot Invention = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "Invention",
            _prerequisite = new List<AdvancePivot> { Literacy, Engineering }
        };
        public static readonly AdvancePivot Gunpowder = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "Gunpowder",
            _prerequisite = new List<AdvancePivot> { Invention, IronWorking }
        };
        public static readonly AdvancePivot Physics = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "Physics",
            _prerequisite = new List<AdvancePivot> { Mathematics, Navigation }
        };
        public static readonly AdvancePivot Magnetism = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "Magnetism",
            _prerequisite = new List<AdvancePivot> { Navigation, Physics }
        };
        public static readonly AdvancePivot Metallurgy = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "Metallurgy",
            _prerequisite = new List<AdvancePivot> { Gunpowder, University }
        };
        public static readonly AdvancePivot Chemistry = new AdvancePivot
        {
            Era = EraPivot.MiddleAge,
            Name = "Chemistry",
            _prerequisite = new List<AdvancePivot> { University, Medicine }
        };

        public static readonly AdvancePivot AtomicTheory = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Atomic Theory",
            _prerequisite = new List<AdvancePivot> { TheoryOfGravity, Physics }
        };
        public static readonly AdvancePivot Electricity = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Electricity",
            _prerequisite = new List<AdvancePivot> { Magnetism, Metallurgy }
        };
        public static readonly AdvancePivot AdvancedFlight = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Advanced Flight",
            _prerequisite = new List<AdvancePivot> { Flight, Electricity }
        };
        public static readonly AdvancePivot Flight = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Flight",
            _prerequisite = new List<AdvancePivot> { Physics, Combustion }
        };
        public static readonly AdvancePivot Combustion = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Combustion",
            _prerequisite = new List<AdvancePivot> { Refining, Explosives }
        };
        public static readonly AdvancePivot Refining = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Refining",
            _prerequisite = new List<AdvancePivot> { Chemistry, Corporation }
        };
        public static readonly AdvancePivot Explosives = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Explosives",
            _prerequisite = new List<AdvancePivot> { Gunpowder, Chemistry }
        };
        public static readonly AdvancePivot Corporation = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Corporation",
            _prerequisite = new List<AdvancePivot> { Banking, Industrialization }
        };
        public static readonly AdvancePivot Industrialization = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Industrialization",
            _prerequisite = new List<AdvancePivot> { Banking, Railroad }
        };
        public static readonly AdvancePivot Railroad = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Railroad",
            _prerequisite = new List<AdvancePivot> { SteamEngine, BridgeBuilding }
        };
        public static readonly AdvancePivot SteamEngine = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Steam Engine",
            _prerequisite = new List<AdvancePivot> { Physics, Invention }
        };
        public static readonly AdvancePivot Superconductor = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Superconductor",
            _prerequisite = new List<AdvancePivot> { MassProduction, Plastics }
        };
        public static readonly AdvancePivot MassProduction = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "MassProduction",
            _prerequisite = new List<AdvancePivot> { Automobile, Corporation }
        };
        public static readonly AdvancePivot Plastics = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Plastics",
            _prerequisite = new List<AdvancePivot> { Refining, SpaceFlight }
        };
        public static readonly AdvancePivot SpaceFlight = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "SpaceFlight",
            _prerequisite = new List<AdvancePivot> { Rocketry, Computers }
        };
        public static readonly AdvancePivot Rocketry = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Rocketry",
            _prerequisite = new List<AdvancePivot> { AdvancedFlight, Electronics }
        };
        public static readonly AdvancePivot Computers = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Computers",
            _prerequisite = new List<AdvancePivot> { Electronics, Mathematics }
        };
        public static readonly AdvancePivot Electronics = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Electronics",
            _prerequisite = new List<AdvancePivot> { Electricity, Engineering }
        };
        public static readonly AdvancePivot Automobile = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Automobile",
            _prerequisite = new List<AdvancePivot> { Combustion, Steel }
        };
        public static readonly AdvancePivot Steel = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Steel",
            _prerequisite = new List<AdvancePivot> { Industrialization, Metallurgy }
        };
        public static readonly AdvancePivot Robotics = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Robotics",
            _prerequisite = new List<AdvancePivot> { Plastics, Computers }
        };
        public static readonly AdvancePivot Recycling = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Recycling",
            _prerequisite = new List<AdvancePivot> { Democracy, MassProduction }
        };
        public static readonly AdvancePivot NuclearFission = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Nuclear Fission",
            _prerequisite = new List<AdvancePivot> { MassProduction, AtomicTheory }
        };
        public static readonly AdvancePivot NuclearPower = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Nuclear Power",
            _prerequisite = new List<AdvancePivot> { NuclearFission, Electronics }
        };
        public static readonly AdvancePivot LaborUnion = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Labor Union",
            _prerequisite = new List<AdvancePivot> { MassProduction, Communism }
        };
        public static readonly AdvancePivot Communism = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Communism",
            _prerequisite = new List<AdvancePivot> { Philosophy, Industrialization }
        };
        public static readonly AdvancePivot Conscription = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Conscription",
            _prerequisite = new List<AdvancePivot> { Republic, Explosives }
        };
        public static readonly AdvancePivot FusionPower = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Fusion Power",
            _prerequisite = new List<AdvancePivot> { NuclearPower, Superconductor }
        };
        public static readonly AdvancePivot GeneticEngineering = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Genetic Engineering",
            _prerequisite = new List<AdvancePivot> { Medicine, Corporation }
        };

        public static readonly AdvancePivot FutureTech = new AdvancePivot
        {
            Era = EraPivot.ModernAge,
            Name = "Future Tech",
            _prerequisite = new List<AdvancePivot> { FusionPower }
        };

        #endregion
    }
}
