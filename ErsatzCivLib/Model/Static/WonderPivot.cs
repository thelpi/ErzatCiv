using System;
using System.Collections.Generic;

namespace ErsatzCivLib.Model.Static
{
    /// <summary>
    /// Represents a wonder.
    /// Wonders are similar to <see cref="CityImprovementPivot"/>, but can't be built more than once overall.
    /// </summary>
    /// <seealso cref="BuildablePivot"/>
    /// <seealso cref="IEquatable{T}"/>
    [Serializable]
    public class WonderPivot : BuildablePivot, IEquatable<WonderPivot>
    {
        private WonderPivot(int productivityCost, AdvancePivot advancePrerequisite, AdvancePivot advanceObsolescence,
            string name, bool hasCitizenHappinessEffect) :
            base(productivityCost, advancePrerequisite, advanceObsolescence, -1, name, hasCitizenHappinessEffect)
        { }

        #region IEquatable implementation

        /// <summary>
        /// Checks if this instance is equal to another one.
        /// </summary>
        /// <param name="other">The other instance.</param>
        /// <returns><c>True</c> if equals; <c>False</c> otherwise.</returns>
        public bool Equals(WonderPivot other)
        {
            return Name == other?.Name;
        }

        /// <summary>
        /// Operator "==" override. Checks equality between two instances.
        /// </summary>
        /// <param name="w1">The first <see cref="WonderPivot"/>.</param>
        /// <param name="w2">The second <see cref="WonderPivot"/>.</param>
        /// <returns><c>True</c> if equals; <c>False</c> otherwise.</returns>
        public static bool operator ==(WonderPivot w1, WonderPivot w2)
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
        /// <param name="w">A <see cref="WonderPivot"/> instance.</param>
        /// <param name="any">Any <see cref="BuildablePivot"/> instance.</param>
        /// <returns><c>True</c> if equals; <c>False</c> otherwise.</returns>
        public static bool operator ==(WonderPivot w, BuildablePivot any)
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
        /// <param name="w1">The first <see cref="WonderPivot"/>.</param>
        /// <param name="w2">The second <see cref="WonderPivot"/>.</param>
        /// <returns><c>False</c> if equals; <c>True</c> otherwise.</returns>
        public static bool operator !=(WonderPivot w1, WonderPivot w2)
        {
            return !(w1 == w2);
        }

