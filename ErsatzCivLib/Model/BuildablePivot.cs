using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ErsatzCivLib.Model
{
    /// <summary>
    /// Represents an item a city can build.
    /// </summary>
    /// <remarks>
    /// Each concrete implementation must implement :
    /// - A <c>static internal readonly</c> field "Default" used as a template.
    /// - A <c>static internal</c> method "CreateAtLocation" used to create instances, based on the default template. The method must have a single parameter, of type <see cref="MapSquarePivot"/>.
    /// - Every constructors must be <c>private</c>.
    /// </remarks>
    [Serializable]
    public abstract class BuildablePivot
    {
        /// <summary>
        /// Name.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Productivity amount required to build the item.
        /// </summary>
        public int ProductivityCost { get; private set; }
        /// <summary>
        /// Indicates if the item, when built, has impact on citizens mood.
        /// </summary>
        public bool HasCitizenMoodEffect { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="productivityCost">The <see cref="ProductivityCost"/> value.</param>
        /// <param name="name">Optionnal; the <see cref="Name"/> value.
        /// IF <c>Null</c>, the class name is used without the "Pivot" suffix.</param>
        /// <param name="hasCitizenMoodEffect">Optionnal; the <see cref="HasCitizenMoodEffect"/> value.</param>
        protected BuildablePivot(int productivityCost, string name = null, bool hasCitizenMoodEffect = false)
        {
            Name = name ?? GetType().Name.Replace("Pivot", string.Empty);
            ProductivityCost = productivityCost;
            HasCitizenMoodEffect = hasCitizenMoodEffect;
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

        /// <summary>
        /// Creates a new instance from the current one.
        /// </summary>
        /// <returns>The instance location; might be <c>Null</c> if non-pertinent.</returns>
        internal BuildablePivot CreateInstance(MapSquarePivot location)
        {
            var method = GetType().GetMethod(
                "CreateAtLocation",
                BindingFlags.Static | BindingFlags.NonPublic,
                null,
                new[] { typeof(MapSquarePivot) },
                null);

            return (BuildablePivot)method?.Invoke(null, new[] { location });
        }

        /// <summary>
        /// Gets every <c>Default</c> instances for each concrete type which inherits from <see cref="BuildablePivot"/>.
        /// </summary>
        /// <returns>A collection of <see cref="BuildablePivot"/>, one for each concrete type.</returns>
        internal static IReadOnlyCollection<BuildablePivot> GetEveryDefaultInstances()
        {
            return Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(BuildablePivot)) && !t.IsAbstract)
                .Select(t => (BuildablePivot)t.GetField("Default", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null))
                .ToList();
        }
    }
}
