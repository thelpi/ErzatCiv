using System;
using System.Collections.Generic;

namespace ErsatzCivLib.Model.Static
{
    /// <summary>
    /// Represents a city improvement.
    /// </summary>
    /// <seealso cref="BuildablePivot"/>
    /// <seealso cref="IEquatable{T}"/>
    [Serializable]
    public class CityImprovementPivot : BuildablePivot, IEquatable<CityImprovementPivot>
    {
        #region Embedded properties

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
        /// <summary>
        /// A <see cref="CityImprovementPivot"/> requried to built this instance.
        /// </summary>
        public CityImprovementPivot ImprovementPrerequisite { get; private set; }

        #endregion

        private CityImprovementPivot(int productivityCost, int maintenanceCost, int purchasePrice, int sellValue, string name,
            bool hasCitizenMoodEffect, AdvancePivot advancePrerequisite, CityImprovementPivot improvementPrerequisite) :
            base (productivityCost, advancePrerequisite, null, name, hasCitizenMoodEffect)
        {
            MaintenanceCost = maintenanceCost;
            PurchasePrice = purchasePrice;
            SellValue = sellValue;
            ImprovementPrerequisite = improvementPrerequisite;
        }

        #region IEquatable implementation

        /// <summary>
        /// Checks if this instance is equal to another one.
        /// </summary>
        /// <param name="other">The other instance.</param>
        /// <returns><c>True</c> if equals; <c>False</c> otherwise.</returns>
        public bool Equals(CityImprovementPivot other)
        {
            return Name == other?.Name;
        }

        /// <summary>
        /// Operator "==" override. Checks equality between two instances.
        /// </summary>
        /// <param name="w1">The first <see cref="CityImprovementPivot"/>.</param>
        /// <param name="w2">The second <see cref="CityImprovementPivot"/>.</param>
        /// <returns><c>True</c> if equals; <c>False</c> otherwise.</returns>
        public static bool operator ==(CityImprovementPivot w1, CityImprovementPivot w2)
        {
            if (w1 is null)
            {
                return w2 is null;
            }

            return w1.Equals(w2) == true;
        }

        /// <summary>
        /// Operator "==" override. Checks equality between two instances.
        /// </summary>
        /// <param name="w">A <see cref="CityImprovementPivot"/> instance.</param>
        /// <param name="any">Any <see cref="BuildablePivot"/> instance.</param>
        /// <returns><c>True</c> if equals; <c>False</c> otherwise.</returns>
        public static bool operator ==(CityImprovementPivot w, BuildablePivot any)
        {
            if (w is null)
            {
                return any is null;
            }

            return w.Equals(any) == true;
        }

        /// <summary>
        /// Operator "!=" override. Checks non-equality between two instances.
        /// </summary>
        /// <param name="w1">The first <see cref="CityImprovementPivot"/>.</param>
        /// <param name="w2">The second <see cref="CityImprovementPivot"/>.</param>
        /// <returns><c>False</c> if equals; <c>True</c> otherwise.</returns>
        public static bool operator !=(CityImprovementPivot w1, CityImprovementPivot w2)
        {
            return !(w1 == w2);
        }

