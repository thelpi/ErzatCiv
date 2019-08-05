using System;
using ErsatzCivLib.Model.Persistent;

namespace ErsatzCivLib.Model
{
    /// <summary>
    /// Represents a player in-game.
    /// </summary>
    /// <seealso cref="IEquatable{T}"/>
    [Serializable]
    public class PlayerPivot : IEquatable<PlayerPivot>
    {
        /// <summary>
        /// The player civilization.
        /// </summary>
        public CivilizationPivot Civilization { get; private set; }
        /// <summary>
        /// IA player yes/no.
        /// </summary>
        public bool IsIA { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="civilization">The <see cref="Civilization"/> value.</param>
        /// <param name="isIa">The <see cref="IsIA"/> value.</param>
        internal PlayerPivot(CivilizationPivot civilization, bool isIa)
        {
            Civilization = civilization;
            IsIA = isIa;
        }

        #region IEquatable implementation

        /// <summary>
        /// Checks if this instance is equal to another one.
        /// </summary>
        /// <param name="other">The other instance.</param>
        /// <returns><c>True</c> if equals; <c>False</c> otherwise.</returns>
        public bool Equals(PlayerPivot other)
        {
            return Civilization == other?.Civilization;
        }

        /// <summary>
        /// Operator "==" override. Checks equality between two instances.
        /// </summary>
        /// <param name="p1">The first <see cref="PlayerPivot"/>.</param>
        /// <param name="p2">The second <see cref="PlayerPivot"/>.</param>
        /// <returns><c>True</c> if equals; <c>False</c> otherwise.</returns>
        public static bool operator ==(PlayerPivot p1, PlayerPivot p2)
        {
            if (p1 is null)
            {
                return p2 is null;
            }

            return p1.Equals(p2) == true;
        }

        /// <summary>
        /// Operator "!=" override. Checks non-equality between two instances.
        /// </summary>
        /// <param name="p1">The first <see cref="PlayerPivot"/>.</param>
        /// <param name="p2">The second <see cref="PlayerPivot"/>.</param>
        /// <returns><c>False</c> if equals; <c>True</c> otherwise.</returns>
        public static bool operator !=(PlayerPivot p1, PlayerPivot p2)
        {
            return !(p1 == p2);
        }

        /// <inhrritdoc />
        public override bool Equals(object obj)
        {
            return obj is PlayerPivot && Equals(obj as PlayerPivot);
        }

        /// <inhrritdoc />
        public override int GetHashCode()
        {
            return Civilization.GetHashCode();
        }

        #endregion
    }
}
