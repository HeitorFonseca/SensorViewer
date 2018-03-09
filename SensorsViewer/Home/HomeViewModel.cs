// <copyright file="HomeViewModel.cs" company="GM">
//     gm.com. All rights reserved.
// </copyright>

namespace SensorsViewer.Home
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using MahApps.Metro.Controls.Dialogs;
    using Microsoft.Win32;
    using Newtonsoft.Json;
    using RabbitMQ.Client.Events;
    using SensorsViewer.Connection;
    using SensorsViewer.Home.Commands;
    using SensorsViewer.ProjectB;
    using SensorsViewer.Result;
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
        /// Selected Tab
        /// </summary>
        private ObservableCollection<ProjectGroupVm> selectedTabCategory;

        /// <summary>
        /// Selected tab
        /// </summary>
        private ProjectGroupVm selectedTab;

        /// <summary>
        /// Result content
        /// </summary>
        private UserControl resultContent;

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
        /// Private selected sensor list
        /// </summary>
        private ObservableCollection<Sensor> selectedSensorList;

        /// <summary>
        /// Path of sensors file
        /// </summary>
        private string fileSensorsPath;

        /// <summary>
        /// Tab index
        /// </summary>
        private int tabIndex = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeViewModel"/> class
        /// </summary>
        public HomeViewModel()
        {
            this.proc = new MqttConnection("localhost", 5672, "userTest", "userTest", "hello");
            this.proc.Connect();
            this.proc.ReadDataEvnt(WhenMessageReceived);

            this.InitializeMenu();

            this.CloseWindowCommand = new RelayCommand(this.WindowClosingAction);
            this.CreateNewProjectCommand = new RelayCommand(this.CreateNewProjectAction);
            this.SelectProjectCommand = new RelayCommand(this.SelectProjectAction);
            this.DeleteSensorCommand = new DeleteItemCommand(this);
            this.AddNewSensorCommand = new AddSensorCommand(this);
            this.ClickInOptionVmCommand = new RelayCommand(this.ClickInOptionAction); //new ClickInOptionCommand(this);
            this.EditSensorDataCommand = new ChangeSensorDataCommand(this);
            this.BrowseFileCommand = new RelayCommand(this.BrowseFileAction);
        }

        #region Properties Declarations
        /// <summary>
        /// Event for when change property
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///  Gets or sets Close window command
        /// </summary>
        public ICommand CloseWindowCommand { get; set; }

        /// <summary>
        ///  Gets or sets Create new project command
        /// </summary>
        public ICommand CreateNewProjectCommand { get; set; }

        /// <summary>
        ///  Gets or sets Select project command
        /// </summary>
        public ICommand SelectProjectCommand { get; set; }        

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
        ///  Gets or sets browse sensor file command
        /// </summary>
        public ICommand BrowseFileCommand { get; set; }

        /// <summary>
        ///  Gets or sets sub tab item command
        /// </summary>
        public ICommand ClickInSubTabCommand { get; set; }

        /// <summary>
        /// Gets or sets selected sensor
        /// </summary>
        public ObservableCollection<Sensor> SelectedSensorList
        {
            get
            {
                return this.selectedSensorList;
            }

            set
            {
                this.selectedSensorList = value;
                this.OnPropertyChanged("SelectedSensorList");
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
        /// Gets or sets Project Items
        /// </summary>
        public ObservableCollection<OptionVm> ProjectItems
        {
            get
            {
                return this.projectItems;
            }

            set
            {
                this.projectItems = value;
                this.OnPropertyChanged("ProjectItems");
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
        /// Gets or sets user control content
        /// </summary>
        public ObservableCollection<ProjectGroupVm> SelectedTabCategory
        {
            get
            {
                return this.selectedTabCategory;
            }

            set
            {
                this.selectedTabCategory = value;
                this.OnPropertyChanged("SelectedTabCategory");
            }
        }

        /// <summary>
        /// Gets or sets user control content
        /// </summary>
        public ProjectGroupVm SelectedTab
        {
            get
            {
                return this.selectedTab;
            }

            set
            {
                this.selectedTab = value;
                this.OnPropertyChanged("SelectedTab");
            }
        }

        /// <summary>
        /// Gets or sets user control content
        /// </summary>
        public UserControl ResultContent
        {
            get
            {
                return this.resultContent;
            }

            set
            {
                this.resultContent = value;
                this.OnPropertyChanged("ResultContent");
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

        #region Actions
        /// <summary>
        /// Event when close window
        /// </summary>
        private void WindowClosingAction(object parameter)
        {
            this.proc.Disconnect();
        }

        /// <summary>
        /// Event to create new project
        /// </summary>
        private void CreateNewProjectAction(object parameter)
        {

            AddProjectDialog addProjectDialog = new AddProjectDialog();

            addProjectDialog.ShowDialog();

            string projectName = "", modelPath = "";

            // User clicked OK
            if (addProjectDialog.DialogResult.HasValue && addProjectDialog.DialogResult.Value)
            {
                projectName = addProjectDialog.ProjectName;
                modelPath = addProjectDialog.ModelPath;
                OptionVm newOpt = new OptionVm(projectName, modelPath);
                this.ProjectItems.Add(newOpt);
            }            
        }

        private void SelectProjectAction(object parameter)
        {
            var parent = ((MouseButtonEventArgs)parameter).Source as TextBlock;
            var option = (OptionVm)parent.DataContext;

            this.SelectedTabCategory = option.Tabs;
            
            this.SelectedTab = this.selectedTabCategory[tabIndex];
            this.SelectedProjectContent = option.Tabs[0].ProjectChartContent;
        }

        private void ClickInOptionAction(object parameter)
        {
            var textBlock = ((MouseButtonEventArgs)parameter).Source as TextBlock;
            var dsa = (ProjectGroupVm)textBlock.DataContext;

            this.SelectedProjectContent = dsa.ProjectChartContent;

            if (dsa.Name == "Draw-In")
            {
                this.tabIndex = 0;
            }
            else
            {
                this.tabIndex = 1;
            }
        }

        /// <summary>
        /// Event to open file browser
        /// </summary>
        private void BrowseFileAction(object parameter)
        {
            var textBox = (TextBox)parameter;

            var dialog = new OpenFileDialog();
            dialog.Filter = "Text (*.txt)|*.txt|All Files(*.*)|*.*";

            bool? result = dialog.ShowDialog();

            if ((result.HasValue) && (result.Value))
            {
                this.fileSensorsPath = dialog.FileName;

                ((OpticalSensorView)this.SelectedProjectContent).OpticalSensorViewModel.SensorsFilePath = System.IO.Path.GetFileName(this.fileSensorsPath);

                string[] lines = System.IO.File.ReadAllLines(this.fileSensorsPath);

                int counter = 1;

                foreach (string line in lines)
                {
                    string[] data = line.Split(' ');

                    Sensor sensor = new Sensor("Sensor Name " + counter, Convert.ToDouble(data[0]), Convert.ToDouble(data[1]), Convert.ToDouble(data[2]));

                    ((OpticalSensorView)this.SelectedProjectContent).OpticalSensorViewModel.AddSensorToGraph(sensor);

                    counter++;
                }
            }
        }

        #endregion

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {

            Random rnd = new Random();

            double value2 = Convert.ToDouble(rnd.Next(0, 50));

            ((OpticalSensorView)SelectedProjectContent).OpticalSensorViewModel.AddValue("Sensor 1", value2);

        }

        /// <summary>
        /// Event for when receive a mqtt message
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ea"></param>
        private void WhenMessageReceived(object sender, BasicDeliverEventArgs ea)
        {
            var body = ea.Body;
            var message = Encoding.UTF8.GetString(body);

            JsonData jsonData = JsonConvert.DeserializeObject<JsonData>(message);
            
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                UpdateValue(jsonData);
            }));                            
        }

        private void UpdateValue(JsonData jsonData)
        {
          
            foreach (List<string> data in jsonData.values)
            {
                string sensorName = data[0];
                long timestamp = Convert.ToInt64(data[1]);
                DateTimeOffset dateTimeOffset = DateTimeOffset.Now;

                double value;
                double.TryParse(data[2], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out value);
               
                string parameter = data[3];
                string status = data[4];

                Sensor sensor = new Sensor(sensorName, parameter);
                sensor.Values.Add(value);

                string dateTime = UnixTimeStampToDateTime(timestamp);
                sensor.TimeStamp.Add(dateTime);

                ((OpticalSensorView)SelectedProjectContent).OpticalSensorViewModel.AddValue(sensorName, value);

                ((OpticalSensorView)SelectedProjectContent).OpticalSensorViewModel.AddSensorLogData(sensor);
            }
        }

        private static string UnixTimeStampToDateTime(long unixTimeStamp)
        {
            System.DateTime dtDateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            dtDateTime = dtDateTime.AddMilliseconds(unixTimeStamp);
            return dtDateTime.ToString();
        }


        /// <summary>
        /// Initialize left bar menu
        /// </summary>
        private void InitializeMenu()
        {
            this.ProjectItems = new ObservableCollection<OptionVm>();

            this.tabCategory = new ObservableCollection<ProjectGroupVm>();
            ObservableCollection<ProjectGroupVm> tabCat2 = new ObservableCollection<ProjectGroupVm>();

            ProjectGroupVm p = new ProjectGroupVm { Name = "Draw-In" };
            ProjectGroupVm p2 = new ProjectGroupVm { Name = "Adjustment" };

            ProjectGroupVm t = new ProjectGroupVm { Name = "Draw-In" };
            ProjectGroupVm t2 = new ProjectGroupVm { Name = "Adjustment" };

            Sensor asd = new Sensor("Sensor 1", 10, 11, 0);
            Sensor asd2 = new Sensor("Sensor 2", 5, 24, 0);
            Sensor asd3 = new Sensor("Sensor 3", 7, 3, 0);

            Analysis an = new Analysis("Analysis 1", "3 FEV 2018", "10:10:01");
            Analysis an2 = new Analysis("Analysis 2", "3 FEV 2018", "10:20:47");

            p.Sensors.Add(asd);
            p.Sensors.Add(asd2);
            p.Sensors.Add(asd3);

            p.Analysis.Add(an);
            p.Analysis.Add(an2);

            p2.Sensors.Add(asd3);
            p2.Analysis.Add(an);

            p.ProjectChartContent = (UserControl)(new OpticalSensorView());

            this.SelectedProjectContent = p.ProjectChartContent;
            this.SelectedSensorList = ((OpticalSensorView)p.ProjectChartContent).OpticalSensorViewModel.SensorList;

            ((OpticalSensorView)p.ProjectChartContent).OpticalSensorViewModel.AddSensorToGraph(asd);
            ((OpticalSensorView)SelectedProjectContent).OpticalSensorViewModel.AddValue("Sensor 1", 1.0);

            this.TabCategory.Add(p);
            this.TabCategory.Add(p2);

            tabCat2.Add(t);
            tabCat2.Add(t2);

            OptionVm opt = new OptionVm();
            OptionVm opt2 = new OptionVm();

            opt.Title = "Project 1";
            opt.Tabs = tabCategory;

            this.SelectedTab = opt.Tabs[0];

            opt2.Title = "Project 2";
            opt2.Tabs = tabCat2;


            SelectedTabCategory = tabCategory;

            this.ProjectItems.Add(opt);
            this.ProjectItems.Add(opt2);

            this.ResultContent = new ResultView();
        }
    }
}
