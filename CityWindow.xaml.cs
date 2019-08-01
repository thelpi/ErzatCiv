using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ErsatzCivLib;
using ErsatzCivLib.Model;

namespace ErsatzCiv
{
    /// <summary>
    /// Interaction logic for the window.
    /// </summary>
    public partial class CityWindow : Window
    {
        private readonly Engine _engine;
        private readonly CityPivot _city;
        private bool _checkComboSelection = false;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="engine">The <see cref="Engine"/>.</param>
        /// <param name="city">The <see cref="CityPivot"/> to manage.</param>
        public CityWindow(Engine engine, CityPivot city)
        {
            InitializeComponent();

            _engine = engine;
            _city = city;

            RefreshDisplay();
        }

        private void RefreshDisplay()
        {
            LabelCityInfos.Content = $"{_city.Name}, founded in {_city.CreationTurn}";
            LabelCommerceStat.Content = _city.Commerce;
            LabelFoodStat.Content = _city.Food;
            LabelPollutionStat.Content = _city.Pollution;
            LabelProductionStat.Content = _city.Productivity;
            LabelScienceStat.Content = _city.Science;
            LabelTaxStat.Content = _city.Tax;

            var buildableItems = _engine.BuildableItems().ToList();
            ComboBoxProduction.ItemsSource = buildableItems.Select(x => new KeyValuePair<string, Type>(x == null ? "Capitalization" : x.Name.Replace("Pivot", string.Empty), x));
            ComboBoxProduction.SelectedIndex = buildableItems.IndexOf(_city.Production?.GetType());

            var foodCost = CityPivot.CitizenPivot.FOOD_BY_TURN * _city.Citizens.Count;
            var nextFood = CityPivot.FOOD_RATIO_TO_NEXT_CITIZEN * _city.Citizens.Count;
            var ratioGrowth = (nextFood - _city.FoodStorage) / _city.Food;
            PanelNextCitizen.Children.Clear();
            PanelNextCitizen.Children.Add(new Label
            {
                Content = $"Food informations",
                FontSize = 12,
                Foreground = System.Windows.Media.Brushes.Red
            });
            PanelNextCitizen.Children.Add(new Label
            {
                Content = $"Stock : {_city.FoodStorage}"
            });
            PanelNextCitizen.Children.Add(new Label
            {
                Content = $"Next : {nextFood}"
            });
            PanelNextCitizen.Children.Add(new Label
            {
                Content = "Status : " + (foodCost < _city.Food ? "Growth (" + ratioGrowth + " turns)" : (foodCost == _city.Food ? "Balanced" : "Famine (" + ratioGrowth + " turns)"))
            });

            var prodFinish = _city.Production == null ? 0 : _city.Production.ProductivityCost;
            PanelNextProduction.Children.Clear();
            PanelNextProduction.Children.Add(new Label
            {
                Content = $"Production informations",
                FontSize = 12,
                Foreground = System.Windows.Media.Brushes.Red
            });
            PanelNextProduction.Children.Add(new Label
            {
                Content = $"Produced : {_city.ProductivityStorage}"
            });
            PanelNextProduction.Children.Add(new Label
            {
                Content = $"Finish : {prodFinish}"
            });
            PanelNextProduction.Children.Add(new Label
            {
                Content = $"Remaining : {prodFinish - _city.ProductivityStorage} (" + (prodFinish - _city.ProductivityStorage) / _city.Productivity + " turns)"
            });

            StackCitizens.Children.Clear();
            foreach (var citizen in _city.Citizens)
            {
                string resourceName = (citizen.Type == CityPivot.CitizenTypePivot.Regular ?
                    citizen.Mood.ToString() : citizen.Type.ToString());
                StackCitizens.Children.Add(new Image
                {
                    Width = 25,
                    Height = 25,
                    Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(Properties.Settings.Default.datasPath + "citizen_" + resourceName.ToLowerInvariant() + ".png")),
                    ToolTip = resourceName
                });
            }

            ListBoxImprovements.ItemsSource = _city.ImprovementsAndWonders;

            // GridCityMap
        }

        private void ComboBoxProduction_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_checkComboSelection)
            {
                return;
            }
            _engine.ChangeCityProduction(_city, ((KeyValuePair<string, Type>)ComboBoxProduction.SelectedItem).Value);
            RefreshDisplay();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _checkComboSelection = true;
        }
    }
}
