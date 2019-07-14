namespace ErsatzCiv.Model
{
    /// <summary>
    /// Available actions on a <see cref="MapSquareTypeData"/>
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
        /// Transforms jungle into plain, and forest into grassland or toundra.
        /// </remarks>
        Clear,
        /// <summary>
        /// Plants a forest.
        /// </summary>
        Plant
    }
}
