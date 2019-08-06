using System;
using System.Collections.Generic;
using System.Linq;
using ErsatzCivLib.Model.Persistent;
using ErsatzCivLib.Model.Units;

namespace ErsatzCivLib.Model
{
    /// <summary>
    /// Represents a player in-game.
    /// </summary>
    /// <seealso cref="IEquatable{T}"/>
    [Serializable]
    public class PlayerPivot : IEquatable<PlayerPivot>
    {
        private const int TREASURE_START = 100;

        private List<CityPivot> _cities = new List<CityPivot>();
        private List<AdvancePivot> _advances = new List<AdvancePivot>();

        #region Embedded properties

        /// <summary>
        /// The player civilization.
        /// </summary>
        public CivilizationPivot Civilization { get; private set; }
        /// <summary>
        /// IA player yes/no.
        /// </summary>
        public bool IsIA { get; private set; }
        /// <summary>
        /// Science points stacked.
        /// </summary>
        public int ScienceStack { get; private set; }
        /// <summary>
        /// <see cref="AdvancePivot"/> currently in discovery.
        /// </summary>
        public AdvancePivot CurrentAdvance { get; private set; }
        /// <summary>
        /// Current <see cref="RegimePivot"/>.
        /// </summary>
        public RegimePivot CurrentRegime { get; private set; }
        /// <summary>
        /// Current gold amount in the treasure.
        /// </summary>
        public int Treasure { get; private set; }

        #endregion

        #region Inferred properties

        /// <summary>
        /// List of <see cref="CityPivot"/> built by the player.
        /// </summary>
        public IReadOnlyCollection<CityPivot> Cities { get { return _cities; } }
        /// <summary>
        /// List of discovered <see cref="AdvancePivot"/>.
        /// </summary>
        public IReadOnlyCollection<AdvancePivot> Advances { get { return _advances; } }
        /// <summary>
        /// Science points at each turn.
        /// </summary>
        public int ScienceByTurn
        {
            get
            {
                return _cities.Sum(c => c.Science);
            }
        }
        /// <summary>
        /// Number of turns before current advance discovered.
        /// </summary>
        public int TurnsBeforeNewAdvance
        {
            get
            {
                return CurrentAdvance == null ? 0 : (
                    (int)Math.Ceiling((AdvancePivot.SCIENCE_COST - ScienceStack) / (double)ScienceByTurn)
                );
            }
        }
        /// <summary>
        /// Current player <see cref="EraPivot"/>.
        /// </summary>
        public EraPivot CurrentEra
        {
            get
            {
                return (EraPivot)(_advances.Count == 0 ? 0 : _advances.Max(a => (int)a.Era));
            }
        }
        /// <summary>
        /// Gold added (or substract) to the treasure at each turn.
        /// </summary>
        public int TreasureByTurn
        {
            get
            {
                // TODO
                return _cities.Sum(c => c.Commerce + c.Tax);
            }
        }

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="civilization">The <see cref="Civilization"/> value.</param>
        /// <param name="isIa">The <see cref="IsIA"/> value.</param>
        internal PlayerPivot(CivilizationPivot civilization, bool isIa)
        {
            Civilization = civilization;
            IsIA = isIa;
            _advances.AddRange(civilization.Advances);

            CurrentRegime = RegimePivot.Despotism;
            Treasure = TREASURE_START;
        }

        /// <summary>
        /// At the end of a turn, computes the new value of properties relatives to science.
        /// </summary>
        internal void CheckScienceAtNextTurn()
        {
            if (CurrentAdvance != null)
            {
                ScienceStack += ScienceByTurn;
                if (ScienceStack >= AdvancePivot.SCIENCE_COST)
                {
                    _advances.Add(CurrentAdvance);
                    ScienceStack = 0;
                    CurrentAdvance = null;
                }
            }
        }

