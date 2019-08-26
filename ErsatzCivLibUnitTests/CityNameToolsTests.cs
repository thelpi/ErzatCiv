using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ErsatzCivLibUnitTests
{
    [TestClass]
    public class CityNameToolsTests
    {
        [TestMethod]
        public void GenerateNames()
        {
            for (int i = 0; i < 100000; i++)
            {
                string city = ErsatzCivLib.CityNameTools.GenerateCityName(ErsatzCivLib.Model.Static.CivilizationPivot.French);
                System.Diagnostics.Debug.WriteLine(city);
            }
        }
    }
}
