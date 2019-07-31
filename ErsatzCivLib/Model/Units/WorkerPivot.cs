using System;
using ErsatzCivLib.Model.Persistent;

namespace ErsatzCivLib.Model.Units
{
    [Serializable]
    public class WorkerPivot : UnitPivot
    {
        private const string RENDER_IMAGE_FILENAME = "unit_worker.png";
        private const int SPEED = 1;
        private const int PRODUCTIVITY_COST = 10;
        private const int LIFE_POINTS = 1;

        private InProgressWorkerActionPivot _currentAction = null;
        internal bool BusyOnAction { get { return _currentAction != null; } }

        internal WorkerPivot(MapSquarePivot location) :
            base(location, false, true, 0, 0, RENDER_IMAGE_FILENAME, LIFE_POINTS, LIFE_POINTS, PRODUCTIVITY_COST)
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
