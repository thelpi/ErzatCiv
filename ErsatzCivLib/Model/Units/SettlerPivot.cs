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
        private SettlerPivot(MapSquarePivot location) :
            base(location, false, true, 0, 0, 1, 1, 20, null, null, null, 2)
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
