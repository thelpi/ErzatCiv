using System;

namespace ErsatzCivLib.Model
{
    [Serializable]
    public abstract class WonderPivot : BuildablePivot
    {
        protected WonderPivot(int productivityCost, string name = null) : base(productivityCost, name)
        {

        }
    }
}
