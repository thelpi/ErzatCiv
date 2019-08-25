using System;
using System.Collections.Generic;

namespace ErsatzCivLib.Model.Events
{
    /// <summary>
    /// Event informations when an unit is killed (or several on the same square at the same time).
    /// </summary>
    /// <seealso cref="EventArgs"/>
    public class KilledUnitEventArgs : EventArgs
    {
        private readonly List<UnitPivot> _units;
        /// <summary>
        /// List of killed <see cref="UnitPivot"/> on the same square at the same time.
        /// </summary>
        public IReadOnlyCollection<UnitPivot> Units { get { return _units; } }
        /// <summary>
        /// The killer <see cref="PlayerPivot"/>.
        /// </summary>
        public PlayerPivot Killer { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="unit">The <see cref="Units"/> value, when a single unit is killed.</param>
        /// <param name="killer">The <see cref="Killer"/> value.</param>
        internal KilledUnitEventArgs(UnitPivot unit, PlayerPivot killer) : this(new[] { unit }, killer) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="units">The <see cref="Units"/> value.</param>
        /// <param name="killer">The <see cref="Killer"/> value.</param>
        internal KilledUnitEventArgs(IEnumerable<UnitPivot> units, PlayerPivot killer)
        {
            _units = new List<UnitPivot>(units);
            Killer = killer;
        }
    }
}
