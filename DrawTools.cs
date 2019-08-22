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
using Civ = ErsatzCivLib.Model.Static.CivilizationPivot;

namespace ErsatzCiv
{
    internal static class DrawTools
    {
        // [filename (with color if required) / output bitmap]
        private static readonly Dictionary<string, BitmapImage> _imagedSourcesCache = new Dictionary<string, BitmapImage>();

        internal const string DEFAULT_RESOURCE_FORMAT = "ErsatzCiv.datas.{0}.png";
        internal const string DEFAULT_RESOURCE_FORMAT_JPG = "ErsatzCiv.datas.{0}.jpg";
        internal const string UNIT_RESOURCE_FORMAT = "ErsatzCiv.datas.units.{0}.png";
        internal const string BIOME_RESOURCE_FORMAT = "ErsatzCiv.datas.biomes.{0}.png";
        internal const string BIOME_BONUS_RESOURCE_FORMAT = "ErsatzCiv.datas.biomes.bonus.{0}.png";
        internal const string CITIZEN_RESOURCE_FORMAT = "ErsatzCiv.datas.citizens.{0}.png";

        internal const int DEFAULT_PIXEL_COLOR_VALUE_R = 235;
        internal const int DEFAULT_PIXEL_COLOR_VALUE_G = 235;
        internal const int DEFAULT_PIXEL_COLOR_VALUE_B = 235;
        internal const double HUT_DISPLAY_RATIO = 0.8;

