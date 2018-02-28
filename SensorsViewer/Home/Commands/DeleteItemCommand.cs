// <copyright file="DeleteItemCommand.cs" company="GM">
//     gm.com. All rights reserved.
// </copyright>

namespace SensorsViewer.Home.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using SensorsViewer.ProjectB;
    using SensorsViewer.SensorOption;

    /// <summary>
    /// Class for delete item command
    /// </summary>
    public class DeleteItemCommand : ICommand
    {
        /// <summary>
        /// View Model variable
        /// </summary>
        private readonly HomeViewModel viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteItemCommand"/> class
        /// </summary>
        /// <param name="viewModel">view model</param>
        public DeleteItemCommand(HomeViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        /// <summary>
        /// Whether this command can be executed.
        /// </summary>
        /// <param name="parameter">object parameter</param>
        /// <returns>if can execute command</returns>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Object parameter
        /// </summary>
        /// <param name="parameter">object parameter</param>
        /// <returns>returns if can execute command</returns>
        public bool CanExecute(object parameter)
        {
            return this.viewModel.SelectedSensor != null;
        }

        /// <summary>
        /// Invokes this command to perform its intended task.
        /// </summary>
        /// <param name="parameter">Object parameter</param>
        public void Execute(object parameter)
        {
            var sensor = parameter as Sensor;

            var selectedItem = this.viewModel.SelectedSensor;

            ((OpticalSensorView)this.viewModel.SelectedProjectContent).OpticalSensorViewModel.RemoveSensorFromGraph(sensor);

            ////foreach (ProjectGroupVm pg in this.viewModel.TabCategory)
            ////{
            ////    if (pg.Sensors.Remove(sensor))
            ////    {
            ////        break;
            ////    }
            ////}
        }
    }
}
