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
        private Engine _generatedEngine = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MapGeneratorWindow()
        {
            InitializeComponent();
            ShowAgain();
            ComboLandRatio.ItemsSource = Enum.GetValues(typeof(MapPivot.SizePivot));
            ComboLandType.ItemsSource = Enum.GetValues(typeof(MapPivot.LandShapePivot));
            ComboSize.ItemsSource = Enum.GetValues(typeof(MapPivot.LandCoveragePivot));
            ComboTemperature.ItemsSource = Enum.GetValues(typeof(MapPivot.TemperaturePivot));
            ComboLandRatio.SelectedValue = MapPivot.SizePivot.Medium;
            ComboLandType.SelectedValue = MapPivot.LandShapePivot.Continent;
            ComboSize.SelectedValue = MapPivot.LandCoveragePivot.Medium;
            ComboTemperature.SelectedValue = MapPivot.TemperaturePivot.Temperate;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (ComboSize.SelectedIndex < 0 || ComboLandType.SelectedIndex < 0
                || ComboLandRatio.SelectedIndex < 0 || ComboTemperature.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a value for each parameter !");
                return;
            }

            GridForm.Visibility = Visibility.Collapsed;
            ProgressBarLoading.Visibility = Visibility.Visible;

            BackgroundWorker bgw = new BackgroundWorker
            {
                WorkerSupportsCancellation = false,
                WorkerReportsProgress = false
            };
            bgw.DoWork += GenerateMapBackground;
            bgw.RunWorkerCompleted += EndOfMapGeneration;
            bgw.RunWorkerAsync(new object[]
            {
                ComboSize.SelectedItem,
                ComboLandType.SelectedItem,
                ComboLandRatio.SelectedItem,
                ComboTemperature.SelectedItem
            });
        }

        private void GenerateMapBackground(object sender, DoWorkEventArgs e)
        {
            try
            {
                object[] parameters = e.Argument as object[];

                _generatedEngine = new Engine((MapPivot.SizePivot)parameters[0], (MapPivot.LandShapePivot)parameters[1],
                    (MapPivot.LandCoveragePivot)parameters[2], (MapPivot.TemperaturePivot)parameters[3]);
            }
            catch (Exception ex)
            {
                e.Result = ex;
            }
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
                new MainWindow(_generatedEngine).ShowDialog();
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
