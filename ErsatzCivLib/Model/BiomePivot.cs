using System.Collections.Generic;

namespace ErsatzCivLib.Model
{
    /// <summary>
    /// Represents a type of square on the map.
    /// </summary>
    public class BiomePivot
    {
        /// <summary>
        /// Productivity, food and commerce bonus when the square is through by a river.
        /// </summary>
        public const int RIVER_BONUS = 1;

        #region Properties

        private List<WorkerActionPivot> _actions;
        private List<TemperaturePivot> _temperatures;

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
        public RenderTypePivot RenderType { get; private set; }
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
        /// Available <see cref="WorkerActionPivot"/>.
        /// Doesn't include <see cref="WorkerActionPivot.AlwaysAvailable"/>.
        /// </summary>
        public IReadOnlyCollection<WorkerActionPivot> Actions
        {
            get { return _actions; }
        }
        /// <summary>
        /// List of <see cref="TemperaturePivot"/> (depending on latitude) where the biome can appears.
        /// </summary>
        public IReadOnlyCollection<TemperaturePivot> Temperatures
        {
            get { return _temperatures; }
        }

        #endregion

        private BiomePivot() { }

        #region Direct access to instances

        public static BiomePivot Grassland { get; } = new BiomePivot
        {
            Commerce = 1,
            Food = 2,
            Productivity = 1,
            Defense = 0,
            RenderValue = "#32CD32",
            RenderType = RenderTypePivot.PlainBrush,
            RiverCrossable = true,
            _actions = new List<WorkerActionPivot>
                {
                    WorkerActionPivot.Irrigate,
                    WorkerActionPivot.Mine,
                    WorkerActionPivot.Plant,
                    WorkerActionPivot.RailRoad,
                    WorkerActionPivot.Road,
                    WorkerActionPivot.BuildFortress
                },
            IsSeaType = false,
            IsCityBuildable = true,
            SpeedCost = 1
        };
        public static BiomePivot Sea { get; } = new BiomePivot
        {
            Commerce = 1,
            Food = 1,
            Productivity = 0,
            Defense = 0,
            RenderValue = "#1E90FF",
            RenderType = RenderTypePivot.PlainBrush,
            RiverCrossable = false,
            _actions = new List<WorkerActionPivot>(),
            IsSeaType = true,
            IsCityBuildable = false,
            SpeedCost = 1
        };
        public static BiomePivot Ice { get; } = new BiomePivot
        {
            Commerce = 0,
            Food = 0,
            Productivity = 0,
            Defense = 1,
            RenderValue = "#FFFAF0",
            RenderType = RenderTypePivot.PlainBrush,
            RiverCrossable = true,
            _actions = new List<WorkerActionPivot>
                {
                    WorkerActionPivot.Mine,
                    WorkerActionPivot.RailRoad,
                    WorkerActionPivot.Road,
                    WorkerActionPivot.BuildFortress
                },
            IsSeaType = false,
            IsCityBuildable = false,
            SpeedCost = 2
        };
        public static BiomePivot Toundra { get; } = new BiomePivot
        {
            Commerce = 0,
            Food = 1,
            Productivity = 1,
            Defense = 0,
            RenderValue = "#2F4F4F",
            RenderType = RenderTypePivot.PlainBrush,
            RiverCrossable = true,
            _actions = new List<WorkerActionPivot>
                {
                    WorkerActionPivot.Irrigate,
                    WorkerActionPivot.Mine,
                    WorkerActionPivot.Plant,
                    WorkerActionPivot.RailRoad,
                    WorkerActionPivot.Road,
                    WorkerActionPivot.BuildFortress
                },
            IsSeaType = false,
            IsCityBuildable = true,
            SpeedCost = 1
        };
        public static BiomePivot Desert { get; } = new BiomePivot
        {
            Commerce = 0,
            Food = 0,
            Productivity = 1,
            Defense = 1,
            RenderValue = "#FF7F50",
            RenderType = RenderTypePivot.PlainBrush,
            RiverCrossable = true,
            _actions = new List<WorkerActionPivot>
                {
                    WorkerActionPivot.Irrigate,
                    WorkerActionPivot.Mine,
                    WorkerActionPivot.RailRoad,
                    WorkerActionPivot.Road,
                    WorkerActionPivot.BuildFortress
                },
            IsSeaType = false,
            IsCityBuildable = true,
            SpeedCost = 2
        };
        public static BiomePivot Jungle { get; } = new BiomePivot
        {
            Commerce = 0,
            Food = 1,
            Productivity = 0,
            Defense = 2,
            RenderValue = "#9ACD32",
            RenderType = RenderTypePivot.PlainBrush,
            RiverCrossable = true,
            _actions = new List<WorkerActionPivot>
                {
                    WorkerActionPivot.Clear,
                    WorkerActionPivot.RailRoad,
                    WorkerActionPivot.Road,
                    WorkerActionPivot.BuildFortress
                },
            IsSeaType = false,
            IsCityBuildable = false,
            SpeedCost = 3
        };
        public static BiomePivot Mountain { get; } = new BiomePivot
        {
            Commerce = 0,
            Food = 0,
            Productivity = 2,
            Defense = 2,
            RenderValue = "#A52A2A",
            RenderType = RenderTypePivot.PlainBrush,
            RiverCrossable = true,
            _actions = new List<WorkerActionPivot>
                {
                    WorkerActionPivot.Mine,
                    WorkerActionPivot.RailRoad,
                    WorkerActionPivot.Road,
                    WorkerActionPivot.BuildFortress
                },
            IsSeaType = false,
            IsCityBuildable = false,
            SpeedCost = 3
        };
        public static BiomePivot Hill { get; } = new BiomePivot
        {
            Commerce = 1,
            Food = 1,
            Productivity = 1,
            Defense = 1,
            RenderValue = "#556B2F",
            RenderType = RenderTypePivot.PlainBrush,
            RiverCrossable = true,
            _actions = new List<WorkerActionPivot>
                {
                    WorkerActionPivot.Mine,
                    WorkerActionPivot.RailRoad,
                    WorkerActionPivot.Road,
                    WorkerActionPivot.BuildFortress
                },
            IsSeaType = false,
            IsCityBuildable = true,
            SpeedCost = 2
        };
        public static BiomePivot Swamp { get; } = new BiomePivot
        {
            Commerce = 0,
            Food = 1,
            Productivity = 0,
            Defense = 1,
            RenderValue = "#3CB371",
            RenderType = RenderTypePivot.PlainBrush,
            RiverCrossable = true,
            _actions = new List<WorkerActionPivot>
                {
                    WorkerActionPivot.RailRoad,
                    WorkerActionPivot.Road,
                    WorkerActionPivot.BuildFortress,
                    WorkerActionPivot.Clear
                },
            IsSeaType = false,
            IsCityBuildable = false,
            SpeedCost = 2
        };
        public static BiomePivot Forest { get; } = new BiomePivot
        {
            Commerce = 1,
            Food = 1,
            Productivity = 2,
            Defense = 1,
            RenderValue = "#006400",
            RenderType = RenderTypePivot.PlainBrush,
            RiverCrossable = true,
            _actions = new List<WorkerActionPivot>
                {
                    WorkerActionPivot.Clear,
                    WorkerActionPivot.RailRoad,
                    WorkerActionPivot.Road,
                    WorkerActionPivot.BuildFortress
                },
            IsSeaType = false,
            IsCityBuildable = true,
            SpeedCost = 2
        };
        public static BiomePivot Plain { get; } = new BiomePivot
        {
            Commerce = 1,
            Food = 1,
            Productivity = 1,
            Defense = 0,
            RenderValue = "#EEE8AA",
            RenderType = RenderTypePivot.PlainBrush,
            RiverCrossable = true,
            _actions = new List<WorkerActionPivot>
                {
                    WorkerActionPivot.Irrigate,
                    WorkerActionPivot.Mine,
                    WorkerActionPivot.Plant,
                    WorkerActionPivot.RailRoad,
                    WorkerActionPivot.Road,
                    WorkerActionPivot.BuildFortress
                },
            IsSeaType = false,
            IsCityBuildable = true,
            SpeedCost = 1
        };
        public static BiomePivot Coast { get; } = new BiomePivot
        {
            Commerce = 1,
            Food = 1,
            Productivity = 0,
            Defense = 0,
            RenderValue = "#00BFFF",
            RenderType = RenderTypePivot.PlainBrush,
            RiverCrossable = false,
            _actions = new List<WorkerActionPivot>(),
            IsSeaType = true,
            IsCityBuildable = false,
            SpeedCost = 1
        };

        #endregion
    }
}
