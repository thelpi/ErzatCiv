using System;
using System.Collections.Generic;
using ErsatzCivLib.Model.Static;
using ErsatzCivLib.Model.Units;

namespace ErsatzCivLib.Model
{
    /// <summary>
    /// Represents a <see cref="WorkerActionPivot"/> in progress.
    /// </summary>
    [Serializable]
    public class InProgressWorkerActionPivot
    {
        #region Embedded properties

        private List<WorkerPivot> _workers;

        /// <summary>
        /// Related <see cref="WorkerActionPivot"/>.
        /// </summary>
        public WorkerActionPivot Action { get; private set; }
        /// <summary>
        /// Number of turns already spent.
        /// </summary>
        public int TurnsCount { get; private set; }

        #endregion

        #region Inferred properties

        /// <summary>
        /// Inferred; indicates if the action has at least one worker.
        /// </summary>
        public bool HasWorkers { get { return _workers.Count > 0; } }
        /// <summary>
        /// Inferred; indicates if the action is done.
        /// </summary>
        public bool IsDone { get { return TurnsCount >= Action.TurnCost; } }

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <remarks>The task has no worker by default.</remarks>
        /// <param name="action">The action to start.</param>
        internal InProgressWorkerActionPivot(WorkerActionPivot action)
        {
            Action = action ?? throw new ArgumentNullException(nameof(action));
            _workers = new List<WorkerPivot>();
            TurnsCount = 0;
        }

        /// <summary>
        /// Adds a worker to the action.
        /// </summary>
        /// <param name="worker">The worker.</param>
        /// <returns><c>True</c> if success; <c>False</c> otherwise.</returns>
        internal bool AddWorker(WorkerPivot worker)
        {
            bool canWork = !worker.BusyOnAction;
            if (canWork)
            {
                _workers.Add(worker);
                worker.SetAction(this);
            }
            return canWork;
        }

        /// <summary>
        /// Removes every workers from the current action.
        /// </summary>
        internal void RemoveWorkers()
        {
            _workers.ForEach(w => w.SetAction(null));
            _workers.Clear();
        }

        /// <summary>
        /// Recomputes <see cref="TurnsCount"/>.
        /// </summary>
        internal void ForwardProgression()
        {
            TurnsCount += _workers.Count;
        }
    }
}
