using System;

namespace ErsatzCivLib.Model.Persistent
{
    [Serializable]
    public class CitizenPivot : IComparable<CitizenPivot>, IEquatable<CitizenPivot>
    {
        public const int FOOD_BY_TURN = 2;

        private Guid _uniqueId;

        public MapSquarePivot MapSquare { get; private set; }
        public MoodPivot Mood { get; internal set; }
        public CitizenTypePivot? Type { get; private set; }

        internal CitizenPivot(MapSquarePivot mapSquare)
        {
            _uniqueId = Guid.NewGuid();
            MapSquare = mapSquare;
            Mood = MoodPivot.Content;
            Type = mapSquare == null ? CitizenTypePivot.Entertainer : (CitizenTypePivot?)null;
        }

        internal void ToSpecialist(CitizenTypePivot citizenType)
        {
            Mood = MoodPivot.Content;
            Type = citizenType;
            MapSquare = null;
        }

        internal void ToCitizen(MapSquarePivot mapSquare)
        {
            MapSquare = mapSquare ?? throw new ArgumentNullException("Argument is null !", nameof(mapSquare));
            Mood = MoodPivot.Content;
            Type = null;
        }

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

        public bool Equals(CitizenPivot other)
        {
            return _uniqueId == other?._uniqueId;
        }

        public static bool operator ==(CitizenPivot ms1, CitizenPivot ms2)
        {
            if (ms1 is null)
            {
                return ms2 is null;
            }

            return ms1.Equals(ms2) == true;
        }

        public static bool operator !=(CitizenPivot ms1, CitizenPivot ms2)
        {
            return !(ms1 == ms2);
        }

        public override bool Equals(object obj)
        {
            return obj is CitizenPivot && Equals(obj as CitizenPivot);
        }

        public override int GetHashCode()
        {
            return _uniqueId.GetHashCode();
        }
    }
}
