using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using ErsatzCivLib;
using ErsatzCivLib.Model;

namespace ErsatzCiv
{
    /// <summary>
    /// Interaction logic for the window.
    /// </summary>
    public partial class CityWindow : Window
    {
        private const int CITY_GRID_SIZE = 75;
        private const int CITIZEN_SIZE = 25;
        private const int COUNT_SHOW_CITY_SQUARES = 7; // ODD NUMBER !

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

            for (int i = 0; i < COUNT_SHOW_CITY_SQUARES; i++)
            {
                GridCityMap.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(CITY_GRID_SIZE) });
                GridCityMap.RowDefinitions.Add(new RowDefinition { Height = new GridLength(CITY_GRID_SIZE) });
            }

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
                StackCitizens.Children.Add(DrawCitizen(citizen, CITIZEN_SIZE));
            }

            ListBoxImprovements.ItemsSource = _city.ImprovementsAndWonders;

            GridCityMap.Children.Clear();
            var gridOffset = new Tuple<int, int>(
                _city.MapSquareLocation.Row - ((COUNT_SHOW_CITY_SQUARES - 1) / 2),
                _city.MapSquareLocation.Column - ((COUNT_SHOW_CITY_SQUARES - 1) / 2)
            );
            var squares = _engine.GetMapSquaresAroundCity(_city);
            for (int i = 0; i < COUNT_SHOW_CITY_SQUARES; i++)
            {
                for (var j = 0; j < COUNT_SHOW_CITY_SQUARES; j++)
                {
                    DrawCitySquare(gridOffset, squares, i, j);
                }
            }
        }

        private static Image DrawCitizen(CityPivot.CitizenPivot citizen, int size)
        {
            string resourceName = (citizen.Type == CityPivot.CitizenTypePivot.Regular ?
                                citizen.Mood.ToString() : citizen.Type.ToString());
            var imgCitizen = new Image
            {
                Width = size,
                Height = size,
                Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(Properties.Settings.Default.datasPath + "citizen_" + resourceName.ToLowerInvariant() + ".png")),
                ToolTip = resourceName
            };
            return imgCitizen;
        }

        private void DrawCitySquare(Tuple<int, int> gridOffset, Dictionary<MapSquarePivot, Tuple<CityPivot.CitizenPivot, bool>> squares, int i, int j)
        {
            var current = squares.Keys.SingleOrDefault(msq => msq.Row == (i + gridOffset.Item1) && msq.Column == (j + gridOffset.Item2));
            if (current == null)
            {
                GridCityMap.Children.Add(DrawDisableSquare(i, j, 1, 1));
            }
            else
            {
                GridCityMap.DrawSingleMapSquare(CITY_GRID_SIZE, current, false, gridOffset);
                if (i + gridOffset.Item1 == _city.MapSquareLocation.Row
                    && j + gridOffset.Item2 == _city.MapSquareLocation.Column)
                {
                    GridCityMap.DrawMapCity(_city, CITY_GRID_SIZE, 5, true, gridOffset, MouseClickOnCity);
                }
                else if (IsOutOfCityBounds(gridOffset, i, j) || squares[current].Item2)
                {
                    GridCityMap.Children.Add(DrawDisableSquare(i, j, 5, 0.4));
                }

                if (squares[current].Item1 != null)
                {
                    var imgCitizen = DrawCitizen(squares[current].Item1, CITIZEN_SIZE);
                    imgCitizen.SetValue(Panel.ZIndexProperty, 2);
                    imgCitizen.SetValue(Grid.RowProperty, i);
                    imgCitizen.SetValue(Grid.ColumnProperty, j);
                    GridCityMap.Children.Add(imgCitizen);
                }
            }
        }

        private bool IsOutOfCityBounds(Tuple<int, int> gridOffset, int i, int j)
        {
            return (i + gridOffset.Item1 < _city.MapSquareLocation.Row - 2)
                                        || (i + gridOffset.Item1 > _city.MapSquareLocation.Row + 2)
                                        || (j + gridOffset.Item2 < _city.MapSquareLocation.Column - 2)
                                        || (j + gridOffset.Item2 > _city.MapSquareLocation.Column + 2)
                                        || (i + gridOffset.Item1 == _city.MapSquareLocation.Row - 2 && j + gridOffset.Item2 == _city.MapSquareLocation.Column - 2)
                                        || (i + gridOffset.Item1 == _city.MapSquareLocation.Row - 2 && j + gridOffset.Item2 == _city.MapSquareLocation.Column + 2)
                                        || (i + gridOffset.Item1 == _city.MapSquareLocation.Row + 2 && j + gridOffset.Item2 == _city.MapSquareLocation.Column - 2)
                                        || (i + gridOffset.Item1 == _city.MapSquareLocation.Row + 2 && j + gridOffset.Item2 == _city.MapSquareLocation.Column + 2);
        }

        private static Rectangle DrawDisableSquare(int i, int j, int zindex, double opacity)
        {
            var children = new Rectangle
            {
                Width = CITY_GRID_SIZE,
                Height = CITY_GRID_SIZE,
                Stroke = System.Windows.Media.Brushes.Black,
                StrokeThickness = 2,
                Fill = System.Windows.Media.Brushes.Black,
                Opacity = opacity
            };
            children.SetValue(Grid.RowProperty, i);
            children.SetValue(Grid.ColumnProperty, j);
            children.SetValue(Panel.ZIndexProperty, zindex);
            return children;
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

        private void MouseClickOnCity(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                _engine.ResetCitizens(_city);
                RefreshDisplay();
            }
        }
    }
}
