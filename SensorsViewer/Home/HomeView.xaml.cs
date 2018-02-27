﻿// <copyright file="HomeView.xaml.cs" company="GM">
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
    using SensorsViewer.Connection;
    using SensorsViewer.SensorOption;

    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class HomeView : MetroWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HomeView"/> class
        /// </summary>
        public HomeView()
        {           
            this.InitializeComponent();           
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

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var tab = sender as TabItem;
            var tab2 = sender as TextBlock;

            var asdasd = this.TabCategoryTabItems;

            TabControl tas = FindVisualChild<TabControl>(this.TabCategoryTabItems);

            tas.SelectedIndex = 0;

            if (tab != null)
            {
                // this tab is selected!
            }
        }

        private childItem FindVisualChild<childItem>(DependencyObject obj) where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                    return (childItem)child;
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        private void delSensorBtn_Click(object sender, RoutedEventArgs e)
        {
            // var asd = ((Button)sender).DataContext;

            //object clicked = (e.OriginalSource as FrameworkElement).DataContext;
            //var lbi = listbo ItemContainerGenerator.ContainerFromItem(clicked) as ListBoxItem;
            
            //lbi.IsSelected = true;

        }
    }
}