        /// <summary>
        /// At the end of a turn, computes the new value of properties relatives to treasure / gold.
        /// </summary>
        internal void CheckTreasureAtNextTurn()
        {
            Treasure += TreasureByTurn;
            if (Treasure < 0)
            {
                // TODO
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Tries to modify the <see cref="AdvancePivot"/> currently in discovery.
        /// </summary>
        /// <remarks>
        /// If an <see cref="AdvancePivot"/> was already in progress,
        /// calling this method divide <see cref="ScienceStack"/> in half.
        /// </remarks>
        /// <param name="advance">The <see cref="AdvancePivot"/>.</param>
        /// <returns><c>True</c> if the current advance has been changed; <c>False</c> otherwise.</returns>
        internal bool ChangeCurrentAdvance(AdvancePivot advance)
        {
            // Conditions to search :
            // - not already known
            // - prerequisites are known
            // - same era as the current one, or next one if the current era is done
            if (_advances.Contains(advance)
                || (advance.Prerequisites.Any() && !advance.Prerequisites.All(_advances.Contains))
                || ((int)advance.Era > (int)CurrentEra && GetAvailableAdvances().Any()))
            {
                return false;
            }

            if (CurrentAdvance != null && CurrentAdvance != advance)
            {
                ScienceStack /= 2;
            }
            CurrentAdvance = advance;
            return true;
        }

        /// <summary>
        /// Tries to create a city at the location of the <see cref="CurrentUnit"/>.
        /// </summary>
        /// <param name="currentTurn">The <see cref="Engine.CurrentTurn"/> value.</param>
        /// <param name="name">Name of the city.</param>
        /// <param name="notUniqueNameError">Out; indicates a failure caused by a non-unique city name.</param>
        /// <param name="cityAtThisLocationCallback">Callback method to check if a city has been built by any civilisation at this location.</param>
        /// <returns>The <see cref="CityPivot"/> built; <c>Null</c> if failure.</returns>
        internal CityPivot BuildCity(int currentTurn, string name, out bool notUniqueNameError,
            Func<MapSquarePivot, bool> cityAtThisLocationCallback)
        {
            notUniqueNameError = false;

            if (!CanBuildCity(cityAtThisLocationCallback))
            {
                return null;
            }

            if (_cities.Any(c => c.Name.Equals(name.ToLower(), StringComparison.InvariantCultureIgnoreCase)))
            {
                notUniqueNameError = true;
                return null;
            }

            var settler = CurrentUnit as SettlerPivot;
            var sq = CurrentUnit.MapSquareLocation;

            var city = new CityPivot(currentTurn, name, sq, ComputeCityAvailableMapSquares, CapitalizationPivot.Default);
            sq.ApplyCityActions(city);

            _cities.Add(city);
            _units.Remove(settler);
            SetUnitIndex(true, false);

            return city;
        }

        private IReadOnlyCollection<AdvancePivot> GetAvailableAdvances()
        {
            var currentEraList = AdvancePivot.AdvancesByEra[CurrentEra]
                .Where(a => !_advances.Contains(a) && a.Prerequisites.All(_advances.Contains))
                .ToList();

            if (!currentEraList.Any())
            {
                if (CurrentEra != EraPivot.ModernAge)
                {
                    // TODO : "futures advances"
                }
                else
                {
                    currentEraList = AdvancePivot.AdvancesByEra[(EraPivot)(((int)CurrentEra) + 1)]
                        .Where(a => !_advances.Contains(a) && a.Prerequisites.All(_advances.Contains))
                        .ToList();
                }
            }

            return currentEraList;
        }

        private bool CanBuildCity(Func<MapSquarePivot, bool> cityAtThisLocationCallback)
        {
            if (CurrentUnit?.Is<SettlerPivot>() != true)
            {
                return false;
            }

            var sq = CurrentUnit.MapSquareLocation;

            return sq?.Biome?.IsCityBuildable == true
                && !cityAtThisLocationCallback(sq)
                && sq.Pollution != true;
        }

        #region IEquatable implementation

        /// <summary>
        /// Checks if this instance is equal to another one.
        /// </summary>
        /// <param name="other">The other instance.</param>
        /// <returns><c>True</c> if equals; <c>False</c> otherwise.</returns>
        public bool Equals(PlayerPivot other)
        {
            return Civilization == other?.Civilization;
        }

        /// <summary>
        /// Operator "==" override. Checks equality between two instances.
        /// </summary>
        /// <param name="p1">The first <see cref="PlayerPivot"/>.</param>
        /// <param name="p2">The second <see cref="PlayerPivot"/>.</param>
        /// <returns><c>True</c> if equals; <c>False</c> otherwise.</returns>
        public static bool operator ==(PlayerPivot p1, PlayerPivot p2)
        {
            if (p1 is null)
            {
                return p2 is null;
            }

            return p1.Equals(p2) == true;
        }

        /// <summary>
        /// Operator "!=" override. Checks non-equality between two instances.
        /// </summary>
        /// <param name="p1">The first <see cref="PlayerPivot"/>.</param>
        /// <param name="p2">The second <see cref="PlayerPivot"/>.</param>
        /// <returns><c>False</c> if equals; <c>True</c> otherwise.</returns>
        public static bool operator !=(PlayerPivot p1, PlayerPivot p2)
        {
            return !(p1 == p2);
        }

        /// <inhrritdoc />
        public override bool Equals(object obj)
        {
            return obj is PlayerPivot && Equals(obj as PlayerPivot);
        }

        /// <inhrritdoc />
        public override int GetHashCode()
        {
            return Civilization.GetHashCode();
        }

        #endregion
    }
}
