using System;
using System.Collections.Generic;

namespace ErsatzCivLib.Model.Persistent
{
    [Serializable]
    public class WonderPivot : BuildablePivot
    {
        private WonderPivot(int productivityCost, string name = null, bool hasCitizenMoodEffect = false) :
            base(productivityCost, name, hasCitizenMoodEffect)
        { }

        public bool Equals(WonderPivot other)
        {
            return Name == other?.Name;
        }

        public static bool operator ==(WonderPivot ms1, WonderPivot ms2)
        {
            if (ms1 is null)
            {
                return ms2 is null;
            }

            return ms1.Equals(ms2) == true;
        }

        public static bool operator ==(WonderPivot ms1, BuildablePivot ms2)
        {
            if (ms1 is null)
            {
                return ms2 is null;
            }

            return ms1.Equals(ms2) == true;
        }

        public static bool operator !=(WonderPivot ms1, WonderPivot ms2)
        {
            return !(ms1 == ms2);
        }

        public static bool operator !=(WonderPivot ms1, BuildablePivot ms2)
        {
            return !(ms1 == ms2);
        }

        public override bool Equals(object obj)
        {
            return obj is WonderPivot && Equals(obj as WonderPivot);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        #region Static instances

        // TODO : add new instances to "Instances".

        public static readonly IReadOnlyCollection<WonderPivot> Instances = new List<WonderPivot>
        {

        };

        #endregion
    }
}
