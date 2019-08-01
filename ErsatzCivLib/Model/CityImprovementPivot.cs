using System;

namespace ErsatzCivLib.Model
{
    [Serializable]
    public abstract class CityImprovementPivot : BuildablePivot
    {
        protected CityImprovementPivot(string name, int productivityCost) : base (name, productivityCost)
        {

        }
    }
}
