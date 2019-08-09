using System;

namespace ErsatzCivLib.Model.Units
{
    /// <summary>
    /// Represents a unit of settlers.
    /// </summary>
    /// <seealso cref="UnitPivot"/>
    [Serializable]
    public class SettlersPivot : UnitPivot
    {
        private SettlersPivot(MapSquarePivot location) :
            base(location, false, true, 1, 0, 1, 1, 40, null, null, 320, null, 1)
        { }

        /// <summary>
        /// Default instance.
        /// </summary>
        internal static readonly SettlersPivot Default = new SettlersPivot(null);

        /// <summary>
        /// Static constructior.
        /// </summary>
        /// <param name="location">Builder location.</param>
        /// <returns>An instance of <see cref="SettlersPivot"/>.</returns>
        internal static SettlersPivot CreateAtLocation(MapSquarePivot location)
        {
            return new SettlersPivot(location);
        }
    }
}
