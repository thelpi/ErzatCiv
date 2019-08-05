﻿using System;

namespace ErsatzCivLib.Model
{
    /// <summary>
    /// Capitalization (city production).
    /// </summary>
    /// <seealso cref="BuildablePivot"/>
    [Serializable]
    public class CapitalizationPivot : BuildablePivot
    {
        private const int PRODUCTIVITY_COST = 0;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="mapSquare">Not used.</param>
        private CapitalizationPivot() : base(PRODUCTIVITY_COST) { }

        /// <summary>
        /// Default instance.
        /// </summary>
        public static readonly CapitalizationPivot Default = new CapitalizationPivot();
    }
}
