﻿using System;
using System.Collections.Generic;

namespace ErsatzCivLib.Model
{
    /// <summary>
    /// Represents a city improvement.
    /// </summary>
    /// <seealso cref="BuildablePivot"/>
    [Serializable]
    public class CityImprovementPivot : BuildablePivot, IEquatable<CityImprovementPivot>
    {
        /// <summary>
        /// The maintenance cost, in gold by turn.
        /// </summary>
        public int MaintenanceCost { get; private set; }
        /// <summary>
        /// The purchse price, in gold.
        /// </summary>
        public int PurchasePrice { get; private set; }
        /// <summary>
        /// The sell value, in gold.
        /// </summary>
        public int SellValue { get; private set; }

        private CityImprovementPivot(int productivityCost, int maintenanceCost, int purchasePrice,
            int sellValue, string name, bool hasCitizenMoodEffect) :
            base (productivityCost, name, hasCitizenMoodEffect)
        {
            MaintenanceCost = maintenanceCost;
            PurchasePrice = purchasePrice;
            SellValue = sellValue;
        }

        public bool Equals(CityImprovementPivot other)
        {
            return Name == other?.Name;
        }

        public static bool operator ==(CityImprovementPivot ms1, CityImprovementPivot ms2)
        {
            if (ms1 is null)
            {
                return ms2 is null;
            }

            return ms1.Equals(ms2) == true;
        }

        public static bool operator ==(CityImprovementPivot ms1, BuildablePivot ms2)
        {
            if (ms1 is null)
            {
                return ms2 is null;
            }

            return ms1.Equals(ms2) == true;
        }

        public static bool operator !=(CityImprovementPivot ms1, CityImprovementPivot ms2)
        {
            return !(ms1 == ms2);
        }

        public static bool operator !=(CityImprovementPivot ms1, BuildablePivot ms2)
        {
            return !(ms1 == ms2);
        }

        public override bool Equals(object obj)
        {
            return obj is CityImprovementPivot && Equals(obj as CityImprovementPivot);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        #region Static instances

        public static readonly CityImprovementPivot Aqueduc = new CityImprovementPivot(120, 2, 480, 120, "Aqueduc", false);
        public static readonly CityImprovementPivot Barracks = new CityImprovementPivot(40, 2, 160, 40, "Barracks", false);
        public static readonly CityImprovementPivot CityWalls = new CityImprovementPivot(120, 2, 480, 120, "CityWalls", false);
        public static readonly CityImprovementPivot Colosseum = new CityImprovementPivot(100, 4, 400, 100, "Colosseum", true);
        public static readonly CityImprovementPivot Courthouse = new CityImprovementPivot(80, 1, 320, 80, "Courthouse", false);
        public static readonly CityImprovementPivot Granary = new CityImprovementPivot(60, 1, 240, 60, "Granary", false);
        public static readonly CityImprovementPivot Library = new CityImprovementPivot(80, 1, 320, 80, "Library", false);
        public static readonly CityImprovementPivot Marketplace = new CityImprovementPivot(80, 1, 320, 80, "Marketplace", false);
        public static readonly CityImprovementPivot Temple = new CityImprovementPivot(40, 1, 160, 40, "Temple", true);
        // TODO : add new instances to "Instances".

        public static readonly IReadOnlyCollection<CityImprovementPivot> Instances = new List<CityImprovementPivot>
        {
            Aqueduc,
            Barracks,
            CityWalls,
            Colosseum,
            Courthouse,
            Granary,
            Library,
            Marketplace,
            Temple
        };

        #endregion
    }
}
