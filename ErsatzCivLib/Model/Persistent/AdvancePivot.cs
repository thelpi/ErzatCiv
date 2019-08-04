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

        #endregion
    }
}
