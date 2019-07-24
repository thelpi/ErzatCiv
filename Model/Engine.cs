using System;
using System.Collections.Generic;
using System.Linq;

namespace ErsatzCiv.Model
{
    public class Engine
    {
        private List<UnitPivot> _units = new List<UnitPivot>();
        private List<CityPivot> _cities = new List<CityPivot>();

        public event EventHandler NextUnitEvent;

        private int _currentUnitIndex_DO_NOT_USE;
        private int CurrentUnitIndex
        {
            get
            {
                return _currentUnitIndex_DO_NOT_USE;
            }
            set
            {
                _currentUnitIndex_DO_NOT_USE = value;
                NextUnitEvent?.Invoke(this, new EventArgs());
            }
        }

        public int CurrentTurn { get; private set; }
        public MapData Map { get; private set; }
        public IReadOnlyCollection<UnitPivot> Units { get { return _units; } }
        public IReadOnlyCollection<CityPivot> Cities { get { return _cities; } }
        public UnitPivot CurrentUnit { get { return CurrentUnitIndex >= 0 ? _units[CurrentUnitIndex] : null; } }

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

            CurrentUnitIndex = 0;

            CurrentTurn = 1;
        }

        public CityPivot BuildCity()
        {
            if (CurrentUnit?.GetType() != typeof(SettlerPivot))
            {
                return null;
            }

            var settler = CurrentUnit as SettlerPivot;

            var sq = Map.MapSquareList.FirstOrDefault(ms => ms.Row == settler.Row && ms.Column == settler.Column);
            if (sq?.MapSquareType?.IsCityBuildable != true)
            {
                return null;
            }
            var city = new CityPivot(sq.Row, sq.Column);

            _cities.Add(city);
            _units.Remove(settler);
            SetUnitIndex(true);

            return city;
        }

        public void SetUnitIndex(bool currentJustBeenRemoved)
        {
            for (int i = CurrentUnitIndex + 1; i < _units.Count; i++)
            {
                if (!_units[i].Locked)
                {
                    CurrentUnitIndex = i;
                    return;
                }
            }
            for (int i = 0; i < CurrentUnitIndex + (currentJustBeenRemoved ? 1 : 0); i++)
            {
                if (!_units[i].Locked)
                {
                    CurrentUnitIndex = i;
                    return;
                }
            }
            CurrentUnitIndex = -1;
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
            CurrentUnitIndex = 0;
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

        public bool WorkerAction(MapSquareActionPivot actionPivot)
        {
            if (CurrentUnit?.GetType() != typeof(WorkerPivot) || actionPivot == null)
            {
                return false;
            }

            var worker = CurrentUnit as WorkerPivot;
            var sq = Map.MapSquareList.FirstOrDefault(ms => ms.Row == worker.Row && ms.Column == worker.Column);
            if (sq == null)
            {
                return false;
            }

            var result = sq.ApplyAction(this, worker, actionPivot);
            if (result)
            {
                worker.Move(this, null);
            }

            return result;
        }
    }
}
