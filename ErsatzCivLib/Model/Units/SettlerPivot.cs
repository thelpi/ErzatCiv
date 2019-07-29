﻿namespace ErsatzCivLib.Model.Units
{
    public class SettlerPivot : UnitPivot
    {
        public const int LifePoints = 1;

        internal SettlerPivot(int row, int column) :
            base(row, column, false, true, 0, 0, "unit_settler.png", RenderTypePivot.Image, LifePoints, 1)
        {

        }
    }
}