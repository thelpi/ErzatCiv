namespace ErsatzCiv.Model
{
    public class CityPivot
    {
        public const double DISPLAY_RATIO = 0.8;

        private const string CITY_RENDER_PATH = "city.png";

        public int Row { get; private set; }
        public int Column { get; private set; }
        public string RenderValue { get; private set; }

        public CityPivot(int row, int column)
        {
            Row = row;
            Column = column;
            RenderValue = CITY_RENDER_PATH;
        }
    }
}
