using System.Windows;
using ErsatzCivLib;
using ErsatzCivLib.Model.Static;

namespace ErsatzCiv
{
    /// <summary>
    /// Logique d'interaction pour WindowRegimePick.xaml
    /// </summary>
    public partial class WindowRegimePick : Window
    {
        private Engine _engine;

        public WindowRegimePick(Engine engine)
        {
            InitializeComponent();
            _engine = engine;
            ComboBoxRegimes.ItemsSource = _engine.HumanPlayer.GetAvailableRegimes();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ComboBoxRegimes.SelectedIndex < 0)
            {
                e.Cancel = true;
                MessageBox.Show("Please select a new regime !", "ErsatzCiv");
                return;
            }
            else if (_engine.ChangeCurrentRegime(ComboBoxRegimes.SelectedItem as RegimePivot))
            {
                MessageBox.Show($"Your civilization is now a {_engine.HumanPlayer.CurrentRegime.Name} !", "ErsatzCiv");
            }
            else
            {
                e.Cancel = true;
                MessageBox.Show("Please select a new regime !", "ErsatzCiv");
            }
        }
    }
}
