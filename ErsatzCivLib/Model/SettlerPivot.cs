namespace ErsatzCivLib.Model
{
    public class SettlerPivot : UnitPivot
    {
        public const int LifePoints = 1;

        internal SettlerPivot(int row, int column) :
            base(row, column, false, true, 0, 0, "unit_settler.png", RenderTypeEnum.Image, LifePoints)
        {

        }
    }
}
