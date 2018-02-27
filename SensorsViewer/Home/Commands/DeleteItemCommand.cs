using SensorsViewer.SensorOption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SensorsViewer.Home.Commands
{
    public class DeleteItemCommand : ICommand
    {
        // Member variables
        private readonly HomeViewModel m_ViewModel;

        public DeleteItemCommand(HomeViewModel viewModel)
        {
            m_ViewModel = viewModel;
        }

        /// <summary>
        /// Whether this command can be executed.
        /// </summary>
        public bool CanExecute(object parameter)
        {
            return (m_ViewModel.SelectedSensor != null);
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
            var sensor = parameter as Sensor;

            var selectedItem = m_ViewModel.SelectedSensor;

            foreach(ProjectGroupVm pg in m_ViewModel.TabCategory)
            {
                if(pg.Sensors.Remove(sensor))
                {
                    break;
                }
            }
        }
    }
}
