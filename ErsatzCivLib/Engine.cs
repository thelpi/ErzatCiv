using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using ErsatzCivLib.Model;
using ErsatzCivLib.Model.Events;
using ErsatzCivLib.Model.MapEnums;
using ErsatzCivLib.Model.Persistent;

namespace ErsatzCivLib
{
    [Serializable]
    public class Engine
    {
        private readonly List<PlayerPivot> _iaPlayers = new List<PlayerPivot>();

        public int CurrentTurn { get; private set; }
        // TODO : wrong version.
        public int CurrentYear { get { return (CurrentTurn * 10) - 4000; } }
        public MapPivot Map { get; private set; }
        public IReadOnlyCollection<CityPivot> GlobalCities
        {
            get
            {
                return _iaPlayers.SelectMany(iap => iap.Cities).Concat(HumanPlayer.Cities).ToList();
            }
        }
        public PlayerPivot HumanPlayer { get; }

        public IReadOnlyCollection<PlayerPivot> Players
        {
            get
            {
                return _iaPlayers.Concat(new[] { HumanPlayer }).ToList();
            }
        }

        public Engine(SizePivot mapSize, LandShapePivot mapShape, LandCoveragePivot landCoverage, TemperaturePivot temperature,
            AgePivot age, HumidityPivot humidity, CivilizationPivot playerCivilization, int iaPlayersCount)
        {
            if (playerCivilization == null)
            {
                throw new ArgumentNullException(nameof(playerCivilization));
            }
            iaPlayersCount = iaPlayersCount < 0 ? 0 : iaPlayersCount;
            if (iaPlayersCount > CivilizationPivot.Instances.Count / (6 - (int)mapSize))
            {
                throw new ArgumentException("The IA players count is too high for this map size !", nameof(iaPlayersCount));
            }

            Map = new MapPivot(mapSize, mapShape, landCoverage, temperature, age, humidity);

            List<MapSquarePivot> excludedSpots = new List<MapSquarePivot>();

            HumanPlayer = new PlayerPivot(playerCivilization, false, GetRandomLocation(excludedSpots));
            for (int i = 0; i < iaPlayersCount; i++)
            {
                CivilizationPivot iaCiv = null;
                do
                {
                    iaCiv = CivilizationPivot.Instances.ElementAt(Tools.Randomizer.Next(0, CivilizationPivot.Instances.Count));
                }
                while (HumanPlayer.Civilization == iaCiv || _iaPlayers.Any(ia => ia.Civilization == iaCiv));
                _iaPlayers.Add(new PlayerPivot(iaCiv, true, GetRandomLocation(excludedSpots)));
            }

            CurrentTurn = 1;
        }

        private MapSquarePivot GetRandomLocation(List<MapSquarePivot> excludedSpots)
        {
            MapSquarePivot ms = null;
            do
            {
                var row = Tools.Randomizer.Next(0, Map.Height);
                var column = Tools.Randomizer.Next(0, Map.Width);
                ms = Map[row, column];
            }
            while (ms == null || ms.Biome.IsSeaType || excludedSpots.Contains(ms));

            excludedSpots.Add(ms);
            return ms;
        }

        public CityPivot BuildCity(string name, out bool notUniqueNameError)
        {
            notUniqueNameError = false;
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            return HumanPlayer.BuildCity(CurrentTurn, name, out notUniqueNameError, IsCity, ComputeCityAvailableMapSquares);
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
            return HumanPlayer.CanBuildCity(IsCity);
        }

        public void ToNextUnit()
        {
            HumanPlayer.SetUnitIndex(false, false);
        }

        public TurnConsequencesPivot NewTurn()
        {
            // TODO : run IA actions !

            foreach (var ms in Map)
            {
                ms.UpdateActionsProgress();
            }

            var turnConsequences = HumanPlayer.NextTurn(Map.GetAdjacentMapSquares);
            foreach (var iaPlayer in _iaPlayers)
            {
                var turnConsequencesIa = iaPlayer.NextTurn(Map.GetAdjacentMapSquares);
                // TODO : what do with this ?
            }

            CurrentTurn++;

            return turnConsequences;
        }

        internal bool IsCity(MapSquarePivot square)
        {
            return GlobalCities.Any(c => c.MapSquareLocation == square);
        }

        public bool WorkerAction(WorkerActionPivot actionPivot)
        {
            if (actionPivot == null)
            {
                return false;
            }

            return HumanPlayer.WorkerAction(actionPivot, IsCity, Map.GetAdjacentMapSquares);
        }

        public void SubscribeToMapSquareChangeEvent(EventHandler<SquareChangedEventArgs> handler)
        {
            foreach (var ms in Map)
            {
                ms.SquareChangeEvent += handler;
            }
        }

