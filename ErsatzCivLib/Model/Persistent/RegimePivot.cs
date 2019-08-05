using System;

namespace ErsatzCivLib.Model.Persistent
{
    [Serializable]
    public class RegimePivot : IEquatable<RegimePivot>
    {
        public string Name { get; private set; }

        private RegimePivot() { }

        public bool Equals(RegimePivot other)
        {
            return Name == other?.Name;
        }

        public static bool operator ==(RegimePivot ms1, RegimePivot ms2)
        {
            if (ms1 is null)
            {
                return ms2 is null;
            }

            return ms1.Equals(ms2) == true;
        }

        public static bool operator !=(RegimePivot ms1, RegimePivot ms2)
        {
            return !(ms1 == ms2);
        }

        public override bool Equals(object obj)
        {
            return obj is RegimePivot && Equals(obj as RegimePivot);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        #region Static instances

        public static readonly RegimePivot Despotism = new RegimePivot { Name = "Despotism" };

        #endregion
    }
}
