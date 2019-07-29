using System;
using ErsatzCivLib.Model.Persistent;

namespace ErsatzCivLib.Model.Units
{
    [Serializable]
    public class SettlerPivot : UnitPivot
    {
        public const int LifePoints = 1;

        internal SettlerPivot(Engine owner, int row, int column) :
            base(owner, row, column, false, true, 0, 0, "unit_settler.png", RenderTypePivot.Image, LifePoints, 1)
        {

        }
    }
}
