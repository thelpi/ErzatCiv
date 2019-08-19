using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ErsatzCiv.Properties;
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

            if (Settings.Default.datasPath == Settings.Default.defaultDatasPath)
            {
                Settings.Default.datasPath = string.Concat(
                    System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase),
                    Settings.Default.datasPath);
                Settings.Default.datasPath = Settings.Default.datasPath.Replace("file:\\", string.Empty);
                Settings.Default.Save();
            }

            if (!System.IO.Directory.Exists(Settings.Default.datasPath))
            {
                MessageBox.Show($"The {Settings.Default.defaultDatasPath} folder inside the app folder doesn't exist !", "ErsatzCiv");
                Environment.Exit(0);
            }

            (GridContent.Background as ImageBrush).ImageSource = DrawTools.GetBitmap("intro", isJpg: true);
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                InitialDirectory = Settings.Default.datasPath + Settings.Default.savesSubFolder
            };
            if (openFileDialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(openFileDialog.FileName))
            {
                var desRes = ErsatzCivLib.EnginePivot.DeserializeSave(openFileDialog.FileName);
                if (string.IsNullOrWhiteSpace(desRes.Item2))
                {

                    Hide();
                    new MainWindow(desRes.Item1).ShowDialog();
                    ShowDialog();
                }
                else
                {
                    MessageBox.Show($"The save can't be loaded because of the following error : {desRes.Item2}", "ErsatzCiv");
                }
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
