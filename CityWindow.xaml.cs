using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
        private const int CITIZEN_SIZE_TOPBAR = 25;
        private const int CITIZEN_SIZE_CITYGRID = 35;
        private const int COUNT_SHOW_CITY_SQUARES = 7; // ODD NUMBER !

        private readonly EnginePivot _engine;
        private readonly CityPivot _city;
        private bool _checkComboSelection = false;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="engine">The <see cref="EnginePivot"/>.</param>
        /// <param name="city">The <see cref="CityPivot"/> to manage.</param>
        public CityWindow(EnginePivot engine, CityPivot city)
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
            LabelTreasureStat.Content = _city.Treasure;
            LabelFoodStat.Content = _city.Food;
            LabelPollutionStat.Content = _city.Pollution;
            LabelProductionStat.Content = _city.Productivity;
            LabelScienceStat.Content = _city.Science;
            
            ComboBoxProduction.ItemsSource = _engine.GetBuildableItemsForCity(_city, out int indexOfDefault);
            ComboBoxProduction.SelectedIndex = indexOfDefault;
            
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
                Content = $"Next : {_city.NextCitizenFoodRequirement}"
            });
            PanelNextCitizen.Children.Add(new Label
            {
                Content = $"Status : {_city.FoodStatus}"
            });
            
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
                Content = $"Finish : {_city.Production.ProductivityCost}"
            });
            PanelNextProduction.Children.Add(new Label
            {
                Content = $"Remaining : {_city.RemainingProductionCost} (" + _city.RemainingProductionTurns + " turns)"
            });

            StackCitizens.Children.Clear();
            foreach (var citizen in _city.Citizens)
            {
                StackCitizens.Children.Add(DrawCitizen(citizen, CITIZEN_SIZE_TOPBAR, 1, MouseClickOnCitizen));
            }

            ListBoxImprovements.ItemsSource = new List<BuildablePivot>(_city.Improvements).Concat(_city.Wonders);

            GridCityMap.Children.Clear();
            var gridOffset = new Tuple<int, int>(
                _city.MapSquareLocation.Row - ((COUNT_SHOW_CITY_SQUARES - 1) / 2),
                _city.MapSquareLocation.Column - ((COUNT_SHOW_CITY_SQUARES - 1) / 2)
            );

            foreach (var sq in _engine.GetMapSquaresAroundCity(_city))
            {
                DrawCitySquare(gridOffset, sq.Key, sq.Value.Item1, sq.Value.Item2,
                    sq.Key.Row - gridOffset.Item1, sq.Key.Column - gridOffset.Item2);
            }
        }

        private static Image DrawCitizen(CitizenPivot citizen, int size, double opacity,
            Action<object, MouseButtonEventArgs> MouseLeftButtonCallback)
        {
            Style style = new Style
            {
                TargetType = typeof(Image)
            };
            style.Setters.Add(new Setter(OpacityProperty, opacity));

            if (opacity < 1)
            {
                var trigger = new Trigger
                {
                    Property = IsMouseOverProperty,
                    Value = true
                };
                trigger.Setters.Add(new Setter(OpacityProperty, Convert.ToDouble(1)));
                style.Triggers.Add(trigger);
            }

            var imgCitizen = new Image
            {
                Width = size,
                Height = size,
                Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(Properties.Settings.Default.datasPath + "citizens\\" + citizen.ToString().ToLowerInvariant() + ".png")),
                ToolTip = citizen.ToString(),
                Style = style,
                Stretch = System.Windows.Media.Stretch.Uniform,
                Tag = citizen
            };

            if (MouseLeftButtonCallback != null)
            {
                imgCitizen.MouseLeftButtonDown += new MouseButtonEventHandler(MouseLeftButtonCallback);
            }

            return imgCitizen;
        }

        private void DrawCitySquare(Tuple<int, int> gridOffset, MapSquarePivot current,
            CitizenPivot citizen, bool occupiedByOtherCity, int row, int column)
        {
            if (current == null)
            {
                GridCityMap.Children.Add(DrawDisableSquare(row, column, 1, 1));
            }
            else
            {
                bool isCityRadius = _city.CoordinatesAreCityRadius(row + gridOffset.Item1, column + gridOffset.Item2);
                bool isCity = _city.CoordinatesAreCityCenter(row + gridOffset.Item1, column + gridOffset.Item2);

                Action<object, MouseButtonEventArgs> callback = MouseClickOnCityGridEmpty;
                if (!isCityRadius || occupiedByOtherCity || isCity || citizen != null)
                {
                    callback = null;
                }

                GridCityMap.DrawSingleMapSquare(CITY_GRID_SIZE, current, true, gridOffset, callback);
                if (isCity)
                {
                    GridCityMap.DrawMapCity(_city, CITY_GRID_SIZE, 5, true, gridOffset, MouseClickOnCity);
                }
                else if (!isCityRadius || occupiedByOtherCity)
                {
                    GridCityMap.Children.Add(DrawDisableSquare(row, column, 5, 0.4));
                }
                else if (citizen != null)
                {
                    var imgCitizen = DrawCitizen(citizen, CITIZEN_SIZE_CITYGRID, 0.6, MouseClickOnCitizen);
                    imgCitizen.SetValue(Panel.ZIndexProperty, 2);
                    imgCitizen.SetValue(Grid.RowProperty, row);
                    imgCitizen.SetValue(Grid.ColumnProperty, column);
                    GridCityMap.Children.Add(imgCitizen);
                }
            }
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
            if (!_checkComboSelection || ComboBoxProduction.SelectedIndex < 0)
            {
                return;
            }
            _engine.ChangeCityProduction(_city, (BuildablePivot)ComboBoxProduction.SelectedItem);
            RefreshDisplay();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _checkComboSelection = true;
        }

        private void MouseClickOnCity(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                _engine.ResetCitizens(_city);
                RefreshDisplay();
            }
        }

        private void MouseClickOnCitizen(object sender, MouseButtonEventArgs e)
        {
            if ((sender as Image)?.Tag is CitizenPivot citizenSource)
            {
                _engine.SwitchCitizenType(citizenSource);
                RefreshDisplay();
            }
        }

        public void MouseClickOnCityGridEmpty(object sender, MouseButtonEventArgs e)
        {
            if ((sender as Rectangle)?.Tag is MapSquarePivot squareSource)
            {
                _engine.ChangeAnySpecialistToRegular(_city, squareSource);
                RefreshDisplay();
            }
        }
    }
}
