using System;
using ErsatzCivLib.Model.Persistent;

namespace ErsatzCivLib.Model.Units
{
    [Serializable]
    public class WorkerPivot : UnitPivot
    {
        public const int LifePoints = 1;

        internal WorkerPivot(Engine owner, int row, int column) : base(owner, row, column, false, true, 0, 0,
            "unit_worker.png", RenderTypePivot.Image, LifePoints, 1)
        {

        }

        internal override void Release()
        {
            RemainingMoves = Owner.WorkerIsBusy(this) ? 0 : Speed;
        }
    }
}
