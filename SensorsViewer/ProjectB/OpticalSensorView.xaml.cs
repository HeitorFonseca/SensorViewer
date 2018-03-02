// <copyright file="OpticalSensorView.xaml.cs" company="GM">
//     gm.com. All rights reserved.
// </copyright>

namespace SensorsViewer.ProjectB
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using LiveCharts;

    /// <summary>
    /// Interaction logic for OpticalSensorView.xaml
    /// </summary>
    public partial class OpticalSensorView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpticalSensorView"/> class
        /// </summary>
        public OpticalSensorView()
        {
            this.InitializeComponent();

            this.OpticalSensorViewModel = new OpticalSensorViewModel();

            this.DataContext = this.OpticalSensorViewModel;            
        }

        /// <summary>
        /// Gets or sets optical sensor view model
        /// </summary>
        public OpticalSensorViewModel OpticalSensorViewModel { get; set; }


        private void ListBox_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var item = ItemsControl.ContainerFromElement(ListBox, (DependencyObject)e.OriginalSource) as ListBoxItem;
            if (item == null) return;

            var series = (LiveCharts.Wpf.LineSeries)item.Content;

            series.Visibility = series.Visibility == Visibility.Visible
                ? Visibility.Hidden
                : Visibility.Visible;
        }
    }
}
