using System;
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
using System.Windows.Shapes;
using ErsatzCivLib;

namespace ErsatzCiv
{
    /// <summary>
    /// Logique d'interaction pour WindowAdvancePick.xaml
    /// </summary>
    public partial class WindowAdvancePick : Window
    {
        private Engine _engine;

        public WindowAdvancePick(Engine engine)
        {
            InitializeComponent();
            _engine = engine;
        }
    }
}
