using System.Windows;

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
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Not implemented yet !");
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            new MapGeneratorWindow().ShowDialog();
            ShowDialog();
        }
    }
}
