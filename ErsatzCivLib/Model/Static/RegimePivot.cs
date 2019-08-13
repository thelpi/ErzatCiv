using System;
using System.Collections.Generic;

namespace ErsatzCivLib.Model.Static
{
    /// <summary>
    /// Represents a political regime.
    /// </summary>
    /// <seealso cref="IEquatable{T}"/>
    [Serializable]
    public class RegimePivot : IEquatable<RegimePivot>
    {
        #region Embedded properties

        /// <summary>
        /// Name.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Corruption rate.
        /// </summary>
        public double CorruptionRate { get; private set; }
        /// <summary>
        /// Flat corruption rate (as if every cities were at the same distance from the <see cref="CityImprovementPivot.Palace"/>).
        /// </summary>
        public bool FlatCorruptionRate { get; private set; }
        /// <summary>
        /// Gold cost, as each turn, for miltary units.
        /// </summary>
        public int UnitCost { get; private set; }
        /// <summary>
        /// Commerce bonus, on each <see cref="MapSquarePivot"/> of the city area, including the city itself (if already one).
        /// </summary>
        public int CommerceBonus { get; private set; }
        /// <summary>
        /// If <c>True</c>, applies a -1 on production [food / productivity / commerce] when greater than 2.
        /// </summary>
        public bool ProductionMalus { get; private set; }
        /// <summary>
        /// Number of military units with citizen happiness effect.
        /// </summary>
        public int MartialLawUnitCount { get; private set; }
        /// <summary>
        /// <see cref="AdvancePivot"/> required to access the instance.
        /// </summary>
        public AdvancePivot AdvancePrerequisite { get; private set; }
        /// <summary>
        /// Food consumption by turn for <see cref="Units.Land.SettlerPivot"/>.
        /// </summary>
        public int SettlerFoodConsumption { get; private set; }
        /// <summary>
        /// Indicates if the war can be declared; otherwise, peace traty must be accepted.
        /// </summary>
        public bool CanDeclareWar { get; private set; }
        /// <summary>
        /// Number of unhappy citizens for each military unit outside the city.
        /// </summary>
        public int MilitaryUnhappiness { get; private set; }
        /// <summary>
        /// Number of turn with trouble inside a city before becoming an anarchy.
        /// </summary>
        public int CityTroubleTurnsBeforeAnarchy { get; private set; }
        /// <summary>
        /// Indicates if <see cref="CityImprovementPivot"/> have a maintenance cost.
        /// </summary>
        public bool MaintenanceCost { get; private set; }

        #endregion

        private RegimePivot() { }

        #region IEquatable implementation

        /// <summary>
        /// Checks if this instance is equal to another one.
        /// </summary>
        /// <param name="other">The other instance.</param>
        /// <returns><c>True</c> if equals; <c>False</c> otherwise.</returns>
        public bool Equals(RegimePivot other)
        {
            return Name == other?.Name;
        }

        /// <summary>
        /// Operator "==" override. Checks equality between two instances.
        /// </summary>
        /// <param name="r1">The first <see cref="RegimePivot"/>.</param>
        /// <param name="r2">The second <see cref="RegimePivot"/>.</param>
        /// <returns><c>True</c> if equals; <c>False</c> otherwise.</returns>
        public static bool operator ==(RegimePivot r1, RegimePivot r2)
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
        /// <param name="r1">The first <see cref="RegimePivot"/>.</param>
        /// <param name="r2">The second <see cref="RegimePivot"/>.</param>
        /// <returns><c>False</c> if equals; <c>True</c> otherwise.</returns>
        public static bool operator !=(RegimePivot r1, RegimePivot r2)
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

        #region Static instances