        public bool MoveCurrentUnit(DirectionPivot? direction)
        {
            return HumanPlayer.MoveCurrentUnit(direction, delegate(int x, int y) { return Map[x, y]; });
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

        private void ChangeCitizenToSpecialist(CitizenPivot citizenSource, CitizenTypePivot citizenType)
        {
            var theCity = GetCityFromCitizen(citizenSource);
            if (theCity == null)
            {
                return;
            }

            citizenSource.ToSpecialist(citizenType);
            theCity.CheckCitizensMood();
        }

        public void ChangeCitizenToDefault(CitizenPivot citizenSource, MapSquarePivot mapSquare)
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

        private CityPivot GetCityFromCitizen(CitizenPivot citizenSource)
        {
            return GlobalCities.SingleOrDefault(c => c.Citizens.Contains(citizenSource));
        }

        private bool OccupiedByCity(MapSquarePivot mapSquare, CityPivot exceptCity = null)
        {
            return GlobalCities.Any(c => (exceptCity == null || exceptCity != c) && c.Citizens.Any(cc =>  cc.MapSquare == mapSquare));
        }

        /// <summary>
        /// Gets, for a specified city, the list of <see cref="MapSquarePivot"/> around it.
        /// </summary>
        /// <param name="city">The <see cref="CityPivot"/>.</param>
        /// <returns>A dictionary where the key is the <see cref="MapSquarePivot"/>,
        /// and the value is a tuple [<see cref="CitizenPivot"/> status, occupied by another city y/n].</returns>
        /// <exception cref="ArgumentNullException">The parameter <paramref name="city"/> is <c>Null</c>.</exception>
        public Dictionary<MapSquarePivot, Tuple<CitizenPivot, bool>> GetMapSquaresAroundCity(CityPivot city)
        {
            if (city == null)
            {
                throw new ArgumentNullException(nameof(city));
            }

            var result = new Dictionary<MapSquarePivot, Tuple<CitizenPivot, bool>>();
            for (var i = city.MapSquareLocation.Row - 3; i <= city.MapSquareLocation.Row + 3; i++)
            {
                for (var j = city.MapSquareLocation.Column - 3; j <= city.MapSquareLocation.Column + 3; j++)
                {
                    var msq = Map[i, j];
                    if (msq != null)
                    {
                        result.Add(msq, new Tuple<CitizenPivot, bool>(
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

            if (city == null || !HumanPlayer.Cities.Contains(city))
            {
                return null;
            }

            var buildableDefaultInstances = HumanPlayer.GetBuildableItemsForCity(city, out indexOfDefault, Map.GetAdjacentMapSquares);

            // TODO : remove wonders already built globally.

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

            var invokedInstance = buildableDefaultInstance.CreateOrGetInstance(city.MapSquareLocation);
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

        public void SwitchCitizenType(CitizenPivot citizenSource)
        {
            if (citizenSource == null)
            {
                return;
            }

            if (!citizenSource.Type.HasValue)
            {
                ChangeCitizenToSpecialist(citizenSource, CitizenTypePivot.Entertainer);
            }
            else
            {
                switch (citizenSource.Type.Value)
                {
                    case CitizenTypePivot.Entertainer:
                        ChangeCitizenToSpecialist(citizenSource, CitizenTypePivot.Scientist);
                        break;
                    case CitizenTypePivot.Scientist:
                        ChangeCitizenToSpecialist(citizenSource, CitizenTypePivot.TaxCollector);
                        break;
                    case CitizenTypePivot.TaxCollector:
                        ChangeCitizenToDefault(citizenSource, null);
                        break;
                }
            }
        }

        /// <summary>
        /// Calls <see cref="PlayerPivot.ChangeCurrentAdvance(AdvancePivot)"/> for <see cref="HumanPlayer"/>.
        /// </summary>
        /// <param name="player">The <see cref="PlayerPivot"/>.</param>
        /// <param name="advance">The <see cref="AdvancePivot"/>.</param>
        /// <returns><c>True</c> if success; <c>False</c> if failure.</returns>
        public bool ChangeCurrentAdvance(AdvancePivot advance)
        {
            if (advance == null)
            {
                return false;
            }

            return HumanPlayer.ChangeCurrentAdvance(advance);
        }

        public bool ChangeCurrentRegime(RegimePivot regimePivot)
        {
            if (regimePivot is null || regimePivot == RegimePivot.Anarchy || !HumanPlayer.RevolutionIsDone)
            {
                return false;
            }

            HumanPlayer.ChangeCurrentRegime(regimePivot);
            return true;
        }

        public void TriggerRevolution()
        {
            HumanPlayer.TriggerRevolution();
        }
    }
}
