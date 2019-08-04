using System;

namespace ErsatzCivLib.Model
{
    [Serializable]
    public abstract class WonderPivot : BuildablePivot
    {
        protected WonderPivot(int productivityCost, string name = null, bool hasCitizenMoodEffect = false) :
            base(productivityCost, name, hasCitizenMoodEffect)
        {

        }
    }
}
