using System;
using System.Collections.Generic;
using ErsatzCivLib.Model.Static;
using ErsatzCivLib.Model.Units.Land;

namespace ErsatzCivLib.Model
{
    /// <summary>
    /// Represents an hut.
    /// </summary>
    [Serializable]
    public class HutPivot : IEquatable<HutPivot>
    {
        #region Embedded properties

        /// <summary>
        /// Hut location on the map.
        /// </summary>
        public MapSquarePivot MapSquareLocation { get; private set; }
        /// <summary>
        /// Gold amount inside the hut.
        /// </summary>
        public int Gold { get; private set; }
        /// <summary>
        /// <c>True</c> if an <see cref="AdvancePivot"/> is discoverd inside the hut; <c>False</c> otherwise.
        /// </summary>
        public bool IsAdvance { get; private set; }
        /// <summary>
        /// <c>True</c> if a friendly <see cref="CavalryPivot"/> is inside the hut; <c>False</c> otherwise.
        /// </summary>
        public bool IsFriendlyUnit { get; private set; }
        /// <summary>
        /// <c>True</c> if the hut is a city; <c>False</c> otherwise.
        /// </summary>
        public bool IsCity { get; private set; }
        /// <summary>
        /// <c>True</c> if the hut contains an horde of barbarians; <c>False</c> otherwise.
        /// </summary>
        public bool IsBarbarians { get; private set; }

        #endregion

        private HutPivot() { }

        #region Static constructors

        /// <summary>
        /// Creates an <see cref="HutPivot"/> with some gold inside.
        /// </summary>
        /// <param name="mapSquare">The <see cref="MapSquareLocation"/> value.</param>
        /// <returns>An instance of <see cref="HutPivot"/>.</returns>
        internal static HutPivot GoldHut(MapSquarePivot mapSquare)
        {
            return new HutPivot
            {
                MapSquareLocation = mapSquare,
                IsAdvance = false,
                Gold = 50,
                IsFriendlyUnit = false,
                IsCity = false,
                IsBarbarians = false
            };
        }

        /// <summary>
        /// Creates an <see cref="HutPivot"/> with a city inside.
        /// </summary>
        /// <param name="mapSquare">The <see cref="MapSquareLocation"/> value.</param>
        /// <returns>An instance of <see cref="HutPivot"/>.</returns>
        internal static HutPivot CityHut(MapSquarePivot mapSquare)
        {
            return new HutPivot
            {
                MapSquareLocation = mapSquare,
                IsAdvance = false,
                Gold = 0,
                IsFriendlyUnit = false,
                IsCity = true,
                IsBarbarians = false
            };
        }

        /// <summary>
        /// Creates an <see cref="HutPivot"/> with a friendly <see cref="CavalryPivot"/> inside.
        /// </summary>
        /// <param name="mapSquare">The <see cref="MapSquareLocation"/> value.</param>
        /// <returns>An instance of <see cref="HutPivot"/>.</returns>
        internal static HutPivot FriendlyUnitHut(MapSquarePivot mapSquare)
        {
            return new HutPivot
            {
                MapSquareLocation = mapSquare,
                IsAdvance = false,
                Gold = 0,
                IsFriendlyUnit = true,
                IsCity = false,
                IsBarbarians = false
            };
        }

        /// <summary>
        /// Creates an <see cref="HutPivot"/> with an <see cref="AdvancePivot"/> inside.
        /// </summary>
        /// <param name="mapSquare">The <see cref="MapSquareLocation"/> value.</param>
        /// <returns>An instance of <see cref="HutPivot"/>.</returns>
        internal static HutPivot AdvanceHut(MapSquarePivot mapSquare)
        {
            return new HutPivot
            {
                MapSquareLocation = mapSquare,
                IsAdvance = true,
                Gold = 0,
                IsFriendlyUnit = false,
                IsCity = false,
                IsBarbarians = false
            };
        }

        /// <summary>
        /// Creates an <see cref="HutPivot"/> with an horde of barbarians inside.
        /// </summary>
        /// <param name="mapSquare">The <see cref="MapSquareLocation"/> value.</param>
        /// <returns>An instance of <see cref="HutPivot"/>.</returns>
        internal static HutPivot BarbariansHut(MapSquarePivot mapSquare)
        {
            return new HutPivot
            {
                MapSquareLocation = mapSquare,
                IsAdvance = false,
                Gold = 0,
                IsFriendlyUnit = false,
                IsCity = false,
                IsBarbarians = true
            };
        }

        #endregion

        #region IEquatable implementation

        /// <summary>
        /// Checks if this instance is equal to another one.
        /// </summary>
        /// <param name="other">The other instance.</param>
        /// <returns><c>True</c> if equals; <c>False</c> otherwise.</returns>
        public bool Equals(HutPivot other)
        {
            return MapSquareLocation == other?.MapSquareLocation;
        }

        /// <summary>
        /// Operator "==" override. Checks equality between two instances.
        /// </summary>
        /// <param name="r1">The first <see cref="HutPivot"/>.</param>
        /// <param name="r2">The second <see cref="HutPivot"/>.</param>
        /// <returns><c>True</c> if equals; <c>False</c> otherwise.</returns>
        public static bool operator ==(HutPivot r1, HutPivot r2)
        {
            if (r1 is null)
            {
                return r2 is null;
            }

            return r1.Equals(r2) == true;
        }

        /// <summary>
        /// Operator "!=" override. Checks non-equality between two instances.
        /// </summary>
        /// <param name="r1">The first <see cref="HutPivot"/>.</param>
        /// <param name="r2">The second <see cref="HutPivot"/>.</param>
        /// <returns><c>False</c> if equals; <c>True</c> otherwise.</returns>
        public static bool operator !=(HutPivot r1, HutPivot r2)
        {
            return !(r1 == r2);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is HutPivot && Equals(obj as HutPivot);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return MapSquareLocation.GetHashCode();
        }

        #endregion
    }
}
