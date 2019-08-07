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
        private const int SPEED = 1;
        private const int PRODUCTIVITY_COST = 10;
        private const int LIFE_POINTS = 1;
        private const int CITIZENS_COST = 1;

        private InProgressWorkerActionPivot _currentAction = null;

        /// <summary>
        /// Indicates if the worker is busy.
        /// </summary>
        internal bool BusyOnAction { get { return _currentAction != null; } }

        private WorkerPivot(MapSquarePivot location)
            : base(location, false, true, 0, 0, LIFE_POINTS, LIFE_POINTS, PRODUCTIVITY_COST, null, null, null, CITIZENS_COST)
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
