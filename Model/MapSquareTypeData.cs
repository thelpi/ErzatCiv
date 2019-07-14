using System.Collections.Generic;
using System.Linq;

namespace ErsatzCiv.Model
{
    public class MapSquareTypeData
    {
        public string Name { get; private set; }
        public int Productivity { get; private set; }
        public int Commerce { get; private set; }
        public int Food { get; private set; }
        public string RenderColor { get; private set; }
        public int? Temperature { get; private set; }
        public int Flatness { get; private set; }
        public int? Minable { get; private set; }
        public int? Irrigable { get; private set; }

        private static List<MapSquareTypeData> _mapSquareTypeList = new List<MapSquareTypeData>();

        public static IReadOnlyCollection<MapSquareTypeData> MapSquareTypeList
        {
            get
            {
                if (_mapSquareTypeList.Count == 0)
                {
                    InitializeMapSquareTypeList();
                }
                return _mapSquareTypeList;
            }
        }

        private static void InitializeMapSquareTypeList()
        {
            _mapSquareTypeList.Add(new MapSquareTypeData
            {
                Name = COAST_NAME,
                Commerce = 2,
                Food = 1,
                Productivity = 0,
                RenderColor = "#00BFFF",
                Flatness = 5,
                Temperature = null
            });
            _mapSquareTypeList.Add(new MapSquareTypeData
            {
                Name = GRASS_NAME,
                Commerce = 1,
                Food = 2,
                Productivity = 1,
                RenderColor = "#32CD32",
                Flatness = 5,
                Temperature = 2,
                Irrigable = 2
            });
            _mapSquareTypeList.Add(new MapSquareTypeData
            {
                Name = "Mountain",
                Commerce = 0,
                Food = 0,
                Productivity = 1,
                RenderColor = "#800000",
                Flatness = 1,
                Temperature = null,
                Minable = 2
            });
            _mapSquareTypeList.Add(new MapSquareTypeData
            {
                Name = "Hill",
                Commerce = 1,
                Food = 1,
                Productivity = 1,
                RenderColor = "#800000",
                Flatness = 1,
                Temperature = 2,
                Minable = 1
            });
            _mapSquareTypeList.Add(new MapSquareTypeData
            {
                Name = "Desert",
                Commerce = 0,
                Food = 0,
                Productivity = 0,
                RenderColor = "#F4A460",
                Flatness = 2,
                Temperature = 5,
                Minable = 1
            });
        }

        private MapSquareTypeData() { }

        public static MapSquareTypeData Grass
        {
            get
            {
                return MapSquareTypeList.Single(x => x.Name == GRASS_NAME);
            }
        }
        public static MapSquareTypeData Coast
        {
            get
            {
                return MapSquareTypeList.Single(x => x.Name == COAST_NAME);
            }
        }

        private const string GRASS_NAME = "Grass";
        private const string COAST_NAME = "Coast";
    }
}
