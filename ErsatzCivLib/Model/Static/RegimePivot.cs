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
        /// Gold cost, as each turn, for miltary units.
        /// </summary>
        public int UnitCost { get; private set; }
        /// <summary>
        /// Commerce bonus, on each <see cref="MapSquarePivot"/> of the city area, including the city itself.
        /// </summary>
        public int CommerceBonus { get; private set; }
        /// <summary>
        /// Worker's efficiency rate.
        /// </summary>
        public double WorkerEffiencyRate { get; private set; }
        /// <summary>
        /// Impact on mood for each military unit in garrison in a city.
        /// </summary>
        /// <example>A <c>0.5</c> value means that 2 units make 1 <see cref="CitizenPivot"/> from unhappy to content.</example>
        public double GarrisonMoodRate { get; private set; }
        /// <summary>
        /// Science discovery speed rate.
        /// </summary>
        public double ScienceRate { get; private set; }
        /// <summary>
        /// <see cref="AdvancePivot"/> required to access the instance.
        /// </summary>
        public AdvancePivot AdvancePrerequisite { get; private set; }

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
            UnitCost = 0,
            GarrisonMoodRate = 1,
            CommerceBonus = 0,
            CorruptionRate = 0.5,
            WorkerEffiencyRate = 0.5,
            ScienceRate = 0.5,
            AdvancePrerequisite = null
        };
        /// <summary>
        /// Monarchy regime.
        /// </summary>
        public static readonly RegimePivot Monarchy = new RegimePivot
        {
            Name = "Monarchy",
            UnitCost = 0,
            GarrisonMoodRate = 0.5,
            CommerceBonus = 0,
            CorruptionRate = 0.25,
            WorkerEffiencyRate = 1,
            ScienceRate = 0.75,
            AdvancePrerequisite = AdvancePivot.Monarchy
        };
        /// <summary>
        /// Republic regime.
        /// </summary>
        public static readonly RegimePivot Republic = new RegimePivot
        {
            Name = "Republic",
            UnitCost = 1,
            GarrisonMoodRate = 1 / (double)3,
            CommerceBonus = 0,
            CorruptionRate = 0.25,
            WorkerEffiencyRate = 1,
            ScienceRate = 1,
            AdvancePrerequisite = AdvancePivot.Republic
        };
        /// <summary>
        /// Anarchy regime.
        /// </summary>
        public static readonly RegimePivot Anarchy = new RegimePivot
        {
            Name = "Anarchy",
            UnitCost = 0,
            GarrisonMoodRate = 0,
            CommerceBonus = 0,
            CorruptionRate = 1,
            WorkerEffiencyRate = 0.5,
            ScienceRate = 0,
            AdvancePrerequisite = null
        };
        /// <summary>
        /// Democraty regime.
        /// </summary>
        public static readonly RegimePivot Democracy = new RegimePivot
        {
            Name = "Democraty",
            UnitCost = 1,
            GarrisonMoodRate = 1 / (double)5,
            CommerceBonus = 1,
            CorruptionRate = 0,
            WorkerEffiencyRate = 1.5,
            ScienceRate = 1.5,
            AdvancePrerequisite = AdvancePivot.Democracy
        };
        /// <summary>
        /// Communism regime.
        /// </summary>
        public static readonly RegimePivot Communism = new RegimePivot
        {
            Name = "Communism",
            UnitCost = 0,
            GarrisonMoodRate = 0.5,
            CommerceBonus = 0,
            CorruptionRate = 0.25,
            WorkerEffiencyRate = 1,
            ScienceRate = 1,
            AdvancePrerequisite = AdvancePivot.Communism
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