        /// <summary>
        /// Operator "!=" override. Checks non-equality between two instances.
        /// </summary>
        /// <param name="w">A <see cref="CityImprovementPivot"/> instance.</param>
        /// <param name="any">Any <see cref="BuildablePivot"/> instance.</param>
        /// <returns><c>False</c> if equals; <c>True</c> otherwise.</returns>
        public static bool operator !=(CityImprovementPivot w, BuildablePivot any)
        {
            return !(w == any);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is CityImprovementPivot && Equals(obj as CityImprovementPivot);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        #endregion

        #region Static instances

        /// <summary>
        /// Colosseum.
        /// </summary>
        public static readonly CityImprovementPivot Colosseum = new CityImprovementPivot(100, 4, 400, 100, "Colosseum", true, AdvancePivot.Construction, null);
        /// <summary>
        /// Temple.
        /// </summary>
        public static readonly CityImprovementPivot Temple = new CityImprovementPivot(40, 1, 160, 40, "Temple", true, AdvancePivot.CeremonialBurial, null);
        /// <summary>
        /// Cathedral.
        /// </summary>
        public static readonly CityImprovementPivot Cathedral = new CityImprovementPivot(160, 3, 640, 160, "Cathedral", true, AdvancePivot.Religion, null);
        /// <summary>
        /// Marketplace.
        /// </summary>
        public static readonly CityImprovementPivot Marketplace = new CityImprovementPivot(80, 1, 320, 80, "Marketplace", false, AdvancePivot.Currency, null);
        /// <summary>
        /// Bank.
        /// </summary>
        public static readonly CityImprovementPivot Bank = new CityImprovementPivot(120, 3, 480, 120, "Bank", false, AdvancePivot.Banking, Marketplace);
        /// <summary>
        /// Power Plant.
        /// </summary>
        public static readonly CityImprovementPivot PowerPlant = new CityImprovementPivot(160, 3, 640, 160, "Power Plant", false, AdvancePivot.Refining, null);
        /// <summary>
        /// Nuclear Plant.
        /// </summary>
        public static readonly CityImprovementPivot NuclearPlant = new CityImprovementPivot(160, 2, 640, 160, "Nuclear Plant", false, AdvancePivot.NuclearPower, null);
        /// <summary>
        /// Manufacturing Plant.
        /// </summary>
        public static readonly CityImprovementPivot MfgPlant = new CityImprovementPivot(320, 6, 1280, 320, "Manufacturing Plant", false, AdvancePivot.Robotics, null);
        /// <summary>
        /// Hydro Plant.
        /// </summary>
        public static readonly CityImprovementPivot HydroPlant = new CityImprovementPivot(240, 4, 960, 240, "Hydro Plant", false, AdvancePivot.Electronics, null);
        /// <summary>
        /// Factory.
        /// </summary>
        public static readonly CityImprovementPivot Factory = new CityImprovementPivot(200, 4, 800, 200, "Factory", false, AdvancePivot.Industrialization, null);
        /// <summary>
        /// University.
        /// </summary>
        public static readonly CityImprovementPivot University = new CityImprovementPivot(160, 3, 640, 160, "University", false, AdvancePivot.University, Library);
        /// <summary>
        /// Library.
        /// </summary>
        public static readonly CityImprovementPivot Library = new CityImprovementPivot(80, 1, 320, 80, "Library", false, AdvancePivot.Writing, null);
        /// <summary>
        /// Palace.
        /// </summary>
        public static readonly CityImprovementPivot Palace = new CityImprovementPivot(200, 0, 800, 200, "Palace", false, AdvancePivot.Masonry, null);
        /// <summary>
        /// Courthouse.
        /// </summary>
        public static readonly CityImprovementPivot Courthouse = new CityImprovementPivot(80, 1, 320, 80, "Courthouse", false, AdvancePivot.CodeOfLaws, null);
        /// <summary>
        /// Aqueduc.
        /// </summary>
        public static readonly CityImprovementPivot Aqueduc = new CityImprovementPivot(120, 2, 480, 120, "Aqueduc", false, AdvancePivot.Construction, null);
        /// <summary>
        /// Granary.
        /// </summary>
        public static readonly CityImprovementPivot Granary = new CityImprovementPivot(60, 1, 240, 60, "Granary", false, AdvancePivot.Pottery, null);
        /// <summary>
        /// Recycling Center.
        /// </summary>
        public static readonly CityImprovementPivot RecyclingCenter = new CityImprovementPivot(200, 2, 800, 200, "Recycling Center", false, AdvancePivot.Recycling, null);
        /// <summary>
        /// Mass Transit.
        /// </summary>
        public static readonly CityImprovementPivot MassTransit = new CityImprovementPivot(160, 4, 640, 160, "Mass Transit", false, AdvancePivot.MassProduction, null);
        /// <summary>
        /// Barracks.
        /// </summary>
        public static readonly CityImprovementPivot Barracks = new CityImprovementPivot(40, 2, 160, 40, "Barracks", false, null, null);
        /// <summary>
        /// City Walls.
        /// </summary>
        public static readonly CityImprovementPivot CityWalls = new CityImprovementPivot(120, 2, 480, 120, "City Walls", false, AdvancePivot.Masonry, null);
        /// <summary>
        /// SDI Defense.
        /// </summary>
        public static readonly CityImprovementPivot SdiDefense = new CityImprovementPivot(200, 4, 800, 200, "SDI Defense", false, AdvancePivot.Superconductor, null);

        #endregion

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
    }
}
