using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;

using MahApps.Metro.Controls;

namespace SensorsViewer.Home
{
    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class HomeView : MetroWindow

    {
        public HomeView()
        {
            InitializeComponent();
        }
            
        private void RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var sample = (OptionVm)((Border)sender).DataContext;
            var hvm = (HomeViewModel)DataContext;
            hvm.Content = (UserControl)Activator.CreateInstance(sample.Content);
            //hvm.IsMenuOpen = false;
        }
    }


}