        /// <summary>
        /// Despotism regime.
        /// </summary>
        public static readonly RegimePivot Despotism = new RegimePivot
        {
            Name = "Despotism",
            UnitCost = 1,
            MartialLawUnitCount = 3,
            CommerceBonus = 0,
            CorruptionRate = 0.5, // TODO : get the real value.
            AdvancePrerequisite = null,
            CanDeclareWar = true,
            SettlerFoodConsumption = 1,
            FlatCorruptionRate = false,
            MilitaryUnhappiness = 0,
            CityTroubleTurnsBeforeAnarchy = 9999,
            ProductionMalus = true,
            MaintenanceCost = true
        };
        /// <summary>
        /// Monarchy regime.
        /// </summary>
        public static readonly RegimePivot Monarchy = new RegimePivot
        {
            Name = "Monarchy",
            UnitCost = 1,
            MartialLawUnitCount = 3,
            CommerceBonus = 0,
            CorruptionRate = 0.10, // TODO : get the real value.
            AdvancePrerequisite = AdvancePivot.Monarchy,
            CanDeclareWar = true,
            SettlerFoodConsumption = 2,
            FlatCorruptionRate = false,
            MilitaryUnhappiness = 0,
            CityTroubleTurnsBeforeAnarchy = 9999,
            ProductionMalus = false,
            MaintenanceCost = true
        };
        /// <summary>
        /// Republic regime.
        /// </summary>
        public static readonly RegimePivot Republic = new RegimePivot
        {
            Name = "Republic",
            UnitCost = 1,
            MartialLawUnitCount = 0,
            CommerceBonus = 1,
            CorruptionRate = 0.10, // TODO : get the real value.
            AdvancePrerequisite = AdvancePivot.Republic,
            CanDeclareWar = false,
            SettlerFoodConsumption = 2,
            FlatCorruptionRate = false,
            MilitaryUnhappiness = 1,
            CityTroubleTurnsBeforeAnarchy = 9999,
            ProductionMalus = false,
            MaintenanceCost = true
        };
        /// <summary>
        /// Anarchy regime.
        /// </summary>
        public static readonly RegimePivot Anarchy = new RegimePivot
        {
            Name = "Anarchy",
            UnitCost = 1,
            MartialLawUnitCount = 3,
            CommerceBonus = 0,
            CorruptionRate = 1,
            AdvancePrerequisite = null,
            CanDeclareWar = true,
            SettlerFoodConsumption = 1,
            FlatCorruptionRate = false,
            MilitaryUnhappiness = 0,
            CityTroubleTurnsBeforeAnarchy = 9999,
            ProductionMalus = true,
            MaintenanceCost = false
        };
        /// <summary>
        /// Democraty regime.
        /// </summary>
        public static readonly RegimePivot Democracy = new RegimePivot
        {
            Name = "Democraty",
            UnitCost = 1,
            MartialLawUnitCount = 0,
            CommerceBonus = 1,
            CorruptionRate = 0,
            AdvancePrerequisite = AdvancePivot.Democracy,
            CanDeclareWar = false,
            SettlerFoodConsumption = 2,
            FlatCorruptionRate = false,
            MilitaryUnhappiness = 2,
            CityTroubleTurnsBeforeAnarchy = 2,
            ProductionMalus = false,
            MaintenanceCost = true
        };
        /// <summary>
        /// Communism regime.
        /// </summary>
        public static readonly RegimePivot Communism = new RegimePivot
        {
            Name = "Communism",
            UnitCost = 1,
            MartialLawUnitCount = 3,
            CommerceBonus = 0,
            CorruptionRate = 0.1, // TODO : get the real value.
            AdvancePrerequisite = AdvancePivot.Communism,
            CanDeclareWar = true,
            SettlerFoodConsumption = 2,
            FlatCorruptionRate = true,
            MilitaryUnhappiness = 0,
            CityTroubleTurnsBeforeAnarchy = 9999,
            ProductionMalus = false,
            MaintenanceCost = true
        };

        #endregion

        private static List<RegimePivot> _instances = null;
        /// <summary>
        /// List of every <see cref="RegimePivot"/> instances, except <see cref="Anarchy"/>.
        /// </summary>
        public static IReadOnlyCollection<RegimePivot> Instances
        {
            get
            {
                if (_instances == null)
                {
                    _instances = Tools.GetInstancesOfTypeFromStaticFields<RegimePivot>();
                    _instances.Remove(RegimePivot.Anarchy);
                }
                return _instances;
            }
        }
    }
}
