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
                var desRes = ErsatzCivLib.Engine.DeserializeSave(openFileDialog.FileName);
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