        internal static readonly Dictionary<string, string> CIVILIZATION_COLORS = new Dictionary<string, string>
        {
            { Civ.American.Name, "#08dede" },
            { Civ.Aztec.Name, "#fff890" },
            { Civ.Babylonian.Name, "#5fdf60" },
            { Civ.Chinese.Name, "#08dede" },
            { Civ.Egyptian.Name, "#fff890" },
            { Civ.English.Name, "#ff50ff" },
            { Civ.French.Name, "#7088ff" },
            { Civ.German.Name, "#7088ff" },
            { Civ.Greek.Name, "#ff50ff" },
            { Civ.Indian.Name, "#80878f" },
            { Civ.Japanese.Name, "#5fdf60" },
            { Civ.Mongolian.Name, "#80878f" },
            { Civ.Roman.Name, "#FFFFFF" },
            { Civ.Russian.Name, "#FFFFFF" },
            { Civ.Zulu.Name, "#fff890" },
            { Civ.Barbarian.Name, "#DC143C" }
        };
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
            { Biome.River.Name, "#1E90FF" }
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
                Source = GetUnitBitmap(unit),
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
                DrawImageOnSquare(defaultDim, newElementsWithZIndex, 0.5, "mine", 2);
            }
            if (square.Pollution)
            {
                DrawImageOnSquare(defaultDim, newElementsWithZIndex, 0.8, "pollution", 3);
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

        private static void DrawImageOnSquare(double defaultDim, Dictionary<FrameworkElement, int> newElementsWithZIndex, double ratio, string contentName, int zIndex)
        {
            Image img = new Image
            {
                Width = defaultDim * ratio,
                Height = defaultDim * ratio,
                Source = GetBitmap(contentName),
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

            var squareRender = new Border
            {
                Child = new Image
                {
                    Width = defaultDim,
                    Height = defaultDim,
                    Source = GetBitmap(square.Biome.Name, isBiome: true, isBiomeBonus: square.HasBonus),
                    Stretch = Stretch.Uniform,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                }
            };

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

            Label lbl = new Label
            {
                Width = defaultDim,
                Height = defaultDim,
                Content = string.Concat(city.CitizensCount, "\r\n", city.Name),
                Background = Brushes.Transparent,
                FontSize = 14,
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center
            };
            lbl.SetValue(Grid.RowProperty, city.MapSquareLocation.Row - (gridPositionOffset == null ? 0 : gridPositionOffset.Item1));
            lbl.SetValue(Grid.ColumnProperty, city.MapSquareLocation.Column - (gridPositionOffset == null ? 0 : gridPositionOffset.Item2));
            lbl.SetValue(Panel.ZIndexProperty, cityZindex + 1);
            lbl.Tag = city;

            Rectangle img = new Rectangle
            {
                Width = defaultDim,
                Height = defaultDim,
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(CIVILIZATION_COLORS[city.Player.Civilization.Name]))
            };
            img.SetValue(Grid.RowProperty, city.MapSquareLocation.Row - (gridPositionOffset == null ? 0 : gridPositionOffset.Item1));
            img.SetValue(Grid.ColumnProperty, city.MapSquareLocation.Column - (gridPositionOffset == null ? 0 : gridPositionOffset.Item2));
            img.SetValue(Panel.ZIndexProperty, cityZindex);
            img.Tag = city;

            if (mouseLeftButtonDownCallback != null)
            {
                img.MouseLeftButtonDown += new MouseButtonEventHandler(mouseLeftButtonDownCallback);
            }

            panel.Children.Add(lbl);
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
                Width = defaultDim * HUT_DISPLAY_RATIO,
                Height = defaultDim * HUT_DISPLAY_RATIO,
                Source = GetBitmap("hut"),
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

        internal static BitmapImage GetBitmap(string baseName, bool isBiome = false, bool isBiomeBonus = false, bool isCitizen = false, bool isJpg = false)
        {
            string resourcePathFormat = isJpg ? DEFAULT_RESOURCE_FORMAT_JPG : DEFAULT_RESOURCE_FORMAT;
            if (isBiomeBonus)
            {
                resourcePathFormat = BIOME_BONUS_RESOURCE_FORMAT;
            }
            else if (isBiome)
            {
                resourcePathFormat = BIOME_RESOURCE_FORMAT;
            }
            else if (isCitizen)
            {
                resourcePathFormat = CITIZEN_RESOURCE_FORMAT;
            }

            baseName = string.Format(resourcePathFormat, baseName.Trim().ToLowerInvariant());

            if (_imagedSourcesCache.ContainsKey(baseName))
            {
                return _imagedSourcesCache[baseName];
            }

            var resourceStream =
                System.Reflection.Assembly
                    .GetEntryAssembly()
                    .GetManifestResourceStream(baseName);

            var bitmapImage = StreamToBitmapImage(resourceStream);

            _imagedSourcesCache.Add(baseName, bitmapImage);

            return bitmapImage;
        }

        internal static BitmapImage GetUnitBitmap(UnitPivot unit)
        {
            var cacheKey = string.Concat(unit.Name.ToLowerInvariant(), "_", CIVILIZATION_COLORS[unit.Player.Civilization.Name]);
            if (_imagedSourcesCache.ContainsKey(cacheKey))
            {
                return _imagedSourcesCache[cacheKey];
            }
            
            var resourceStream =
                System.Reflection.Assembly
                    .GetEntryAssembly()
                    .GetManifestResourceStream(string.Format(UNIT_RESOURCE_FORMAT, unit.Name.ToLowerInvariant()));

            var scrBitmap = new System.Drawing.Bitmap(resourceStream);

            var newColor = System.Drawing.ColorTranslator.FromHtml(CIVILIZATION_COLORS[unit.Player.Civilization.Name]);

            var newBitmap = new System.Drawing.Bitmap(scrBitmap.Width, scrBitmap.Height);
            for (var i = 0; i < scrBitmap.Width; i++)
            {
                for (var j = 0; j < scrBitmap.Height; j++)
                {
                    var actualColor = scrBitmap.GetPixel(i, j);
                    if (actualColor.G == DEFAULT_PIXEL_COLOR_VALUE_G
                        && actualColor.B == DEFAULT_PIXEL_COLOR_VALUE_B
                        && actualColor.R == DEFAULT_PIXEL_COLOR_VALUE_R)
                    {
                        newBitmap.SetPixel(i, j, newColor);
                    }
                    else
                    {
                        newBitmap.SetPixel(i, j, actualColor);
                    }
                }
            }

            BitmapImage bitmapImage;
            using (var memory = new System.IO.MemoryStream())
            {
                newBitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;
                bitmapImage = StreamToBitmapImage(memory);
            }

            _imagedSourcesCache.Add(cacheKey, bitmapImage);

            return bitmapImage;
        }

        private static BitmapImage StreamToBitmapImage(System.IO.Stream resourceStream)
        {
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = resourceStream;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            return bitmapImage;
        }
    }
}
