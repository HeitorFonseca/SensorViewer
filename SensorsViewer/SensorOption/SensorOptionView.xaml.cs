
namespace SensorsViewer.SensorOption
{
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
    using System.Windows.Navigation;
    using System.Windows.Shapes;

    /// <summary>
    /// Interaction logic for SensorOptionView.xaml
    /// </summary>
    public partial class SensorOptionView : UserControl
    {

        public SensorOptionView(string t, Color c)
        {
            InitializeComponent();

            textoT.Text = t;

            SensorOptionWindow.Background = new SolidColorBrush(c);
        }
    }
}
