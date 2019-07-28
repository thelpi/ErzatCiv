using System;
using System.Collections.Generic;
using System.Linq;
using ErsatzCivLib.Model;
using ErsatzCivLib.Model.Units;

namespace ErsatzCivLib
{
    public class Engine
    {
        private List<UnitPivot> _units = new List<UnitPivot>();
        private List<CityPivot> _cities = new List<CityPivot>();

        public event EventHandler NextUnitEvent;

        private int _currentUnitIndex;
        private int _previousUnitIndex;

        public int CurrentTurn { get; private set; }
        public MapPivot Map { get; private set; }
        public IReadOnlyCollection<UnitPivot> Units { get { return _units; } }
        public IReadOnlyCollection<CityPivot> Cities { get { return _cities; } }
        public UnitPivot CurrentUnit { get { return _currentUnitIndex >= 0 ? _units[_currentUnitIndex] : null; } }
        public UnitPivot PreviousUnit { get { return _previousUnitIndex >= 0 && _previousUnitIndex != _currentUnitIndex ? _units[_previousUnitIndex] : null; } }

        public Engine(MapPivot.MapSizeEnum mapSize,
            MapPivot.MapLandShape mapShape,
            MapPivot.MapLandCoverage landCoverage,
            TemperaturePivot temperature)
        {
            Map = new MapPivot(mapSize, mapShape, landCoverage, temperature);

            int x = 0;
            int y = 0;
            do
            {
                x = Tools.Randomizer.Next(0, Map.Width);
                y = Tools.Randomizer.Next(0, Map.Height);
            }
            while (!Map.MapSquareList.Any(ms => ms.Same(x, y) && !ms.Biome.IsSeaType));

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

            var sq = Map.MapSquareList.FirstOrDefault(ms => ms.Same(settler));
            if (sq?.Biome?.IsCityBuildable != true
                || _cities.Any(c => c.Same(settler))
                || sq?.Pollution == true)
            {
                return null;
            }
            var city = new CityPivot(sq);
            sq.ApplyCityActions(city);

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
                _currentUnitIndex = _units.IndexOf(_units.FirstOrDefault(u => u.RemainingMoves > 0));
                _previousUnitIndex = -1;
                NextUnitEvent?.Invoke(this, new EventArgs());
                return;
            }

            _previousUnitIndex = _currentUnitIndex;

            for (int i = _currentUnitIndex + 1; i < _units.Count; i++)
            {
                if (_units[i].RemainingMoves > 0)
                {
                    _currentUnitIndex = i;
                    NextUnitEvent?.Invoke(this, new EventArgs());
                    return;
                }
            }
            for (int i = 0; i < _currentUnitIndex + (currentJustBeenRemoved ? 1 : 0); i++)
            {
                if (_units[i].RemainingMoves > 0)
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

        internal bool IsCity(IOnMapPivot onMapInstance)
        {
            return _cities.Any(c => c.Same(onMapInstance));
        }

        internal bool IsCity(int row, int column)
        {
            return _cities.Any(c => c.Same(row, column));
        }

        public bool WorkerAction(WorkerActionPivot actionPivot)
        {
            if (CurrentUnit?.GetType() != typeof(WorkerPivot) || actionPivot == null)
            {
                return false;
            }

            var worker = CurrentUnit as WorkerPivot;
            var sq = Map.MapSquareList.FirstOrDefault(ms => ms.Same(worker));
            if (sq == null)
            {
                return false;
            }

            if (actionPivot == WorkerActionPivot.RailRoad && !sq.Road)
            {
                actionPivot = WorkerActionPivot.Road;
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

        public bool MoveCurrentUnit(DirectionPivot direction)
        {
            return CurrentUnit.Move(this, direction);
        }
    }
}
