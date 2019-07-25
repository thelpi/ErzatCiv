using System;
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
            // Ensures a refresh of the blinking current unit.
            FocusOnUnit(null, null);
        }

        private void RefreshView(bool full)
        {
            IEnumerable<MapSquareData> toDraw = _engine.Map.MapSquareList;
            if (!full)
            {
                toDraw = toDraw.Where(s => s.Redraw).ToList();
                var mapSquaresToRemove = GetGraphicRenderList<Rectangle>(toDraw).ToList();
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
                UnitToMap(unit);
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

        private void UnitToMap(UnitPivot unit, bool blinkAndZindex = false)
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

        private void FocusOn(double nexX, double newY)
        {
            MapScroller.UpdateLayout();
            MapScroller.ScrollToHorizontalOffset(nexX);
            MapScroller.ScrollToVerticalOffset(newY);
            MapScroller.UpdateLayout();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // just in case th event in the constructor doesn't work.
            FocusOnUnit(null, null);

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
                    var unitToMove = _engine.CurrentUnit;
                    if (unitToMove.Move(_engine, e.Key.Move().Value))
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

                            Image rct = new Image
                            {
                                Width = DEFAULT_SIZE * CityPivot.DISPLAY_RATIO,
                                Height = DEFAULT_SIZE * CityPivot.DISPLAY_RATIO,
                                Source = new BitmapImage(new Uri(Properties.Settings.Default.imagesPath + city.RenderValue)),
                                Stretch = Stretch.Uniform,
                                HorizontalAlignment = HorizontalAlignment.Center,
                                VerticalAlignment = VerticalAlignment.Center
                            };
                            rct.SetValue(Grid.RowProperty, city.Row);
                            rct.SetValue(Grid.ColumnProperty, city.Column);
                            rct.Tag = city;
                            MapGrid.Children.Add(rct);
                        }
                        // Ensures a refresh of the blinking current unit.
                        FocusOnUnit(null, null);
                    }
                }
            }
            // Forces next turn.
            else if (e.Key == System.Windows.Input.Key.Space)
            {
                BtnNextTurn_Click(sender, e);
            }
            // Centers the screen on current unit.
            else if (e.Key == System.Windows.Input.Key.C)
            {
                FocusOnUnit(null, null);
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
                FocusOnUnit(null, null);
            }
        }

        private T GetGraphicRender<T>(object tagValue) where T : FrameworkElement
        {
            return MapGrid.Children.OfType<T>().FirstOrDefault(x => x.Tag == tagValue);
        }

        private IEnumerable<T> GetGraphicRenderList<T>(params object[] tagValues) where T : FrameworkElement
        {
            return MapGrid.Children.OfType<T>().Where(x => tagValues.Contains(x.Tag));
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
            if (_engine.PreviousUnit != null)
            {
                UnitToMap(_engine.PreviousUnit);
            }

            if (_engine.CurrentUnit != null)
            {
                var newX = ((MapGrid.ActualWidth * _engine.CurrentUnit.Column) / _engine.Map.Width) - (MapScroller.ActualWidth / 2);
                var newY = ((MapGrid.ActualHeight * _engine.CurrentUnit.Row) / _engine.Map.Height) - (MapScroller.ActualHeight / 2);
                FocusOn(newX, newY);
                UnitToMap(_engine.CurrentUnit, true);
            }
        }
    }
}
