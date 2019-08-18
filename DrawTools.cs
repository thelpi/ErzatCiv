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
using ErsatzCivLib.Model;
using Biome = ErsatzCivLib.Model.Static.BiomePivot;

namespace ErsatzCiv
{
    internal static class DrawTools
    {
        internal const double CITY_DISPLAY_RATIO = 0.8;
        internal const string CITY_RENDER_PATH = "city.png";
        internal const string HUT_RENDER_PATH = "hut.png";
        internal const string UNIT_RENDER_PATH = "units\\{0}.png";
        internal const string UNIT_RENDER_PATH_BARBARIAN = "units\\red\\{0}.png";
        internal static readonly Dictionary<string, string> MAP_SQUARE_COLORS = new Dictionary<string, string>
        {
            { Biome.Grassland.Name, "#32CD32" },
            { Biome.Ocean.Name, "#1E90FF" },
            { Biome.Arctic.Name, "#FFFAF0" },
            { Biome.Tundra.Name, "#2F4F4F" },
            { Biome.Desert.Name, "#FF7F50" },
            { Biome.Jungle.Name, "#9ACD32" },
            { Biome.Mountain.Name, "#A52A2A" },
            { Biome.Hills.Name, "#556B2F" },
            { Biome.Swamp.Name, "#3CB371" },
            { Biome.Forest.Name, "#006400" },
            { Biome.Plains.Name, "#EEE8AA" },
            { Biome.River.Name, "#1E90FF" },
        };

        internal static void CleanPreviousChildrenByTag(this Panel panel, object tagValue)
        {
            var elements = panel.GetChildrenByTag(tagValue);
            foreach (var elem in elements)
            {
                panel.Children.Remove(elem);
            }
        }

        internal static List<FrameworkElement> GetChildrenByTag(this Panel panel, object tagValue)
        {
            if (panel == null)
            {
                return null;
            }

            return panel.Children.OfType<FrameworkElement>().Where(x => x.Tag == tagValue).ToList();
        }

