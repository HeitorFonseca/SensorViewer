// <copyright file="HomeView.xaml.cs" company="GM">
//     gm.com. All rights reserved.
// </copyright>

namespace SensorsViewer.Home
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Navigation;
    using MahApps.Metro.Controls;
    using MahApps.Metro.Controls.Dialogs;
    using SensorsViewer.Connection;
    using SensorsViewer.SensorOption;

    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class HomeView : MetroWindow
    {
        /// <summary>
        /// Private home view model
        /// </summary>
        private readonly HomeViewModel homeViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeView"/> class
        /// </summary>
        public HomeView()
        {           
            this.InitializeComponent();

            this.homeViewModel = new HomeViewModel(DialogCoordinator.Instance);

            this.DataContext = this.homeViewModel;
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
            var sample = (ProjectItem)((Border)sender).DataContext;
            var hvm = (HomeViewModel)DataContext;
        }        

        /// <summary>
        /// Find visual child
        /// </summary>
        /// <typeparam name="childItem">Child item</typeparam>
        /// <param name="obj">Object parameter</param>
        /// <returns>return the child of the visual</returns>
        private childItem FindVisualChild<childItem>(DependencyObject obj) where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                {
                    return (childItem)child;
                }
                else
                {
                    childItem childOfChild = this.FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                    {
                        return childOfChild;
                    }
                }
            }

            return null;
        }
    }
}
