using System;
using System.Collections.Generic;

namespace ErsatzCivLib.Model.Static
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

        private CityImprovementPivot(int productivityCost, int maintenanceCost, int purchasePrice, int sellValue, string name,
            bool hasCitizenMoodEffect, AdvancePivot advancePrerequisite, AdvancePivot advanceObsolescence) :
            base (productivityCost, advancePrerequisite, advanceObsolescence, name, hasCitizenMoodEffect)
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

        public static readonly CityImprovementPivot Colosseum = new CityImprovementPivot(100, 4, 400, 100, "Colosseum", true, AdvancePivot.Construction, null);
        public static readonly CityImprovementPivot Temple = new CityImprovementPivot(40, 1, 160, 40, "Temple", true, AdvancePivot.CeremonialBurial, null);
        public static readonly CityImprovementPivot Cathedral = new CityImprovementPivot(160, 3, 640, 160, "Cathedral", true, AdvancePivot.Religion, null);

        public static readonly CityImprovementPivot Marketplace = new CityImprovementPivot(80, 1, 320, 80, "Marketplace", false, AdvancePivot.Currency, null);
        public static readonly CityImprovementPivot Bank = new CityImprovementPivot(120, 3, 480, 120, "Bank", false, AdvancePivot.Banking, null);

        public static readonly CityImprovementPivot PowerPlant = new CityImprovementPivot(160, 3, 640, 160, "Power Plant", false, AdvancePivot.Refining, null);
        public static readonly CityImprovementPivot NuclearPlant = new CityImprovementPivot(160, 2, 640, 160, "Nuclear Plant", false, AdvancePivot.NuclearPower, null);
        public static readonly CityImprovementPivot MfgPlant = new CityImprovementPivot(320, 6, 1280, 320, "Mfg. Plant", false, AdvancePivot.Robotics, null);
        public static readonly CityImprovementPivot HydroPlant = new CityImprovementPivot(240, 4, 960, 240, "Hydro Plant", false, AdvancePivot.Electronics, null);
        public static readonly CityImprovementPivot Factory = new CityImprovementPivot(200, 4, 800, 200, "Factory", false, AdvancePivot.Industrialization, null);

        public static readonly CityImprovementPivot University = new CityImprovementPivot(160, 3, 640, 160, "University", false, AdvancePivot.University, null);
        public static readonly CityImprovementPivot Library = new CityImprovementPivot(80, 1, 320, 80, "Library", false, AdvancePivot.Writing, null);

        public static readonly CityImprovementPivot Palace = new CityImprovementPivot(200, 0, 800, 200, "Palace", false, AdvancePivot.Masonry, null);
        public static readonly CityImprovementPivot Courthouse = new CityImprovementPivot(80, 1, 320, 80, "Courthouse", false, AdvancePivot.CodeOfLaws, null);

        public static readonly CityImprovementPivot Aqueduc = new CityImprovementPivot(120, 2, 480, 120, "Aqueduc", false, AdvancePivot.Construction, null);
        public static readonly CityImprovementPivot Granary = new CityImprovementPivot(60, 1, 240, 60, "Granary", false, AdvancePivot.Pottery, null);
        public static readonly CityImprovementPivot RecyclingCenter = new CityImprovementPivot(200, 2, 800, 200, "Recycling Center", false, AdvancePivot.Recycling, null);
        public static readonly CityImprovementPivot MassTransit = new CityImprovementPivot(160, 4, 640, 160, "Mass Transit", false, AdvancePivot.MassProduction, null);

        public static readonly CityImprovementPivot Barracks = new CityImprovementPivot(40, 2, 160, 40, "Barracks", false, null, null);
        public static readonly CityImprovementPivot CityWalls = new CityImprovementPivot(120, 2, 480, 120, "CityWalls", false, AdvancePivot.Masonry, null);
        public static readonly CityImprovementPivot SdiDefense = new CityImprovementPivot(200, 4, 800, 200, "SDI Defense", false, AdvancePivot.Superconductor, null);

        private static List<CityImprovementPivot> _instances = null;

        /// <summary>
        /// List of every <see cref="CityImprovementPivot"/> instances.
        /// </summary>
        public static IReadOnlyCollection<CityImprovementPivot> Instances
        {
            get
            {
                if (_instances == null)
                {
                    _instances = Tools.GetInstancesOfTypeFromStaticFields<CityImprovementPivot>();
                }
                return _instances;
            }
        }

        #endregion
    }
}
