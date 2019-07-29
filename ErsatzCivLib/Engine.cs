﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using ErsatzCivLib.Model;
using ErsatzCivLib.Model.Units;

namespace ErsatzCivLib
{
    [Serializable]
    public class Engine
    {
        private List<UnitPivot> _units = new List<UnitPivot>();
        private List<CityPivot> _cities = new List<CityPivot>();
        private List<InProgressWorkerActionPivot> _globalActions = new List<InProgressWorkerActionPivot>();

        [field: NonSerialized]
        public event EventHandler<NextUnitEventArgs> NextUnitEvent;

        private int _currentUnitIndex;
        private int _previousUnitIndex;

        public int CurrentTurn { get; private set; }
        public MapPivot Map { get; private set; }
        public IReadOnlyCollection<UnitPivot> Units { get { return _units; } }
        public IReadOnlyCollection<CityPivot> Cities { get { return _cities; } }
        public UnitPivot CurrentUnit { get { return _currentUnitIndex >= 0 ? _units[_currentUnitIndex] : null; } }
        public UnitPivot PreviousUnit { get { return _previousUnitIndex >= 0 && _previousUnitIndex != _currentUnitIndex ? _units[_previousUnitIndex] : null; } }

        public Engine(MapPivot.SizePivot mapSize,
            MapPivot.LandShapePivot mapShape,
            MapPivot.LandCoveragePivot landCoverage,
            MapPivot.TemperaturePivot temperature)
        {
            Map = new MapPivot(mapSize, mapShape, landCoverage, temperature);

            int x = 0;
            int y = 0;
            do
            {
                x = Tools.Randomizer.Next(0, Map.Width);
                y = Tools.Randomizer.Next(0, Map.Height);
            }
            while (Map[x, y] == null || Map[x, y].Biome.IsSeaType);

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

            var sq = Map[settler.Row, settler.Column];
            if (sq?.Biome?.IsCityBuildable != true
                || _cities.Any(c => settler.Row == c.Row && settler.Column == c.Column)
                || sq?.Pollution == true)
            {
                return null;
            }
            var city = new CityPivot(this, settler.Row, settler.Column);
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
            }
            else
            {
                _previousUnitIndex = _currentUnitIndex;

                for (int i = _currentUnitIndex + 1; i < _units.Count; i++)
                {
                    if (_units[i].RemainingMoves > 0)
                    {
                        _currentUnitIndex = i;
                        NextUnitEvent?.Invoke(this, new NextUnitEventArgs(true));
                        return;
                    }
                }
                for (int i = 0; i < _currentUnitIndex + (currentJustBeenRemoved ? 1 : 0); i++)
                {
                    if (_units[i].RemainingMoves > 0)
                    {
                        _currentUnitIndex = i;
                        NextUnitEvent?.Invoke(this, new NextUnitEventArgs(true));
                        return;
                    }
                }
                _currentUnitIndex = -1;
            }
            NextUnitEvent?.Invoke(this, new NextUnitEventArgs(_currentUnitIndex >= 0));
        }

        public bool NewTurn()
        {
            for (int i = 0; i < Map.Height; i++)
            {
                for (var j = 0; j < Map.Width; j++)
                {
                    Map[i, j].UpdateActionsProgress(this, i, j);
                }
            }
            foreach (var u in _units)
            {
                u.Release(this);
            }
            CurrentTurn++;
            SetUnitIndex(false, true);
            return true;
        }

        internal bool IsCity(int row, int column)
        {
            return _cities.Any(c => c.Row == row && c.Column == column);
        }

        public bool WorkerAction(WorkerActionPivot actionPivot)
        {
            if (CurrentUnit?.GetType() != typeof(WorkerPivot) || actionPivot == null)
            {
                return false;
            }

            var worker = CurrentUnit as WorkerPivot;
            var sq = Map[worker.Row, worker.Column];
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

        public void SubscribeToMapSquareChangeEvent(EventHandler<MapSquarePivot.SquareChangedEventArgs> handler)
        {
            for (int i = 0; i < Map.Height; i++)
            {
                for (var j = 0; j < Map.Width; j++)
                {
                    Map[i, j].SquareChangeEvent += handler;
                }
            }
        }

        public bool MoveCurrentUnit(DirectionPivot direction)
        {
            return CurrentUnit.Move(this, direction);
        }

        /// <summary>
        /// Deserializes a save file into an <see cref="Engine"/>.
        /// </summary>
        /// <param name="saveFullPath">Path to save do deserialize.</param>
        /// <returns>Engine and error message.</returns>
        public static Tuple<Engine, string> DeserializeSave(string saveFullPath)
        {
            try
            {
                string settings = null;
                using (StreamReader sr = new StreamReader(saveFullPath))
                {
                    settings = sr.ReadToEnd();
                }

                byte[] b = Convert.FromBase64String(settings);
                using (var stream = new MemoryStream(b))
                {
                    var formatter = new BinaryFormatter();
                    stream.Seek(0, SeekOrigin.Begin);
                    var engine = (Engine)formatter.Deserialize(stream);
                    return new Tuple<Engine, string>(engine, engine == null ? "Failure to deserialize the save !" : null);
                }
            }
            catch (Exception ex)
            {
                return new Tuple<Engine, string>(null, ex.Message);
            }
        }

        /// <summary>
        /// Serialize the instance into a file.
        /// </summary>
        /// <param name="folder">Folder.</param>
        /// <returns>Error message.</returns>
        public string SerializeToFile(string folder)
        {
            try
            {
                string fileContent = null;

                using (var stream = new MemoryStream())
                {
                    var formatter = new BinaryFormatter();
                    formatter.Serialize(stream, this);
                    stream.Flush();
                    stream.Position = 0;
                    fileContent = Convert.ToBase64String(stream.ToArray());
                }

                string fileName = "SAVE_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".data";

                using (StreamWriter sw = new StreamWriter(folder + fileName, false))
                {
                    sw.Write(fileContent);
                }

                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        internal bool OccupiedByAnotherCity(CityPivot currentCity, int row, int column)
        {
            return !_cities.Any(c => c != currentCity && c.Citizens.Any(cc => cc.Row == row && cc.Column == column));
        }

        internal bool WorkerIsBusy(WorkerPivot worker)
        {
            return _globalActions.Any(a => a.ContainsWorker(worker));
        }

        internal void AddWorkerAction(InProgressWorkerActionPivot action)
        {
            _globalActions.Add(action);
        }

        internal void RemoveWorkerAction(InProgressWorkerActionPivot action)
        {
            _globalActions.Remove(action);
        }

        public class NextUnitEventArgs : EventArgs
        {
            public bool MoreUnit { get; private set; }

            internal NextUnitEventArgs(bool moreUnit)
            {
                MoreUnit = moreUnit;
            }
        }
    }
}
