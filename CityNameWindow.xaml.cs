using System.Windows;
using ErsatzCivLib;
using ErsatzCivLib.Model;

namespace ErsatzCiv
{
    /// <summary>
    /// Interaction logic of the window.
    /// </summary>
    public partial class CityNameWindow : Window
    {
        private readonly Engine _engine;

        /// <summary>
        /// The city built; <c>Null</c> by default.
        /// </summary>
        public CityPivot City { get; private set; }
        /// <summary>
        /// The unit used to built the city.
        /// </summary>
        public UnitPivot UnitUsed { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="engine">The <see cref="Engine"/>.</param>
        public CityNameWindow(Engine engine)
        {
            InitializeComponent();
            _engine = engine;
            City = null;
            TextBoxCityName.Text = _engine.HumanPlayer.GetNextCityName;
        }

        private void ButtonValidate_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TextBoxCityName.Text))
            {
                UnitUsed = _engine.HumanPlayer.CurrentUnit; // important because "BuildCity()" changes the "CurrentUnit" value !
                City = _engine.BuildCity(TextBoxCityName.Text.Trim(), out bool nonUniquenameError);
                if (nonUniquenameError)
                {
                    MessageBox.Show("Please specify a unique city name.", "ErsatzCiv");
                }
                else
                {
                    Close();
                }
            }
            else
            {
                MessageBox.Show("Please specify a non-empty city name.", "ErsatzCiv");
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
