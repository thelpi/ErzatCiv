using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using ErsatzCivLib.Model;
using ErsatzCivLib.Model.CityImprovements;
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
        public UnitPivot CurrentUnit
        {
            get
            {
                if (_currentUnitIndex == -1)
                {
                    return null;
                }

                if (_currentUnitIndex < -1 || _currentUnitIndex >= _units.Count)
                {
                    // Anormal behavior.
                    return null;
                }

                return _units[_currentUnitIndex];
            }
        }
        public UnitPivot PreviousUnit
        {
            get
            {
                if (_previousUnitIndex == -1 || _previousUnitIndex == _currentUnitIndex)
                {
                    return null;
                }

                if (_previousUnitIndex < -1 || _previousUnitIndex >= _units.Count)
                {
                    // Anormal behavior.
                    return null;
                }

                return _units[_previousUnitIndex];
            }
        }

        public Engine(MapPivot.SizePivot mapSize,
            MapPivot.LandShapePivot mapShape,
            MapPivot.LandCoveragePivot landCoverage,
            MapPivot.TemperaturePivot temperature,
            MapPivot.AgePivot age,
            MapPivot.HumidityPivot humidity)
        {
            Map = new MapPivot(mapSize, mapShape, landCoverage, temperature, age, humidity);

            MapSquarePivot ms = null;
            do
            {
                var row = Tools.Randomizer.Next(0, Map.Height);
                var column = Tools.Randomizer.Next(0, Map.Width);
                ms = Map[row, column];
            }
            while (ms == null || ms.Biome.IsSeaType);

            _units.Add(SettlerPivot.CreateAtLocation(ms));
            _units.Add(WorkerPivot.CreateAtLocation(ms));

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
            var sq = CurrentUnit.MapSquareLocation;

            var city = new CityPivot(CurrentTurn, name, sq, ComputeCityAvailableMapSquares, CapitalizationPivot.CreateAtLocation(sq));
            sq.ApplyCityActions(city);

            _cities.Add(city);
            _units.Remove(settler);
            SetUnitIndex(true, false);

            return city;
        }

        private List<MapSquarePivot> ComputeCityAvailableMapSquares(CityPivot city)
        {
            var sq = city.MapSquareLocation;

            var citySquares = new List<MapSquarePivot>();
            for (var i = sq.Row - 2; i <= sq.Row + 2; i++)
            {
                for (var j = sq.Column - 2; j <= sq.Column + 2; j++)
                {
                    var mapSquare = Map[i, j];
                    if (mapSquare != null
                        && mapSquare != sq
                        && !IsCity(mapSquare)
                        && !OccupiedByCity(mapSquare)
                        && !(i == sq.Row - 2 && (j == sq.Column - 2 || j == sq.Column + 2))
                        && !(i == sq.Row + 2 && (j == sq.Column - 2 || j == sq.Column + 2)))
                    {
                        citySquares.Add(mapSquare);
                    }
                }
            }

            return citySquares;
        }

        public bool CanBuildCity()
        {
            if (CurrentUnit?.Is<SettlerPivot>() != true)
            {
                return false;
            }

            var sq = CurrentUnit.MapSquareLocation;

            return sq?.Biome?.IsCityBuildable == true
                && !IsCity(sq)
                && sq.Pollution != true;
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
                    if (_units.Count > i && _units[i].RemainingMoves > 0)
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

        public IDictionary<CityPivot, BuildablePivot> NewTurn()
        {
            var citiesWithDoneProduction = new Dictionary<CityPivot, BuildablePivot>();
            foreach (var city in _cities)
            {
                var produced = city.UpdateStatus();
                if (produced != null)
                {
                    if (produced.Is<UnitPivot>())
                    {
                        _units.Add(produced as UnitPivot);
                        SetUnitIndex(false, false);
                    }
                    else if (!produced.Is<CapitalizationPivot>())
                    {
                        citiesWithDoneProduction.Add(city, produced);
                    }
                }
            }
            foreach (var ms in Map)
            {
                ms.UpdateActionsProgress();
            }
            foreach (var u in _units)
            {
                u.Release();
            }
            CurrentTurn++;
            SetUnitIndex(false, true);

            return citiesWithDoneProduction;
        }

        internal bool IsCity(MapSquarePivot square)
        {
            return _cities.Any(c => c.MapSquareLocation == square);
        }

        public bool WorkerAction(WorkerActionPivot actionPivot)
        {
            if (CurrentUnit == null || !CurrentUnit.Is<WorkerPivot>() || actionPivot == null)
            {
                return false;
            }

            var worker = CurrentUnit as WorkerPivot;
            var sq = worker.MapSquareLocation;
            if (sq == null || IsCity(sq))
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
            foreach (var ms in Map)
            {
                ms.SquareChangeEvent += handler;
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

            var prevSq = CurrentUnit.MapSquareLocation;

            var x = direction.Value.Row(prevSq.Row);
            var y = direction.Value.Column(prevSq.Column);

            var square = Map[x, y];
            if (square == null)
            {
                return false;
            }

            bool isCity = IsCity(square);

            var res = CurrentUnit.Move(direction.Value, isCity, prevSq, square);
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

        private void ChangeCitizenToSpecialist(CityPivot.CitizenPivot citizenSource, CityPivot.CitizenTypePivot citizenType)
        {
            var theCity = GetCityFromCitizen(citizenSource);
            if (theCity == null)
            {
                return;
            }

            citizenSource.ToSpecialist(citizenType);
            theCity.CheckCitizensMood();
        }

        public void ChangeCitizenToDefault(CityPivot.CitizenPivot citizenSource, MapSquarePivot mapSquare)
        {
            if (citizenSource == null)
            {
                return;
            }

            var theCity = GetCityFromCitizen(citizenSource);
            if (theCity == null)
            {
                return;
            }

            if (mapSquare == null)
            {
                mapSquare = theCity.BestVacantSpot();
            }
            else if (!ComputeCityAvailableMapSquares(theCity).Contains(mapSquare))
            {
                return;
            }

            if (mapSquare != null)
            {
                citizenSource.ToCitizen(mapSquare);
                theCity.CheckCitizensMood();
            }
        }

        private CityPivot GetCityFromCitizen(CityPivot.CitizenPivot citizenSource)
        {
            return _cities.SingleOrDefault(c => c.Citizens.Contains(citizenSource));
        }

        internal bool OccupiedByCity(MapSquarePivot mapSquare, CityPivot exceptCity = null)
        {
            return _cities.Any(c => (exceptCity == null || exceptCity != c) && c.Citizens.Any(cc =>  cc.MapSquare == mapSquare));
        }

        /// <summary>
        /// Gets, for a specified city, the list of <see cref="MapSquarePivot"/> around it.
        /// </summary>
        /// <param name="city">The <see cref="CityPivot"/>.</param>
        /// <returns>A dictionary where the key is the <see cref="MapSquarePivot"/>,
        /// and the value is a tuple [<see cref="CityPivot.CitizenPivot"/> status, occupied by another city y/n].</returns>
        /// <exception cref="ArgumentNullException">The parameter <paramref name="city"/> is <c>Null</c>.</exception>
        public Dictionary<MapSquarePivot, Tuple<CityPivot.CitizenPivot, bool>> GetMapSquaresAroundCity(CityPivot city)
        {
            if (city == null)
            {
                throw new ArgumentNullException(nameof(city));
            }

            var result = new Dictionary<MapSquarePivot, Tuple<CityPivot.CitizenPivot, bool>>();
            for (var i = city.MapSquareLocation.Row - 3; i <= city.MapSquareLocation.Row + 3; i++)
            {
                for (var j = city.MapSquareLocation.Column - 3; j <= city.MapSquareLocation.Column + 3; j++)
                {
                    var msq = Map[i, j];
                    if (msq != null)
                    {
                        result.Add(msq, new Tuple<CityPivot.CitizenPivot, bool>(
                            city.Citizens.SingleOrDefault(c => c.MapSquare == msq), OccupiedByCity(msq, city)));
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets, for a specified <see cref="CityPivot"/>, the list of <see cref="BuildablePivot"/> which can be built.
        /// </summary>
        /// <param name="city">The <see cref="CityPivot"/>.</param>
        /// <param name="indexOfDefault">Out; the index, in the result list, of the city current production.</param>
        /// <returns>List of <see cref="BuildablePivot"/> (<c>Default</c> instance for each) the city can build.</returns>
        public IReadOnlyCollection<BuildablePivot> GetBuildableItemsForCity(CityPivot city, out int indexOfDefault)
        {
            indexOfDefault = -1;

            if (city == null)
            {
                return null;
            }

            var buildableDefaultInstances = BuildablePivot.GetEveryDefaultInstances().ToList();
            if (buildableDefaultInstances == null)
            {
                return null;
            }

            // Already built for the current city.
            buildableDefaultInstances.RemoveAll(b => city.Improvements.Contains(b));

            // No aqueduc required if a river is close to the city.
            if (city.MapSquareLocation.HasRiver)
            {
                buildableDefaultInstances.RemoveAll(b => AqueducPivot.Default == b);
            }

            // TODO : remove wonders already built globally.

            indexOfDefault = buildableDefaultInstances.FindIndex(b => b.GetType() == city.Production.GetType());
            return buildableDefaultInstances;
        }

        /// <summary>
        /// Tries to change the production of a <see cref="CityPivot"/>.
        /// </summary>
        /// <param name="city">The <see cref="CityPivot"/>.</param>
        /// <param name="buildableDefaultInstance">The <see cref="BuildablePivot"/> (<c>Default</c> instance).</param>
        /// <returns><c>True</c> if success; <c>False</c> otherwise.</returns>
        public bool ChangeCityProduction(CityPivot city, BuildablePivot buildableDefaultInstance)
        {
            if (city == null || buildableDefaultInstance == null)
            {
                return false;
            }

            var invokedInstance = buildableDefaultInstance.CreateInstance(city.MapSquareLocation);
            if (invokedInstance == null)
            {
                return false;
            }

            city.ChangeProduction(invokedInstance);
            return true;
        }

        /// <summary>
        /// Resets the position on the working area of each citizen of a <see cref="CityPivot"/>.
        /// </summary>
        /// <param name="city">The city to reset.</param>
        public void ResetCitizens(CityPivot city)
        {
            if (city == null)
            {
                throw new ArgumentNullException(nameof(city));
            }

            city.ResetCitizens();
        }

        public void SwitchCitizenType(CityPivot.CitizenPivot citizenSource)
        {
            if (citizenSource == null)
            {
                return;
            }

            if (!citizenSource.Type.HasValue)
            {
                ChangeCitizenToSpecialist(citizenSource, CityPivot.CitizenTypePivot.Entertainer);
            }
            else
            {
                switch (citizenSource.Type.Value)
                {
                    case CityPivot.CitizenTypePivot.Entertainer:
                        ChangeCitizenToSpecialist(citizenSource, CityPivot.CitizenTypePivot.Scientist);
                        break;
                    case CityPivot.CitizenTypePivot.Scientist:
                        ChangeCitizenToSpecialist(citizenSource, CityPivot.CitizenTypePivot.TaxCollector);
                        break;
                    case CityPivot.CitizenTypePivot.TaxCollector:
                        ChangeCitizenToDefault(citizenSource, null);
                        break;
                }
            }
        }

        public bool CityOnLeftBorder(CityPivot city)
        {
            return city?.MapSquareLocation.Row == 0;
        }

        public bool CityOnRightBorder(CityPivot city)
        {
            return city?.MapSquareLocation.Row == Map.Width - 1;
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
