﻿using System;

namespace ErsatzCivLib.Model.Units
{
    [Serializable]
    public class WorkerPivot : UnitPivot
    {
        public const int LifePoints = 1;

        internal WorkerPivot(int row, int column) : base(row, column, false, true, 0, 0,
            "unit_worker.png", RenderTypePivot.Image, LifePoints, 1)
        {

        }

        internal override void Release(Engine engine)
        {
            RemainingMoves = engine.WorkerIsBusy(this) ? 0 : Speed;
        }
    }
}
