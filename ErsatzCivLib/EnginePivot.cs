using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using ErsatzCivLib.Model;
using ErsatzCivLib.Model.Enums;
using ErsatzCivLib.Model.Static;

namespace ErsatzCivLib
{
    /// <summary>
    /// Game engine; manage actions for every <see cref="PlayerPivot"/>.
    /// </summary>
    [Serializable]
    public class EnginePivot
    {
        #region Embedded properties

        private readonly List<PlayerPivot> _iaPlayers = new List<PlayerPivot>();

        /// <summary>
        /// Current turn.
        /// </summary>
        public int CurrentTurn { get; private set; }
        /// <summary>
        /// The <see cref="MapPivot"/>.
        /// </summary>
        public MapPivot Map { get; private set; }
        /// <summary>
        /// The human player.
        /// </summary>
        public PlayerPivot HumanPlayer { get; }

        #endregion

        #region Inferred properties

        /// <summary>
        /// List of every players (human and IA).
        /// </summary>
        public IReadOnlyCollection<PlayerPivot> Players
        {
            get
            {
                return _iaPlayers.Concat(new[] { HumanPlayer }).ToList();
            }
        }
        /// <summary>
        /// Inferred; current year based on <see cref="CurrentTurn"/>.
        /// </summary>
        public int CurrentYear { get { return (CurrentTurn * 10) - 4000; } }

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="mapSize">Map size.</param>
        /// <param name="mapShape">Map shape.</param>
        /// <param name="landCoverage">Map land coverage.</param>
        /// <param name="temperature">Map temperature.</param>
        /// <param name="age">Map age.</param>
        /// <param name="humidity">Map humidity.</param>
        /// <param name="playerCivilization">Human player civilization.</param>
        /// <param name="playerGender">The <see cref="PlayerPivot.Gender"/> value.</param>
        /// <param name="iaPlayersCount">Number of IA civilizations.</param>
        /// <exception cref="ArgumentNullException"><paramref name="playerCivilization"/> is <c>Null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="iaPlayersCount"/> invalid.</exception>
        public EnginePivot(SizePivot mapSize, LandShapePivot mapShape, LandCoveragePivot landCoverage, TemperaturePivot temperature,
            AgePivot age, HumidityPivot humidity, CivilizationPivot playerCivilization, bool playerGender, int iaPlayersCount)
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

            HumanPlayer = new PlayerPivot(this, playerCivilization, false, GetRandomLocation(excludedSpots), playerGender);
            for (int i = 0; i < iaPlayersCount; i++)
            {
                CivilizationPivot iaCiv = null;
                do
                {
                    iaCiv = CivilizationPivot.Instances.ElementAt(Tools.Randomizer.Next(0, CivilizationPivot.Instances.Count));
                }
                while (HumanPlayer.Civilization == iaCiv || _iaPlayers.Any(ia => ia.Civilization == iaCiv));
                _iaPlayers.Add(new PlayerPivot(this, iaCiv, true, GetRandomLocation(excludedSpots), Tools.Randomizer.Next(0, 2) == 0));
            }

            CurrentTurn = 0;
        }

        #region Private methods

