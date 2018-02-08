
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

        public SensorOptionView()
        {
            InitializeComponent();           
        }    

        public SensorOptionView(string t, string x, string y, string z, Color c)
        {
            InitializeComponent();

            SensorOptionViewModel model = new SensorOptionViewModel(t, x, y, z);
          
            SensorOptionWindow.Background = new SolidColorBrush(c);

            this.DataContext = model;
        }
    }
}
