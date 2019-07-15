using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErsatzCiv.Model
{
    public class UnitTypeData
    {
        private static readonly List<UnitTypeData> _unitTypeData = new List<UnitTypeData>
        {

        };

        public string Name { get; private set; }
        public string RenderValue { get; private set; }
        public RenderTypeEnum RenderType { get; private set; }

        private UnitTypeData() { }


    }
}
