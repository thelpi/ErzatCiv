using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace ErsatzCiv
{
    /// <summary>
    /// Interaction logic for the introduction window.
    /// </summary>
    public partial class IntroWindow : Window
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public IntroWindow()
        {
            InitializeComponent();
            (GridContent.Background as ImageBrush).ImageSource =
                new BitmapImage(new System.Uri(Properties.Settings.Default.datasPath + "intro.jpg"));
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                InitialDirectory = Properties.Settings.Default.datasPath + Properties.Settings.Default.savesSubFolder
            };
            if (openFileDialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(openFileDialog.FileName))
            {
                try
                {
                    ErsatzCivLib.EnginePivot.DeserializeSave(openFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"The save can't be loaded because of the following error : {ex.Message}", "ErsatzCiv");
                    return;
                }

                Hide();
                new MainWindow().ShowDialog();
                ShowDialog();
            }
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            new MapGeneratorWindow().ShowDialog();
            ShowDialog();
        }
    }
}
