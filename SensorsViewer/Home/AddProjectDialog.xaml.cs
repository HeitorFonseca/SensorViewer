// <copyright file="AddProjectDialog.xaml.cs" company="GM">
//     gm.com. All rights reserved.
// </copyright>

namespace SensorsViewer.Home
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using MahApps.Metro.Controls;
    using MahApps.Metro.Controls.Dialogs;    
    using Microsoft.Win32;

    /// <summary>
    /// Interaction logic for AddProjectDialog.xaml
    /// </summary>
    public partial class AddProjectDialog : MetroWindow, INotifyPropertyChanged
    {
        /// <summary>
        /// Project Name
        /// </summary>
        private string projectName;

        /// <summary>
        /// Model Path
        /// </summary>
        private string modelPath;

        /// <summary>
        /// Projects items
        /// </summary>
        private ObservableCollection<ProjectItem> projectItems;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddProjectDialog"/> class
        /// </summary>
        /// <param name="projectItems">Projects List</param>
        public AddProjectDialog(ObservableCollection<ProjectItem> projectItems)
        {
            this.InitializeComponent();

            this.EnterKeyDownCommand = new RelayCommand(this.EnterKeyDownAction);
            this.projectItems = projectItems;
            this.DataContext = this;
        }

        /// <summary>
        /// Event for when change property
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///  Gets or sets Close window command
        /// </summary>
        public ICommand EnterKeyDownCommand { get; set; }

        /// <summary>
        /// Gets or sets Project name
        /// </summary>
        public string ProjectName
        {
            get
            {
                return this.projectName;
            }

            set
            {
                this.projectName = value;
                this.OnPropertyChanged("ProjectName");
            }
        }

        /// <summary>
        /// Gets or sets model path
        /// </summary>
        public string ModelPath
        {
            get
            {
                return this.modelPath;
            }

            set
            {
                this.modelPath = value;
                this.OnPropertyChanged("ModelPath");
            }
        }

        /// <summary>
        /// When changes property
        /// </summary>
        /// <param name="propertyName">Property name</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Event for when click to create the project
        /// </summary>
        /// <param name="sender">Object Sender</param>
        /// <param name="e">Event e</param>
        private async void OkBtnClickAsync(object sender, RoutedEventArgs e)
        {
            var mySettings = new MetroDialogSettings()
            {
                ColorScheme = MetroDialogOptions.ColorScheme, 
                DialogTitleFontSize = 13,
                DialogMessageFontSize = 17,                
            };

            // Check If the user do not enter the project name
            // Check If the user do not enter the model path
            // Check If the user type a project name that already exist
            // Check If the user enter a model path that not exist
            if (string.IsNullOrEmpty(this.projectName)) 
            {                
                MessageDialogResult result = await this.ShowMessageAsync("Error!", "Empty project name", MessageDialogStyle.Affirmative, mySettings);
            }
            else if (string.IsNullOrEmpty(this.modelPath))  
            {
                MessageDialogResult result = await this.ShowMessageAsync("Error!", "Empty model path", MessageDialogStyle.Affirmative, mySettings);
            }           
            else if (this.CheckIfProjectNameExists(this.projectName))   
            {
                MessageDialogResult result = await this.ShowMessageAsync("Error!", "Project already exist", MessageDialogStyle.Affirmative, mySettings);
            }            
            else if (!File.Exists(this.modelPath))  
            {
                MessageDialogResult result = await this.ShowMessageAsync("Error!", "Model path does not exist", MessageDialogStyle.Affirmative, mySettings);                
            }
            else
            {
                this.DialogResult = true;
            }
        }

        /// <summary>
        /// Event for when the user click in cancel button
        /// </summary>
        /// <param name="sender">Object Sender</param>
        /// <param name="e">Event e</param>
        private void CancelBtnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        /// <summary>
        /// File for when the user click to select a model path
        /// </summary>
        /// <param name="sender">Object Sender</param>
        /// <param name="e">Event e</param>
        private void FileDialogBtnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog()
            {
                Filter = "STL (*.stl)|*.stl"
            };

            if (fileDialog.ShowDialog() == true)
            {
                this.ModelPath = fileDialog.FileName;
            }
        }

        /// <summary>
        /// Check if project name already exist
        /// </summary>
        /// <param name="newName">Project Name</param>
        /// <returns>True if project name exist</returns>
        private bool CheckIfProjectNameExists(string newName)
        {
            foreach (ProjectItem opt in this.projectItems)
            {
                if (opt.Name == newName)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Event when close window
        /// </summary>
        /// <param name="parameter">object parameter</param>
        private void EnterKeyDownAction(object parameter)
        {
            this.OkBtnClickAsync(parameter, new RoutedEventArgs());
        }
    }
}
