// <copyright file="HomeView.xaml.cs" company="GM">
//     gm.com. All rights reserved.
// </copyright>

namespace SensorsViewer.Home
{
    using System;
    using System.Diagnostics;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Navigation;
    using MahApps.Metro.Controls;
    using SensorsViewer.Connection;

    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class HomeView : MetroWindow
    {
        /// <summary>
        /// Private mqtt connection
        /// </summary>
        private MqttConnection proc;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeView"/> class
        /// </summary>
        public HomeView()
        {
            this.InitializeComponent();

            this.proc = new MqttConnection("localhost", 5672, "userTest", "userTest", "hello");
            this.proc.Connect();
        }

        /// <summary>
        /// Request navigate
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">function event</param>
        private void RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        /// <summary>
        /// Event for when click in button
        /// </summary>
        /// <param name="sender">object event</param>
        /// <param name="e">OnMouseDown event</param>
        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var sample = (OptionVm)((Border)sender).DataContext;
            var hvm = (HomeViewModel)DataContext;
            hvm.ProjectBContent = (UserControl)Activator.CreateInstance(sample.Content);
            ////hvm.IsMenuOpen = false;
        }

        /// <summary>
        /// Event when close window
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">event e</param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.proc.Disconnect();
        }
    }
}
