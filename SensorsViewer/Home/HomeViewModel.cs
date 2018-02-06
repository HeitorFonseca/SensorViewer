// <copyright file="HomeViewModel.cs" company="GM">
//     gm.com. All rights reserved.
// </copyright>

namespace SensorsViewer.Home
{
    using SensorsViewer.Connection;
    using SensorsViewer.ProjectB;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows.Controls;

    /// <summary>
    /// Home view model class
    /// </summary>
    public class HomeViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Private enumerable datasource
        /// </summary>
        private readonly IList<ProjectGroupVm> dataSource;

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
        private ObservableCollection<OptionVm> analysisItems;

        /// <summary>
        /// Private project A menu left bar
        /// </summary>
        private ObservableCollection<string> tabCategory;

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

            CloseWindowCommand = new RelayCommand(WindowClosingAction);
            CreateNewProjectCommand = new RelayCommand(CreateNewProjectAction);

            tabCategory = new ObservableCollection<string>();


            TabCategory.Add("header1");
            TabCategory.Add("header2");

        }

        /// <summary>
        /// Event for when change property
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        public RelayCommand CloseWindowCommand { get; set; }
        public RelayCommand CreateNewProjectCommand { get; set; }

        public ObservableCollection<string> TabCategory
        {
            get
            {
                return tabCategory;
            }
            set
            {
                this.tabCategory = value;
                this.OnPropertyChanged("TabCategory");
            }
        }

        /// <summary>
        /// Gets or sets Analysis Items
        /// </summary>
        public ObservableCollection<OptionVm> AnalysisItems
        {
            get
            {
                return analysisItems;
            }
            set
            {
                this.analysisItems = value;
                this.OnPropertyChanged("AnalysisItems");
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
            AnalysisItems = new ObservableCollection<OptionVm> { new OptionVm { Title = "test" }, new OptionVm { Title = "test2" }};
        }

        /// <summary>
        /// Event when close window
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">event e</param>
        private void WindowClosingAction()
        {           
            this.proc.Disconnect();           
        }

        private void CreateNewProjectAction()
        {
            AnalysisItems.Add(new OptionVm("whatr"));
        }
    }
}
