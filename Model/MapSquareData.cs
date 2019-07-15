using System;
using System.Linq;

namespace ErsatzCiv.Model
{
    /// <summary>
    /// Represents a map square.
    /// </summary>
    public class MapSquareData
    {
        #region Properties

        /// <summary>
        /// Row on the map grid.
        /// </summary>
        public int Row { get; private set; }
        /// <summary>
        /// Column on the map grid.
        /// </summary>
        public int Column { get; private set; }

        /// <summary>
        /// Square type.
        /// </summary>
        public MapSquareTypeData MapSquareType { get; private set; }
        /// <summary>
        /// Underlying type in case of clearage (forest, jungle...).
        /// </summary>
        public MapSquareTypeData UnderlyingMapSquareType { get; private set; }
        /// <summary>
        /// Mine built y/n.
        /// </summary>
        public bool Mine { get; private set; }
        /// <summary>
        /// Irrigation system built y/n.
        /// </summary>
        public bool Irrigate { get; private set; }
        /// <summary>
        /// Road built y/n.
        /// </summary>
        public bool Road { get; private set; }
        /// <summary>
        /// RailRoad built y/n.
        /// </summary>
        public bool RailRoad { get; private set; }

        #endregion

        public MapSquareData(MapSquareTypeData mapSquareType, int row, int column, MapSquareTypeData underlyingType = null)
        {
            MapSquareType = mapSquareType ?? throw new ArgumentNullException(nameof(mapSquareType));
            Row = row < 0 ? throw new ArgumentException("Invalid value.", nameof(row)) : row;
            Column = column < 0 ? throw new ArgumentException("Invalid value.", nameof(column)) : column;
            if (MapSquareType.Actions.Contains(MapSquareActionEnum.Clear))
            {
                UnderlyingMapSquareType = underlyingType ?? throw new ArgumentNullException(nameof(underlyingType));
            }
        }

        /// <summary>
        /// Tries to apply a <see cref="MapSquareActionEnum"/> on the current instance.
        /// </summary>
        /// <param name="action">The action to apply.</param>
        /// <returns>
        /// <c>True</c> if the action can be apply; <c>False</c> otherwise.
        /// Returns <c>True</c> if the action has been already applied.
        /// </returns>
        public bool ApplyAction(MapSquareActionEnum action)
        {
            if (!MapSquareType.Actions.Contains(action))
            {
                return false;
            }

            switch (action)
            {
                case MapSquareActionEnum.Irrigate:
                    Irrigate = true;
                    Mine = false;
                    break;
                case MapSquareActionEnum.Mine:
                    Mine = true;
                    Irrigate = false;
                    break;
                case MapSquareActionEnum.Road:
                    Road = true;
                    break;
                case MapSquareActionEnum.DestroyImprovement:
                    Mine = false;
                    Irrigate = false;
                    break;
                case MapSquareActionEnum.DestroyRoad:
                    if (RailRoad)
                    {
                        RailRoad = false;
                    }
                    else
                    {
                        Road = false;
                    }
                    break;
                // TODO : below this line, action depends on technology
                case MapSquareActionEnum.Clear:
                    MapSquareType = UnderlyingMapSquareType;
                    Mine = false;
                    Irrigate = false;
                    break;
                case MapSquareActionEnum.RailRoad:
                    RailRoad = true;
                    break;
                case MapSquareActionEnum.Plant:
                    MapSquareType = MapSquareTypeData.Forest;
                    Mine = false;
                    Irrigate = false;
                    break;
            }

            return true;
        }
    }
}
