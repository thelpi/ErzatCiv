using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ErsatzCiv.Properties;
using ErsatzCivLib.Model;

namespace ErsatzCiv
{
    internal static class DrawTools
    {
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

        internal static void DrawUnit(this Panel panel, UnitPivot unit, double defaultDim, int zIndex, bool blinkAndZindex, bool cleanBefore)
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
                Source = new BitmapImage(new Uri(Settings.Default.datasPath + unit.RenderValue)),
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

        internal static void DrawSquareRivers(this Panel panel, double size, MapSquarePivot square, double canvasOffsetRatio = 1, int grdPositionOffset = 0)
        {
            if (panel == null)
            {
                return;
            }

            if (!square.RiverTopToBottom.HasValue)
            {
                panel.DrawRiver(size, square, false);
                panel.DrawRiver(size, square, true);
                panel.DrawRiver(size, square, false, canvasOffsetRatio);
                panel.DrawRiver(size, square, true, canvasOffsetRatio);
            }
            else
            {
                panel.DrawRiver(size, square, square.RiverTopToBottom.Value);
                panel.DrawRiver(size, square, square.RiverTopToBottom.Value, canvasOffsetRatio);
            }
        }

        private static void DrawRiver(this Panel panel, double defaultDim, MapSquarePivot square, bool topToBottom, double canvasOffsetRatio = 1, int grdPositionOffset = 0)
        {
            Rectangle riverRect = new Rectangle
            {
                Width = defaultDim / (topToBottom ? 10 : 1),
                Height = defaultDim / (topToBottom ? 1 : 10),
                Fill = Brushes.Blue
            };

            if (panel.GetType() == typeof(Grid))
            {
                riverRect.SetValue(Grid.RowProperty, square.Row - grdPositionOffset);
                riverRect.SetValue(Grid.ColumnProperty, square.Column - grdPositionOffset);
            }
            else
            {
                riverRect.SetValue(Canvas.TopProperty, (square.Row - grdPositionOffset) * canvasOffsetRatio);
                riverRect.SetValue(Canvas.LeftProperty, (square.Column - grdPositionOffset) * canvasOffsetRatio);
            }

            riverRect.Tag = square;
            panel.Children.Add(riverRect);
        }

        internal static void DrawSquareImprovements(this Panel panel, MapSquarePivot square, double defaultDim, int gridOffset = 0)
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
                element.SetValue(Grid.RowProperty, square.Row - gridOffset);
                element.SetValue(Grid.ColumnProperty, square.Column - gridOffset);
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

        internal static void DrawSingleMapSquare(this Panel panel, double defaultDim, MapSquarePivot square, bool cleanPreviousSquare, int gridOffset = 0)
        {
            if (cleanPreviousSquare)
            {
                panel.CleanPreviousChildrenByTag(square);
            }

            FrameworkElement squareRender;
            string imgPath = Settings.Default.datasPath + Settings.Default.squareImageSubFolder + $"{square.Biome.Name}.jpg";
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
                    Width = defaultDim,
                    Height = defaultDim,
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(square.Biome.ColorValue))
                };
            }

            squareRender.SetValue(Grid.RowProperty, square.Row - gridOffset);
            squareRender.SetValue(Grid.ColumnProperty, square.Column - gridOffset);
            squareRender.Tag = square;
            panel.Children.Add(squareRender);

            if (square.CrossedByRiver)
            {
                panel.DrawSquareRivers(defaultDim, square, 1, gridOffset);
            }

            panel.DrawSquareImprovements(square, defaultDim, gridOffset);
        }
    }
}
