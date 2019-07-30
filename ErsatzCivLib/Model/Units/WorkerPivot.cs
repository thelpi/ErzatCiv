using System;
using ErsatzCivLib.Model.Persistent;

namespace ErsatzCivLib.Model.Units
{
    [Serializable]
    public class WorkerPivot : UnitPivot
    {
        public const int LifePoints = 1;

        private InProgressWorkerActionPivot _currentAction = null;
        internal bool BusyOnAction { get { return _currentAction != null; } }

        internal WorkerPivot(int row, int column) : base(row, column, false, true, 0, 0,
            "unit_worker.png", RenderTypePivot.Image, LifePoints, 1)
        {

        }

        internal override void Release()
        {
            RemainingMoves = BusyOnAction ? 0 : Speed;
        }

        internal void SetAction(InProgressWorkerActionPivot action)
        {
            _currentAction = action;
        }
    }
}
