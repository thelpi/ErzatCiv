﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using ErsatzCiv.Properties;
using ErsatzCivLib;
using ErsatzCivLib.Model;
using ErsatzCivLib.Model.Enums;
using ErsatzCivLib.Model.Events;
using ErsatzCivLib.Model.Static;

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
        
        private double _minimapSquareSize;
        private Rectangle _rCapture;
        private bool _freezeActions = false;
        private readonly EnginePivot _engine;

        public MainWindow()
        {
            InitializeComponent();
            ExpanderMenu.ExpandDirection = ExpandDirection.Up;
            
            CheckBoxWaitTurn.IsChecked = Settings.Default.waitEndTurn;

            _engine = EnginePivot.Default;
            InitializeEngineEvents();

            DrawFullMapAndMiniMap();
            RefreshDynamicView();
        }

        #region Events

        private void OnTriggerUpdateMapSquares(object sender, DiscoverNewSquareEventArgs evt)
        {
            if (evt?.MapSquares != null)
            {
                UpdateMapSquares(evt.MapSquares);
            }
        }

        private void OnTriggerUpdateMapSquare(object sender, SquareChangedEventArgs evt)
        {
            if (evt?.MapSquare != null)
            {
                UpdateMapSquares(new[] { evt.MapSquare });
            }
        }

        private void UpdateMapSquares(IEnumerable<MapSquarePivot> msqList)
        {
            foreach (var msq in msqList)
            {
                MapGrid.DrawSingleMapSquare(DEFAULT_SIZE, msq, true);
                DrawSingleMiniMapSquare(msq, true);
            }
        }

        private void BtnNextTurn_Click(object sender, RoutedEventArgs e)
        {
            if (_freezeActions)
            {
                return;
            }
            _freezeActions = true;
            MoveToNextTurnAndActAccordingly();
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
                if (_engine.HumanPlayer.CurrentUnit != null)
                {
                    var unitToMove = _engine.HumanPlayer.CurrentUnit; // do not remove this line ! ("MoveCurrentUnit()" changes the value of "CurrentUnit")
                    if (_engine.MoveCurrentUnit(Move(e.Key).Value))
                    {
                        var spritesToMove = MapGrid.GetChildrenByTag(unitToMove);
                        foreach (var sprite in spritesToMove)
                        {
                            sprite.SetValue(Grid.RowProperty, unitToMove.MapSquareLocation.Row);
                            sprite.SetValue(Grid.ColumnProperty, unitToMove.MapSquareLocation.Column);
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

                        MapGrid.CleanPreviousChildrenByTag(windowCity.UnitUsed);
                        MapGrid.DrawMapCity(windowCity.City, DEFAULT_SIZE, CITY_ZINDEX, true);
                        DisplayCityName(windowCity.City);
                        DrawMiniMapCity(windowCity.City, true);

                        // Ensures a refresh of the blinking current unit.
                        RecomputeFocus();
                    }
                }
            }
            // Forces turn for current unit.
            else if (e.Key == Key.Space)
            {
                if (_engine.HumanPlayer.CurrentUnit != null)
                {
                    _engine.MoveCurrentUnit(null);
                }
                else
                {
                    MoveToNextTurnAndActAccordingly();
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

        private void FocusOnUnit(object sender, NextUnitEventArgs eventArgs)
        {
            if (!eventArgs.MoreUnit && !Settings.Default.waitEndTurn)
            {
                MoveToNextTurnAndActAccordingly();
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

        private void CheckBoxOpenCityWindowAtProductionEnd_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.openCityWindowAtProductionEnd = !Settings.Default.openCityWindowAtProductionEnd;
            Settings.Default.Save();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var res = MessageBox.Show("Save the game ?", "ErsatzCiv", MessageBoxButton.YesNoCancel);
            if (res == MessageBoxResult.Yes)
            {
                try
                {
                    EnginePivot.SerializeToFile(Settings.Default.datasPath + Settings.Default.savesSubFolder);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Save has failed with the following error : {ex.Message}", "ErsatzCiv");
                    e.Cancel = true;
                    return;
                }

                MessageBox.Show("Save done !", "ErsatzCiv");
                // Quit hard.
                Environment.Exit(0);
            }
            else if (res == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
            }
            else
            {
                // Quit hard.
                Environment.Exit(0);
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

        private void ButtonChangeRegime_Click(object sender, RoutedEventArgs e)
        {
            if (_freezeActions)
            {
                return;
            }
            _freezeActions = true;

            if (_engine.HumanPlayer.CurrentRegime == RegimePivot.Anarchy)
            {
                MessageBox.Show("A revolution is already in progress !", "ErsatzCiv");
            }
            else
            {
                var promptAnswer = MessageBox.Show("Are you sure you want to start a revolution ?", "ErsatzCiv", MessageBoxButton.YesNo);
                if (promptAnswer == MessageBoxResult.Yes)
                {
                    _engine.TriggerRevolution();
                    MessageBox.Show($"Revolution ! Anarchy will stays for {_engine.HumanPlayer.RevolutionTurnsCount} turns.", "ErsatzCiv");
                }
            }

            _freezeActions = false;
        }

        private void ButtonChangeAdvance_Click(object sender, RoutedEventArgs e)
        {
            if (_freezeActions)
            {
                return;
            }
            _freezeActions = true;

            new WindowAdvancePick(_engine).ShowDialog();

            _freezeActions = false;
        }

        private void MiniMapCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                MiniMapCanvas_MouseDown(sender, new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, MouseButton.Left));
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
                MapGrid.DrawSingleMapSquare(DEFAULT_SIZE, ms, false);
                DrawSingleMiniMapSquare(ms, false);
            }

            // Without this line, no square at the first turn !
            UpdateMapSquares(_engine.HumanPlayer.KnownMapSquares);
        }
        
        private void DrawSingleMiniMapSquare(MapSquarePivot square, bool cleanPreviousSquare)
        {
            var brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(DrawTools.MAP_SQUARE_COLORS[square.Biome.Name]));
            if (cleanPreviousSquare)
            {
                MiniMapCanvas.CleanPreviousChildrenByTag(square);
            }
            else
            {
                brush = Brushes.Black;
            }

            Rectangle rctMinimap = new Rectangle
            {
                Width = _minimapSquareSize,
                Height = _minimapSquareSize,
                Fill = brush
            };
            rctMinimap.SetValue(Canvas.TopProperty, square.Row * _minimapSquareSize);
            rctMinimap.SetValue(Canvas.LeftProperty, square.Column * _minimapSquareSize);
            rctMinimap.Tag = square;
            MiniMapCanvas.Children.Add(rctMinimap);
        }

        private void DrawMiniMapCity(CityPivot city, bool skipPreviousCheck)
        {
            if (!skipPreviousCheck)
            {
                MiniMapCanvas.CleanPreviousChildrenByTag(city);
            }

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

        private void DisplayCityName(CityPivot city)
        {
            bool stuckOnLeft = city.OnLeftBorder();
            bool stuckOnRight = city.OnRightBorder(_engine.Map.Width);

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
        }

        #endregion

        #region Other methods

        private void InitializeEngineEvents()
        {
            _engine.HumanPlayer.NextUnitEvent += FocusOnUnit;
            _engine.HumanPlayer.NewAdvanceEvent += delegate (object sender, EventArgs e) { RefreshAdvanceInformations(); };
            _engine.HumanPlayer.NewRegimeEvent += delegate (object sender, EventArgs e) { RefreshRegimeInformations(); };
            foreach (var msq in _engine.Map)
            {
                msq.SquareChangeEvent += OnTriggerUpdateMapSquare;
            }
            _engine.HumanPlayer.DiscoverNewSquareEvent += OnTriggerUpdateMapSquares;
        }

        private void RefreshDynamicView()
        {
            LabelYearInfo.Content = $"Current year : {_engine.CurrentYear} (turn {_engine.CurrentTurn})";
            RefreshRegimeInformations();
            LabelTreasureInfo.Content = $"Treasure : {_engine.HumanPlayer.Treasure} gold ({_engine.HumanPlayer.TreasureByTurn} by turn)";
            RefreshAdvanceInformations();

            foreach (var player in _engine.Players)
            {
                foreach (var unit in player.Units.Where(u => player == _engine.HumanPlayer
                    || _engine.HumanPlayer.KnownMapSquares.Contains(u.MapSquareLocation)))
                {
                    MapGrid.DrawUnit(unit, DEFAULT_SIZE, UNIT_ZINDEX, false, true);
                }

                foreach (var city in player.Cities.Where(u => player == _engine.HumanPlayer
                    || _engine.HumanPlayer.KnownMapSquares.Contains(u.MapSquareLocation)))
                {
                    MapGrid.DrawMapCity(city, DEFAULT_SIZE, CITY_ZINDEX, false);
                    DisplayCityName(city);
                    DrawMiniMapCity(city, false);
                }
            }

            // Ensures a refresh of the blinking current unit.
            RecomputeFocus();
        }

        private void RefreshAdvanceInformations()
        {
            LabelCurrentAdvance.Content = $"Advance in proress : {_engine.HumanPlayer.CurrentAdvance?.Name ?? "{none}"} (in {_engine.HumanPlayer.TurnsBeforeNewAdvance} turn(s))";
        }

        private void RefreshRegimeInformations()
        {
            LabelCurrentRegime.Content = $"Current regime : {_engine.HumanPlayer.CurrentRegime.Name}";
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
            if (_engine.HumanPlayer.PreviousUnit != null)
            {
                MapGrid.DrawUnit(_engine.HumanPlayer.PreviousUnit, DEFAULT_SIZE, UNIT_ZINDEX, false, true);
            }

            if (_engine.HumanPlayer.CurrentUnit != null)
            {
                var newX = ((MapGrid.ActualWidth * _engine.HumanPlayer.CurrentUnit.MapSquareLocation.Column) / _engine.Map.Width) - (MapScroller.ActualWidth / 2);
                var newY = ((MapGrid.ActualHeight * _engine.HumanPlayer.CurrentUnit.MapSquareLocation.Row) / _engine.Map.Height) - (MapScroller.ActualHeight / 2);
                FocusOn(newX, newY);
                MapGrid.DrawUnit(_engine.HumanPlayer.CurrentUnit, DEFAULT_SIZE, UNIT_ZINDEX, true, true);
            }
        }

        private static DirectionPivot? Move(Key key)
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

        private void MoveToNextTurnAndActAccordingly()
        {
            var turnConsequences = _engine.NextTurn();
            if (turnConsequences.EndOfRevolution)
            {
                new WindowRegimePick(_engine).ShowDialog();
            }
            if (Settings.Default.openCityWindowAtProductionEnd)
            {
                var citiesWithDoneProduction = turnConsequences.EndOfProduction;
                foreach (var city in citiesWithDoneProduction.Keys)
                {
                    var result = MessageBox.Show($"{city.Name} has built {citiesWithDoneProduction[city].Name} ! Zoom on city ?", "ErsatzCiv", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        new CityWindow(_engine, city).ShowDialog();
                    }
                }
            }
            if (turnConsequences.EndOfAdvance)
            {
                new WindowAdvancePick(_engine).ShowDialog();
            }
            RefreshDynamicView();
        }

        #endregion
    }
}
