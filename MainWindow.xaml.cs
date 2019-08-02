using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ErsatzCiv.Properties;
using ErsatzCivLib;
using ErsatzCivLib.Model;
using ErsatzCivLib.Model.Persistent;

namespace ErsatzCiv
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int DEFAULT_SIZE = 50;
        private const int MENU_HEIGHT = 200;
        private const int UNIT_ZINDEX = 50;
        private const int CITY_ZINDEX = 25;

        private Engine _engine;
        private double _minimapSquareSize;
        private Rectangle _rCapture;
        private bool _freezeActions = false;

        public MainWindow(Engine engine)
        {
            InitializeComponent();
            MenuDock.Height = MENU_HEIGHT + MiniMapCanvas.Margin.Top + MiniMapCanvas.Margin.Bottom;

            _engine = engine;
            CheckBoxWaitTurn.IsChecked = Settings.Default.waitEndTurn;

            InitializeEngineEvents();

            DrawFullMapAndMiniMap();
            RefreshDynamicView();
        }

        #region Events

        private void UpdateSquareMap(object sender, MapSquarePivot.SquareChangedEventArgs evt)
        {
            if (evt?.MapSquare != null)
            {
                DrawSingleMapAndMiniMapSquare(evt.MapSquare, true);
            }
        }

        private void BtnNextTurn_Click(object sender, RoutedEventArgs e)
        {
            if (_freezeActions)
            {
                return;
            }
            _freezeActions = true;
            _engine.NewTurn();
            RefreshDynamicView();
            _freezeActions = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // just in case the event in the constructor doesn't work.
            RecomputeFocus();

            var rWidth = (MapScroller.ActualWidth * (MENU_HEIGHT * _engine.Map.WidthHeighRatio)) / MapGrid.ActualWidth;
            var rHeight = (MapScroller.ActualHeight * MENU_HEIGHT) / MapGrid.ActualHeight;

            _rCapture = new Rectangle
            {
                Fill = Brushes.Transparent,
                Stroke = Brushes.DarkGray,
                StrokeThickness = 2,
                Width = rWidth,
                Height = rHeight
            };
            MiniMapCanvas.Children.Add(_rCapture);

            RefreshMiniMapSelector();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (_freezeActions)
            {
                return;
            }
            _freezeActions = true;

            // Worker actions.
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                switch (e.Key)
                {
                    case Key.M:
                        _engine.WorkerAction(WorkerActionPivot.Mine);
                        break;
                    case Key.I:
                        _engine.WorkerAction(WorkerActionPivot.Irrigate);
                        break;
                    case Key.R:
                        _engine.WorkerAction(WorkerActionPivot.RailRoad);
                        break;
                    case Key.C:
                        _engine.WorkerAction(WorkerActionPivot.Clear);
                        break;
                    case Key.D:
                        _engine.WorkerAction(WorkerActionPivot.DestroyImprovement);
                        break;
                    case Key.F:
                        _engine.WorkerAction(WorkerActionPivot.BuildFortress);
                        break;
                    case Key.P:
                        _engine.WorkerAction(WorkerActionPivot.Plant);
                        break;
                    case Key.A:
                        _engine.WorkerAction(WorkerActionPivot.ClearPollution);
                        break;
                    case Key.X:
                        _engine.WorkerAction(WorkerActionPivot.DestroyRoad);
                        break;
                }
                // Ensures a refresh of the blinking current unit.
                RecomputeFocus();
            }
            // Moves a unit.
            else if (Move(e.Key).HasValue)
            {
                if (_engine.CurrentUnit != null)
                {
                    var unitToMove = _engine.CurrentUnit; // do not remove this line ! ("MoveCurrentUnit()" changes the value of "CurrentUnit")
                    if (_engine.MoveCurrentUnit(Move(e.Key).Value))
                    {
                        var associatedSprites = MapGrid.GetChildrenByTag(unitToMove);
                        if (associatedSprites.Count > 0)
                        {
                            associatedSprites.First().SetValue(Grid.RowProperty, unitToMove.MapSquareLocation.Row);
                            associatedSprites.First().SetValue(Grid.ColumnProperty, unitToMove.MapSquareLocation.Column);
                        }
                    }
                }
            }
            // Buils city.
            else if (e.Key == Key.B)
            {
                if (_engine.CanBuildCity())
                {
                    var windowCity = new CityNameWindow(_engine);
                    windowCity.ShowDialog();
                    if (windowCity.City != null)
                    {
                        new CityWindow(_engine, windowCity.City).ShowDialog();
                        var associatedSprites = MapGrid.GetChildrenByTag(windowCity.UnitUsed);
                        if (associatedSprites.Count > 0)
                        {
                            MapGrid.Children.Remove(associatedSprites.First());

                            DrawMapCity(windowCity.City, true);
                        }
                        // Ensures a refresh of the blinking current unit.
                        RecomputeFocus();
                    }
                }
            }
            // Forces turn for current unit.
            else if (e.Key == Key.Space)
            {
                if (_engine.CurrentUnit != null)
                {
                    _engine.MoveCurrentUnit(null);
                }
                else
                {
                    _engine.NewTurn();
                    RefreshDynamicView();
                }
            }
            // Centers the screen on current unit.
            else if (e.Key == Key.C)
            {
                RecomputeFocus();
            }
            // Goes to next unit.
            else if (e.Key == Key.W)
            {
                _engine.ToNextUnit();
            }

            _freezeActions = false;
        }

        private void MiniMapCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point p = Mouse.GetPosition(MiniMapCanvas);

            var ratioX = p.X / MiniMapCanvas.Width;
            var ratioY = p.Y / MiniMapCanvas.Height;

            var newX = (MapGrid.ActualWidth * ratioX) - (MapScroller.ActualWidth / 2);
            var newY = (MapGrid.ActualHeight * ratioY) - (MapScroller.ActualHeight / 2);

            FocusOn(newX, newY);
        }

        private void MapScroller_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            RefreshMiniMapSelector();
        }

        private void FocusOnUnit(object sender, Engine.NextUnitEventArgs eventArgs)
        {
            if (!eventArgs.MoreUnit && !Settings.Default.waitEndTurn)
            {
                _engine.NewTurn();
                RefreshDynamicView();
            }
            else
            {
                RecomputeFocus();
            }
        }

        private void CheckBoxWaitTurn_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.waitEndTurn = !Settings.Default.waitEndTurn;
            Settings.Default.Save();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var res = MessageBox.Show("Save the game ?", "ErsatzCiv", MessageBoxButton.YesNoCancel);
            if (res == MessageBoxResult.Yes)
            {
                var serRes = _engine.SerializeToFile(Settings.Default.datasPath + Settings.Default.savesSubFolder);
                if (!string.IsNullOrWhiteSpace(serRes))
                {
                    MessageBox.Show($"Save has failed with the following error : {serRes}", "ErsatzCiv");
                    e.Cancel = true;
                }
                else
                {
                    MessageBox.Show("Save done !", "ErsatzCiv");
                }
            }
            else if (res == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
            }
        }

        private void MapGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (e.Source != null && e.Source is FrameworkElement)
                {
                    if ((e.Source as FrameworkElement).Tag is CityPivot sourceCity)
                    {
                        if (_freezeActions)
                        {
                            return;
                        }
                        _freezeActions = true;

                        new CityWindow(_engine, sourceCity).ShowDialog();

                        _freezeActions = false;
                    }
                }
            }
        }

        #endregion

        #region Draw methods

        private void RefreshMiniMapSelector()
        {
            if (_rCapture != null)
            {
                var x = MapScroller.ContentHorizontalOffset;
                var y = MapScroller.ContentVerticalOffset;

                var rX = (x * (MENU_HEIGHT * _engine.Map.WidthHeighRatio)) / MapGrid.ActualWidth;
                var rY = (y * MENU_HEIGHT) / MapGrid.ActualHeight;

                _rCapture.SetValue(Canvas.LeftProperty, rX);
                _rCapture.SetValue(Canvas.TopProperty, rY);
                _rCapture.SetValue(Panel.ZIndexProperty, 2);
            }
        }

        private void DrawFullMapAndMiniMap()
        {
            MiniMapCanvas.Height = MENU_HEIGHT;
            MiniMapCanvas.Width = MENU_HEIGHT * _engine.Map.WidthHeighRatio;

            for (int i = 0; i < _engine.Map.Width; i++)
            {
                MapGrid.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(DEFAULT_SIZE)
                });
            }
            for (int j = 0; j < _engine.Map.Height; j++)
            {
                MapGrid.RowDefinitions.Add(new RowDefinition
                {
                    Height = new GridLength(DEFAULT_SIZE)
                });
            }

            _minimapSquareSize = (double)MENU_HEIGHT / _engine.Map.Height;

            foreach (MapSquarePivot ms in _engine.Map)
            {
                DrawSingleMapAndMiniMapSquare(ms, false);
            }
        }

        private void DrawSingleMapAndMiniMapSquare(MapSquarePivot square, bool cleanPreviousSquare)
        {
            if (cleanPreviousSquare)
            {
                CleanPreviousRenderOnMapAndMiniMap(square);
            }

            FrameworkElement squareRender;
            string imgPath = Settings.Default.datasPath + Settings.Default.squareImageSubFolder + $"{square.Biome.Name}.jpg";
            if (System.IO.File.Exists(imgPath))
            {
                squareRender = new Border
                {
                    Child = new Image
                    {
                        Width = DEFAULT_SIZE,
                        Height = DEFAULT_SIZE,
                        Source = new BitmapImage(new Uri(imgPath)),
                        Stretch = Stretch.Uniform,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    },
                    BorderBrush = Brushes.DarkGray,
                    BorderThickness = new Thickness(1)
                };
            }
            else
            {
                squareRender = new Rectangle
                {
                    Stroke = Brushes.DarkGray,
                    StrokeThickness = 1,
                    Width = DEFAULT_SIZE,
                    Height = DEFAULT_SIZE,
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(square.Biome.ColorValue))
                };
            }

            squareRender.SetValue(Grid.RowProperty, square.Row);
            squareRender.SetValue(Grid.ColumnProperty, square.Column);
            squareRender.Tag = square;
            MapGrid.Children.Add(squareRender);

            Rectangle rctMinimap = new Rectangle
            {
                Width = _minimapSquareSize,
                Height = _minimapSquareSize,
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(square.Biome.ColorValue))
            };
            rctMinimap.SetValue(Canvas.TopProperty, square.Row * _minimapSquareSize);
            rctMinimap.SetValue(Canvas.LeftProperty, square.Column * _minimapSquareSize);
            rctMinimap.Tag = square;
            MiniMapCanvas.Children.Add(rctMinimap);

            if (square.CrossedByRiver)
            {
                DrawSquareRivers(square);
            }

            DrawSquareImprovements(square);
        }

        private void CleanPreviousRenderOnMapAndMiniMap(MapSquarePivot square)
        {
            var elements = MapGrid.GetChildrenByTag(square);
            foreach (var elem in elements)
            {
                MapGrid.Children.Remove(elem);
            }

            var elementsMiniMap = MiniMapCanvas.GetChildrenByTag(square);
            foreach (var elem in elementsMiniMap)
            {
                MiniMapCanvas.Children.Remove(elem);
            }
        }

        private void DrawSquareRivers(MapSquarePivot square)
        {
            if (!square.RiverTopToBottom.HasValue)
            {
                DrawRiver(square, false, false);
                DrawRiver(square, true, false);
                DrawRiver(square, false, true);
                DrawRiver(square, true, true);
            }
            else
            {
                DrawRiver(square, square.RiverTopToBottom.Value, false);
                DrawRiver(square, square.RiverTopToBottom.Value, true);
            }
        }

        private void DrawSquareImprovements(MapSquarePivot square)
        {
            var newElementsWithZIndex = new Dictionary<FrameworkElement, int>();
            if (square.RailRoad)
            {
                DrawRoad(newElementsWithZIndex, true);
            }
            else if (square.Road)
            {
                DrawRoad(newElementsWithZIndex, false);
            }
            if (square.Irrigate)
            {
                DrawIrrigationSystem(newElementsWithZIndex);
            }
            if (square.Mine)
            {
                DrawImageOnSquare(newElementsWithZIndex, 0.5, "mine.png", 2);
            }
            if (square.Pollution)
            {
                DrawImageOnSquare(newElementsWithZIndex, 0.8, "pollution.png", 3);
            }
            if (square.Fortress)
            {
                DrawFortress(newElementsWithZIndex);
            }

            foreach (var element in newElementsWithZIndex.Keys)
            {
                element.SetValue(Grid.RowProperty, square.Row);
                element.SetValue(Grid.ColumnProperty, square.Column);
                element.SetValue(Panel.ZIndexProperty, newElementsWithZIndex[element]);
                element.Tag = square;
                MapGrid.Children.Add(element);
            }
        }

        private static void DrawFortress(Dictionary<FrameworkElement, int> newElementsWithZIndex)
        {
            var rect = new Rectangle
            {
                Width = DEFAULT_SIZE * 0.7,
                Height = DEFAULT_SIZE * 0.7,
                Fill = Brushes.Transparent,
                Stroke = Brushes.Maroon,
                StrokeThickness = 2,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            newElementsWithZIndex.Add(rect, 4);
        }

        private static void DrawImageOnSquare(Dictionary<FrameworkElement, int> newElementsWithZIndex, double ratio, string imgName, int zIndex)
        {
            Image img = new Image
            {
                Width = DEFAULT_SIZE * ratio,
                Height = DEFAULT_SIZE * ratio,
                Source = new BitmapImage(new Uri(Settings.Default.datasPath + imgName)),
                Stretch = Stretch.Uniform,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            newElementsWithZIndex.Add(img, zIndex);
        }

        private static void DrawIrrigationSystem(Dictionary<FrameworkElement, int> newElementsWithZIndex)
        {
            var firstThird = (DEFAULT_SIZE / (double)3) - 1;
            var secondThird = ((DEFAULT_SIZE / (double)3) * 2) - 1;
            for (int i = 0; i < 4; i++)
            {
                var line = new Line
                {
                    X1 = (i == 3 ? secondThird : (i == 2 ? firstThird : 1)),
                    Y1 = (i == 0 ? firstThird : (i == 1 ? secondThird : 1)),
                    X2 = (i == 3 ? secondThird : (i == 2 ? firstThird : DEFAULT_SIZE - 1)),
                    Y2 = (i == 0 ? firstThird : (i == 1 ? secondThird : DEFAULT_SIZE - 1)),
                    StrokeThickness = 2,
                    Stroke = Brushes.Aquamarine
                };
                newElementsWithZIndex.Add(line, 2);
            }
        }

        private void DrawRiver(MapSquarePivot square, bool topToBottom, bool miniMap)
        {
            Rectangle riverRect = new Rectangle
            {
                Width = (miniMap ? _minimapSquareSize : DEFAULT_SIZE) / (topToBottom ? 10 : 1),
                Height = (miniMap ? _minimapSquareSize : DEFAULT_SIZE) / (topToBottom ? 1 : 10),
                Fill = Brushes.Blue
            };

            if (miniMap)
            {
                riverRect.SetValue(Canvas.TopProperty, square.Row * _minimapSquareSize);
                riverRect.SetValue(Canvas.LeftProperty, square.Column * _minimapSquareSize);
            }
            else
            {
                riverRect.SetValue(Grid.RowProperty, square.Row);
                riverRect.SetValue(Grid.ColumnProperty, square.Column);
            }

            riverRect.Tag = square;
            (miniMap ? (Panel)MiniMapCanvas : MapGrid).Children.Add(riverRect);
        }

        private static void DrawRoad(Dictionary<FrameworkElement, int> newElementsWithZIndex, bool railRoad)
        {
            Line l1 = new Line { X1 = 0, Y1 = 0, X2 = DEFAULT_SIZE, Y2 = DEFAULT_SIZE, StrokeThickness = 1, Stroke = railRoad ? Brushes.Black : Brushes.Tan };
            Line l2 = new Line { X1 = 0, Y1 = DEFAULT_SIZE / 2, X2 = DEFAULT_SIZE, Y2 = DEFAULT_SIZE / 2, StrokeThickness = 1, Stroke = railRoad ? Brushes.Black : Brushes.Tan };
            Line l3 = new Line { X1 = 0, Y1 = DEFAULT_SIZE, X2 = DEFAULT_SIZE, Y2 = 0, StrokeThickness = 1, Stroke = railRoad ? Brushes.Black : Brushes.Tan };
            Line l4 = new Line { X1 = DEFAULT_SIZE / 2, Y1 = 0, X2 = DEFAULT_SIZE / 2, Y2 = DEFAULT_SIZE, StrokeThickness = 1, Stroke = railRoad ? Brushes.Black : Brushes.Tan };
            newElementsWithZIndex.Add(l1, 1);
            newElementsWithZIndex.Add(l2, 1);
            newElementsWithZIndex.Add(l3, 1);
            newElementsWithZIndex.Add(l4, 1);
        }

        private void DrawMapCity(CityPivot city, bool skipPreviousCheck)
        {
            if (!skipPreviousCheck)
            {
                var elements = MapGrid.GetChildrenByTag(city);
                foreach (var element in elements)
                {
                    MapGrid.Children.Remove(element);
                }

                var elementsMiniMap = MiniMapCanvas.GetChildrenByTag(city);
                foreach (var element in elementsMiniMap)
                {
                    MiniMapCanvas.Children.Remove(element);
                }
            }


            Image img = new Image
            {
                Width = DEFAULT_SIZE * CityPivot.DISPLAY_RATIO,
                Height = DEFAULT_SIZE * CityPivot.DISPLAY_RATIO,
                Source = new BitmapImage(new Uri(Settings.Default.datasPath + city.RenderValue)),
                Stretch = Stretch.Uniform,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            img.SetValue(Grid.RowProperty, city.MapSquareLocation.Row);
            img.SetValue(Grid.ColumnProperty, city.MapSquareLocation.Column);
            img.SetValue(Panel.ZIndexProperty, CITY_ZINDEX);
            img.Tag = city;
            MapGrid.Children.Add(img);

            bool stuckOnLeft = city.MapSquareLocation.Row == 0;
            bool stuckOnRight = city.MapSquareLocation.Row == _engine.Map.Width - 1;

            var citynameBlock = new TextBlock
            {
                HorizontalAlignment = stuckOnLeft ? HorizontalAlignment.Left :
                    (stuckOnRight ? HorizontalAlignment.Right : HorizontalAlignment.Center),
                VerticalAlignment = VerticalAlignment.Bottom,
                Text = $"{city.Name} - {city.Citizens.Count}",
                TextAlignment = TextAlignment.Center,
                Background = Brushes.White
            };
            citynameBlock.SetValue(Grid.RowProperty, city.MapSquareLocation.Row);
            citynameBlock.SetValue(Grid.ColumnProperty, city.MapSquareLocation.Column - (stuckOnLeft ? 0 : 1));
            citynameBlock.SetValue(Grid.ColumnSpanProperty, stuckOnLeft || stuckOnRight ? 2 : 3);
            citynameBlock.SetValue(Panel.ZIndexProperty, CITY_ZINDEX + 1);
            citynameBlock.Tag = city;
            MapGrid.Children.Add(citynameBlock);

            Rectangle imgMini = new Rectangle
            {
                Width = _minimapSquareSize,
                Height = _minimapSquareSize,
                Fill = Brushes.White
            };
            imgMini.SetValue(Canvas.TopProperty, city.MapSquareLocation.Row * _minimapSquareSize);
            imgMini.SetValue(Canvas.LeftProperty, city.MapSquareLocation.Column * _minimapSquareSize);
            imgMini.Tag = city;
            MiniMapCanvas.Children.Add(imgMini);
        }

        private void DrawUnit(UnitPivot unit, bool blinkAndZindex = false)
        {
            var elements = MapGrid.GetChildrenByTag(unit);
            foreach (var elem in elements)
            {
                MapGrid.Children.Remove(elem);
            }

            Image img = new Image
            {
                Width = DEFAULT_SIZE,
                Height = DEFAULT_SIZE,
                Source = new BitmapImage(new Uri(Settings.Default.datasPath + unit.RenderValue)),
                Stretch = Stretch.Uniform
            };
            img.SetValue(Grid.RowProperty, unit.MapSquareLocation.Row);
            img.SetValue(Grid.ColumnProperty, unit.MapSquareLocation.Column);
            img.SetValue(Panel.ZIndexProperty, UNIT_ZINDEX);
            img.Tag = unit;
            if (blinkAndZindex)
            {
                img.SetValue(Panel.ZIndexProperty, UNIT_ZINDEX + 1);

                var blinkingAnimation = new DoubleAnimation
                {
                    From = 1.0,
                    To = 0.0,
                    RepeatBehavior = RepeatBehavior.Forever,
                    AutoReverse = true,
                    Duration = new Duration(TimeSpan.FromMilliseconds(500))
                };

                var blinkingStoryboard = new Storyboard();
                blinkingStoryboard.Children.Add(blinkingAnimation);
                Storyboard.SetTargetProperty(blinkingAnimation, new PropertyPath("(Image.Opacity)"));

                Storyboard.SetTarget(blinkingAnimation, img);
                blinkingStoryboard.Begin();
            }
            MapGrid.Children.Add(img);
        }

        #endregion

        #region Other methods

        private void InitializeEngineEvents()
        {
            _engine.NextUnitEvent += FocusOnUnit;
            _engine.SubscribeToMapSquareChangeEvent(UpdateSquareMap);
        }

        private void RefreshDynamicView()
        {
            foreach (var unit in _engine.Units)
            {
                DrawUnit(unit);
            }

            foreach (var city in _engine.Cities)
            {
                DrawMapCity(city, false);
            }

            // Ensures a refresh of the blinking current unit.
            RecomputeFocus();
        }

        private void FocusOn(double nexX, double newY)
        {
            MapScroller.UpdateLayout();
            MapScroller.ScrollToHorizontalOffset(nexX);
            MapScroller.ScrollToVerticalOffset(newY);
            MapScroller.UpdateLayout();
        }

        private void RecomputeFocus()
        {
            if (_engine.PreviousUnit != null)
            {
                DrawUnit(_engine.PreviousUnit);
            }

            if (_engine.CurrentUnit != null)
            {
                var newX = ((MapGrid.ActualWidth * _engine.CurrentUnit.MapSquareLocation.Column) / _engine.Map.Width) - (MapScroller.ActualWidth / 2);
                var newY = ((MapGrid.ActualHeight * _engine.CurrentUnit.MapSquareLocation.Row) / _engine.Map.Height) - (MapScroller.ActualHeight / 2);
                FocusOn(newX, newY);
                DrawUnit(_engine.CurrentUnit, true);
            }
        }

        public static DirectionPivot? Move(Key key)
        {
            switch (key)
            {
                case Key.NumPad1:
                    return DirectionPivot.BottomLeft;
                case Key.NumPad2:
                    return DirectionPivot.Bottom;
                case Key.NumPad3:
                    return DirectionPivot.BottomRight;
                case Key.NumPad6:
                    return DirectionPivot.Right;
                case Key.NumPad9:
                    return DirectionPivot.TopRight;
                case Key.NumPad8:
                    return DirectionPivot.Top;
                case Key.NumPad7:
                    return DirectionPivot.TopLeft;
                case Key.NumPad4:
                    return DirectionPivot.Left;
                default:
                    return null;
            }
        }

        #endregion
    }
}
