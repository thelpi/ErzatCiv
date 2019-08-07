using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ErsatzCivLib;
using ErsatzCivLib.Model.Enums;
using ErsatzCivLib.Model.Static;

namespace ErsatzCiv
{
    /// <summary>
    /// Logique d'interaction pour WindowAdvancePick.xaml
    /// </summary>
    public partial class WindowAdvancePick : Window
    {
        private EnginePivot _engine;

        public WindowAdvancePick(EnginePivot engine)
        {
            InitializeComponent();
            _engine = engine;
            ComboBoxAdvancePick.ItemsSource = _engine.HumanPlayer.GetAvailableAdvances();
            ComboBoxAdvancePick.SelectedItem = _engine.HumanPlayer.CurrentAdvance;
            ComboBoxEra.ItemsSource = Enum.GetValues(typeof(EraPivot));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ComboBoxAdvancePick.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a scientific advance !", "ErsatzCiv");
                e.Cancel = true;
            }
            else
            {
                if (!_engine.ChangeCurrentAdvance((AdvancePivot)ComboBoxAdvancePick.SelectedItem))
                {
                    MessageBox.Show("Failure of advance change !", "ErsatzCiv");
                }
            }
        }

        private void ComboBoxEra_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxEra.SelectedIndex >= 0)
            {
                var advances = AdvancePivot.AdvancesByEra[(EraPivot)ComboBoxEra.SelectedItem].ToList();

                GridAdvances.Children.Clear();
                GridAdvances.RowDefinitions.Clear();
                for (int i = 0; i <= advances.Count; i++)
                {
                    GridAdvances.RowDefinitions.Add(new RowDefinition { MinHeight = 25 });

                    var lbl1 = new Label
                    {
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        HorizontalContentAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        VerticalContentAlignment = VerticalAlignment.Center
                    };
                    var lbl2 = new Label
                    {
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        HorizontalContentAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        VerticalContentAlignment = VerticalAlignment.Center
                    };
                    var lbl3 = new Label
                    {
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        HorizontalContentAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        VerticalContentAlignment = VerticalAlignment.Center
                    };

                    if (i == 0)
                    {
                        lbl1.Content = "Name";
                        lbl2.Content = "Requires";
                        lbl3.Content = "Leads to";
                        lbl1.Background = Brushes.Gainsboro;
                        lbl2.Background = Brushes.Gainsboro;
                        lbl3.Background = Brushes.Gainsboro;
                    }
                    else
                    {
                        var iReal = i - 1;

                        var prerequisitesTxt = new StringBuilder();
                        foreach (var pr in advances[iReal].Prerequisites)
                        {
                            if (!string.IsNullOrWhiteSpace(pr?.Name))
                            {
                                prerequisitesTxt.AppendLine(pr.Name);
                            }
                        }

                        lbl1.Content = advances[iReal].Name;
                        lbl2.Content = prerequisitesTxt.ToString();
                        lbl3.Content = string.Empty;
                    }

                    lbl1.SetValue(Grid.ColumnProperty, 0);
                    lbl2.SetValue(Grid.ColumnProperty, 1);
                    lbl3.SetValue(Grid.ColumnProperty, 2);

                    lbl1.SetValue(Grid.RowProperty, i);
                    lbl2.SetValue(Grid.RowProperty, i);
                    lbl3.SetValue(Grid.RowProperty, i);

                    GridAdvances.Children.Add(lbl1);
                    GridAdvances.Children.Add(lbl2);
                    GridAdvances.Children.Add(lbl3);
                }
            }
        }
    }
}
