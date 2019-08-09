using System;
using System.Collections.Generic;
using System.Linq;

namespace ErsatzCivLib.Model.Static
{
    /// <summary>
    /// Represents a civilization pick.
    /// </summary>
    /// <seealso cref="IEquatable{T}"/>
    [Serializable]
    public class CivilizationPivot : IEquatable<CivilizationPivot>
    {
        #region Embedded properties

        /// <summary>
        /// Name.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Leader name (man).
        /// </summary>
        public string ManLeaderName { get; private set; }
        /// <summary>
        /// Leader name (woman).
        /// </summary>
        public string WomanLeaderName { get; private set; }

        private List<string> _cities;
        /// <summary>
        /// List of city names.
        /// </summary>
        public IReadOnlyCollection<string> Cities { get { return _cities; } }

        private List<AdvancePivot> _advances;
        /// <summary>
        /// List of <see cref="AdvancePivot"/> the civilization knowns at the beginning of the game.
        /// </summary>
        public IReadOnlyCollection<AdvancePivot> Advances { get { return _advances; } }

        #endregion

        private CivilizationPivot() { }

        /// <summary>
        /// Tries to determinate the name of the next city of the civilization, based on the names already used.
        /// </summary>
        /// <param name="cities">List of cities.</param>
        /// <returns>The name suggestion.</returns>
        public string NextCityName(IEnumerable<CityPivot> cities)
        {
            cities = cities?.Where(c => c != null);
            if (cities?.Any() != true)
            {
                return _cities[0];
            }

            var ranks = cities.Select(c =>
            {
                var topIndexTmp = 0;
                var parts = c.Name.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 1)
                {
                    if (!int.TryParse(parts.Last(), out topIndexTmp) || topIndexTmp < 0)
                    {
                        topIndexTmp = 0;
                    }
                }
                return topIndexTmp;
            }).ToList();

            var topRank = ranks.Count > 0 ? ranks.Max() : 0;
            var lastIndexWithThisRank = -1;
            for (var i = 0; i < _cities.Count; i++)
            {
                if (cities.Any(c =>
                    c.Name.Equals(
                        (topRank == 0 ? _cities[i] : string.Concat(_cities[i], " ", topRank)),
                        StringComparison.InvariantCultureIgnoreCase)))
                {
                    lastIndexWithThisRank = i;
                }
            }

            if (lastIndexWithThisRank == _cities.Count - 1)
            {
                lastIndexWithThisRank = -1;
                topRank++;
            }

            if (topRank == 0)
            {
                return _cities[lastIndexWithThisRank + 1];
            }
            else
            {
                return string.Concat(_cities[lastIndexWithThisRank + 1], " ", topRank);
            }
        }

        #region IEquatable implementation

        /// <summary>
        /// Checks if this instance is equal to another one.
        /// </summary>
        /// <param name="other">The other instance.</param>
        /// <returns><c>True</c> if equals; <c>False</c> otherwise.</returns>
        public bool Equals(CivilizationPivot other)
        {
            return Name == other?.Name;
        }

        /// <summary>
        /// Operator "==" override. Checks equality between two instances.
        /// </summary>
        /// <param name="c1">The first <see cref="CivilizationPivot"/>.</param>
        /// <param name="c2">The second <see cref="CivilizationPivot"/>.</param>
        /// <returns><c>True</c> if equals; <c>False</c> otherwise.</returns>
        public static bool operator ==(CivilizationPivot c1, CivilizationPivot c2)
        {
            if (c1 is null)
            {
                return c2 is null;
            }

            return c1.Equals(c2) == true;
        }

