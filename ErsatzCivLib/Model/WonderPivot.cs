using System;

namespace ErsatzCivLib.Model
{
    [Serializable]
    public class WonderPivot : BuildablePivot
    {
        protected WonderPivot(string name, int productivityCost) : base(name, productivityCost)
        {

        }
    }
}
