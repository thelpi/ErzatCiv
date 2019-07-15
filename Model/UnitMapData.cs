using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErsatzCiv.Model
{
    public class UnitMapData
    {
        public UnitTypeData UnitType { get; private set; }
        public int Row { get; private set; }
        public int Column { get; private set; }
    }
}
