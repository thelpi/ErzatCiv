﻿using System;
using ErsatzCivLib.Model.Persistent;

namespace ErsatzCivLib.Model.Units
{
    [Serializable]
    public class SettlerPivot : UnitPivot
    {
        private const string RENDER_IMAGE_FILENAME = "unit_settler.png";
        private const int SPEED = 1;
        private const int PRODUCTIVITY_COST = 20;
        private const int LIFE_POINTS = 1;

        internal SettlerPivot(MapSquarePivot location) :
            base(location, false, true, 0, 0, RENDER_IMAGE_FILENAME, LIFE_POINTS, SPEED, PRODUCTIVITY_COST, "Settler")
        {

        }
    }
}
