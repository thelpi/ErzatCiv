namespace ErsatzCiv.Model
{
    public class WorkerPivot : UnitPivot
    {
        public const int LifePoints = 1;

        public WorkerPivot(int row, int column) : base(row, column, false, true, 0, 0,
            "unit_worker.png", RenderTypeEnum.Image, LifePoints)
        {

        }
    }
}
