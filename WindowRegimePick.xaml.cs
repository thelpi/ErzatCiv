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
        private EnginePivot _engine;

        public WindowRegimePick(EnginePivot engine)
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
                MessageBox.Show($"Your civilization is now a {_engine.HumanPlayer.GetCurrentRegime().Name} !", "ErsatzCiv");
            }
            else
            {
                e.Cancel = true;
                MessageBox.Show("Please select a new regime !", "ErsatzCiv");
            }
        }
    }
}
