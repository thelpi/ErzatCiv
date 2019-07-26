namespace ErsatzCivLib.Model
{
    public class WorkerPivot : UnitPivot
    {
        public const int LifePoints = 1;

        internal WorkerPivot(int row, int column) : base(row, column, false, true, 0, 0,
            "unit_worker.png", RenderTypeEnum.Image, LifePoints)
        {

        }

        internal override void Release()
        {
            Locked = MapSquareData.CurrentActionPivot.WorkerIsBusy(this);
        }
    }
}
