using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErsatzCiv.Model
{
    public class SettlerPivot : UnitPivot
    {
        public const int LifePoints = 1;

        public SettlerPivot(int row, int column) :
            base(row, column, false, true, 0, 0, "unit_settler.png", RenderTypeEnum.Image, LifePoints)
        {

        }
    }
}
