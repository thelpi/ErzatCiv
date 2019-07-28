namespace ErsatzCivLib.Model
{
    public class CityPivot
    {
        public const double DISPLAY_RATIO = 0.8;
        internal const int CITY_SPEED_COST = 1;

        private const string CITY_RENDER_PATH = "city.png";

        public int Row { get; private set; }
        public int Column { get; private set; }
        public string RenderValue { get; private set; }

        internal CityPivot(int row, int column)
        {
            Row = row;
            Column = column;
            RenderValue = CITY_RENDER_PATH;
        }
    }
}
