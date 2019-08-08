using System;
using System.Linq;
using ErsatzCivLib.Model.Enums;

namespace ErsatzCivLib.Model
{
    /// <summary>
    /// Represents a citizen inside a city.
    /// </summary>
    /// <seealso cref="IComparable{T}"/>
    /// <seealso cref="IEquatable{T}"/>
    [Serializable]
    public class CitizenPivot : IComparable<CitizenPivot>, IEquatable<CitizenPivot>
    {
        /// <summary>
        /// Food unit consumption by turn.
        /// </summary>
        public const int FOOD_BY_TURN = 2;

        private Guid _uniqueId;

        /// <summary>
        /// Working <see cref="MapSquarePivot"/> for regular citizen. <c>Null</c> for specialist.
        /// </summary>
        public MapSquarePivot MapSquare { get; private set; }
        /// <summary>
        /// Citizen mood.
        /// </summary>
        public MoodPivot Mood { get; internal set; }
        /// <summary>
        /// Type of citizen. <c>Null</c> for regular.
        /// </summary>
        public CitizenTypePivot? Type { get; private set; }

        /// <summary>
        /// The <see cref="CityPivot"/> which owns the citizen.
        /// </summary>
        public CityPivot City
        {
            get
            {
                return EnginePivot.Default.Players.SelectMany(p => p.Cities).Single(c => c.Citizens.Contains(this));
            }
        }

        /// <summary>
        /// Constructor.
        /// By default the citizen is <see cref="MoodPivot.Content"/> and regular if <paramref name="mapSquare"/> is specified;
        /// otherwise it's an <see cref="CitizenTypePivot.Entertainer"/>.
        /// </summary>
        /// <param name="mapSquare">The <see cref="MapSquare"/> value.</param>
        internal CitizenPivot(MapSquarePivot mapSquare)
        {
            _uniqueId = Guid.NewGuid();
            MapSquare = mapSquare;
            Mood = MoodPivot.Content;
            Type = mapSquare == null ? CitizenTypePivot.Entertainer : (CitizenTypePivot?)null;
        }

        /// <summary>
        /// Transforms a citizen into a specialist.
        /// </summary>
        /// <param name="citizenType">Type of specialist.</param>
        internal void ToSpecialist(CitizenTypePivot citizenType)
        {
            Mood = MoodPivot.Content;
            Type = citizenType;
            MapSquare = null;
        }

        /// <summary>
        /// Transforms a specialist into a citizen.
        /// </summary>
        /// <param name="mapSquare">Working map square.</param>
        internal void ToCitizen(MapSquarePivot mapSquare)
        {
            MapSquare = mapSquare ?? throw new ArgumentNullException("Argument is null !", nameof(mapSquare));
            Mood = MoodPivot.Content;
            Type = null;
        }

        /// <inheritdoc />
        public int CompareTo(CitizenPivot other)
        {
            if (other == null)
            {
                return -1;
            }

            var compareType = Type.HasValue ?
                (other.Type.HasValue ? ((int)Type.Value).CompareTo((int)other.Type.Value) : 1) :
                (other.Type.HasValue ? -1 : 0);
            var compareMood = ((int)Mood).CompareTo((int)other.Mood);
            var compareMapS = (MapSquare?.TotalValue).GetValueOrDefault(0).CompareTo((other.MapSquare?.TotalValue).GetValueOrDefault(0));

            return compareType == 0 ? (compareMood == 0 ? compareMapS : compareMood) : compareType;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return (!Type.HasValue ? Mood.ToString() : Type.ToString());
        }

        #region IEquatable implementation

        /// <summary>
        /// Checks if this instance is equal to another one.
        /// </summary>
        /// <param name="other">The other instance.</param>
        /// <returns><c>True</c> if equals; <c>False</c> otherwise.</returns>
        public bool Equals(CitizenPivot other)
        {
            return _uniqueId == other?._uniqueId;
        }

        /// <summary>
        /// Operator "==" override. Checks equality between two instances.
        /// </summary>
        /// <param name="r1">The first <see cref="CitizenPivot"/>.</param>
        /// <param name="r2">The second <see cref="CitizenPivot"/>.</param>
        /// <returns><c>True</c> if equals; <c>False</c> otherwise.</returns>
        public static bool operator ==(CitizenPivot r1, CitizenPivot r2)
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
        /// <param name="r1">The first <see cref="CitizenPivot"/>.</param>
        /// <param name="r2">The second <see cref="CitizenPivot"/>.</param>
        /// <returns><c>False</c> if equals; <c>True</c> otherwise.</returns>
        public static bool operator !=(CitizenPivot r1, CitizenPivot r2)
        {
            return !(r1 == r2);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is CitizenPivot && Equals(obj as CitizenPivot);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return _uniqueId.GetHashCode();
        }

        #endregion
    }
}
