using System;

namespace ErsatzCivLib.Model.Units
{
    /// <summary>
    /// Represents a settler.
    /// </summary>
    /// <seealso cref="UnitPivot"/>
    [Serializable]
    public class SettlerPivot : UnitPivot
    {
        private const int SPEED = 1;
        private const int PRODUCTIVITY_COST = 20;
        private const int LIFE_POINTS = 1;
        private const int CITIZENS_COST = 2;

        private SettlerPivot(MapSquarePivot location) :
            base(location, false, true, 0, 0, LIFE_POINTS, SPEED, PRODUCTIVITY_COST, null, null, CITIZENS_COST)
        { }

        /// <summary>
        /// Default instance.
        /// </summary>
        internal static readonly SettlerPivot Default = new SettlerPivot(null);

        /// <summary>
        /// Static constructior.
        /// </summary>
        /// <param name="location">Builder location.</param>
        /// <returns>An instance of <see cref="SettlerPivot"/>.</returns>
        internal static SettlerPivot CreateAtLocation(MapSquarePivot location)
        {
            return new SettlerPivot(location);
        }
    }
}
