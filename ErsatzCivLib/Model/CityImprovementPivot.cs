using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErsatzCivLib.Model
{
    public abstract class CityImprovementPivot : BuildablePivot
    {
        protected CityImprovementPivot(int productivityCost) : base (productivityCost)
        {

        }
    }
}
