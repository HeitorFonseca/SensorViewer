// <copyright file="ChangeSensorDataCommand.cs" company="GM">
//     gm.com. All rights reserved.
// </copyright>

namespace SensorsViewer.Home.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using System.Windows.Input;
    using SensorsViewer.ProjectB;
    using SensorsViewer.Result;
    using SensorsViewer.SensorOption;

    /// <summary>
    /// Class for when change sensor data command
    /// </summary>
    public class ChangeSensorDataCommand : ICommand
    {
        /// <summary>
        /// view model variable
        /// </summary>
        private readonly HomeViewModel viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeSensorDataCommand"/> class
        /// </summary>
        /// <param name="viewModel">view model parameter</param>
        public ChangeSensorDataCommand(HomeViewModel viewModel)
        {
            this.viewModel = viewModel;
        }
       
        /// <summary>
        /// Fires when the CanExecute status of this command changes.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Whether this command can be executed.
        /// </summary>
        /// <param name="parameter">object parameter</param>
        /// <returns>if can execute command</returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Invokes this command to perform its intended task.        
        /// </summary>
        /// <param name="parameter">object parameter</param>
        public void Execute(object parameter)
        {
            
            var source = ((System.Windows.RoutedEventArgs)parameter).Source;

            Sensor sensor = ((TextBox)source).DataContext as Sensor;

            // Change in tab sensor list
            var element = this.viewModel.SelectedTab.Sensors.FirstOrDefault(a => a.Id == sensor.Id);
            element.SensorName = sensor.SensorName;

            if (this.viewModel.SelectedAnalysis != null && this.viewModel.SelectedAnalysis.ProjectChartContent != null)
            {
                var seriesCollection = this.viewModel.SelectedAnalysis.ProjectChartContent.OpticalSensorViewModel.SeriesCollection;

                // Search in graph sensor and change its name/localization
                foreach (LiveCharts.Wpf.LineSeries ls in seriesCollection)
                {
                    if (ls.Tag.ToString() == sensor.Id)
                    {
                        ls.Title = sensor.SensorName;
                    }
                }
            }
            
            var visibleSensors = this.viewModel.SelectedTab.Sensors.Where(a => a.Visibility == true);
            var obsCol = new ObservableCollection<Sensor>(visibleSensors);

            ((ResultView)this.viewModel.SelectedProjectResultContent).ResultViewModel.LoadSensorsInModel(obsCol);
            
        }
    }
}
