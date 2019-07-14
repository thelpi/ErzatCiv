using System;
using System.Collections.Generic;
using System.Linq;

namespace ErsatzCiv.Model
{
    /// <summary>
    /// Represents a type of square on the map.
    /// </summary>
    public class MapSquareTypeData
    {
        /// <summary>
        /// Productivity, food and commerce bonus when the square is through by a river.
        /// </summary>
        public const int RIVER_BONUS = 1;

        private List<MapSquareActionEnum> _actions = new List<MapSquareActionEnum>();
        private MapSquareTypeEnum _type;
        private MapSquareTypeEnum? _underlyingType;

        public string Name { get { return _type.ToString(); } }
        public int Productivity { get; private set; }
        public int Commerce { get; private set; }
        public int Food { get; private set; }
        public string RenderColor { get; private set; }
        public RenderTypeEnum RenderType { get; private set; }
        public bool Riverable { get; private set; }

        // Use 'MapSquareTypeList' instead (even inside this class).
        private static List<MapSquareTypeData> _mapSquareTypeList = new List<MapSquareTypeData>();

        /// <summary>
        /// List of every instances.
        /// </summary>
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
            // sea
            _mapSquareTypeList.Add(new MapSquareTypeData
            {
                _type = MapSquareTypeEnum.Sea,
                Commerce = 1,
                Food = 1,
                Productivity = 0,
                RenderColor = "#00BFFF",
                RenderType = RenderTypeEnum.PlainBrush,
                Riverable = false,
                _underlyingType = null/*,
                _actions.Add()*/
            });
            // grassland
            _mapSquareTypeList.Add(new MapSquareTypeData
            {
                _type = MapSquareTypeEnum.Grassland,
                Commerce = 1,
                Food = 2,
                Productivity = 1,
                RenderColor = "#32CD32",
                RenderType = RenderTypeEnum.PlainBrush,
                Riverable = true
            });
        }

        private MapSquareTypeData() { }

        #region Direct access to instances

        /// <summary>
        /// <see cref="MapSquareTypeEnum.Grassland"/> intance.
        /// </summary>
        public static MapSquareTypeData Grassland
        {
            get
            {
                return MapSquareTypeList.Single(x => x._type == MapSquareTypeEnum.Grassland);
            }
        }

        /// <summary>
        /// <see cref="MapSquareTypeEnum.Sea"/> intance.
        /// </summary>
        public static MapSquareTypeData Sea
        {
            get
            {
                return MapSquareTypeList.Single(x => x._type == MapSquareTypeEnum.Sea);
            }
        }

        /// <summary>
        /// <see cref="MapSquareTypeEnum.Coast"/> intance.
        /// </summary>
        public static MapSquareTypeData Coast
        {
            get
            {
                return MapSquareTypeList.Single(x => x._type == MapSquareTypeEnum.Coast);
            }
        }

        /// <summary>
        /// <see cref="MapSquareTypeEnum.Ice"/> intance.
        /// </summary>
        public static MapSquareTypeData Ice
        {
            get
            {
                return MapSquareTypeList.Single(x => x._type == MapSquareTypeEnum.Ice);
            }
        }

        /// <summary>
        /// <see cref="MapSquareTypeEnum.Toundra"/> intance.
        /// </summary>
        public static MapSquareTypeData Toundra
        {
            get
            {
                return MapSquareTypeList.Single(x => x._type == MapSquareTypeEnum.Toundra);
            }
        }

        /// <summary>
        /// <see cref="MapSquareTypeEnum.Desert"/> intance.
        /// </summary>
        public static MapSquareTypeData Desert
        {
            get
            {
                return MapSquareTypeList.Single(x => x._type == MapSquareTypeEnum.Desert);
            }
        }

        /// <summary>
        /// <see cref="MapSquareTypeEnum.Jungle"/> intance.
        /// </summary>
        public static MapSquareTypeData Jungle
        {
            get
            {
                return MapSquareTypeList.Single(x => x._type == MapSquareTypeEnum.Jungle);
            }
        }

        /// <summary>
        /// <see cref="MapSquareTypeEnum.Mountain"/> intance.
        /// </summary>
        public static MapSquareTypeData Mountain
        {
            get
            {
                return MapSquareTypeList.Single(x => x._type == MapSquareTypeEnum.Mountain);
            }
        }

        /// <summary>
        /// <see cref="MapSquareTypeEnum.Hill"/> intance.
        /// </summary>
        public static MapSquareTypeData Hill
        {
            get
            {
                return MapSquareTypeList.Single(x => x._type == MapSquareTypeEnum.Hill);
            }
        }

        /// <summary>
        /// <see cref="MapSquareTypeEnum.Swamp"/> intance.
        /// </summary>
        public static MapSquareTypeData Swamp
        {
            get
            {
                return MapSquareTypeList.Single(x => x._type == MapSquareTypeEnum.Swamp);
            }
        }

        /// <summary>
        /// <see cref="MapSquareTypeEnum.Forest"/> intance.
        /// </summary>
        public static MapSquareTypeData Forest
        {
            get
            {
                return MapSquareTypeList.Single(x => x._type == MapSquareTypeEnum.Forest);
            }
        }

        /// <summary>
        /// <see cref="MapSquareTypeEnum.Plain"/> intance.
        /// </summary>
        public static MapSquareTypeData Plain
        {
            get
            {
                return MapSquareTypeList.Single(x => x._type == MapSquareTypeEnum.Plain);
            }
        }

        #endregion

        /// <summary>
        /// Types enumeration.
        /// </summary>
        private enum MapSquareTypeEnum
        {
            Coast,
            Sea,
            Ice,
            Toundra,
            Desert,
            Jungle,
            Mountain,
            Hill,
            Grassland,
            Swamp,
            Forest,
            Plain
        }
    }
}
