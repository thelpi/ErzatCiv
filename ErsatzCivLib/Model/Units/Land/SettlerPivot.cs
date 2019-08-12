using System;

namespace ErsatzCivLib.Model.Units.Land
{
    /// <summary>
    /// Represents a unit of settlers.
    /// </summary>
    /// <seealso cref="LandUnitPivot"/>
    [Serializable]
    public class SettlerPivot : LandUnitPivot
    {
        #region Embedded properties

        private InProgressMapSquareImprovementPivot _currentAction = null;
        /// <summary>
        /// Indicates if the settler is busy.
        /// </summary>
        internal bool BusyOnAction { get { return _currentAction != null; } }

        #endregion

        private SettlerPivot(CityPivot city, MapSquarePivot location) :
            base(city, 1, 0, 1, 40, null, null, 320, null, 1, location)
        { }

        /// <summary>
        /// Resets the <see cref="UnitPivot.RemainingMoves"/> if the instance is not <see cref="BusyOnAction"/>.
        /// </summary>
        internal override void Release()
        {
            RemainingMoves = BusyOnAction ? 0 : ComputeRealSpeed();
        }

        /// <summary>
        /// Sets a <see cref="InProgressMapSquareImprovementPivot"/> to the instance.
        /// </summary>
        /// <param name="action">The action to set.</param>
        internal void SetAction(InProgressMapSquareImprovementPivot action)
        {
            _currentAction = action;
        }

        /// <summary>
        /// Default instance.
        /// </summary>
        internal static readonly SettlerPivot Default = new SettlerPivot(null, null);

        /// <summary>
        /// Static constructior.
        /// </summary>
        /// <param name="city">The <see cref="UnitPivot.City"/> value.</param>
        /// <param name="location">The <see cref="UnitPivot.MapSquareLocation"/> value if <paramref name="city"/> is <c>Null</c>.</param>
        /// <returns>An instance of <see cref="SettlerPivot"/>.</returns>
        internal static SettlerPivot CreateAtLocation(CityPivot city, MapSquarePivot location)
        {
            return new SettlerPivot(city, location);
        }
    }
}
