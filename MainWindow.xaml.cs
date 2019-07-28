﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ErsatzCivLib;
using ErsatzCivLib.Model;

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

        public MainWindow()
        {
            InitializeComponent();
            InitializeEngine();
            DrawFullMapAndMiniMap();
            RefreshDynamicView();
        }

        #region Events

        private void UpdateSquareMap(object sender, EventArgs evt)
        {
            if (sender?.GetType() == typeof(MapSquareData))
            {
                var ms = (MapSquareData)sender;
                DrawSingleMapAndMiniMapSquare(ms, true);
            }
        }

        private void BtnNextTurn_Click(object sender, RoutedEventArgs e)
        {
            _engine.NewTurn();
            RefreshDynamicView();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // just in case th event in the constructor doesn't work.
            RecomputeFocus();

            var rWidth = (MapScroller.ActualWidth * (MENU_HEIGHT * MapData.RATIO_WIDTH_HEIGHT)) / MapGrid.ActualWidth;
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

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // Worker actions.
            if ((System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.LeftShift)
                || System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.RightShift))
                && _engine.CurrentUnit?.GetType() == typeof(WorkerPivot))
            {
                switch (e.Key)
                {
                    case System.Windows.Input.Key.M:
                        _engine.WorkerAction(MapSquareActionPivot.Mine);
                        break;
                    case System.Windows.Input.Key.I:
                        _engine.WorkerAction(MapSquareActionPivot.Irrigate);
                        break;
                    case System.Windows.Input.Key.R:
                        _engine.WorkerAction(MapSquareActionPivot.RailRoad);
                        break;
                    case System.Windows.Input.Key.C:
                        _engine.WorkerAction(MapSquareActionPivot.Clear);
                        break;
                    case System.Windows.Input.Key.D:
                        _engine.WorkerAction(MapSquareActionPivot.DestroyImprovement);
                        break;
                    case System.Windows.Input.Key.F:
                        _engine.WorkerAction(MapSquareActionPivot.BuildFortress);
                        break;
                    case System.Windows.Input.Key.P:
                        _engine.WorkerAction(MapSquareActionPivot.Plant);
                        break;
                    case System.Windows.Input.Key.A:
                        _engine.WorkerAction(MapSquareActionPivot.ClearPollution);
                        break;
                    case System.Windows.Input.Key.X:
                        _engine.WorkerAction(MapSquareActionPivot.DestroyRoad);
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
                        var associatedSprites = GetGraphicRenders(unitToMove);
                        if (associatedSprites.Count > 0)
                        {
                            associatedSprites.First().SetValue(Grid.RowProperty, unitToMove.Row);
                            associatedSprites.First().SetValue(Grid.ColumnProperty, unitToMove.Column);
                        }
                    }
                }
            }
            // Buils city.
            else if (e.Key == System.Windows.Input.Key.B)
            {
                if (_engine.CurrentUnit?.GetType() == typeof(SettlerPivot))
                {
                    var unitToMove = _engine.CurrentUnit; // do not remove this line ! ("BuildCity()" changes the value of "CurrentUnit")
                    var city = _engine.BuildCity();
                    if (city != null)
                    {
                        var associatedSprites = GetGraphicRenders(unitToMove);
                        if (associatedSprites.Count > 0)
                        {
                            MapGrid.Children.Remove(associatedSprites.First());

                            DrawMapCity(city, true);
                        }
                        // Ensures a refresh of the blinking current unit.
                        RecomputeFocus();
                    }
                }
            }
            // Forces next turn.
            else if (e.Key == System.Windows.Input.Key.Space)
            {
                _engine.NewTurn();
                RefreshDynamicView();
            }
            // Centers the screen on current unit.
            else if (e.Key == System.Windows.Input.Key.C)
            {
                RecomputeFocus();
            }
            // Goes to next unit.
            else if (e.Key == System.Windows.Input.Key.W)
            {
                _engine.ToNextUnit();
            }
        }

        private void MiniMapCanvas_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Point p = System.Windows.Input.Mouse.GetPosition(MiniMapCanvas);

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

        private void FocusOnUnit(object sender, EventArgs eventArgs)
        {
            RecomputeFocus();
        }

        #endregion

        #region Draw methods

        private void RefreshMiniMapSelector()
        {
            if (_rCapture != null)
            {
                var x = MapScroller.ContentHorizontalOffset;
                var y = MapScroller.ContentVerticalOffset;

                var rX = (x * (MENU_HEIGHT * MapData.RATIO_WIDTH_HEIGHT)) / MapGrid.ActualWidth;
                var rY = (y * MENU_HEIGHT) / MapGrid.ActualHeight;

                _rCapture.SetValue(Canvas.LeftProperty, rX);
                _rCapture.SetValue(Canvas.TopProperty, rY);
                _rCapture.SetValue(Panel.ZIndexProperty, 2);
            }
        }

        private void DrawFullMapAndMiniMap()
        {
            MiniMapCanvas.Height = MENU_HEIGHT;
            MiniMapCanvas.Width = MENU_HEIGHT * MapData.RATIO_WIDTH_HEIGHT;

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

            foreach (var square in _engine.Map.MapSquareList)
            {
                DrawSingleMapAndMiniMapSquare(square, false);
            }
        }

        private void DrawSingleMapAndMiniMapSquare(MapSquareData square, bool cleanPreviousSquare)
        {
            if (cleanPreviousSquare)
            {
                CleanPreviousRenderOnMapAndMiniMap(square);
            }

            Rectangle rct = new Rectangle
            {
                Stroke = Brushes.DarkGray,
                StrokeThickness = 1,
                Width = DEFAULT_SIZE,
                Height = DEFAULT_SIZE,
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(square.MapSquareType.RenderValue))
            };
            rct.SetValue(Grid.RowProperty, square.Row);
            rct.SetValue(Grid.ColumnProperty, square.Column);
            rct.Tag = square;
            MapGrid.Children.Add(rct);

            Rectangle rctMinimap = new Rectangle
            {
                Width = _minimapSquareSize,
                Height = _minimapSquareSize,
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(square.MapSquareType.RenderValue))
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

        private void CleanPreviousRenderOnMapAndMiniMap(MapSquareData square)
        {
            var elements = GetGraphicRenders(square);
            foreach (var elem in elements)
            {
                MapGrid.Children.Remove(elem);
            }

            var elementsMiniMap = GetMiniGraphicRenders(square);
            foreach (var elem in elementsMiniMap)
            {
                MiniMapCanvas.Children.Remove(elem);
            }
        }

        private void DrawSquareRivers(MapSquareData square)
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

        private void DrawSquareImprovements(MapSquareData square)
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
                Source = new BitmapImage(new Uri(Properties.Settings.Default.imagesPath + imgName)),
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

        private void DrawRiver(MapSquareData square, bool topToBottom, bool miniMap)
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
                var elements = GetGraphicRenders(city);
                foreach (var element in elements)
                {
                    MapGrid.Children.Remove(element);
                }

                var elementsMiniMap = GetMiniGraphicRenders(city);
                foreach (var element in elementsMiniMap)
                {
                    MiniMapCanvas.Children.Remove(element);
                }
            }

            Image img = new Image
            {
                Width = DEFAULT_SIZE * CityPivot.DISPLAY_RATIO,
                Height = DEFAULT_SIZE * CityPivot.DISPLAY_RATIO,
                Source = new BitmapImage(new Uri(Properties.Settings.Default.imagesPath + city.RenderValue)),
                Stretch = Stretch.Uniform,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            img.SetValue(Grid.RowProperty, city.Row);
            img.SetValue(Grid.ColumnProperty, city.Column);
            img.SetValue(Panel.ZIndexProperty, CITY_ZINDEX);
            img.Tag = city;
            MapGrid.Children.Add(img);

            Rectangle imgMini = new Rectangle
            {
                Width = _minimapSquareSize * CityPivot.DISPLAY_RATIO,
                Height = _minimapSquareSize * CityPivot.DISPLAY_RATIO,
                Fill = Brushes.White
            };
            imgMini.SetValue(Canvas.TopProperty, city.Row * _minimapSquareSize);
            imgMini.SetValue(Canvas.LeftProperty, city.Column * _minimapSquareSize);
            imgMini.Tag = city;
            MiniMapCanvas.Children.Add(imgMini);
        }

        private void DrawUnit(UnitPivot unit, bool blinkAndZindex = false)
        {
            var currentElem = MapGrid.Children.OfType<Image>().FirstOrDefault(x => x.Tag == unit);
            if (currentElem != null)
            {
                MapGrid.Children.Remove(currentElem);
            }
            Image img = new Image
            {
                Width = DEFAULT_SIZE,
                Height = DEFAULT_SIZE,
                Source = new BitmapImage(new Uri(Properties.Settings.Default.imagesPath + unit.RenderValue)),
                Stretch = Stretch.Uniform
            };
            img.SetValue(Grid.RowProperty, unit.Row);
            img.SetValue(Grid.ColumnProperty, unit.Column);
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

        private List<FrameworkElement> GetGraphicRenders(object tagValue)
        {
            return MapGrid.Children.OfType<FrameworkElement>().Where(x => x.Tag == tagValue).ToList();
        }

        private List<FrameworkElement> GetMiniGraphicRenders(object tagValue)
        {
            return MiniMapCanvas.Children.OfType<FrameworkElement>().Where(x => x.Tag == tagValue).ToList();
        }

        #endregion

        #region Other methods

        private void InitializeEngine()
        {
            _engine = new Engine(MapData.MapSizeEnum.VeryLarge, 5, 0.6);
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
                var newX = ((MapGrid.ActualWidth * _engine.CurrentUnit.Column) / _engine.Map.Width) - (MapScroller.ActualWidth / 2);
                var newY = ((MapGrid.ActualHeight * _engine.CurrentUnit.Row) / _engine.Map.Height) - (MapScroller.ActualHeight / 2);
                FocusOn(newX, newY);
                DrawUnit(_engine.CurrentUnit, true);
            }
        }

        public static DirectionEnumPivot? Move(System.Windows.Input.Key key)
        {
            switch (key)
            {
                case System.Windows.Input.Key.NumPad1:
                    return DirectionEnumPivot.BottomLeft;
                case System.Windows.Input.Key.NumPad2:
                    return DirectionEnumPivot.Bottom;
                case System.Windows.Input.Key.NumPad3:
                    return DirectionEnumPivot.BottomRight;
                case System.Windows.Input.Key.NumPad6:
                    return DirectionEnumPivot.Right;
                case System.Windows.Input.Key.NumPad9:
                    return DirectionEnumPivot.TopRight;
                case System.Windows.Input.Key.NumPad8:
                    return DirectionEnumPivot.Top;
                case System.Windows.Input.Key.NumPad7:
                    return DirectionEnumPivot.TopLeft;
                case System.Windows.Input.Key.NumPad4:
                    return DirectionEnumPivot.Left;
                default:
                    return null;
            }
        }

        #endregion
    }
}
