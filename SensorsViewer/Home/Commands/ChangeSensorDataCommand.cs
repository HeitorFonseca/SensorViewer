// <copyright file="ChangeSensorDataCommand.cs" company="GM">
//     gm.com. All rights reserved.
// </copyright>

namespace SensorsViewer.Home.Commands
{
    using SensorsViewer.ProjectB;
    using SensorsViewer.SensorOption;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using System.Windows.Input;

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
            var tab = parameter as TabCategory;

            var source = ((System.Windows.RoutedEventArgs)parameter).Source;

            Sensor sensor = ((TextBox)source).DataContext as Sensor;

            var selectedItem = this.viewModel.SelectedProjectChartContent;

            var list = ((OpticalSensorView)this.viewModel.SelectedProjectChartContent).OpticalSensorViewModel.SensorList;
            var list2 = ((OpticalSensorView)this.viewModel.SelectedProjectChartContent).OpticalSensorViewModel.SeriesCollection;

            foreach (LiveCharts.Wpf.LineSeries ls in list2)
            {
                if (ls.Tag.ToString() == sensor.Id)
                {
                    ls.Title = sensor.SensorName;
                }
            }

            var element = list.FirstOrDefault(a => a.Id == sensor.Id);

            element.SensorName = sensor.SensorName;
        }
    }
}
