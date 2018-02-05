// <copyright file="MainWindow.xaml.cs" company="GM">
//     gm.com. All rights reserved.
// </copyright>

namespace SensorsViewer
{
    using System;
    using System.Windows;
    using SensorsViewer.Connection;
    using SensorsViewer.Home;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();            
        }

        /// <summary>
        /// On button click event
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">button event</param>
        private void OnClick(object sender, RoutedEventArgs e)
        {
            HomeView h = new HomeView();

            h.ShowDialog();
        }
    }
}