        private IReadOnlyCollection<CityPivot> GetEveryCities()
        {
            return _iaPlayers.SelectMany(iap => iap.Cities).Concat(HumanPlayer.Cities).ToList();
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

        private bool OccupiedByCity(MapSquarePivot mapSquare, CityPivot exceptCity = null)
        {
            return GetEveryCities().Any(c => (exceptCity == null || exceptCity != c) && c.AreaMapSquares.Any(ams => ams.MapSquare == mapSquare));
        }

        #endregion

        #region Internal methods

        /// <summary>
        /// Checks if a specified <see cref="MapSquarePivot"/> has a <see cref="CityPivot"/> built on it.
        /// </summary>
        /// <param name="square">The <see cref="MapSquarePivot"/>.</param>
        /// <returns><c>True</c> if city; <c>False</c> otherwise.</returns>
        internal bool IsCity(MapSquarePivot square)
        {
            return GetEveryCities().Any(c => c.MapSquareLocation == square);
        }

        /// <summary>
        /// Gets, for a <see cref="CityPivot"/>, the list of available <see cref="MapSquarePivot"/> (for regular citizens).
        /// </summary>
        /// <param name="city">The city.</param>
        /// <returns>List of <see cref="MapSquarePivot"/>.</returns>
        internal IReadOnlyCollection<MapSquarePivot> ComputeCityAvailableMapSquares(CityPivot city)
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

        /// <summary>
        /// Gets a list of every built <see cref="WonderPivot"/>.
        /// </summary>
        /// <returns>List of <see cref="WonderPivot"/>.</returns>
        internal IReadOnlyCollection<WonderPivot> GetEveryWonders()
        {
            return Players.SelectMany(p => p.Wonders).ToList();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Tries to create a city with the current unit.
        /// </summary>
        /// <param name="name">The city name.</param>
        /// <param name="notUniqueNameError">Out; indicates a failure because the name is not unique.</param>
        /// <returns>The new city; <c>Null</c> if failure.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <c>Null</c> (or empty).</exception>
        public CityPivot BuildCity(string name, out bool notUniqueNameError)
        {
            notUniqueNameError = false;
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            return HumanPlayer.BuildCity(CurrentTurn, name, out notUniqueNameError);
        }

        /// <summary>
        /// Checks if a city can be build at the location of the current unit, which must be a <see cref="Model.Units.SettlerPivot"/>.
        /// </summary>
        /// <returns><c>True</c> if a city si buildable; otherwise <c>False</c>.</returns>
        public bool CanBuildCity()
        {
            return HumanPlayer.CanBuildCity();
        }

        /// <summary>
        /// Moves the focus to the next unit.
        /// </summary>
        public void ToNextUnit()
        {
            HumanPlayer.SetUnitIndex(false, false);
        }

        /// <summary>
        /// Recomputes every mechanics for the current turn then pass to the next.
        /// </summary>
        /// <returns>An instance of <see cref="TurnConsequencesPivot"/>.</returns>
        public TurnConsequencesPivot NextTurn()
        {
            // TODO : run IA actions !

            foreach (var ms in Map)
            {
                ms.UpdateActionsProgress();
            }

            var turnConsequences = HumanPlayer.NextTurn();
            foreach (var iaPlayer in _iaPlayers)
            {
                var turnConsequencesIa = iaPlayer.NextTurn();
                // TODO : what do with this ?
            }

            CurrentTurn++;

            return turnConsequences;
        }

        /// <summary>
        /// Tries to trigger a <see cref="MapSquareImprovementPivot"/> for the current unit; the unit must be a <see cref="Model.Units.SettlerPivot"/>.
        /// </summary>
        /// <param name="actionPivot">The settler's action.</param>
        /// <returns><c>True</c> if success; <c>False</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="actionPivot"/> is <c>Null</c>.</exception>
        public bool SettlerAction(MapSquareImprovementPivot actionPivot)
        {
            if (actionPivot == null)
            {
                throw new ArgumentNullException(nameof(actionPivot));
            }

            return HumanPlayer.SettlerAction(actionPivot);
        }

        /// <summary>
        /// Tries to move the current unit in the specified direction.
        /// </summary>
        /// <param name="direction">The direction; <c>Null</c> to "waste" move.</param>
        /// <returns><c>True</c> if success; <c>False</c> otherwise.</returns>
        public bool MoveCurrentUnit(DirectionPivot? direction)
        {
            return HumanPlayer.MoveCurrentUnit(direction);
        }

        /// <summary>
        /// Loads a game save.
        /// </summary>
        /// <param name="saveFullPath">The save file path.</param>
        /// <returns>A tuple with the <see cref="EnginePivot"/> if success; an error message if failure.</returns>
        public static Tuple<EnginePivot, string> DeserializeSave(string saveFullPath)
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
                    var engine = (EnginePivot)formatter.Deserialize(stream);
                    return new Tuple<EnginePivot, string>(engine, engine == null ? "Failure to deserialize the save !" : null);
                }
            }
            catch (Exception ex)
            {
                return new Tuple<EnginePivot, string>(null, ex.Message);
            }
        }
        
        /// <summary>
        /// Creates a game save into the specified folder.
        /// </summary>
        /// <param name="folder">The target folder.</param>
        /// <returns><c>Null</c> if success; otherwise an error message.</returns>
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

        /// <summary>
        /// Gets a specialist citizen of the specified city (if any), and makes it a regular worker on the specified location.
        /// </summary>
        /// <param name="city">The city.</param>
        /// <param name="mapSquare">The location.</param>
        /// <exception cref="ArgumentNullException"><paramref name="city"/> is <c>Null</c>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="mapSquare"/> is <c>Null</c>.</exception>
        /// <exception cref="ArgumentException">The city is not manage by the human player !</exception>
        /// <exception cref="ArgumentException">The specified square can't be used by the citizen !</exception>
        public void ChangeAnySpecialistToRegular(CityPivot city, MapSquarePivot mapSquare)
        {
            if (city == null)
            {
                throw new ArgumentNullException(nameof(city));
            }

            if (mapSquare == null)
            {
                throw new ArgumentNullException(nameof(mapSquare));
            }

            if (!HumanPlayer.Cities.Contains(city))
            {
                throw new ArgumentException("The city is not manage by the human player !", nameof(city));
            }

            if (!ComputeCityAvailableMapSquares(city).Contains(mapSquare))
            {
                throw new ArgumentException("The specified square can't be used by the citizen !");
            }

            city.ChangeAnySpecialistToRegular(mapSquare);
        }

        /// <summary>
        /// Gets an area of 7x7 <see cref="MapSquarePivot"/> around a city.
        /// </summary>
        /// <param name="city">The city.</param>
        /// <returns>A dictionary with the map square as key, and a tuple [citizen on square / square used by another city] as value.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="city"/> is <c>Null</c>.</exception>
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
                            city.AreaWithoutCityMapSquares.SingleOrDefault(ams => ams.MapSquare == msq)?.Citizen, OccupiedByCity(msq, city)));
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the list of available buildable items for the specified city.
        /// </summary>
        /// <param name="city">The city</param>
        /// <param name="indexOfDefault">Out; the index of current production in the output list. <c>-1</c> if not found.</param>
        /// <returns>List of <see cref="BuildablePivot"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="city"/> is <c>Null</c>.</exception>
        /// <exception cref="ArgumentException">The city is not manage by the human player !</exception>
        public IReadOnlyCollection<BuildablePivot> GetBuildableItemsForCity(CityPivot city, out int indexOfDefault)
        {
            indexOfDefault = -1;

            if (city == null)
            {
                throw new ArgumentNullException(nameof(city));
            }

            if (!HumanPlayer.Cities.Contains(city))
            {
                throw new ArgumentException("The city is not manage by the human player !", nameof(city));
            }

            return HumanPlayer.GetBuildableItemsForCity(city, out indexOfDefault);
        }

        /// <summary>
        /// Changes the city's production.
        /// </summary>
        /// <param name="city">The city.</param>
        /// <param name="buildableDefaultInstance">The buildable item (instance template).</param>
        /// <exception cref="ArgumentNullException"><paramref name="city"/> is <c>Null</c>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="buildableDefaultInstance"/> is <c>Null</c>.</exception>
        /// <exception cref="InvalidOperationException">Failure to create an instance of the specified production type !</exception>
        /// <exception cref="ArgumentException">The city is not manage by the human player !</exception>
        public void ChangeCityProduction(CityPivot city, BuildablePivot buildableDefaultInstance)
        {
            if (city == null)
            {
                throw new ArgumentNullException(nameof(city));
            }

            if (buildableDefaultInstance == null)
            {
                throw new ArgumentNullException(nameof(buildableDefaultInstance));
            }

            if (!HumanPlayer.Cities.Contains(city))
            {
                throw new ArgumentException("The city is not manage by the human player !", nameof(city));
            }

            var invokedInstance = buildableDefaultInstance.CreateOrGetInstance(city);
            if (invokedInstance == null)
            {
                throw new InvalidOperationException($"Failure to create an instance of the specified production type !");
            }

            city.ChangeProduction(invokedInstance);
        }

        /// <summary>
        /// Reset the <see cref="CitizenTypePivot"/> of every citizens of a city.
        /// </summary>
        /// <param name="city">The city.</param>
        /// <exception cref="ArgumentNullException"><paramref name="city"/> is <c>Null</c>.</exception>
        /// <exception cref="ArgumentException">The city is not manage by the human player !</exception>
        public void ResetCitizens(CityPivot city)
        {
            if (city == null)
            {
                throw new ArgumentNullException(nameof(city));
            }

            if (!HumanPlayer.Cities.Contains(city))
            {
                throw new ArgumentException("The city is not manage by the human player !", nameof(city));
            }

            city.ResetCitizens();
        }

        /// <summary>
        /// Changes the <see cref="CitizenTypePivot"/> of the specified <see cref="CitizenPivot"/>;
        /// using a static sequence of type [Regular -> Entertainer -> Scientist -> TaxCollector -> Regular].
        /// </summary>
        /// <param name="citizenSource">The citizen.</param>
        /// <exception cref="ArgumentNullException"><paramref name="citizenSource"/> is <c>Null</c>.</exception>
        /// <exception cref="ArgumentException">The city is not manage by the human player !</exception>
        public void SwitchCitizenType(CitizenPivot citizenSource)
        {
            if (citizenSource == null)
            {
                throw new ArgumentNullException(nameof(citizenSource));
            }

            if (!HumanPlayer.Cities.Contains(citizenSource.City))
            {
                throw new ArgumentException("The city is not manage by the human player !", nameof(citizenSource));
            }

            citizenSource.City.SwitchCitizenType(citizenSource);
        }

        /// <summary>
        /// Changes the <see cref="AdvancePivot"/> in progress.
        /// </summary>
        /// <param name="advance">The new advance.</param>
        /// <returns><c>True</c> if success; <c>False</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="advance"/> is <c>Null</c>.</exception>
        public bool ChangeCurrentAdvance(AdvancePivot advance)
        {
            if (advance == null)
            {
                throw new ArgumentNullException(nameof(advance));
            }

            return HumanPlayer.ChangeCurrentAdvance(advance);
        }

        /// <summary>
        /// Changes the <see cref="RegimePivot"/> after a revolution.
        /// </summary>
        /// <param name="regimePivot">The new regime.</param>
        /// <returns><c>True</c> if success; <c>False</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="regimePivot"/> is <c>Null</c>.</exception>
        public bool ChangeCurrentRegime(RegimePivot regimePivot)
        {
            if (regimePivot is null)
            {
                throw new ArgumentNullException(nameof(regimePivot));
            }

            if (regimePivot == RegimePivot.Anarchy || !HumanPlayer.RevolutionIsDone)
            {
                return false;
            }

            HumanPlayer.ChangeCurrentRegime(regimePivot);
            return true;
        }

        /// <summary>
        /// Triggers a revolution.
        /// </summary>
        public void TriggerRevolution()
        {
            HumanPlayer.TriggerRevolution();
        }

        #endregion
    }
}
