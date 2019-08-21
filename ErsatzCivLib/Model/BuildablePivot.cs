using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
//using ErsatzCivLib.Model.SpaceShipParts;
using ErsatzCivLib.Model.Static;

namespace ErsatzCivLib.Model
{
    /// <summary>
    /// Represents an item a city can build.
    /// </summary>
    [Serializable]
    public abstract class BuildablePivot
    {
        #region Embedded properties

        /// <summary>
        /// Name.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Productivity amount required to build the item.
        /// </summary>
        public int ProductivityCost { get; private set; }
        /// <summary>
        /// Indicates if the item, when built, has impact on citizens happiness.
        /// </summary>
        public bool HasCitizenHappinessEffect { get; private set; }
        /// <summary>
        /// <see cref="AdvancePivot"/> required to access the instance.
        /// </summary>
        public AdvancePivot AdvancePrerequisite { get; private set; }
        /// <summary>
        /// <see cref="AdvancePivot"/> which makes the instance obsolete.
        /// </summary>
        public AdvancePivot AdvanceObsolescence { get; private set; }
        /// <summary>
        /// The purchse price, in gold. <c>-1</c> when the item can't be sold.
        /// </summary>
        public int PurchasePrice { get; private set; }

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="productivityCost">The <see cref="ProductivityCost"/> value.</param>
        /// <param name="advancePrerequisite">The <see cref="AdvancePrerequisite"/> value.</param>
        /// <param name="advanceObsolescence">The <see cref="AdvanceObsolescence"/> value.</param>
        /// <param name="purchasePrice">The <see cref="PurchasePrice"/> value.</param>
        /// <param name="name">The <see cref="Name"/> value.
        /// IF <c>Null</c>, the class name is used without the "Pivot" suffix.</param>
        /// <param name="hasCitizenHappinessEffect">The <see cref="HasCitizenHappinessEffect"/> value.</param>
        protected BuildablePivot(int productivityCost, AdvancePivot advancePrerequisite, AdvancePivot advanceObsolescence,
            int purchasePrice, string name, bool hasCitizenHappinessEffect)
        {
            Name = name ?? GetType().Name.Replace("Pivot", string.Empty);
            ProductivityCost = productivityCost;
            HasCitizenHappinessEffect = hasCitizenHappinessEffect;
            AdvancePrerequisite = advancePrerequisite;
            AdvanceObsolescence = advanceObsolescence;
            PurchasePrice = purchasePrice;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Name} ({ProductivityCost})";
        }

        /// <summary>
        /// Indicates if the instance is from the specified type (or a child of it).
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> to check.</typeparam>
        /// <returns><c>True</c> if it is; <c>False</c> otherwise.</returns>
        public bool Is<T>() where T : BuildablePivot
        {
            return GetType() == typeof(T) || GetType().IsSubclassOf(typeof(T));
        }

        private static List<BuildablePivot> _defaultUnitInstances = null;
        /// <summary>
        /// Gets every <c>Default</c> instances for each concrete type which inherits from <see cref="BuildablePivot"/>.
        /// </summary>
        internal static IReadOnlyCollection<BuildablePivot> GetEveryDefaultInstances
        {
            get
            {
                if (_defaultUnitInstances == null)
                {
                    _defaultUnitInstances = Assembly
                        .GetExecutingAssembly()
                        .GetTypes()
                        .Where(t => t.IsSubclassOf(typeof(UnitPivot)) && !t.IsAbstract)
                        .Select(t => (BuildablePivot)t.GetField("Default", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null))
                        .ToList();
                    
                    _defaultUnitInstances.AddRange(CityImprovementPivot.Instances);
                    _defaultUnitInstances.AddRange(WonderPivot.Instances);
                    // TODO : spaceship instances.
                }

                return _defaultUnitInstances;
            }
        }
    }
}
