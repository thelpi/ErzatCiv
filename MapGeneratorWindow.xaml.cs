using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using ErsatzCivLib;
using ErsatzCivLib.Model.Enums;
using ErsatzCivLib.Model.Static;

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

            ComboBoxSize.ItemsSource = Enum.GetValues(typeof(SizePivot));
            ComboBoxLandShape.ItemsSource = Enum.GetValues(typeof(LandShapePivot));
            ComboBoxLandCoverage.ItemsSource = Enum.GetValues(typeof(LandCoveragePivot));
            ComboBoxTemperature.ItemsSource = Enum.GetValues(typeof(TemperaturePivot));
            ComboBoxAge.ItemsSource = Enum.GetValues(typeof(AgePivot));
            ComboBoxHumidity.ItemsSource = Enum.GetValues(typeof(HumidityPivot));
            ComboBoxCivilization.ItemsSource = CivilizationPivot.Instances;

            ComboBoxSize.SelectedValue = SizePivot.Medium;
            ComboBoxLandShape.SelectedValue = LandShapePivot.Continent;
            ComboBoxLandCoverage.SelectedValue = LandCoveragePivot.Medium;
            ComboBoxTemperature.SelectedValue = TemperaturePivot.Temperate;
            ComboBoxAge.SelectedValue = AgePivot.Average;
            ComboBoxHumidity.SelectedValue = HumidityPivot.Average;
            ComboBoxCivilization.SelectedValue = CivilizationPivot.French;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBoxSize.SelectedIndex < 0 || ComboBoxLandShape.SelectedIndex < 0
                || ComboBoxLandCoverage.SelectedIndex < 0 || ComboBoxTemperature.SelectedIndex < 0
                || ComboBoxAge.SelectedIndex < 0 || ComboBoxHumidity.SelectedIndex < 0
                || ComboBoxCivilization.SelectedIndex < 0 || ComboBoxIaPlayersCount.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a value for each parameter !", "ErsatzCiv");
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
                ComboBoxTemperature.SelectedItem,
                ComboBoxAge.SelectedItem,
                ComboBoxHumidity.SelectedItem,
                ComboBoxCivilization.SelectedItem,
                ComboBoxIaPlayersCount.SelectedItem
            });
        }

        private void GenerateMapBackground(object sender, DoWorkEventArgs e)
        {
            object[] parameters = e.Argument as object[];

            e.Result = new Engine((SizePivot)parameters[0], (LandShapePivot)parameters[1],
                (LandCoveragePivot)parameters[2], (TemperaturePivot)parameters[3],
                (AgePivot)parameters[4], (HumidityPivot)parameters[5],
                (CivilizationPivot)parameters[6], (int)parameters[7]);
        }

        private void EndOfMapGeneration(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                ShowAgain();
                MessageBox.Show($"The following error has occurred : {e.Error.Message}", "ErsatzCiv");
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

        private void ComboBoxSize_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var count = CivilizationPivot.Instances.Count;
            if (ComboBoxSize.SelectedItem != null)
            {
                count /= (6 - (int)ComboBoxSize.SelectedItem);
            }

            ComboBoxIaPlayersCount.ItemsSource = Enumerable.Range(0, count);
            ComboBoxIaPlayersCount.SelectedValue = 0;
        }
    }
}
