// <copyright file="HomeViewModel.cs" company="GM">
//     gm.com. All rights reserved.
// </copyright>

namespace SensorsViewer.Home
{
    using SensorsViewer.Connection;
    using SensorsViewer.ProjectB;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows.Controls;

    /// <summary>
    /// Home view model class
    /// </summary>
    public class HomeViewModel
    {
        /// <summary>
        /// Private enumerable datasource
        /// </summary>
        private readonly IEnumerable<ProjectGroupVm> dataSource;

        /// <summary>
        /// Project A User control content
        /// </summary>
        private UserControl projectAContent;

        /// <summary>
        /// Project B User control content
        /// </summary>
        private UserControl projectBContent;

        /// <summary>
        /// Private project A menu left bar
        /// </summary>
        private IEnumerable<ProjectGroupVm> projectAMenu;

        /// <summary>
        /// Private project b menu left bar
        /// </summary>
        private IEnumerable<ProjectGroupVm> projectBMenu;

        /// <summary>
        /// Private mqtt connection
        /// </summary>
        private MqttConnection proc;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeViewModel"/> class
        /// </summary>
        public HomeViewModel()
        {

            this.proc = new MqttConnection("localhost", 5672, "userTest", "userTest", "hello");
            this.proc.Connect();

            InitializeLeftBarMenu();

            //Set the B project content for OpticalSensorView
            this.projectBContent = (UserControl)(new OpticalSensorView());

            CloseWindowCommand = new RelayCommand(Window_Closing);
        }


        /// <summary>
        /// Event for when change property
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        public RelayCommand CloseWindowCommand { get; set; }

        /// <summary>
        /// Gets or sets project A menu left bar
        /// </summary>
        public IEnumerable<ProjectGroupVm> ProjectAMenu
        {
            get
            {
                return this.projectAMenu;
            }

            set
            {
                this.projectAMenu = value;
                this.OnPropertyChanged("ProjectAMenu");
            }
        }

        /// <summary>
        /// Gets or sets project B menu left bar
        /// </summary>
        public IEnumerable<ProjectGroupVm> ProjectBMenu
        {
            get
            {
                return this.projectBMenu;
            }

            set
            {
                this.projectAMenu = value;
                this.OnPropertyChanged("ProjectBMenu");
            }
        }

        /// <summary>
        /// Gets or sets user control content
        /// </summary>
        public UserControl ProjectBContent
        {
            get
            {
                return this.projectBContent;
            }

            set
            {
                this.projectBContent = value;
                this.OnPropertyChanged("ProjectBContent");
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
        /// Initialize left bar menu
        /// </summary>
        private void InitializeLeftBarMenu()
        {
            this.projectAMenu = new[]
            {
                new ProjectGroupVm
                {
                    Name = "A Project",
                    Items = new[]
                    {
                        new OptionVm("Load STL Model"), new OptionVm("Load Sensors"), new OptionVm("Export csv"), new OptionVm("Export txt")
                    }
                }
            };

            this.projectBMenu = new[]
            {
                new ProjectGroupVm
                {
                    Name = "B Project",
                    Items = new[]
                    {
                        new OptionVm("Export csv"), new OptionVm("Export txt")
                    }
                }
            };           
        }

        /// <summary>
        /// Event when close window
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">event e</param>
        private void Window_Closing()
        {           
            this.proc.Disconnect();           
        }
    }
}
