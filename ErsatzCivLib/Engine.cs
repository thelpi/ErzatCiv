using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using ErsatzCivLib.Model;
using ErsatzCivLib.Model.Persistent;
using ErsatzCivLib.Model.Units;

namespace ErsatzCivLib
{
    [Serializable]
    public class Engine
    {
        private List<UnitPivot> _units = new List<UnitPivot>();
        private List<CityPivot> _cities = new List<CityPivot>();

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

        public CityPivot BuildCity(string name, out bool notUniqueNameError)
        {
            notUniqueNameError = false;

            if (!CanBuildCity() || string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            if (_cities.Any(c => c.Name.Equals(name.ToLower(), StringComparison.InvariantCultureIgnoreCase)))
            {
                notUniqueNameError = true;
                return null;
            }

            var settler = CurrentUnit as SettlerPivot;
            var sq = Map[CurrentUnit.Row, CurrentUnit.Column];

            var citySquares = new List<MapSquarePivot>();
            for (var i = settler.Row - 2; i <= settler.Row + 2; i++)
            {
                for (var j = settler.Column - 2; j <= settler.Column + 2; j++)
                {
                    var mapSquare = Map[i, j];
                    if (mapSquare != null
                        && (i != settler.Row || j != settler.Column)
                        && !OccupiedByCity(mapSquare)
                        && !(Math.Abs(i) == 2 && Math.Abs(j) == 2))
                    {
                        citySquares.Add(mapSquare);
                    }
                }
            }

            var city = new CityPivot(name, settler, citySquares);
            sq.ApplyCityActions(city);

            _cities.Add(city);
            _units.Remove(settler);
            SetUnitIndex(true, false);

            return city;
        }

        public bool CanBuildCity()
        {
            if (CurrentUnit?.GetType() != typeof(SettlerPivot))
            {
                return false;
            }

            var sq = Map[CurrentUnit.Row, CurrentUnit.Column];
            if (sq?.Biome?.IsCityBuildable != true
                || _cities.Any(c => CurrentUnit.Row == c.Row && CurrentUnit.Column == c.Column)
                || sq?.Pollution == true)
            {
                return false;
            }

            return true;
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
                    Map[i, j].UpdateActionsProgress();
                }
            }
            foreach (var u in _units)
            {
                u.Release();
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
            if (sq == null || IsCity(worker.Row, worker.Column))
            {
                return false;
            }

            if (actionPivot == WorkerActionPivot.RailRoad && !sq.Road)
            {
                actionPivot = WorkerActionPivot.Road;
            }

            var result = sq.ApplyAction(worker, actionPivot);
            if (result)
            {
                worker.ForceNoMove();
                ToNextUnit();
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

        public bool MoveCurrentUnit(DirectionPivot? direction)
        {
            if (CurrentUnit == null)
            {
                return false;
            }

            if (direction == null)
            {
                CurrentUnit.ForceNoMove();
                ToNextUnit();
                return true;
            }

            var x = direction.Value.Row(CurrentUnit.Row);
            var y = direction.Value.Column(CurrentUnit.Column);
            var square = Map[x, y];
            var prevSq = Map[CurrentUnit.Row, CurrentUnit.Column];
            bool isCity = IsCity(x, y);

            var res = CurrentUnit.Move(direction.Value, x, y, isCity, prevSq, square);
            if (res && CurrentUnit.RemainingMoves == 0)
            {
                ToNextUnit();
            }
            return res;
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

        internal bool OccupiedByCity(MapSquarePivot mapSquare)
        {
            return _cities.Any(c => c.Citizens.Any(cc => cc.MapSquare == mapSquare));
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
