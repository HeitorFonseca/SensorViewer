using SensorsViewer.SensorOption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SensorsViewer.Home.Commands
{
    public class AddSensorCommand : ICommand
    {
        // Member variables
        private readonly HomeViewModel m_ViewModel;

        public AddSensorCommand(HomeViewModel viewModel)
        {
            m_ViewModel = viewModel;
        }

        /// <summary>
        /// Whether this command can be executed.
        /// </summary>
        public bool CanExecute(object parameter)
        {
            var tab = parameter as ProjectGroupVm;

            return (tab.Sensors != null && tab.Sensors.Count > 0);
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
        /// Invokes this command to perform its intended task.
        /// </summary>
        public void Execute(object parameter)
        {
            var tab = parameter as ProjectGroupVm;

            var selectedItem = m_ViewModel.SelectedSensor;


            tab.Sensors.Add(new Sensor());
        }
    }
}
