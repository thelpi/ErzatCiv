﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ErsatzCiv.Model;

namespace ErsatzCiv
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int DEFAULT_SIZE = 50;
        private const int MENU_HEIGHT = 200;
        private Engine _engine;
        private double _minimapSquareSize;
        private Rectangle _rCapture;

        public MainWindow()
        {
            InitializeComponent();

            MiniMapCanvas.Height = MENU_HEIGHT;
            MiniMapCanvas.Width = MENU_HEIGHT * MapData.RATIO_WIDTH_HEIGHT;
            _engine = new Engine(MapData.MapSizeEnum.VeryLarge, 5, 4);
            _engine.NextUnitEvent += FocusOnUnit;
            _engine.SubscribeToMapSquareChangeEvent(UpdateSquareMap);

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

            DrawFullMapAndMiniMap();
            RefreshDynamicView();
        }

        #region Events

        private void UpdateSquareMap(object sender, EventArgs evt)
        {
            if (sender?.GetType() == typeof(MapSquareData))
            {
                var ms = (MapSquareData)sender;
                DrawSingleMapAndMiniMapSquare(ms, false);
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

            var x = MapScroller.ContentHorizontalOffset;
            var y = MapScroller.ContentVerticalOffset;

            var rX = (x * (MENU_HEIGHT * MapData.RATIO_WIDTH_HEIGHT)) / MapGrid.ActualWidth;
            var rY = (y * MENU_HEIGHT) / MapGrid.ActualHeight;
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
            _rCapture.SetValue(Canvas.LeftProperty, rX);
            _rCapture.SetValue(Canvas.TopProperty, rY);
            _rCapture.SetValue(Panel.ZIndexProperty, 2);
            MiniMapCanvas.Children.Add(_rCapture);
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // Moves a unit.
            if (e.Key.Move().HasValue)
            {
                if (_engine.CurrentUnit != null)
                {
                    var unitToMove = _engine.CurrentUnit; // do not remove this line ! ("MoveCurrentUnit()" changes the value of "CurrentUnit")
                    if (_engine.MoveCurrentUnit(e.Key.Move().Value))
                    {
                        var associatedSprite = GetGraphicRender<Image>(unitToMove);
                        if (associatedSprite != null)
                        {
                            associatedSprite.SetValue(Grid.RowProperty, unitToMove.Row);
                            associatedSprite.SetValue(Grid.ColumnProperty, unitToMove.Column);
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
                        Image associatedSprite = GetGraphicRender<Image>(unitToMove);
                        if (associatedSprite != null)
                        {
                            MapGrid.Children.Remove(associatedSprite);

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
            // Worker actions.
            else if ((System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.LeftShift)
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

        private void FocusOnUnit(object sender, EventArgs eventArgs)
        {
            RecomputeFocus();
        }

        #endregion

        #region Draw methods

        private void DrawFullMapAndMiniMap()
        {
            foreach (var square in _engine.Map.MapSquareList)
            {
                DrawSingleMapAndMiniMapSquare(square, true);
            }
        }

        private void DrawSingleMapAndMiniMapSquare(MapSquareData square, bool skipPreviousCheck)
        {
            if (!skipPreviousCheck)
            {
                var rect = GetGraphicRender<Rectangle>(square);
                if (rect != null)
                {
                    // also deletes the river layer (same tag)
                    MapGrid.Children.Remove(rect);
                }

                var rectMini = GetMiniGraphicRender<Rectangle>(square);
                if (rectMini != null)
                {
                    // also deletes the river layer (same tag)
                    MiniMapCanvas.Children.Remove(rectMini);
                }
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
                Rectangle rctRiver = new Rectangle
                {
                    Width = DEFAULT_SIZE,
                    Height = DEFAULT_SIZE / (double)10,
                    Fill = Brushes.Blue
                };
                rctRiver.SetValue(Grid.RowProperty, square.Row);
                rctRiver.SetValue(Grid.ColumnProperty, square.Column);
                rctRiver.Tag = square;
                MapGrid.Children.Add(rctRiver);

                Rectangle rctRiverMinimap = new Rectangle
                {
                    Width = _minimapSquareSize,
                    Height = _minimapSquareSize / 10,
                    Fill = Brushes.Blue
                };
                rctRiverMinimap.SetValue(Canvas.TopProperty, square.Row * _minimapSquareSize);
                rctRiverMinimap.SetValue(Canvas.LeftProperty, square.Column * _minimapSquareSize);
                rctRiverMinimap.Tag = square;
                MiniMapCanvas.Children.Add(rctRiverMinimap);
            }
        }

        private void DrawMapCity(CityPivot city, bool skipPreviousCheck)
        {
            if (!skipPreviousCheck)
            {
                var currentElem = GetGraphicRender<Image>(city);
                if (currentElem != null)
                {
                    MapGrid.Children.Remove(currentElem);
                }

                var currentElemMiniMap = GetMiniGraphicRender<Rectangle>(city);
                if (currentElemMiniMap != null)
                {
                    MiniMapCanvas.Children.Remove(currentElemMiniMap);
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
            img.Tag = unit;
            if (blinkAndZindex)
            {
                img.SetValue(Panel.ZIndexProperty, 2);

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

        private T GetGraphicRender<T>(object tagValue) where T : FrameworkElement
        {
            return MapGrid.Children.OfType<T>().FirstOrDefault(x => x.Tag == tagValue);
        }

        private T GetMiniGraphicRender<T>(object tagValue) where T : FrameworkElement
        {
            return MiniMapCanvas.Children.OfType<T>().FirstOrDefault(x => x.Tag == tagValue);
        }

        #endregion

        #region Other methods

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

        #endregion
    }
}
