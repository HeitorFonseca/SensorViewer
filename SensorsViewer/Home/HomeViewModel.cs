﻿// <copyright file="HomeViewModel.cs" company="GM">
//     gm.com. All rights reserved.
// </copyright>

namespace SensorsViewer.Home
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using SensorsViewer.Connection;
    using SensorsViewer.Home.Commands;
    using SensorsViewer.ProjectB;
    using SensorsViewer.SensorOption;

    /// <summary>
    /// Home view model class
    /// </summary>
    public class HomeViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Project B User control content
        /// </summary>
        private UserControl selectedProjectContent;

        /// <summary>
        /// Private project A menu left bar
        /// </summary>
        private ObservableCollection<OptionVm> projectItems;

        /// <summary>
        /// Private project A menu left bar
        /// </summary>
        private ObservableCollection<ProjectGroupVm> tabCategory;

        /// <summary>
        /// Private mqtt connection
        /// </summary>
        private MqttConnection proc;

        /// <summary>
        /// Private Sensors Content
        /// </summary>
        private ObservableCollection<Sensor> sensorsContent;

        /// <summary>
        /// private selected sensor
        /// </summary>
        private Sensor selectedSensor;

        private System.Windows.Threading.DispatcherTimer dispatcherTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeViewModel"/> class
        /// </summary>
        public HomeViewModel()
        {
            this.proc = new MqttConnection("localhost", 5672, "userTest", "userTest", "hello");
            this.proc.Connect();

            this.InitializeLeftBarMenu();

            ////Set the B project content for OpticalSensorView
            this.SelectedProjectContent = (UserControl)(new OpticalSensorView());

            this.CloseWindowCommand = new RelayCommand(this.WindowClosingAction);
            this.CreateNewProjectCommand = new RelayCommand(this.CreateNewProjectAction);
            this.DeleteSensorCommand = new DeleteItemCommand(this);
            this.AddNewSensorCommand = new AddSensorCommand(this);
            this.ClickInOptionVmCommand = new ClickInOptionCommand(this);
            this.EditSensorDataCommand = new ChangeSensorDataCommand(this);

            this.tabCategory = new ObservableCollection<ProjectGroupVm>();

            ProjectGroupVm p = new ProjectGroupVm { Name = "Draw-In" };
            ProjectGroupVm p2 = new ProjectGroupVm { Name = "Adjustment" };

            Sensor asd = new Sensor("Sensor 1", "10", "11", "0");
            Sensor asd2 = new Sensor("Sensor 2", "5", "24", "0");
            Sensor asd3 = new Sensor("Sensor 3", "07", "03", "0");

            Analysis an = new Analysis("Analysis 1", "3 FEV 2018", "10:10:01");
            Analysis an2 = new Analysis("Analysis 2", "3 FEV 2018", "10:20:47");

            OpticalSensor optSensor = new OpticalSensor("SensorOpt", "5.4", "8", "4.2");

            p.Sensors.Add(asd);
            p.Sensors.Add(asd2);
            p.Sensors.Add(asd3);

            p.Analysis.Add(an);
            p.Analysis.Add(an2);

            p2.Sensors.Add(asd3);
            p2.Analysis.Add(an);            

            p.ProjectContent = (UserControl)(new OpticalSensorView());

            this.SelectedProjectContent = p.ProjectContent;

            ////((OpticalSensorView)p.ProjectContent).test.SeriesCollection.Add(new LiveCharts.Wpf.LineSeries
            ////{
            ////    Title = "Series 3",
            ////    Values = new LiveCharts.ChartValues<double> { 7, 3, 2, 5, 8 }
            ////});

            ((OpticalSensorView)p.ProjectContent).OpticalSensorViewModel.AddSensorToGraph(asd);

            this.TabCategory.Add(p);
            this.TabCategory.Add(p2);

            this.selectedSensor = new Sensor();

            //  DispatcherTimer setup
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 5);
            dispatcherTimer.Start();

        }

        #region Properties Declarations
        /// <summary>
        /// Event for when change property
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///  Gets or sets Close window command
        /// </summary>
        public RelayCommand CloseWindowCommand { get; set; }

        /// <summary>
        ///  Gets or sets Create new project command
        /// </summary>
        public RelayCommand CreateNewProjectCommand { get; set; }

        /// <summary>
        ///  Gets or sets Create new project command
        /// </summary>
        public ICommand AddNewSensorCommand { get; set; }

        /// <summary>
        ///  Gets or sets delete sensor command
        /// </summary>
        public ICommand DeleteSensorCommand { get; set; }

        /// <summary>
        ///  Gets or sets delete sensor command
        /// </summary>
        public ICommand ClickInOptionVmCommand { get; set; }

        /// <summary>
        ///  Gets or sets delete sensor command
        /// </summary>
        public ICommand EditSensorDataCommand { get; set; }

        /// <summary>
        /// Gets or sets selected sensor
        /// </summary>
        public Sensor SelectedSensor
        {
            get
            {
                return this.selectedSensor;
            }

            set
            {
                this.selectedSensor = value;
                this.OnPropertyChanged("SelectedSensor");
            }
        }

        /// <summary>
        /// Gets or sets sensors content
        /// </summary>
        public ObservableCollection<Sensor> SensorsContent
        {
            get
            {
                return this.sensorsContent;
            }

            set
            {
                this.sensorsContent = value;
                this.OnPropertyChanged("SensorsContent");
            }
        }

        /// <summary>
        ///  Gets or sets Tab category
        /// </summary>
        public ObservableCollection<ProjectGroupVm> TabCategory
        {
            get
            {
                return this.tabCategory;
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
                return this.projectItems;
            }

            set
            {
                this.projectItems = value;
                this.OnPropertyChanged("AnalysisItems");
            }
        }

        /// <summary>
        /// Gets or sets user control content
        /// </summary>
        public UserControl SelectedProjectContent
        {
            get
            {
                return this.selectedProjectContent;
            }

            set
            {
                this.selectedProjectContent = value;
                this.OnPropertyChanged("SelectedProjectContent");
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

        #endregion

        /// <summary>
        /// Initialize left bar menu
        /// </summary>
        private void InitializeLeftBarMenu()
        {
            this.AnalysisItems = new ObservableCollection<OptionVm> { new OptionVm { Title = "test" }, new OptionVm { Title = "test2" } };
        }

        #region Actions
        /// <summary>
        /// Event when close window
        /// </summary>
        private void WindowClosingAction()
        {
            this.proc.Disconnect();
        }

        /// <summary>
        /// Evento to create new project
        /// </summary>
        private void CreateNewProjectAction()
        {
            this.AnalysisItems.Add(new OptionVm("whatr"));
        }
        #endregion

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {

            Random rnd = new Random();

            double value = rnd.Next(0, 50);

            ((OpticalSensorView)SelectedProjectContent).OpticalSensorViewModel.AddValue("Sensor 1", value);


        }
    }
}
