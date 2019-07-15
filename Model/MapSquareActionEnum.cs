namespace ErsatzCiv.Model
{
    /// <summary>
    /// Worker actions on a <see cref="MapSquareTypeData"/>
    /// </summary>
    public enum MapSquareActionEnum
    {
        /// <summary>
        /// Mines.
        /// </summary>
        /// <remarks>
        /// Doubles the productivity value (or makes it 1 if 0).
        /// Not cumulable with <see cref="Irrigate"/>.
        /// </remarks>
        Mine,
        /// <summary>
        /// Irrigates.
        /// </summary>
        /// <remarks>
        /// Doubles the food value (or makes it 1 if 0).
        /// Not cumulable with <see cref="Mine"/>.
        /// </remarks>
        Irrigate,
        /// <summary>
        /// Builds a road.
        /// </summary>
        /// <remarks>
        /// Increase the unit speed (x2), and the commerce by one unit.
        /// </remarks>
        Road,
        /// <summary>
        /// Builds a rail road.
        /// </summary>
        /// <remarks>
        /// Increase the unit speed (no cost), and the commerce and productivity by one unit.
        /// </remarks>
        RailRoad,
        /// <summary>
        /// Clears a forest or a jungle.
        /// </summary>
        /// <remarks>
        /// Transforms a <see cref="MapSquareData"/> into its <see cref="MapSquareData._underlyingType"/>.
        /// </remarks>
        Clear,
        /// <summary>
        /// Plants a forest.
        /// </summary>
        /// <remarks>
        /// Changes the <see cref="MapSquareTypeData"/> when applied on a <see cref="MapSquareData"/>.
        /// </remarks>
        Plant,
        /// <summary>
        /// Removes road and railroad.
        /// </summary>
        /// <remarks>
        /// Removes the railroad if applicable, then the road.
        /// </remarks>
        DestroyRoad,
        /// <summary>
        /// Removes irrigation system or mine.
        /// </summary>
        DestroyImprovement
    }
}
