using System;

namespace ErsatzCivLib.Model.Units
{
    [Serializable]
    public class SettlerPivot : UnitPivot
    {
        public const int LifePoints = 1;

        internal SettlerPivot(Engine engine, int row, int column) :
            base(engine, row, column, false, true, 0, 0, "unit_settler.png", RenderTypePivot.Image, LifePoints, 1)
        {

        }
    }
}
