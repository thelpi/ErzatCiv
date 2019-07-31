using System;
using ErsatzCivLib.Model.Persistent;

namespace ErsatzCivLib.Model.Units
{
    [Serializable]
    public class SettlerPivot : UnitPivot
    {
        public const int LifePoints = 1;

        internal SettlerPivot(MapSquarePivot location) :
            base(location, false, true, 0, 0, "unit_settler.png", RenderTypePivot.Image, LifePoints, 1)
        {

        }
    }
}
