using System;

namespace ErsatzCivLib.Model.Units
{
    [Serializable]
    public class WorkerPivot : UnitPivot
    {
        public const int LifePoints = 1;

        internal WorkerPivot(Engine engine, int row, int column) : base(engine, row, column, false, true, 0, 0,
            "unit_worker.png", RenderTypePivot.Image, LifePoints, 1)
        {

        }

        internal override void Release()
        {
            RemainingMoves = _engine.WorkerIsBusy(this) ? 0 : Speed;
        }
    }
}
