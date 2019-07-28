namespace ErsatzCivLib.Model
{
    public class CityPivot : IOnMapPivot
    {
        public const double DISPLAY_RATIO = 0.8;
        internal const int CITY_SPEED_COST = 1;

        private const string CITY_RENDER_PATH = "city.png";

        public string RenderValue { get; private set; }

        internal CityPivot(MapSquarePivot mapSquare) : base(mapSquare.Row, mapSquare.Column)
        {
            RenderValue = CITY_RENDER_PATH;
        }
    }
}
