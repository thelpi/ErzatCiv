using System.Collections.Generic;
using System.Linq;

namespace ErsatzCivLib.Model
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

        #region Properties

        private List<MapSquareActionPivot> _actions;
        private MapSquareTypeEnum _type;

        /// <summary>
        /// Default productivity points.
        /// </summary>
        public int Productivity { get; private set; }
        /// <summary>
        /// Default commerce points.
        /// </summary>
        public int Commerce { get; private set; }
        /// <summary>
        /// Default food points.
        /// </summary>
        public int Food { get; private set; }
        /// <summary>
        /// Default defensive points.
        /// </summary>
        public int Defense { get; private set; }
        /// <summary>
        /// Render value.
        /// </summary>
        /// <remarks>Path to image; hexadecimal color code.</remarks>
        public string RenderValue { get; private set; }
        /// <summary>
        /// Type of render.
        /// </summary>
        public RenderTypeEnum RenderType { get; private set; }
        /// <summary>
        /// Indicates if the square map can be crossed by a river.
        /// </summary>
        public bool RiverCrossable { get; private set; }
        /// <summary>
        /// Indicates of the type is sea navigable (otherwise, it's ground navigable).
        /// </summary>
        public bool IsSeaType { get; private set; }
        public bool IsCityBuildable { get; private set; }
        public int SpeedCost { get; private set; }

        /// <summary>
        /// Inferred; available actions.
        /// </summary>
        public IReadOnlyCollection<MapSquareActionPivot> Actions
        {
            get { return _actions; }
        }
        /// <summary>
        /// Inferred; Name.
        /// </summary>
        public string Name { get { return _type.ToString(); } }

        #endregion

        private static readonly List<MapSquareTypeData> _mapSquareTypeList = new List<MapSquareTypeData>
        {
            // sea
            new MapSquareTypeData
            {
                _type = MapSquareTypeEnum.Sea,
                Commerce = 1,
                Food = 1,
                Productivity = 0,
                Defense = 0,
                RenderValue = "#1E90FF",
                RenderType = RenderTypeEnum.PlainBrush,
                RiverCrossable = false,
                _actions = new List<MapSquareActionPivot>(),
                IsSeaType = true,
                IsCityBuildable = false,
                SpeedCost = 1
            },
            // grassland
            new MapSquareTypeData
            {
                _type = MapSquareTypeEnum.Grassland,
                Commerce = 1,
                Food = 2,
                Productivity = 1,
                Defense = 0,
                RenderValue = "#32CD32",
                RenderType = RenderTypeEnum.PlainBrush,
                RiverCrossable = true,
                _actions = new List<MapSquareActionPivot>
                {
                    MapSquareActionPivot.Irrigate,
                    MapSquareActionPivot.Mine,
                    MapSquareActionPivot.Plant,
                    MapSquareActionPivot.RailRoad,
                    MapSquareActionPivot.Road,
                    MapSquareActionPivot.BuildFortress
                },
                IsSeaType = false,
                IsCityBuildable = true,
                SpeedCost = 1
            },
            // mountain
            new MapSquareTypeData
            {
                _type = MapSquareTypeEnum.Mountain,
                Commerce = 0,
                Food = 0,
                Productivity = 2,
                Defense = 2,
                RenderValue = "#A52A2A",
                RenderType = RenderTypeEnum.PlainBrush,
                RiverCrossable = true,
                _actions = new List<MapSquareActionPivot>
                {
                    MapSquareActionPivot.Mine,
                    MapSquareActionPivot.RailRoad,
                    MapSquareActionPivot.Road,
                    MapSquareActionPivot.BuildFortress
                },
                IsSeaType = false,
                IsCityBuildable = false,
                SpeedCost = 3
            },
            // forest
            new MapSquareTypeData
            {
                _type = MapSquareTypeEnum.Forest,
                Commerce = 1,
                Food = 1,
                Productivity = 2,
                Defense = 1,
                RenderValue = "#006400",
                RenderType = RenderTypeEnum.PlainBrush,
                RiverCrossable = true,
                _actions = new List<MapSquareActionPivot>
                {
                    MapSquareActionPivot.Clear,
                    MapSquareActionPivot.RailRoad,
                    MapSquareActionPivot.Road,
                    MapSquareActionPivot.BuildFortress
                },
                IsSeaType = false,
                IsCityBuildable = true,
                SpeedCost = 2
            },
            // plain
            new MapSquareTypeData
            {
                _type = MapSquareTypeEnum.Plain,
                Commerce = 1,
                Food = 1,
                Productivity = 1,
                Defense = 0,
                RenderValue = "#EEE8AA",
                RenderType = RenderTypeEnum.PlainBrush,
                RiverCrossable = true,
                _actions = new List<MapSquareActionPivot>
                {
                    MapSquareActionPivot.Irrigate,
                    MapSquareActionPivot.Mine,
                    MapSquareActionPivot.Plant,
                    MapSquareActionPivot.RailRoad,
                    MapSquareActionPivot.Road,
                    MapSquareActionPivot.BuildFortress
                },
                IsSeaType = false,
                IsCityBuildable = true,
                SpeedCost = 1
            },
            // ice
            new MapSquareTypeData
            {
                _type = MapSquareTypeEnum.Ice,
                Commerce = 0,
                Food = 0,
                Productivity = 0,
                Defense = 1,
                RenderValue = "#FFFAF0",
                RenderType = RenderTypeEnum.PlainBrush,
                RiverCrossable = true,
                _actions = new List<MapSquareActionPivot>
                {
                    MapSquareActionPivot.Mine,
                    MapSquareActionPivot.RailRoad,
                    MapSquareActionPivot.Road,
                    MapSquareActionPivot.BuildFortress
                },
                IsSeaType = false,
                IsCityBuildable = false,
                SpeedCost = 2
            },
            // coast
            new MapSquareTypeData
            {
                _type = MapSquareTypeEnum.Coast,
                Commerce = 1,
                Food = 1,
                Productivity = 0,
                Defense = 0,
                RenderValue = "#00BFFF",
                RenderType = RenderTypeEnum.PlainBrush,
                RiverCrossable = false,
                _actions = new List<MapSquareActionPivot>(),
                IsSeaType = true,
                IsCityBuildable = false,
                SpeedCost = 1
            },
            // toundra
            new MapSquareTypeData
            {
                _type = MapSquareTypeEnum.Toundra,
                Commerce = 0,
                Food = 1,
                Productivity = 1,
                Defense = 0,
                RenderValue = "#2F4F4F",
                RenderType = RenderTypeEnum.PlainBrush,
                RiverCrossable = true,
                _actions = new List<MapSquareActionPivot>
                {
                    MapSquareActionPivot.Irrigate,
                    MapSquareActionPivot.Mine,
                    MapSquareActionPivot.Plant,
                    MapSquareActionPivot.RailRoad,
                    MapSquareActionPivot.Road,
                    MapSquareActionPivot.BuildFortress
                },
                IsSeaType = false,
                IsCityBuildable = true,
                SpeedCost = 1
            },
            // desert
            new MapSquareTypeData
            {
                _type = MapSquareTypeEnum.Desert,
                Commerce = 0,
                Food = 0,
                Productivity = 1,
                Defense = 1,
                RenderValue = "#FF7F50",
                RenderType = RenderTypeEnum.PlainBrush,
                RiverCrossable = true,
                _actions = new List<MapSquareActionPivot>
                {
                    MapSquareActionPivot.Irrigate,
                    MapSquareActionPivot.Mine,
                    MapSquareActionPivot.RailRoad,
                    MapSquareActionPivot.Road,
                    MapSquareActionPivot.BuildFortress
                },
                IsSeaType = false,
                IsCityBuildable = true,
                SpeedCost = 2
            },
            // jungle
            new MapSquareTypeData
            {
                _type = MapSquareTypeEnum.Jungle,
                Commerce = 0,
                Food = 1,
                Productivity = 0,
                Defense = 2,
                RenderValue = "#9ACD32",
                RenderType = RenderTypeEnum.PlainBrush,
                RiverCrossable = true,
                _actions = new List<MapSquareActionPivot>
                {
                    MapSquareActionPivot.Clear,
                    MapSquareActionPivot.RailRoad,
                    MapSquareActionPivot.Road,
                    MapSquareActionPivot.BuildFortress
                },
                IsSeaType = false,
                IsCityBuildable = false,
                SpeedCost = 3
            },
            // hill
            new MapSquareTypeData
            {
                _type = MapSquareTypeEnum.Plain,
                Commerce = 1,
                Food = 1,
                Productivity = 1,
                Defense = 1,
                RenderValue = "#556B2F",
                RenderType = RenderTypeEnum.PlainBrush,
                RiverCrossable = true,
                _actions = new List<MapSquareActionPivot>
                {
                    MapSquareActionPivot.Mine,
                    MapSquareActionPivot.RailRoad,
                    MapSquareActionPivot.Road,
                    MapSquareActionPivot.BuildFortress
                },
                IsSeaType = false,
                IsCityBuildable = true,
                SpeedCost = 2
            },
            // swamp
            new MapSquareTypeData
            {
                _type = MapSquareTypeEnum.Swamp,
                Commerce = 0,
                Food = 1,
                Productivity = 0,
                Defense = 1,
                RenderValue = "#3CB371",
                RenderType = RenderTypeEnum.PlainBrush,
                RiverCrossable = true,
                _actions = new List<MapSquareActionPivot>
                {
                    MapSquareActionPivot.RailRoad,
                    MapSquareActionPivot.Road,
                    MapSquareActionPivot.BuildFortress,
                    MapSquareActionPivot.Clear
                },
                IsSeaType = false,
                IsCityBuildable = false,
                SpeedCost = 2
            }
        };

        private MapSquareTypeData() { }

        #region Direct access to instances

        /// <summary>
        /// <see cref="MapSquareTypeEnum.Grassland"/> intance.
        /// </summary>
        public static MapSquareTypeData Grassland
        {
            get
            {
                return _mapSquareTypeList.Single(x => x._type == MapSquareTypeEnum.Grassland);
            }
        }

        /// <summary>
        /// <see cref="MapSquareTypeEnum.Sea"/> intance.
        /// </summary>
        public static MapSquareTypeData Sea
        {
            get
            {
                return _mapSquareTypeList.Single(x => x._type == MapSquareTypeEnum.Sea);
            }
        }

        /// <summary>
        /// <see cref="MapSquareTypeEnum.Coast"/> intance.
        /// </summary>
        public static MapSquareTypeData Coast
        {
            get
            {
                return _mapSquareTypeList.Single(x => x._type == MapSquareTypeEnum.Coast);
            }
        }

        /// <summary>
        /// <see cref="MapSquareTypeEnum.Ice"/> intance.
        /// </summary>
        public static MapSquareTypeData Ice
        {
            get
            {
                return _mapSquareTypeList.Single(x => x._type == MapSquareTypeEnum.Ice);
            }
        }

        /// <summary>
        /// <see cref="MapSquareTypeEnum.Toundra"/> intance.
        /// </summary>
        public static MapSquareTypeData Toundra
        {
            get
            {
                return _mapSquareTypeList.Single(x => x._type == MapSquareTypeEnum.Toundra);
            }
        }

        /// <summary>
        /// <see cref="MapSquareTypeEnum.Desert"/> intance.
        /// </summary>
        public static MapSquareTypeData Desert
        {
            get
            {
                return _mapSquareTypeList.Single(x => x._type == MapSquareTypeEnum.Desert);
            }
        }

        /// <summary>
        /// <see cref="MapSquareTypeEnum.Jungle"/> intance.
        /// </summary>
        public static MapSquareTypeData Jungle
        {
            get
            {
                return _mapSquareTypeList.Single(x => x._type == MapSquareTypeEnum.Jungle);
            }
        }

        /// <summary>
        /// <see cref="MapSquareTypeEnum.Mountain"/> intance.
        /// </summary>
        public static MapSquareTypeData Mountain
        {
            get
            {
                return _mapSquareTypeList.Single(x => x._type == MapSquareTypeEnum.Mountain);
            }
        }

        /// <summary>
        /// <see cref="MapSquareTypeEnum.Hill"/> intance.
        /// </summary>
        public static MapSquareTypeData Hill
        {
            get
            {
                return _mapSquareTypeList.Single(x => x._type == MapSquareTypeEnum.Hill);
            }
        }

        /// <summary>
        /// <see cref="MapSquareTypeEnum.Swamp"/> intance.
        /// </summary>
        public static MapSquareTypeData Swamp
        {
            get
            {
                return _mapSquareTypeList.Single(x => x._type == MapSquareTypeEnum.Swamp);
            }
        }

        /// <summary>
        /// <see cref="MapSquareTypeEnum.Forest"/> intance.
        /// </summary>
        public static MapSquareTypeData Forest
        {
            get
            {
                return _mapSquareTypeList.Single(x => x._type == MapSquareTypeEnum.Forest);
            }
        }

        /// <summary>
        /// <see cref="MapSquareTypeEnum.Plain"/> intance.
        /// </summary>
        public static MapSquareTypeData Plain
        {
            get
            {
                return _mapSquareTypeList.Single(x => x._type == MapSquareTypeEnum.Plain);
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
