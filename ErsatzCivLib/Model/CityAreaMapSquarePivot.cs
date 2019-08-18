using System;
using System.Linq;
using ErsatzCivLib.Model.Static;

namespace ErsatzCivLib.Model
{
    /// <summary>
    /// Represents a <see cref="MapSquarePivot"/> when used by a citizen inside a <see cref="CityPivot"/>.
    /// </summary>
    [Serializable]
    public class CityAreaMapSquarePivot
    {
        private const int CITY_FOOD_MIN = 2;
        private const int CITY_PRODUCTIVITY_MIN = 0;
        private const int CITY_COMMERCE_MIN = 0;

        private const double FACTORY_PRODUCTIVITY_INCREASE_RATIO = 1.5;
        private const double MFGPLANT_PRODUCTIVITY_INCREASE_RATIO = 2;
        private const double HYDROPLANT_PRODUCTIVITY_INCREASE_RATIO = 1.5;
        private const double NUCLEARPLANT_PRODUCTIVITY_INCREASE_RATIO = 1.5;
        private const double POWERPLANT_PRODUCTIVITY_INCREASE_RATIO = 1.5;

        #region Embedded properties

        /// <summary>
        /// The city.
        /// </summary>
        public CityPivot City { get; private set; }
        /// <summary>
        /// The underlying <see cref="MapSquarePivot"/>.
        /// </summary>
        public MapSquarePivot MapSquare { get; private set; }
        /// <summary>
        /// The citizen who works on the square; must not be a specialist; <c>Null for the city square itself</c>.
        /// </summary>
        public CitizenPivot Citizen { get; private set; }

        #endregion

        #region Inferred properties

        /// <summary>
        /// Indicates if the instance is a city center.
        /// </summary>
        public bool IsCityCenter
        {
            get
            {
                return Citizen == null;
            }
        }
        /// <summary>
        /// Food value of this instance.
        /// </summary>
        public int Food
        {
            get
            {
                var baseValue = IsCityCenter && MapSquare.Food < CITY_FOOD_MIN ?
                    CITY_FOOD_MIN : MapSquare.Food;

                return baseValue;
            }
        }
        /// <summary>
        /// Productivity value of this instance.
        /// </summary>
        public int Productivity
        {
            get
            {
                var baseValue = IsCityCenter && MapSquare.Productivity < CITY_PRODUCTIVITY_MIN ?
                    CITY_PRODUCTIVITY_MIN : MapSquare.Productivity;

                bool hasProdImprovement = false;
                if (City.Improvements.Contains(CityImprovementPivot.MfgPlant))
                {
                    baseValue = (int)Math.Floor(MFGPLANT_PRODUCTIVITY_INCREASE_RATIO * baseValue);
                    hasProdImprovement = true;
                }
                else if (City.Improvements.Contains(CityImprovementPivot.Factory))
                {
                    baseValue = (int)Math.Floor(FACTORY_PRODUCTIVITY_INCREASE_RATIO * baseValue);
                    hasProdImprovement = true;
                }

                if (hasProdImprovement)
                {
                    if (City.Improvements.Contains(CityImprovementPivot.HydroPlant))
                    {
                        baseValue = (int)Math.Floor(HYDROPLANT_PRODUCTIVITY_INCREASE_RATIO * baseValue);
                    }
                    else if (City.Improvements.Contains(CityImprovementPivot.NuclearPlant))
                    {
                        baseValue = (int)Math.Floor(NUCLEARPLANT_PRODUCTIVITY_INCREASE_RATIO * baseValue);
                    }
                    else if (City.Improvements.Contains(CityImprovementPivot.PowerPlant))
                    {
                        baseValue = (int)Math.Floor(POWERPLANT_PRODUCTIVITY_INCREASE_RATIO * baseValue);
                    }
                }

                return baseValue;
            }
        }
        /// <summary>
        /// Commerce value of this instance.
        /// </summary>
        /// <remarks>Pollution and settler actions included.</remarks>
        public int Commerce
        {
            get
            {
                var baseValue = IsCityCenter && MapSquare.Commerce < CITY_COMMERCE_MIN ?
                    CITY_COMMERCE_MIN : MapSquare.Commerce;

                baseValue += baseValue > 0 ? City.Player.Regime.CommerceBonus : 0;

                if (baseValue > 0 && City.Player.WonderIsActive(WonderPivot.Colossus))
                {
                    baseValue++;
                }

                return baseValue;
            }
        }
        /// <summary>
        /// Sum of food, productivity and commerce statistics.
        /// </summary>
        public int TotalValue
        {
            get
            {
                return Food + Productivity + Commerce;
            }
        }

        #endregion

        /// <summary>
        /// Constructor (for new citizen).
        /// </summary>
        /// <param name="city">The <see cref="City"/> value.</param>
        /// <param name="mapSquare">The <see cref="MapSquare"/> value.</param>
        internal CityAreaMapSquarePivot(CityPivot city, MapSquarePivot mapSquare)
        {
            City = city;
            MapSquare = mapSquare;
            Citizen = city.MapSquareLocation == MapSquare ? null : new CitizenPivot(city);
        }

        /// <summary>
        /// Constructor (for existing citizen).
        /// </summary>
        /// <param name="mapSquare">The <see cref="MapSquare"/> value.</param>
        /// <param name="citizen">The <see cref="Citizen"/> value.</param>
        internal CityAreaMapSquarePivot(MapSquarePivot mapSquare, CitizenPivot citizen)
        {
            City = citizen.City;
            MapSquare = mapSquare;
            Citizen = citizen;
        }
    }
}
