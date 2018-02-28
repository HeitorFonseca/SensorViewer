// <copyright file="OpticalSensorView.xaml.cs" company="GM">
//     gm.com. All rights reserved.
// </copyright>

namespace SensorsViewer.ProjectB
{
    using System;
    using System.Windows.Controls;
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
    }
}
