using System.Collections.Generic;
using System.Linq;

namespace ErsatzCiv.Model
{
    public class Engine
    {
        private List<UnitPivot> _units = new List<UnitPivot>();

        public int CurrentTurn { get; private set; }
        public MapData Map { get; private set; }
        public IReadOnlyCollection<UnitPivot> Units { get { return _units; } }

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
    }
}
