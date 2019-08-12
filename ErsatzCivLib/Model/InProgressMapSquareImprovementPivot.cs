using System;
using System.Collections.Generic;
using ErsatzCivLib.Model.Static;
using ErsatzCivLib.Model.Units.Land;

namespace ErsatzCivLib.Model
{
    /// <summary>
    /// Represents a <see cref="MapSquareImprovementPivot"/> in progress.
    /// </summary>
    [Serializable]
    public class InProgressMapSquareImprovementPivot
    {
        #region Embedded properties

        private List<SettlerPivot> _settlers;

        /// <summary>
        /// Related <see cref="MapSquareImprovementPivot"/>.
        /// </summary>
        public MapSquareImprovementPivot Action { get; private set; }
        /// <summary>
        /// Number of turns already spent.
        /// </summary>
        public int TurnsCount { get; private set; }

        #endregion

        #region Inferred properties

        /// <summary>
        /// Inferred; indicates if the action has at least one settler.
        /// </summary>
        public bool HasSettlers { get { return _settlers.Count > 0; } }
        /// <summary>
        /// Inferred; indicates if the action is done.
        /// </summary>
        public bool IsDone { get { return TurnsCount >= Action.TurnCost; } }

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <remarks>The task has no settler by default.</remarks>
        /// <param name="action">The action to start.</param>
        internal InProgressMapSquareImprovementPivot(MapSquareImprovementPivot action)
        {
            Action = action ?? throw new ArgumentNullException(nameof(action));
            _settlers = new List<SettlerPivot>();
            TurnsCount = 0;
        }

        /// <summary>
        /// Adds a settler to the action.
        /// </summary>
        /// <param name="settler">The settler.</param>
        /// <returns><c>True</c> if success; <c>False</c> otherwise.</returns>
        internal bool AddSettler(SettlerPivot settler)
        {
            bool canWork = !settler.BusyOnAction;
            if (canWork)
            {
                _settlers.Add(settler);
                settler.SetAction(this);
            }
            return canWork;
        }

        /// <summary>
        /// Removes every settlers from the current action.
        /// </summary>
        internal void RemoveSettlers()
        {
            _settlers.ForEach(w => w.SetAction(null));
            _settlers.Clear();
        }

        /// <summary>
        /// Recomputes <see cref="TurnsCount"/>.
        /// </summary>
        internal void ForwardProgression()
        {
            TurnsCount += _settlers.Count;
        }
    }
}