        /// <summary>
        /// Operator "!=" override. Checks non-equality between two instances.
        /// </summary>
        /// <param name="c1">The first <see cref="CivilizationPivot"/>.</param>
        /// <param name="c2">The second <see cref="CivilizationPivot"/>.</param>
        /// <returns><c>False</c> if equals; <c>True</c> otherwise.</returns>
        public static bool operator !=(CivilizationPivot c1, CivilizationPivot c2)
        {
            return !(c1 == c2);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is CivilizationPivot && Equals(obj as CivilizationPivot);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        #endregion

        #region Static instances

        /// <summary>
        /// Constant; the American civilization.
        /// </summary>
        public static readonly CivilizationPivot American = new CivilizationPivot
        {
            Name = "American",
            ManLeaderName = "Abraham Lincoln",
            WomanLeaderName = "Eleanor Roosevelt",
            _advances = new List<AdvancePivot> { AdvancePivot.Wheel },
            _cities = new List<string> { "Washington", "New York", "Boston", "Philadelphia", "Atlanta", "Chicago", "Buffalo", "St. Louis",
                "Detroit", "New Orleans", "Baltimore", "Denver", "Cincinnati", "Dallas", "Los Angeles", "Las Vegas" }
        };
        /// <summary>
        /// Constant; the Aztec civilization.
        /// </summary>
        public static readonly CivilizationPivot Aztec = new CivilizationPivot
        {
            Name = "Aztec",
            ManLeaderName = "Montezuma",
            WomanLeaderName = "Cihuacóatl",
            _advances = new List<AdvancePivot> { AdvancePivot.Masonry },
            _cities = new List<string> { "Tenochtitlan", "Chiauhtia", "Chapultapec", "Coatepec", "Ayontzinco", "Itzapalapa", "Itzapam",
                "Mitxcoac", "Tucubaya", "Tecamac", "Tepezinco", "Ticoman", "Tlaxcala", "Xaltocan", "Xicalango", "Zumpanco" }
        };
        /// <summary>
        /// Constant; the Babylonian civilization.
        /// </summary>
        public static readonly CivilizationPivot Babylonian = new CivilizationPivot
        {
            Name = "Babylonian",
            ManLeaderName = "Hammurabi",
            WomanLeaderName = "Sabitum",
            _advances = new List<AdvancePivot> { AdvancePivot.Pottery },
            _cities = new List<string> { "Babylon", "Sumer", "Uruk", "Ninevah", "Ashur", "Ellipi", "Akkad", "Eridu", "Kish", "Nippur",
                "Shuruppak", "Zariqum", "Izibia", "Nimrud", "Arbela", "Zamua" }
        };
        /// <summary>
        /// Constant; the Chinese civilization.
        /// </summary>
        public static readonly CivilizationPivot Chinese = new CivilizationPivot
        {
            Name = "Chinese",
            ManLeaderName = "Mao Tse Tung",
            WomanLeaderName = "Wu Zetian",
            _advances = new List<AdvancePivot> { AdvancePivot.Alphabet },
            _cities = new List<string> { "Peking", "Shanghai", "Canton", "Nanking", "Tsingtao", "Hangchow", "Tientsin", "Tatung", "Macao",
                "Anyang", "Shantung", "Chinan", "Kaifeng", "Ningpo", "Paoting", "Yangchow" }
        };
        /// <summary>
        /// Constant; the Egyptian civilization.
        /// </summary>
        public static readonly CivilizationPivot Egyptian = new CivilizationPivot
        {
            Name = "Egyptian",
            ManLeaderName = "Ramesses",
            WomanLeaderName = "Cleopatra",
            _advances = new List<AdvancePivot> { AdvancePivot.Masonry },
            _cities = new List<string> { "Thebes", "Memphis", "Oryx", "Heliopolis", "Gaza", "Alexandria", "Byblos", "Cairo", "Coptos",
                "Edfu", "Pithom", "Busirus", "Athribus", "Mendes", "Tanis", "Abydos" }
        };
        /// <summary>
        /// Constant; the English civilization.
        /// </summary>
        public static readonly CivilizationPivot English = new CivilizationPivot
        {
            Name = "English",
            ManLeaderName = "Elizabeth I",
            WomanLeaderName = "Winston Churchill",
            _advances = new List<AdvancePivot> { AdvancePivot.Alphabet },
            _cities = new List<string> { "London", "Coventry", "Birmingham", "Dover", "Nottingham", "York", "Liverpool", "Brighton",
                "Oxford", "Reading", "Exeter", "Cambridge", "Hastings", "Canterbury", "Banbury", "Newcastle" }
        };
        /// <summary>
        /// Constant; the French civilization.
        /// </summary>
        public static readonly CivilizationPivot French = new CivilizationPivot
        {
            Name = "French",
            ManLeaderName = "Napoleon",
            WomanLeaderName = "Joan of Arc",
            _advances = new List<AdvancePivot> { AdvancePivot.Pottery },
            _cities = new List<string> { "Paris", "Orleans", "Lyons", "Tours", "Chartres", "Bordeaux", "Rouen", "Avignon", "Marseille",
                "Grenoble", "Dijon", "Amiens", "Cherbourg", "Poitiers", "Toulouse", "Bayonne" }
        };
        /// <summary>
        /// Constant; the German civilization.
        /// </summary>
        public static readonly CivilizationPivot German = new CivilizationPivot
        {
            Name = "German",
            ManLeaderName = "Frederick the Great",
            WomanLeaderName = "Angela Merkel",
            _advances = new List<AdvancePivot> { AdvancePivot.Masonry },
            _cities = new List<string> { "Berlin", "Leipzig", "Hamburg", "Bremen", "Frankfurt", "Bonn", "Nuremberg", "Cologne", "Hannover",
                "Munich", "Stuttgart", "Heidelberg", "Salzburg", "Konigsberg", "Dortmond", "Brandenburg" }
        };
        /// <summary>
        /// Constant; the Greek civilization.
        /// </summary>
        public static readonly CivilizationPivot Greek = new CivilizationPivot
        {
            Name = "Greek",
            ManLeaderName = "Alexander",
            WomanLeaderName = "Berenice II of Egypt",
            _advances = new List<AdvancePivot> { AdvancePivot.Wheel },
            _cities = new List<string> { "Athens", "Sparta", "Corinth", "Delphi", "Eretria", "Pharsalos", "Argos", "Mycenae", "Herakleia",
                "Antioch", "Ephesos", "Rhodes", "Knossos", "Troy", "Pergamon", "Miletos" }
        };
        /// <summary>
        /// Constant; the Indian civilization.
        /// </summary>
        public static readonly CivilizationPivot Indian = new CivilizationPivot
        {
            Name = "Indian",
            ManLeaderName = "Mohandas Gandhi",
            WomanLeaderName = "Razia Sultana",
            _advances = new List<AdvancePivot> { AdvancePivot.CeremonialBurial },
            _cities = new List<string> { "Delhi", "Bombay", "Madras", "Bangalore", "Calcutta", "Lahore", "Karachi", "Kolhapur", "Jaipur",
                "Hyderbad", "Bengal", "Chittagong", "Punjab", "Dacca", "Indus", "Ganges" }
        };
        /// <summary>
        /// Constant; the Japanese civilization.
        /// </summary>
        public static readonly CivilizationPivot Japanese = new CivilizationPivot
        {
            Name = "Japanese",
            ManLeaderName = "Tokugawa",
            WomanLeaderName = "Kōgyoku",
            _advances = new List<AdvancePivot> { AdvancePivot.Masonry },
            _cities = new List<string> { "Tokyo", "Kyoto", "Osaka", "Nagoya", "Yokohama", "Kobe", "Fukuoka", "Sendai", "Kanazawa",
                "Sapporo", "Matsuyama", "Akita", "Hiroshima", "Nagasaki", "Nara", "Kamakura" }
        };
        /// <summary>
        /// Constant; the Mongolian civilization.
        /// </summary>
        public static readonly CivilizationPivot Mongolian = new CivilizationPivot
        {
            Name = "Mongolian",
            ManLeaderName = "Genghis Khan",
            WomanLeaderName = "Mandukhai",
            _advances = new List<AdvancePivot> { AdvancePivot.HorsebackRiding },
            _cities = new List<string> { "Samarkand", "Bokhara", "Nishapur", "Karakorum", "Kashgar", "Tabriz", "Aleppo", "Kabul", "Ormuz",
                "Basra", "Khanbaryk", "Khorasan", "Shangtu", "Kazan", "Qyinsay", "Kerman"  }
        };
        /// <summary>
        /// Constant; the Roman civilization.
        /// </summary>
        public static readonly CivilizationPivot Roman = new CivilizationPivot
        {
            Name = "Roman",
            ManLeaderName = "Julius Caesar",
            WomanLeaderName = "Galla Placidia",
            _advances = new List<AdvancePivot> { AdvancePivot.BronzeWorking },
            _cities = new List<string> { "Rome", "Caesarea", "Carthage", "Nicopolis", "Byzantium", "Brundisium", "Syracuse", "Antioch",
                "Palmyra", "Cyrene", "Gordion", "Tyrus", "Jerusalem", "Seleucia", "Ravenna", "Artaxata" }
        };
        /// <summary>
        /// Constant; the Russian civilization.
        /// </summary>
        public static readonly CivilizationPivot Russian = new CivilizationPivot
        {
            Name = "Russian",
            ManLeaderName = "Stalin",
            WomanLeaderName = "Catherine the Great",
            _advances = new List<AdvancePivot> { AdvancePivot.BronzeWorking },
            _cities = new List<string> { "Moscow", "Leningrad", "Kiev", "Minsk", "Smolensk", "Odessa", "Sevastopol", "Tiblisi",
                "Sverdlovsk", "Yakutsk", "Vladivostok", "Novograd", "Krasnoyarsk", "Riga", "Rostov", "Atrakhan" }
        };
        /// <summary>
        /// Constant; the Zulu civilization.
        /// </summary>
        public static readonly CivilizationPivot Zulu = new CivilizationPivot
        {
            Name = "Zulu",
            ManLeaderName = "Shaka",
            WomanLeaderName = "Nandi",
            _advances = new List<AdvancePivot> { AdvancePivot.CeremonialBurial },
            _cities = new List<string> { "Zimbabwe", "Ulundi", "Bapedi", "Hlobane", "Isandhlwala", "Intombe", "Mpondo", "Ngome",
                "Swazi", "Tugela", "Umtata", "Umfolozi", "Ibabanago", "Isipezi", "Amatikulu", "Zunquin" }
        };

        #endregion

        private static List<CivilizationPivot> _instances = null;
        /// <summary>
        /// List of every <see cref="CivilizationPivot"/> instances.
        /// </summary>
        public static IReadOnlyCollection<CivilizationPivot> Instances
        {
            get
            {
                if (_instances == null)
                {
                    _instances = Tools.GetInstancesOfTypeFromStaticFields<CivilizationPivot>();
                }
                return _instances;
            }
        }
    }
}
