﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
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

        public MainWindow()
        {
            InitializeComponent();

            MiniMapCanvas.Height = MENU_HEIGHT;
            MiniMapCanvas.Width = MENU_HEIGHT * MapData.RATIO_WIDTH_HEIGHT;

            var map = new MapData(MapData.MapSizeEnum.VeryLarge, 5, 4);
            for (int i = 0; i < map.Width; i++)
            {
                MapGrid.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(DEFAULT_SIZE)
                });
            }
            for (int j = 0; j < map.Height; j++)
            {
                MapGrid.RowDefinitions.Add(new RowDefinition
                {
                    Height = new GridLength(DEFAULT_SIZE)
                });
            }

            double minimapSquareSize = (double)MENU_HEIGHT / map.Height;

            foreach (var square in map.MapSquareList)
            {
                Rectangle rct = new Rectangle
                {
                    Stroke = Brushes.DarkGray,
                    StrokeThickness = 1,
                    Width = DEFAULT_SIZE,
                    Height = DEFAULT_SIZE,
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(square.MapSquareType.RenderColor))
                };
                rct.SetValue(Grid.RowProperty, square.Row);
                rct.SetValue(Grid.ColumnProperty, square.Column);
                MapGrid.Children.Add(rct);

                Rectangle rctMinimap = new Rectangle
                {
                    Width = minimapSquareSize,
                    Height = minimapSquareSize,
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(square.MapSquareType.RenderColor))
                };
                rctMinimap.SetValue(Canvas.TopProperty, square.Row * minimapSquareSize);
                rctMinimap.SetValue(Canvas.LeftProperty, square.Column * minimapSquareSize);
                MiniMapCanvas.Children.Add(rctMinimap);
            }
        }
    }
}
