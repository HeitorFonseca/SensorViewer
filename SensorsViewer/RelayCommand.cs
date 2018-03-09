// <copyright file="RelayCommand.cs" company="GM">
//     gm.com. All rights reserved.
// </copyright>

namespace SensorsViewer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Input;

    /// <summary>
    /// Class for relay command
    /// </summary>
    public class RelayCommand : ICommand
    {
        /// <summary>
        /// Action to be executed
        /// </summary>
        private Action<object> action;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class
        /// </summary>
        /// <param name="action">Action to be executed</param>
        public RelayCommand(Action<object> action)
        {
            this.action = action;
        }

        /// <summary>
        /// Event for can execute command
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// If can execute command
        /// </summary>
        /// <param name="parameter">object parameter</param>
        /// <returns>returns If can execute command</returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Execute command
        /// </summary>
        /// <param name="parameter">object parameter</param>
        public void Execute(object parameter)
        {
            this.action(parameter);
        }       
    }
}
