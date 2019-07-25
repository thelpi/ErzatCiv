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

        private int _currentUnitIndex;
        private int _previousUnitIndex;

        public int CurrentTurn { get; private set; }
        public MapData Map { get; private set; }
        public IReadOnlyCollection<UnitPivot> Units { get { return _units; } }
        public IReadOnlyCollection<CityPivot> Cities { get { return _cities; } }
        public UnitPivot CurrentUnit { get { return _currentUnitIndex >= 0 ? _units[_currentUnitIndex] : null; } }
        public UnitPivot PreviousUnit { get { return _previousUnitIndex >= 0 && _previousUnitIndex != _currentUnitIndex ? _units[_previousUnitIndex] : null; } }

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

            SetUnitIndex(false, true);

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
            if (sq?.MapSquareType?.IsCityBuildable != true
                && !_cities.Any(c => c.Row == settler.Row && c.Column == settler.Column))
            {
                return null;
            }
            var city = new CityPivot(sq.Row, sq.Column);

            _cities.Add(city);
            _units.Remove(settler);
            SetUnitIndex(true, false);

            return city;
        }

        public void ToNextUnit()
        {
            SetUnitIndex(false, false);
        }

        private void SetUnitIndex(bool currentJustBeenRemoved, bool reset)
        {
            if (reset)
            {
                _currentUnitIndex = _units.Count > 0 ? 0 : -1;
                _previousUnitIndex = -1;
                NextUnitEvent?.Invoke(this, new EventArgs());
                return;
            }

            _previousUnitIndex = _currentUnitIndex;

            for (int i = _currentUnitIndex + 1; i < _units.Count; i++)
            {
                if (!_units[i].Locked)
                {
                    _currentUnitIndex = i;
                    NextUnitEvent?.Invoke(this, new EventArgs());
                    return;
                }
            }
            for (int i = 0; i < _currentUnitIndex + (currentJustBeenRemoved ? 1 : 0); i++)
            {
                if (!_units[i].Locked)
                {
                    _currentUnitIndex = i;
                    NextUnitEvent?.Invoke(this, new EventArgs());
                    return;
                }
            }
            _currentUnitIndex = -1;
            NextUnitEvent?.Invoke(this, new EventArgs());
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
            SetUnitIndex(false, true);
            return true;
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

        public void SubscribeToMapSquareChangeEvent(EventHandler handler)
        {
            foreach (var ms in Map.MapSquareList)
            {
                ms.SquareChangeEvent += handler;
            }
        }

        public bool MoveCurrentUnit(DirectionEnumPivot direction)
        {
            return CurrentUnit.Move(this, direction);
        }
    }
}
