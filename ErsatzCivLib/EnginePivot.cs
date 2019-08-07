using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using ErsatzCivLib.Model;
using ErsatzCivLib.Model.Enums;
using ErsatzCivLib.Model.Events;
using ErsatzCivLib.Model.Static;

namespace ErsatzCivLib
{
    [Serializable]
    public class EnginePivot
    {
        private readonly List<PlayerPivot> _iaPlayers = new List<PlayerPivot>();

        #region Public properties

        public int CurrentTurn { get; private set; }
        // TODO : wrong version.
        public int CurrentYear { get { return (CurrentTurn * 10) - 4000; } }
        public MapPivot Map { get; private set; }
        public PlayerPivot HumanPlayer { get; }
        public IReadOnlyCollection<PlayerPivot> Players
        {
            get
            {
                return _iaPlayers.Concat(new[] { HumanPlayer }).ToList();
            }
        }

        #endregion

        public EnginePivot(SizePivot mapSize, LandShapePivot mapShape, LandCoveragePivot landCoverage, TemperaturePivot temperature,
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

            HumanPlayer = new PlayerPivot(this, playerCivilization, false, GetRandomLocation(excludedSpots));
            for (int i = 0; i < iaPlayersCount; i++)
            {
                CivilizationPivot iaCiv = null;
                do
                {
                    iaCiv = CivilizationPivot.Instances.ElementAt(Tools.Randomizer.Next(0, CivilizationPivot.Instances.Count));
                }
                while (HumanPlayer.Civilization == iaCiv || _iaPlayers.Any(ia => ia.Civilization == iaCiv));
                _iaPlayers.Add(new PlayerPivot(this, iaCiv, true, GetRandomLocation(excludedSpots)));
            }

            CurrentTurn = 1;
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

        private CityPivot GetCityFromCitizen(CitizenPivot citizenSource)
        {
            return GetEveryCities().SingleOrDefault(c => c.Citizens.Contains(citizenSource));
        }

        private bool OccupiedByCity(MapSquarePivot mapSquare, CityPivot exceptCity = null)
        {
            return GetEveryCities().Any(c => (exceptCity == null || exceptCity != c) && c.Citizens.Any(cc => cc.MapSquare == mapSquare));
        }

        #endregion

        #region Internal methods

        internal bool IsCity(MapSquarePivot square)
        {
            return GetEveryCities().Any(c => c.MapSquareLocation == square);
        }

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

        internal IReadOnlyCollection<WonderPivot> GetEveryWonders()
        {
            return Players.SelectMany(p => p.Wonders).ToList();
        }

        #endregion

        #region Public methods (used by the GUI)

        public CityPivot BuildCity(string name, out bool notUniqueNameError)
        {
            notUniqueNameError = false;
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            return HumanPlayer.BuildCity(CurrentTurn, name, out notUniqueNameError);
        }

        public bool CanBuildCity()
        {
            return HumanPlayer.CanBuildCity();
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

            var turnConsequences = HumanPlayer.NextTurn();
            foreach (var iaPlayer in _iaPlayers)
            {
                var turnConsequencesIa = iaPlayer.NextTurn();
                // TODO : what do with this ?
            }

            CurrentTurn++;

            return turnConsequences;
        }

        public bool WorkerAction(WorkerActionPivot actionPivot)
        {
            if (actionPivot == null)
            {
                return false;
            }

            return HumanPlayer.WorkerAction(actionPivot);
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
            return HumanPlayer.MoveCurrentUnit(direction);
        }
        
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
        
        public IReadOnlyCollection<BuildablePivot> GetBuildableItemsForCity(CityPivot city, out int indexOfDefault)
        {
            indexOfDefault = -1;

            if (city == null || !HumanPlayer.Cities.Contains(city))
            {
                return null;
            }

            var buildableDefaultInstances = HumanPlayer.GetBuildableItemsForCity(city, out indexOfDefault);

            // TODO : remove wonders already built globally.

            return buildableDefaultInstances;
        }
        
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

        #endregion
    }
}
