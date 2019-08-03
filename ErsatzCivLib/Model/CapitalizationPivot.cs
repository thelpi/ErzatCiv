namespace ErsatzCivLib.Model
{
    /// <summary>
    /// Capitalization (city production).
    /// </summary>
    /// <seealso cref="BuildablePivot"/>
    public class CapitalizationPivot : BuildablePivot
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="mapSquare">Not used.</param>
        internal CapitalizationPivot(MapSquarePivot mapSquare) : base("Capitalization", 0)
        {

        }
    }
}
