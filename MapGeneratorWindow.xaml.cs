using System;
using System.ComponentModel;
using System.Windows;
using ErsatzCivLib;
using ErsatzCivLib.Model;

namespace ErsatzCiv
{
    /// <summary>
    /// Interaction logic for the map generator window.
    /// </summary>
    public partial class MapGeneratorWindow : Window
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MapGeneratorWindow()
        {
            InitializeComponent();

            ShowAgain();

            ComboBoxSize.ItemsSource = Enum.GetValues(typeof(MapPivot.SizePivot));
            ComboBoxLandShape.ItemsSource = Enum.GetValues(typeof(MapPivot.LandShapePivot));
            ComboBoxLandCoverage.ItemsSource = Enum.GetValues(typeof(MapPivot.LandCoveragePivot));
            ComboBoxTemperature.ItemsSource = Enum.GetValues(typeof(MapPivot.TemperaturePivot));

            ComboBoxSize.SelectedValue = MapPivot.SizePivot.Medium;
            ComboBoxLandShape.SelectedValue = MapPivot.LandShapePivot.Continent;
            ComboBoxLandCoverage.SelectedValue = MapPivot.LandCoveragePivot.Medium;
            ComboBoxTemperature.SelectedValue = MapPivot.TemperaturePivot.Temperate;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBoxSize.SelectedIndex < 0 || ComboBoxLandShape.SelectedIndex < 0
                || ComboBoxLandCoverage.SelectedIndex < 0 || ComboBoxTemperature.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a value for each parameter !");
                return;
            }

            GridForm.Visibility = Visibility.Collapsed;
            ProgressBarLoading.Visibility = Visibility.Visible;

            var bgw = new BackgroundWorker
            {
                WorkerSupportsCancellation = false,
                WorkerReportsProgress = false
            };
            bgw.DoWork += GenerateMapBackground;
            bgw.RunWorkerCompleted += EndOfMapGeneration;
            bgw.RunWorkerAsync(new object[]
            {
                ComboBoxSize.SelectedItem,
                ComboBoxLandShape.SelectedItem,
                ComboBoxLandCoverage.SelectedItem,
                ComboBoxTemperature.SelectedItem
            });
        }

        private void GenerateMapBackground(object sender, DoWorkEventArgs e)
        {
            object[] parameters = e.Argument as object[];

            e.Result = new Engine((MapPivot.SizePivot)parameters[0], (MapPivot.LandShapePivot)parameters[1],
                (MapPivot.LandCoveragePivot)parameters[2], (MapPivot.TemperaturePivot)parameters[3]);
        }

        private void EndOfMapGeneration(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                ShowAgain();
                MessageBox.Show($"The following error occurs : {e.Error.Message}");
            }
            else
            {
                ShowAgain();
                Hide();
                new MainWindow(e.Result as Engine).ShowDialog();
                ShowDialog();
            }
        }

        private void ShowAgain()
        {
            ProgressBarLoading.Visibility = Visibility.Collapsed;
            GridForm.Visibility = Visibility.Visible;
        }
    }
}
