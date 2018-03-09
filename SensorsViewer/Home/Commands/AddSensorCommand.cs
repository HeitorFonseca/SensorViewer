// <copyright file="AddSensorCommand.cs" company="GM">
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
    /// Class for add sensor command
    /// </summary>
    public class AddSensorCommand : ICommand
    {
        /// <summary>
        /// View Model Variable
        /// </summary>
        private readonly HomeViewModel viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddSensorCommand"/> class
        /// </summary>
        /// <param name="viewModel">view model</param>
        public AddSensorCommand(HomeViewModel viewModel)
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
        /// <returns>return if can execute</returns>
        public bool CanExecute(object parameter)
        {
            var tab = parameter as ProjectGroupVm;

            return tab.Sensors != null;
        }

        /// <summary>
        /// Invokes this command to perform its intended task.
        /// </summary>
        /// <param name="parameter">object parameter</param>
        public void Execute(object parameter)
        {
            var tab = parameter as ProjectGroupVm;

            ////var selectedItem = this.viewModel.SelectedSensor;

            ////((OpticalSensorView)viewModel.SelectedProjectContent).OpticalSensorViewModel.AddValue("Sensor 1", 1.0);

            Sensor s = new Sensor();
            ((OpticalSensorView)this.viewModel.SelectedProjectContent).OpticalSensorViewModel.AddSensorToGraph(s);

            tab.Sensors.Add(s);
        }
    }
}