        internal static void DrawUnit(this Panel panel, UnitPivot unit, double defaultDim, int zIndex, bool blinkAndZindex, bool cleanBefore, bool barbarians = false)
        {
            if (panel == null)
            {
                return;
            }

            if (cleanBefore)
            {
                panel.CleanPreviousChildrenByTag(unit);
            }

            Image img = new Image
            {
                Width = defaultDim,
                Height = defaultDim,
                Source = new BitmapImage(new Uri(Settings.Default.datasPath + UnitRenderFileName(unit, barbarians))),
                Stretch = Stretch.Uniform
            };
            img.SetValue(Grid.RowProperty, unit.MapSquareLocation.Row);
            img.SetValue(Grid.ColumnProperty, unit.MapSquareLocation.Column);
            img.SetValue(Panel.ZIndexProperty, zIndex);
            img.Tag = unit;
            if (blinkAndZindex)
            {
                img.SetValue(Panel.ZIndexProperty, zIndex + 1);

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
            panel.Children.Add(img);
        }

        private static string UnitRenderFileName(UnitPivot unit, bool barbarians)
        {
            return string.Format(barbarians ? UNIT_RENDER_PATH_BARBARIAN : UNIT_RENDER_PATH, unit.GetType().Name.Replace("Pivot", string.Empty).ToLowerInvariant());
        }

        internal static void DrawSquareImprovements(this Panel panel, MapSquarePivot square, double defaultDim, Tuple<int, int> gridPositionOffset = null)
        {
            if (panel == null)
            {
                return;
            }

            var newElementsWithZIndex = new Dictionary<FrameworkElement, int>();
            if (square.RailRoad)
            {
                DrawRoad(defaultDim, newElementsWithZIndex, true);
            }
            else if (square.Road)
            {
                DrawRoad(defaultDim, newElementsWithZIndex, false);
            }
            if (square.Irrigate)
            {
                DrawIrrigationSystem(defaultDim, newElementsWithZIndex);
            }
            if (square.Mine)
            {
                DrawImageOnSquare(defaultDim, newElementsWithZIndex, 0.5, "mine.png", 2);
            }
            if (square.Pollution)
            {
                DrawImageOnSquare(defaultDim, newElementsWithZIndex, 0.8, "pollution.png", 3);
            }
            if (square.Fortress)
            {
                DrawFortress(defaultDim, newElementsWithZIndex);
            }

            foreach (var element in newElementsWithZIndex.Keys)
            {
                element.SetValue(Grid.RowProperty, square.Row - (gridPositionOffset == null ? 0 : gridPositionOffset.Item1));
                element.SetValue(Grid.ColumnProperty, square.Column - (gridPositionOffset == null ? 0 : gridPositionOffset.Item2));
                element.SetValue(Panel.ZIndexProperty, newElementsWithZIndex[element]);
                element.Tag = square;
                panel.Children.Add(element);
            }
        }

        private static void DrawFortress(double defaultDim, Dictionary<FrameworkElement, int> newElementsWithZIndex)
        {
            var rect = new Rectangle
            {
                Width = defaultDim * 0.7,
                Height = defaultDim * 0.7,
                Fill = Brushes.Transparent,
                Stroke = Brushes.Maroon,
                StrokeThickness = 2,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            newElementsWithZIndex.Add(rect, 4);
        }

        private static void DrawImageOnSquare(double defaultDim, Dictionary<FrameworkElement, int> newElementsWithZIndex, double ratio, string imgName, int zIndex)
        {
            Image img = new Image
            {
                Width = defaultDim * ratio,
                Height = defaultDim * ratio,
                Source = new BitmapImage(new Uri(Settings.Default.datasPath + imgName)),
                Stretch = Stretch.Uniform,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            newElementsWithZIndex.Add(img, zIndex);
        }

        private static void DrawIrrigationSystem(double defaultDim, Dictionary<FrameworkElement, int> newElementsWithZIndex)
        {
            var firstThird = (defaultDim / 3) - 1;
            var secondThird = ((defaultDim / 3) * 2) - 1;
            for (int i = 0; i < 4; i++)
            {
                var line = new Line
                {
                    X1 = (i == 3 ? secondThird : (i == 2 ? firstThird : 1)),
                    Y1 = (i == 0 ? firstThird : (i == 1 ? secondThird : 1)),
                    X2 = (i == 3 ? secondThird : (i == 2 ? firstThird : defaultDim - 1)),
                    Y2 = (i == 0 ? firstThird : (i == 1 ? secondThird : defaultDim - 1)),
                    StrokeThickness = 2,
                    Stroke = Brushes.Aquamarine
                };
                newElementsWithZIndex.Add(line, 2);
            }
        }

        private static void DrawRoad(double defaultDim, Dictionary<FrameworkElement, int> newElementsWithZIndex, bool railRoad)
        {
            Line l1 = new Line { X1 = 0, Y1 = 0, X2 = defaultDim, Y2 = defaultDim, StrokeThickness = 1, Stroke = railRoad ? Brushes.Black : Brushes.Tan };
            Line l2 = new Line { X1 = 0, Y1 = defaultDim / 2, X2 = defaultDim, Y2 = defaultDim / 2, StrokeThickness = 1, Stroke = railRoad ? Brushes.Black : Brushes.Tan };
            Line l3 = new Line { X1 = 0, Y1 = defaultDim, X2 = defaultDim, Y2 = 0, StrokeThickness = 1, Stroke = railRoad ? Brushes.Black : Brushes.Tan };
            Line l4 = new Line { X1 = defaultDim / 2, Y1 = 0, X2 = defaultDim / 2, Y2 = defaultDim, StrokeThickness = 1, Stroke = railRoad ? Brushes.Black : Brushes.Tan };
            newElementsWithZIndex.Add(l1, 1);
            newElementsWithZIndex.Add(l2, 1);
            newElementsWithZIndex.Add(l3, 1);
            newElementsWithZIndex.Add(l4, 1);
        }

        internal static void DrawSingleMapSquare(this Panel panel, double defaultDim, MapSquarePivot square, bool cleanPreviousSquare,
            Tuple<int, int> gridPositionOffset = null, Action<object, MouseButtonEventArgs> mouseLeftButtonDownCallback = null)
        {
            if (panel == null)
            {
                return;
            }

            if (cleanPreviousSquare)
            {
                panel.CleanPreviousChildrenByTag(square);
            }
            else if (!Settings.Default.showFullMap)
            {
                var blackRectangle = new Rectangle
                {
                    Fill = Brushes.Black,
                    Width = defaultDim,
                    Height = defaultDim,
                    Tag = square
                };

                blackRectangle.SetValue(Grid.RowProperty, square.Row - (gridPositionOffset == null ? 0 : gridPositionOffset.Item1));
                blackRectangle.SetValue(Grid.ColumnProperty, square.Column - (gridPositionOffset == null ? 0 : gridPositionOffset.Item2));
                panel.Children.Add(blackRectangle);
                return;
            }

            FrameworkElement squareRender;
            string imgPath = Settings.Default.datasPath +
                Settings.Default.squareImageSubFolder +
                (square.HasBonus ? Settings.Default.squareImageBonusSubFolder : string.Empty)
                + $"{square.Biome.Name.ToLowerInvariant()}.png";
            if (System.IO.File.Exists(imgPath))
            {
                squareRender = new Border
                {
                    Child = new Image
                    {
                        Width = defaultDim,
                        Height = defaultDim,
                        Source = new BitmapImage(new Uri(imgPath)),
                        Stretch = Stretch.Uniform,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    }
                };
            }
            else
            {
                squareRender = new Rectangle
                {
                    Width = defaultDim,
                    Height = defaultDim,
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(MAP_SQUARE_COLORS[square.Biome.Name]))
                };
            }


            squareRender.SetValue(Grid.RowProperty, square.Row - (gridPositionOffset == null ? 0 : gridPositionOffset.Item1));
            squareRender.SetValue(Grid.ColumnProperty, square.Column - (gridPositionOffset == null ? 0 : gridPositionOffset.Item2));
            squareRender.Tag = square;
            panel.Children.Add(squareRender);

            panel.DrawSquareImprovements(square, defaultDim, gridPositionOffset);

            if (mouseLeftButtonDownCallback != null)
            {
                var transparentLayer = new Rectangle
                {
                    Width = defaultDim,
                    Height = defaultDim,
                    Fill = Brushes.Transparent,
                    Tag = square
                };
                transparentLayer.MouseLeftButtonDown += new MouseButtonEventHandler(mouseLeftButtonDownCallback);
                transparentLayer.SetValue(Grid.RowProperty, square.Row - (gridPositionOffset == null ? 0 : gridPositionOffset.Item1));
                transparentLayer.SetValue(Grid.ColumnProperty, square.Column - (gridPositionOffset == null ? 0 : gridPositionOffset.Item2));
                transparentLayer.SetValue(Panel.ZIndexProperty, 10);
                panel.Children.Add(transparentLayer);
            }
        }

        internal static void DrawMapCity(this Panel panel, CityPivot city, double defaultDim, int cityZindex, bool skipPreviousCheck,
            Tuple<int, int> gridPositionOffset = null, Action<object, MouseButtonEventArgs> mouseLeftButtonDownCallback = null)
        {
            if (panel == null)
            {
                return;
            }

            if (!skipPreviousCheck)
            {
                panel.CleanPreviousChildrenByTag(city);
            }

            Image img = new Image
            {
                Width = defaultDim * CITY_DISPLAY_RATIO,
                Height = defaultDim * CITY_DISPLAY_RATIO,
                Source = new BitmapImage(new Uri(Settings.Default.datasPath + CITY_RENDER_PATH)),
                Stretch = Stretch.Uniform,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            img.SetValue(Grid.RowProperty, city.MapSquareLocation.Row - (gridPositionOffset == null ? 0 : gridPositionOffset.Item1));
            img.SetValue(Grid.ColumnProperty, city.MapSquareLocation.Column - (gridPositionOffset == null ? 0 : gridPositionOffset.Item2));
            img.SetValue(Panel.ZIndexProperty, cityZindex);
            img.Tag = city;

            if (mouseLeftButtonDownCallback != null)
            {
                img.MouseLeftButtonDown += new MouseButtonEventHandler(mouseLeftButtonDownCallback);
            }

            panel.Children.Add(img);
        }

        internal static void DrawHut(this Panel panel, HutPivot hut, double defaultDim, int cityZindex, bool skipPreviousCheck)
        {
            if (panel == null)
            {
                return;
            }

            if (!skipPreviousCheck)
            {
                panel.CleanPreviousChildrenByTag(hut);
            }

            Image img = new Image
            {
                Width = defaultDim * CITY_DISPLAY_RATIO,
                Height = defaultDim * CITY_DISPLAY_RATIO,
                Source = new BitmapImage(new Uri(Settings.Default.datasPath + HUT_RENDER_PATH)),
                Stretch = Stretch.Uniform,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            img.SetValue(Grid.RowProperty, hut.MapSquareLocation.Row);
            img.SetValue(Grid.ColumnProperty, hut.MapSquareLocation.Column);
            img.SetValue(Panel.ZIndexProperty, cityZindex);
            img.Tag = hut;

            panel.Children.Add(img);
        }
    }
}
