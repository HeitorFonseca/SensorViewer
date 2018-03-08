// <copyright file="ClickInOptionCommand.cs" company="GM">
//     gm.com. All rights reserved.
// </copyright>

namespace SensorsViewer.Home.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Class for click in option command
    /// </summary>
    public class ClickInOptionCommand : ICommand
    {
        /// <summary>
        /// View Model Variable
        /// </summary>
        private readonly HomeViewModel viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClickInOptionCommand"/> class
        /// </summary>
        /// <param name="viewModel">view model parameter</param>
        public ClickInOptionCommand(HomeViewModel viewModel)
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
            var asd = ((MouseButtonEventArgs)parameter).Source as TextBlock;
            var dsa = (ProjectGroupVm)asd.DataContext;

            ////this.viewModel.SelectedProjectContent = dsa.ProjectContent;
            ////var tab = parameter as ProjectGroupVm;

            return dsa.ProjectContent != null;
        }

        /// <summary>
        /// Invokes this command to perform its intended task.
        /// </summary>
        /// <param name="parameter">object parameter</param>
        public void Execute(object parameter)
        {
            var textBlock = ((MouseButtonEventArgs)parameter).Source as TextBlock;
            var dsa = (ProjectGroupVm)textBlock.DataContext;

            this.viewModel.SelectedProjectContent = dsa.ProjectContent;

            ////tab.Sensors.Add(new Sensor());
        }
    }
}
