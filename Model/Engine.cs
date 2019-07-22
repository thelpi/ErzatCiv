using System;
using System.Collections.Generic;
using System.Linq;

namespace ErsatzCiv.Model
{
    public class Engine
    {
        private List<UnitPivot> _units = new List<UnitPivot>();
        private List<CityPivot> _cities = new List<CityPivot>();

        public int CurrentTurn { get; private set; }
        public MapData Map { get; private set; }
        public IReadOnlyCollection<UnitPivot> Units { get { return _units; } }
        public IReadOnlyCollection<CityPivot> Cities { get { return _cities; } }

        public Engine(MapData.MapSizeEnum mapSize, int continentsCount, int landRatio1To10)
        {
            Map = new MapData(mapSize, continentsCount, landRatio1To10);

            int x = 0;
            int y = 0;
            do
            {
                x = Tools.Randomizer.Next(0, Map.Width);
                y = Tools.Randomizer.Next(0, Map.Height);
            }
            while (!Map.MapSquareList.Any(ms => ms.Row == x && ms.Column == y && !ms.MapSquareType.IsSeaType));

            _units.Add(new SettlerPivot(x, y));
            _units.Add(new WorkerPivot(x, y));

            CurrentTurn = 1;
        }

        public CityPivot BuildCity(SettlerPivot settler)
        {
            if (settler == null)
            {
                throw new ArgumentNullException(nameof(settler));
            }
            if (!_units.Contains(settler))
            {
                return null;
            }

            var sq = Map.MapSquareList.FirstOrDefault(ms => ms.Row == settler.Row && ms.Column == settler.Column);
            if (sq?.MapSquareType?.IsCityBuildable != true)
            {
                return null;
            }

            _cities.Add(new CityPivot(sq.Row, sq.Column));

            _units.Remove(settler);
            return _cities.Last();
        }

        public bool NewTurn()
        {
            foreach (var ms in Map.MapSquareList)
            {
                ms.UpdateActionsProgress();
            }
            foreach (var u in _units)
            {
                u.Release();
            }
            CurrentTurn++;
            return true;
        }

        public void PostTurnClean()
        {
            foreach (var ms in Map.MapSquareList.Where(s => s.Redraw))
            {
                ms.ResetRedraw();
            }
        }

        public bool IsCity(int row, int column)
        {
            return _cities.Any(c => c.Row == row && c.Column == column);
        }
    }
}
