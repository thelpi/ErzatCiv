using System;

namespace ErsatzCivLib.Model.Enums
{
    /// <summary>
    /// Represents the directions (or cardinals).
    /// </summary>
    [Serializable]
    public enum DirectionPivot
    {
        /// <summary>
        /// Top.
        /// </summary>
        Top,
        /// <summary>
        /// Bottom.
        /// </summary>
        Bottom,
        /// <summary>
        /// Right.
        /// </summary>
        Right,
        /// <summary>
        /// Left.
        /// </summary>
        Left,
        /// <summary>
        /// Top right.
        /// </summary>
        TopRight,
        /// <summary>
        /// Top left.
        /// </summary>
        TopLeft,
        /// <summary>
        /// Bottom right.
        /// </summary>
        BottomRight,
        /// <summary>
        /// Bottom left.
        /// </summary>
        BottomLeft
    }
}
