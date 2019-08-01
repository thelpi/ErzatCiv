using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ErsatzCivLib;
using ErsatzCivLib.Model;

namespace ErsatzCiv
{
    /// <summary>
    /// Interaction logic for the window.
    /// </summary>
    public partial class CityWindow : Window
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="engine">The <see cref="Engine"/>.</param>
        /// <param name="city">The <see cref="CityPivot"/> to manage.</param>
        public CityWindow(Engine engine, CityPivot city)
        {
            InitializeComponent();
            LabelCityInfos.Content = $"{city.Name}, founded in {city.CreationTurn}";
            LabelCommerceStat.Content = city.Commerce;
            LabelFoodStat.Content = city.Food;
            LabelPollutionStat.Content = city.Pollution;
            LabelProductionStat.Content = city.Productivity;
            LabelScienceStat.Content = city.Science;
            LabelTaxStat.Content = city.Tax;

            var buildableItems = engine.BuildableItems().ToList();
            ComboBoxProduction.ItemsSource = buildableItems.Select(x => new KeyValuePair<string, Type>(x == null ? "Capitalization" : x.Name.Replace("Pivot", string.Empty), x));
            ComboBoxProduction.SelectedIndex = buildableItems.IndexOf(city.Production?.GetType());
            // StackCitizens
            // WrapPanelNextCitizen
            // WrapPanelNextProduction
            // GridCityMap
            // ListBoxImprovements
        }
    }
}
