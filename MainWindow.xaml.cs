﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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

            RefreshView(true);
        }

        private void BtnNextTurn_Click(object sender, RoutedEventArgs e)
        {
            _engine.NewTurn();
            RefreshView(false);
            _engine.PostTurnClean();
        }

        private void RefreshView(bool full)
        {
            IEnumerable<MapSquareData> toDraw = _engine.Map.MapSquareList;
            if (!full)
            {
                toDraw = toDraw.Where(s => s.Redraw).ToList();
                var mapSquaresToRemove = MapGrid.Children.OfType<Rectangle>().Where(x => toDraw.Contains(x.Tag)).ToList();
                var minimapSquaresToRemove = MiniMapCanvas.Children.OfType<Rectangle>().Where(x => toDraw.Contains(x.Tag)).ToList();
                foreach (var ms in mapSquaresToRemove)
                {
                    MapGrid.Children.Remove(ms);
                }
                foreach (var ms in minimapSquaresToRemove)
                {
                    MiniMapCanvas.Children.Remove(ms);
                }
                var spritesToRemove = MapGrid.Children.OfType<Image>().ToList();
                foreach (var sprite in spritesToRemove)
                {
                    MapGrid.Children.Remove(sprite);
                }
            }

            foreach (var square in toDraw)
            {
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
                }

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
            }
            
            foreach (var unit in _engine.Units)
            {
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
                MapGrid.Children.Add(img);
            }

            foreach (var city in _engine.Cities)
            {
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
            }
        }

        private void FocusOn(double nexX, double newY)
        {
            MapScroller.UpdateLayout();
            MapScroller.ScrollToHorizontalOffset(nexX);
            MapScroller.ScrollToVerticalOffset(newY);
            MapScroller.UpdateLayout();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (_engine.Units.Count > 0)
            {
                var newX = ((MapGrid.ActualWidth * _engine.Units.First().Column) / _engine.Map.Width) - (MapScroller.ActualWidth / 2);
                var newY = ((MapGrid.ActualHeight * _engine.Units.First().Row) / _engine.Map.Height) - (MapScroller.ActualHeight / 2);
                FocusOn(newX, newY);
            }

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
            var unitToMove = _engine.Units.LastOrDefault(u => !u.Locked);

            var move = e.Key.Move();
            if (move.HasValue)
            {
                if (unitToMove != null)
                {
                    if (unitToMove.Move(_engine, move.Value))
                    {
                        var associatedSprite = MapGrid.Children.OfType<Image>().FirstOrDefault(x => x.Tag == unitToMove);
                        if (associatedSprite != null)
                        {
                            associatedSprite.SetValue(Grid.RowProperty, unitToMove.Row);
                            associatedSprite.SetValue(Grid.ColumnProperty, unitToMove.Column);
                        }
                    }
                }
            }
            else if (e.Key == System.Windows.Input.Key.B)
            {
                if (unitToMove?.GetType() == typeof(SettlerPivot))
                {
                    var city = _engine.BuildCity((SettlerPivot)unitToMove);
                    if (city != null)
                    {
                        var associatedSprite = MapGrid.Children.OfType<Image>().FirstOrDefault(x => x.Tag == unitToMove);
                        if (associatedSprite != null)
                        {
                            MapGrid.Children.Remove(associatedSprite);

                            Image rct = new Image
                            {
                                Width = DEFAULT_SIZE * CityPivot.DISPLAY_RATIO,
                                Height = DEFAULT_SIZE * CityPivot.DISPLAY_RATIO,
                                Source = new BitmapImage(new Uri(Properties.Settings.Default.imagesPath + city.RenderValue)),
                                Stretch = Stretch.Uniform,
                                HorizontalAlignment = HorizontalAlignment.Center,
                                VerticalAlignment = VerticalAlignment.Center
                            };
                            rct.SetValue(Grid.RowProperty, unitToMove.Row);
                            rct.SetValue(Grid.ColumnProperty, unitToMove.Column);
                            rct.Tag = city;
                            MapGrid.Children.Add(rct);
                        }
                    }
                }
            }
            else if (e.Key == System.Windows.Input.Key.Space)
            {
                BtnNextTurn_Click(sender, e);
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
    }
}
