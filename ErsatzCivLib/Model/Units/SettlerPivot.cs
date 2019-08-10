using System;

namespace ErsatzCivLib.Model.Units
{
    /// <summary>
    /// Represents a unit of settlers.
    /// </summary>
    /// <seealso cref="UnitPivot"/>
    [Serializable]
    public class SettlerPivot : UnitPivot
    {
        #region Embedded properties

        private InProgressMapSquareImprovementPivot _currentAction = null;
        /// <summary>
        /// Indicates if the settler is busy.
        /// </summary>
        internal bool BusyOnAction { get { return _currentAction != null; } }

        #endregion

        private SettlerPivot(MapSquarePivot location) :
            base(location, false, true, 1, 0, 1, 1, 40, null, null, 320, null, 1)
        { }

        /// <summary>
        /// Resets the <see cref="UnitPivot.RemainingMoves"/> if the instance is not <see cref="BusyOnAction"/>.
        /// </summary>
        internal override void Release()
        {
            RemainingMoves = BusyOnAction ? 0 : Speed;
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
