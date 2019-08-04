using System;

namespace ErsatzCivLib.Model
{
    [Serializable]
    public abstract class CityImprovementPivot : BuildablePivot
    {
        protected CityImprovementPivot(int productivityCost, string name = null) : base (productivityCost, name)
        {

        }
    }
}
