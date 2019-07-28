using System.Collections.Generic;

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
        /// <summary>
        /// Indicates of a city can be built on it.
        /// </summary>
        public bool IsCityBuildable { get; private set; }
        /// <summary>
        /// Indicates the speed cost when a unit walks through it.
        /// </summary>
        public int SpeedCost { get; private set; }

        /// <summary>
        /// Available <see cref="MapSquareActionPivot"/>.
        /// Doesn't include <see cref="MapSquareActionPivot.AlwaysAvailable"/>.
        /// </summary>
        public IReadOnlyCollection<MapSquareActionPivot> Actions
        {
            get { return _actions; }
        }

        #endregion

        private MapSquareTypeData() { }

        #region Direct access to instances

        public static MapSquareTypeData Grassland { get; } = new MapSquareTypeData
        {
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
        };
        public static MapSquareTypeData Sea { get; } = new MapSquareTypeData
        {
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
        };
        public static MapSquareTypeData Ice { get; } = new MapSquareTypeData
        {
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
        };
        public static MapSquareTypeData Toundra { get; } = new MapSquareTypeData
        {
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
        };
        public static MapSquareTypeData Desert { get; } = new MapSquareTypeData
        {
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
        };
        public static MapSquareTypeData Jungle { get; } = new MapSquareTypeData
        {
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
        };
        public static MapSquareTypeData Mountain { get; } = new MapSquareTypeData
        {
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
        };
        public static MapSquareTypeData Hill { get; } = new MapSquareTypeData
        {
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
        };
        public static MapSquareTypeData Swamp { get; } = new MapSquareTypeData
        {
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
        };
        public static MapSquareTypeData Forest { get; } = new MapSquareTypeData
        {
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
        };
        public static MapSquareTypeData Plain { get; } = new MapSquareTypeData
        {
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
        };
        public static MapSquareTypeData Coast { get; } = new MapSquareTypeData
        {
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
        };

        #endregion
    }
}
