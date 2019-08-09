using System;

namespace ErsatzCivLib.Model.Units
{
    /// <summary>
    /// Represents a worker.
    /// </summary>
    /// <seealso cref="UnitPivot"/>
    [Serializable]
    public class WorkerPivot : UnitPivot
    {
        #region Embedded properties

        private InProgressWorkerActionPivot _currentAction = null;
        /// <summary>
        /// Indicates if the worker is busy.
        /// </summary>
        internal bool BusyOnAction { get { return _currentAction != null; } }

        #endregion

        private WorkerPivot(MapSquarePivot location)
            : base(location, false, true, 0, 0, 1, 1, 10, null, null, -1, null, 1)
        { }

        /// <summary>
        /// Resets the <see cref="UnitPivot.RemainingMoves"/> if the instance is not <see cref="BusyOnAction"/>.
        /// </summary>
        internal override void Release()
        {
            RemainingMoves = BusyOnAction ? 0 : Speed;
        }

        /// <summary>
        /// Sets a <see cref="InProgressWorkerActionPivot"/> to the instance.
        /// </summary>
        /// <param name="action">The action to set.</param>
        internal void SetAction(InProgressWorkerActionPivot action)
        {
            _currentAction = action;
        }

        /// <summary>
        /// Default instance.
        /// </summary>
        internal static readonly WorkerPivot Default = new WorkerPivot(null);

        /// <summary>
        /// Static constructior.
        /// </summary>
        /// <param name="location">Builder location.</param>
        /// <returns>An instance of <see cref="WorkerPivot"/>.</returns>
        internal static WorkerPivot CreateAtLocation(MapSquarePivot location)
        {
            return new WorkerPivot(location);
        }
    }
}