        /// <summary>
        /// Operator "!=" override. Checks non-equality between two instances.
        /// </summary>
        /// <param name="w">A <see cref="WonderPivot"/> instance.</param>
        /// <param name="any">Any <see cref="BuildablePivot"/> instance.</param>
        /// <returns><c>False</c> if equals; <c>True</c> otherwise.</returns>
        public static bool operator !=(WonderPivot w, BuildablePivot any)
        {
            return !(w == any);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is WonderPivot && Equals(obj as WonderPivot);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        #endregion

        #region Static instances

        /// <summary>
        /// Apollo Program <see cref="WonderPivot"/> instance.
        /// </summary>
        public static readonly WonderPivot ApolloProgram = new WonderPivot(600, AdvancePivot.SpaceFlight, null, "Apollo Program", false);
        /// <summary>
        /// Colossus <see cref="WonderPivot"/> instance.
        /// </summary>
        public static readonly WonderPivot Colossus = new WonderPivot(200, AdvancePivot.BronzeWorking, AdvancePivot.Electricity, "Colossus", false);
        /// <summary>
        /// Copernicus <see cref="WonderPivot"/> instance.
        /// </summary>
        public static readonly WonderPivot CopernicusObservatory = new WonderPivot(300, AdvancePivot.Astronomy, AdvancePivot.Automobile, "Copernicus' Observatory", false);
        /// <summary>
        /// Cure for Cancer <see cref="WonderPivot"/> instance.
        /// </summary>
        public static readonly WonderPivot CureForCancer = new WonderPivot(600, AdvancePivot.GeneticEngineering, null, "Cure for Cancer", true);
        /// <summary>
        /// Darwin's Voyage <see cref="WonderPivot"/> instance.
        /// </summary>
        public static readonly WonderPivot DarwinVoyage = new WonderPivot(300, AdvancePivot.Railroad, null, "Darwin's Voyage", false);
        /// <summary>
        /// Great Library <see cref="WonderPivot"/> instance.
        /// </summary>
        public static readonly WonderPivot GreatLibrary = new WonderPivot(300, AdvancePivot.Literacy, AdvancePivot.University, "Great Library", false);
        /// <summary>
        /// Great Wall <see cref="WonderPivot"/> instance.
        /// </summary>
        public static readonly WonderPivot GreatWall = new WonderPivot(300, AdvancePivot.Masonry, AdvancePivot.Gunpowder, "Great Wall", false);
        /// <summary>
        /// Hanging Gardens <see cref="WonderPivot"/> instance.
        /// </summary>
        public static readonly WonderPivot HangingGardens = new WonderPivot(300, AdvancePivot.Pottery, AdvancePivot.Invention, "Hanging Gardens", true);
        /// <summary>
        /// Hoover Dam <see cref="WonderPivot"/> instance.
        /// </summary>
        public static readonly WonderPivot HooverDam = new WonderPivot(300, AdvancePivot.Electronics, null, "Hoover Dam", false);
        /// <summary>
        /// Isaac Newton's College <see cref="WonderPivot"/> instance.
        /// </summary>
        public static readonly WonderPivot IsaacNewtonCollege = new WonderPivot(400, AdvancePivot.TheoryOfGravity, AdvancePivot.NuclearFission, "Isaac Newton's College", false);
        /// <summary>
        /// J.S. Bach's Cathedral <see cref="WonderPivot"/> instance.
        /// </summary>
        public static readonly WonderPivot JsBachsCathedral = new WonderPivot(400, AdvancePivot.Religion, null, "J.S. Bach's Cathedral", true);
        /// <summary>
        /// Lighthouse <see cref="WonderPivot"/> instance.
        /// </summary>
        public static readonly WonderPivot Lighthouse = new WonderPivot(200, AdvancePivot.MapMaking, AdvancePivot.Magnetism, "Lighthouse", false);
        /// <summary>
        /// Magellan's Expedition <see cref="WonderPivot"/> instance.
        /// </summary>
        public static readonly WonderPivot MagellanExpedition = new WonderPivot(400, AdvancePivot.Navigation, null, "Magellan's Expedition", false);
        /// <summary>
        /// Manhattan Project <see cref="WonderPivot"/> instance.
        /// </summary>
        public static readonly WonderPivot ManhattanProject = new WonderPivot(600, AdvancePivot.NuclearFission, null, "Manhattan Project", false);
        /// <summary>
        /// Michelangelo's Chapel <see cref="WonderPivot"/> instance.
        /// </summary>
        public static readonly WonderPivot MichelangeloChapel = new WonderPivot(300, AdvancePivot.Religion, AdvancePivot.Communism, "Michelangelo's Chapel", true);
        /// <summary>
        /// Oracle <see cref="WonderPivot"/> instance.
        /// </summary>
        public static readonly WonderPivot Oracle = new WonderPivot(300, AdvancePivot.Mysticism, AdvancePivot.Recycling, "Oracle", true);
        /// <summary>
        /// Pyramids <see cref="WonderPivot"/> instance.
        /// </summary>
        public static readonly WonderPivot Pyramids = new WonderPivot(300, AdvancePivot.Masonry, AdvancePivot.Communism, "Pyramids", false);
        /// <summary>
        /// SETI Program <see cref="WonderPivot"/> instance.
        /// </summary>
        public static readonly WonderPivot SetiProgram = new WonderPivot(600, AdvancePivot.Computers, null, "SETI Program", false);
        /// <summary>
        /// Shakespeare's Theatre <see cref="WonderPivot"/> instance.
        /// </summary>
        public static readonly WonderPivot ShakespeareTheatre = new WonderPivot(400, AdvancePivot.Medicine, AdvancePivot.Electronics, "Shakespeare's Theatre", true);
        /// <summary>
        /// United Nations <see cref="WonderPivot"/> instance.
        /// </summary>
        public static readonly WonderPivot UnitedNations = new WonderPivot(600, AdvancePivot.Communism, null, "United Nations", false);
        /// <summary>
        /// Women's Suffrage <see cref="WonderPivot"/> instance.
        /// </summary>
        public static readonly WonderPivot WomensSuffrage = new WonderPivot(600, AdvancePivot.Industrialization, null, "Women's Suffrage", true);

        #endregion

        private static List<WonderPivot> _instances = null;
        /// <summary>
        /// List of every <see cref="WonderPivot"/> instances.
        /// </summary>
        public static IReadOnlyCollection<WonderPivot> Instances
        {
            get
            {
                if (_instances == null)
                {
                    _instances = Tools.GetInstancesOfTypeFromStaticFields<WonderPivot>();
                }
                return _instances;
            }
        }
    }
}
