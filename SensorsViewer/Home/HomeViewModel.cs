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
        private OptionVm selectedProjectItem;

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

        private IDialogCoordinator dialogCoordinator;
        /// <summary>
        /// Initializes a new instance of the <see cref="HomeViewModel"/> class
        /// </summary>
        public HomeViewModel(IDialogCoordinator dialogCoordinator)
        {
            this.dialogCoordinator = dialogCoordinator;

            this.proc = new MqttConnection("localhost", 5672, "userTest", "userTest", "hello");
            this.proc.Connect();
            this.proc.ReadDataEvnt(WhenMessageReceived);

            this.ProjectItems = new ObservableCollection<OptionVm>();

            //this.InitializeMenu();

            this.CloseWindowCommand = new RelayCommand(this.WindowClosingAction);
            this.LoadedWindowCommand = new RelayCommand(this.WindowLoadedAction);
            this.ClickInRenameContextMenu = new RelayCommand(this.ClickInRenameActionAsync);
            this.ClickInDeleteContextMenu = new RelayCommand(this.ClickInDeleteAction);

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
        ///  Gets or sets Loaded window command
        /// </summary>
        public ICommand LoadedWindowCommand { get; set; }

        /// <summary>
        ///  Gets or sets Loaded window command
        /// </summary>
        public ICommand ClickInRenameContextMenu { get; set; }

        /// <summary>
        ///  Gets or sets Loaded window command
        /// </summary>
        public ICommand ClickInDeleteContextMenu { get; set; }

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
        /// Gets or sets selected project item
        /// </summary>
        public OptionVm SelectedProjectItem
        {
            get
            {
                return this.selectedProjectItem;
            }

            set
            {
                this.selectedProjectItem = value;
                this.OnPropertyChanged("SelectedProjectItem");
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
            XmlSerialization.WriteToXmlFile<ObservableCollection<OptionVm>>(@"C:\Users\heitor.araujo\source\repos\SensorViewer\SensorsViewer\bin\Debug\optionVm.txt", this.ProjectItems);
        
            this.proc.Disconnect();
        }

        /// <summary>
        /// Event when close window
        /// </summary>
        private void WindowLoadedAction(object parameter)
        {
            try
            {

                this.ProjectItems = XmlSerialization.ReadFromXmlFile<ObservableCollection<OptionVm>>(@"C: \Users\heitor.araujo\source\repos\SensorViewer\SensorsViewer\bin\Debug\optionVm.txt");

                foreach (OptionVm opt in this.ProjectItems)
                {
                    foreach (ProjectGroupVm tab in opt.Tabs)
                    {
                        tab.ProjectChartContent.OpticalSensorViewModel.ShowLoadedSensors();
                    }
                }

                this.tabIndex = 0;

                this.SelectedProjectItem = this.projectItems[0];

                // Select the tabs as the new selected project tabs
                this.SelectedTabCategory = SelectedProjectItem.Tabs;
                // Select the tab item as Draw-In or Adjustment
                this.SelectedTab = this.selectedTabCategory[this.tabIndex];

                this.SelectedProjectContent = SelectedProjectItem.Tabs[this.tabIndex].ProjectChartContent;
            }
            catch(Exception e)
            {
                //throw new Exception("Error when load xml file");
            }

        }

        /// <summary>
        /// Event to create new project
        /// </summary>
        private void CreateNewProjectAction(object parameter)
        {
            AddProjectDialog addProjectDialog = new AddProjectDialog(this.ProjectItems);

            addProjectDialog.ShowDialog();

            string projectName = "", modelPath = "";

            // User clicked OK
            if (addProjectDialog.DialogResult.HasValue && addProjectDialog.DialogResult.Value)
            {
                projectName = addProjectDialog.ProjectName;
                modelPath = addProjectDialog.ModelPath;
                OptionVm newOpt = new OptionVm(projectName, modelPath);
                this.ProjectItems.Add(newOpt);

                this.SelectedProjectItem = newOpt;
                // Select the tabs as the new selected project tabs
                this.SelectedTabCategory = newOpt.Tabs;
                // Select the tab item as Draw-In or Adjustment
                this.SelectedTab = this.selectedTabCategory[this.tabIndex];
                // Select the project content as the tab index chart graph
                this.SelectedProjectContent = newOpt.Tabs[this.tabIndex].ProjectChartContent;
            }                  
        }

        /// <summary>
        /// Event for when the user click in project item
        /// </summary>
        /// <param name="parameter">object parameter</param>
        private void SelectProjectAction(object parameter)
        {
            var parent = ((MouseButtonEventArgs)parameter).Source as TextBlock;
            var option = (OptionVm)parent.DataContext;

            //var option = (OptionVm)parameter;

            // Select the tabs as the new selected project tabs
            this.SelectedTabCategory = option.Tabs;
            // Select the tab item as Draw-In or Adjustment
            this.SelectedTab = this.selectedTabCategory[this.tabIndex];
            // Select the project content as the tab index chart graph
            this.SelectedProjectContent = option.Tabs[this.tabIndex].ProjectChartContent;
        }

        /// <summary>
        /// Event for when click rename in context menu
        /// </summary>
        /// <param name="parameter">Object parameter</param>
        private async void ClickInRenameActionAsync(object parameter)
        {
            var result = await this.dialogCoordinator.ShowInputAsync(this,"Rename project", "Enter Project Name");

            if (result == null) //user pressed cancel
                return;

            OptionVm currentProject = (OptionVm)parameter;
            
            for (int i = 0; i < this.ProjectItems.Count; i++)
            {
                if (this.ProjectItems[i] == currentProject)
                {
                    this.ProjectItems[i].Name = result;
                }
            }
        }

        /// <summary>
        /// Event for when click delete in context menu
        /// </summary>
        /// <param name="parameter">Object parameter</param>
        private void ClickInDeleteAction(object parameter)
        {
            OptionVm currentProject = (OptionVm)parameter;
            int i = 0;

            for (i = 0; i < this.ProjectItems.Count; i++)
            {
                if (this.ProjectItems[i] == currentProject)
                {
                    break;
                }
            }
            
            if (i == this.ProjectItems.Count)
            {
                throw new Exception("Project item not found");
            }

            this.ProjectItems.RemoveAt(i);

            if (this.ProjectItems.Count == 0)
            {
                this.SelectedProjectItem = null;

                this.SelectedTabCategory = null;
                // Select the tab item to null
                this.SelectedTab = null;
                // Select the project content to null
                this.SelectedProjectContent = null;
            }
            else {

                i = (i + (this.ProjectItems.Count - 1)) % this.ProjectItems.Count;

                this.SelectedProjectItem = this.ProjectItems[i];

                this.SelectedTabCategory = this.ProjectItems[i].Tabs;
                // Select the tab item as Draw-In or Adjustment
                this.SelectedTab = this.selectedTabCategory[this.tabIndex];
                // Select the project content as the tab index chart graph
                this.SelectedProjectContent = this.ProjectItems[i].Tabs[this.tabIndex].ProjectChartContent;
            }
        }

        /// <summary>
        /// Event for when click in one of the Tabs 
        /// </summary>
        /// <param name="parameter">Object parameter</param>
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
        /// Event to open file browser for upload sensor file
        /// </summary>
        private void BrowseFileAction(object parameter)
        {

            var dialog = new OpenFileDialog();
            dialog.Filter = "Text (*.txt)|*.txt|All Files(*.*)|*.*";

            bool? result = dialog.ShowDialog();         // Show file dialog

            if ((result.HasValue) && (result.Value))    // If user select some file
            {
                this.fileSensorsPath = dialog.FileName; // Get the path 

                ((OpticalSensorView)this.SelectedProjectContent).OpticalSensorViewModel.SensorsFilePath = System.IO.Path.GetFileName(this.fileSensorsPath); // Get the filename of the path
               
                int counter = 1;
                // For each line in file
                foreach (string line in System.IO.File.ReadAllLines(this.fileSensorsPath))         
                {
                    string[] data = line.Split(' ');
                    // Create a sensor with file data
                    Sensor sensor = new Sensor("Sensor Name " + counter++, Convert.ToDouble(data[0]), Convert.ToDouble(data[1]), Convert.ToDouble(data[2]));
                    // Add the created sensor in graph
                    ((OpticalSensorView)this.SelectedProjectContent).OpticalSensorViewModel.AddSensorToGraph(sensor);
                }
            }
        }

        #endregion

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
                UpdateSensorChart(jsonData);
            }));                            
        }

        /// <summary>
        /// Update sensor chart graph
        /// </summary>
        /// <param name="jsonData">Json data received</param>
        private void UpdateSensorChart(JsonData jsonData)
        {
            //For each sensors sensor values received
            foreach (List<string> data in jsonData.values)
            {
                string sensorName = data[0];                        // Get the sensor name (sensor id)
                long timestamp = Convert.ToInt64(data[1]);          // Get the timestamp                
                double.TryParse(data[2], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double value);   // Get the sensor value             
                string parameter = data[3];                         // Get the parameter
                string status = data[4];                            // Get the sensor status

                Sensor sensor = new Sensor(sensorName, parameter);  // Create the sensor with name and parameter
                sensor.Values.Add(value);                           // Add the value in the created sensor

                string dateTime = UnixTimeStampToDateTime(timestamp);
                sensor.TimeStamp.Add(dateTime);                     // Add timestamp in the created sensor

                //Add value in sensor by name
                ((OpticalSensorView)SelectedProjectContent).OpticalSensorViewModel.AddValue(sensorName, value);
                //Add in the sensor log
                ((OpticalSensorView)SelectedProjectContent).OpticalSensorViewModel.AddSensorLogData(sensor);
            }
        }

        /// <summary>
        /// Convert timestamp to Datetime
        /// </summary>
        /// <param name="unixTimeStamp">timestamp value</param>
        /// <returns>Date time</returns>
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

            this.tabCategory = new ObservableCollection<ProjectGroupVm>();
            //ObservableCollection<ProjectGroupVm> tabCat2 = new ObservableCollection<ProjectGroupVm>();

            ProjectGroupVm p = new ProjectGroupVm { Name = "Draw-In" };
            ProjectGroupVm p2 = new ProjectGroupVm { Name = "Adjustment" };

            //ProjectGroupVm t = new ProjectGroupVm { Name = "Draw-In" };
            //ProjectGroupVm t2 = new ProjectGroupVm { Name = "Adjustment" };

            Sensor asd = new Sensor("Sensor 1", 10, 11, 0);
            //Sensor asd2 = new Sensor("Sensor 2", 5, 24, 0);
            //Sensor asd3 = new Sensor("Sensor 3", 7, 3, 0);

            p.Sensors.Add(asd);
            //p.Sensors.Add(asd2);
            //p.Sensors.Add(asd3);

            Analysis an = new Analysis("Analysis 1", "3 FEV 2018", "10:10:01");
            Analysis an2 = new Analysis("Analysis 2", "3 FEV 2018", "10:20:47");

            p.Analysis.Add(an);
            p.Analysis.Add(an2);

            //p2.Sensors.Add(asd3);
            p2.Analysis.Add(an);

            p.ProjectChartContent = new OpticalSensorView();
            p2.ProjectChartContent = new OpticalSensorView();

            this.SelectedProjectContent = p.ProjectChartContent;
            this.SelectedSensorList = ((OpticalSensorView)p.ProjectChartContent).OpticalSensorViewModel.SensorList;

            ((OpticalSensorView)p.ProjectChartContent).OpticalSensorViewModel.AddSensorToGraph(asd);
            ((OpticalSensorView)SelectedProjectContent).OpticalSensorViewModel.AddValue("Sensor 1", 1.0);

            this.TabCategory.Add(p);
            this.TabCategory.Add(p2);

            //tabCat2.Add(t);
            //tabCat2.Add(t2);

            OptionVm opt = new OptionVm();
            //OptionVm opt2 = new OptionVm();

            opt.Name = "Project 1";
            opt.Tabs = this.tabCategory;

            this.SelectedTab = opt.Tabs[0];

            //opt2.Title = "Project 2";
            //opt2.Tabs = tabCat2;

            //XmlSerialization.WriteToXmlFile<OptionVm>(@"C:\Users\heitor.araujo\source\repos\SensorViewer\SensorsViewer\bin\Debug\optionVm.txt", opt);

            this.SelectedTabCategory = this.tabCategory;

            this.ProjectItems.Add(opt);
            //this.ProjectItems.Add(opt2);

            XmlSerialization.WriteToXmlFile<ObservableCollection<OptionVm>>(@"C:\Users\heitor.araujo\source\repos\SensorViewer\SensorsViewer\bin\Debug\optionVm.txt", this.ProjectItems);

            this.ResultContent = new ResultView();
        }
    }
}
