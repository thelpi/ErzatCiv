namespace ErsatzCivLib.Model
{
    public class WorkerPivot : UnitPivot
    {
        public const int LifePoints = 1;

        internal WorkerPivot(int row, int column) : base(row, column, false, true, 0, 0,
            "unit_worker.png", RenderTypeEnum.Image, LifePoints, 1)
        {

        }

        internal override void Release()
        {
            RemainingMoves = MapSquareData.CurrentActionPivot.WorkerIsBusy(this) ? 0 : Speed;
        }
    }
}
