// <copyright file="HomeViewModel.cs" company="GM">
//     gm.com. All rights reserved.
// </copyright>

namespace SensorsViewer.Home
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
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
        private UserControl selectedProjectChartContent;

        /// <summary>
        /// Project B User control content
        /// </summary>
        private UserControl selectedProjectResultContent;

        /// <summary>
        /// Selected Tab
        /// </summary>
        private ObservableCollection<TabCategory> selectedTabCategory;

        /// <summary>
        /// Selected tab
        /// </summary>
        private TabCategory selectedTab;

        /// <summary>
        /// Selected Analysis
        /// </summary>
        private Analysis selectedAnalysis;

        /// <summary>
        /// Result content
        /// </summary>
        private UserControl resultContent;

        /// <summary>
        /// Private project A menu left bar
        /// </summary>
        private ObservableCollection<ProjectItem> projectItems;

        /// <summary>
        /// Private project A menu left bar
        /// </summary>
        private ProjectItem selectedProjectItem;

        /// <summary>
        /// Private project A menu left bar
        /// </summary>
        private ObservableCollection<TabCategory> tabCategory;

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
        /// Time for last message received
        /// </summary>
        private DateTime lastMessageReceivedTime;

        /// <summary>
        /// Dialog coordinator for show dialog
        /// </summary>
        private IDialogCoordinator dialogCoordinator;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeViewModel"/> class
        /// </summary>
        public HomeViewModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeViewModel"/> class
        /// </summary>
        /// <param name="dialogCoordinator">Dialog coordinator to show dialog</param>
        public HomeViewModel(IDialogCoordinator dialogCoordinator)
        {
            this.dialogCoordinator = dialogCoordinator;

            this.proc = new MqttConnection("localhost", 5672, "userTest", "userTest", "hello");
            this.proc.Connect();
            this.proc.ReadDataEvnt(this.WhenMessageReceived);

            this.ProjectItems = new ObservableCollection<ProjectItem>();

            ////this.InitializeMenu();

            this.CloseWindowCommand = new RelayCommand(this.WindowClosingAction);
            this.LoadedWindowCommand = new RelayCommand(this.WindowLoadedAction);
            this.ClickInRenameContextMenu = new RelayCommand(this.ClickInRenameActionAsync);
            this.ClickInDeleteContextMenu = new RelayCommand(this.ClickInDeleteAction);

            this.CreateProjectCommand = new RelayCommand(this.CreateProjectAction);
            this.SelectProjectCommand = new RelayCommand(this.SelectProjectAction);
            this.DeleteSensorCommand = new RelayCommand(this.DeleteSensorAction);
            this.DeleteAnalysisCommand = new RelayCommand(this.DeleteAnalysisAction);

            this.AddNewSensorCommand = new AddSensorCommand(this);
            this.ClickInTabCategoryCommand = new RelayCommand(this.ClickInTabCategoryAction);
            this.EditSensorDataCommand = new ChangeSensorDataCommand(this);
            this.BrowseFileCommand = new RelayCommand(this.BrowseFileAction);

            this.ClickInAnalysisItem = new RelayCommand(this.ClickInAnalysisAction);

            this.lastMessageReceivedTime = DateTime.Now;
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
        public ICommand CreateProjectCommand { get; set; }

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
        public ICommand DeleteAnalysisCommand { get; set; }

        /// <summary>
        ///  Gets or sets delete sensor command
        /// </summary>
        public ICommand ClickInTabCategoryCommand { get; set; }

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
        ///  Gets or sets sub tab item command
        /// </summary>
        public ICommand ClickInAnalysisItem { get; set; }

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
        public ObservableCollection<TabCategory> TabCategory
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
        public ObservableCollection<ProjectItem> ProjectItems
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
        public ProjectItem SelectedProjectItem
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
        /// Gets or sets user control content SelectedProjectContent
        /// </summary>
        public UserControl SelectedProjectChartContent
        {
            get
            {
                return this.selectedProjectChartContent;
            }

            set
            {
                this.selectedProjectChartContent = value;
                this.OnPropertyChanged("SelectedProjectChartContent");
            }
        }

        /// <summary>
        /// Gets or sets user control content SelectedProjectContent
        /// </summary>
        public UserControl SelectedProjectResultContent
        {
            get
            {
                return this.selectedProjectResultContent;
            }

            set
            {
                this.selectedProjectResultContent = value;
                this.OnPropertyChanged("SelectedProjectResultContent");
            }
        }

        /// <summary>
        /// Gets or sets user control content
        /// </summary>
        public ObservableCollection<TabCategory> SelectedTabCategory
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
        public TabCategory SelectedTab
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
        public Analysis SelectedAnalysis
        {
            get
            {
                return this.selectedAnalysis;
            }

            set
            {
                this.selectedAnalysis = value;
                this.OnPropertyChanged("SelectedAnalysis");
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
        /// Gets or sets subtab index
        /// </summary>
        public int SubTabIndex { get; set; }

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
        ///  Event when close window
        /// </summary>
        /// <param name="parameter">Object parameter</param>
        private void WindowClosingAction(object parameter)
        {
            XmlSerialization.WriteToXmlFile<ObservableCollection<ProjectItem>>(@"C:\Users\heitor.araujo\source\repos\SensorViewer\SensorsViewer\bin\Debug\optionVm.txt", this.ProjectItems);
        
            this.proc.Disconnect();
        }

        /// <summary>
        ///  Event when load window
        /// </summary>
        /// <param name="parameter">Object parameter</param>
        private void WindowLoadedAction(object parameter)
        {
            try
            {
                this.ProjectItems = XmlSerialization.ReadFromXmlFile<ObservableCollection<ProjectItem>>(@"C: \Users\heitor.araujo\source\repos\SensorViewer\SensorsViewer\bin\Debug\optionVm.txt");

                foreach (ProjectItem opt in this.ProjectItems)
                {
                    foreach (TabCategory tab in opt.Tabs)
                    {
                        // Each Tab has its chart and values
                        for (int i = 0; i < tab.Analysis.Count; i++)
                        {
                            ObservableCollection<Sensor> analysisSensors = this.GetSensorsFromAnalysis(tab.Sensors, tab.Analysis[i].SensorsIds);

                            tab.Analysis[i].ProjectChartContent.OpticalSensorViewModel.ShowLoadedSensors(analysisSensors, tab.Analysis[i].Name);
                            tab.Analysis[i].ProjectResutContent = new ResultView(analysisSensors, opt.ModelPath);
                        }
                    }
                }

                this.SubTabIndex = 0;
                this.tabIndex = 0;

                this.SelectedProjectItem = this.projectItems[0];                
                this.SelectedTabCategory = this.SelectedProjectItem.Tabs;   // Select the tabs as the new selected project tabs         
                this.SelectedTab = this.selectedTabCategory[this.tabIndex]; // Select the tab item as Draw-In or Adjustment
                this.SelectedAnalysis = this.SelectedTab.Analysis.Count > 0 ? this.SelectedTab.Analysis[this.SelectedProjectItem.AnalysisIndex] : null;
                this.SelectedProjectChartContent = this.SelectedAnalysis != null ? this.SelectedAnalysis.ProjectChartContent : null;
                this.SelectedProjectResultContent = this.SelectedAnalysis != null ? this.SelectedAnalysis.ProjectResutContent : new ResultView(this.SelectedTab.Sensors, this.SelectedProjectItem.ModelPath);

                if (this.SelectedTab.Analysis.Count == 0)
                {
                    ((ResultView)this.SelectedProjectResultContent).ResultViewModel.LoadSensorsInModel(this.SelectedTab.Sensors.Where(a => a.Visibility == true));
                }
            }
            catch (Exception e)
            {
               ////throw new Exception("Error when load xml file");
            }
        }

        private ObservableCollection<Sensor> GetSensorsFromAnalysis(ObservableCollection<Sensor> tabSensor, ObservableCollection<string> sensorsId)
        {
            ObservableCollection<Sensor> analysisSensors = new ObservableCollection<Sensor>();

            foreach (Sensor s in  tabSensor)
            {
                if (sensorsId.Contains(s.Id))
                {
                    analysisSensors.Add(s);
                }
            }

            return analysisSensors;
        }

        /// <summary>
        ///  Event to create project
        /// </summary>
        /// <param name="parameter">Object parameter</param>
        private void CreateProjectAction(object parameter)
        {
            AddProjectDialog addProjectDialog = new AddProjectDialog(this.ProjectItems);
            addProjectDialog.ShowDialog();

            string projectName = string.Empty, modelPath = string.Empty;

            // User clicked OK
            if (addProjectDialog.DialogResult.HasValue && addProjectDialog.DialogResult.Value)
            {
                projectName = addProjectDialog.ProjectName;
                modelPath = addProjectDialog.ModelPath;
                
                ProjectItem newOpt = new ProjectItem(projectName, modelPath);
                this.ProjectItems.Add(newOpt);

                this.SelectedProjectItem = newOpt;

                // Select the tabs as the new selected project tabs
                this.SelectedTabCategory = newOpt.Tabs;

                // Select the tab item as Draw-In or Adjustment
                this.SelectedTab = this.selectedTabCategory[this.tabIndex];

                // Select the project content as the tab index chart graph
                this.SelectedProjectChartContent = newOpt.Tabs[this.tabIndex].Analysis.Count > 0 ? newOpt.Tabs[this.tabIndex].Analysis[0].ProjectChartContent : null;

                // Show the slt model when does not have the analysis yet
                this.SelectedProjectResultContent = newOpt.Tabs[this.tabIndex].Analysis.Count > 0 ? newOpt.Tabs[this.tabIndex].Analysis[0].ProjectResutContent : new ResultView(this.SelectedTab.Sensors, modelPath);
            }
        }

        /// <summary>
        /// Event for when the user click in project item
        /// </summary>
        /// <param name="parameter">Object parameter</param>
        private void SelectProjectAction(object parameter)
        {
            var parent = ((MouseButtonEventArgs)parameter).Source as TextBlock;
            var option = (ProjectItem)parent.DataContext;

            // Select the tabs as the new selected project tabs
            this.SelectedTabCategory = option.Tabs;

            // Select the tab item as Draw-In or Adjustment
            this.SelectedTab = this.SelectedTabCategory[this.tabIndex];

            // Select the project content as the tab index chart graph
            this.SelectedAnalysis = this.SelectedTab.Analysis.Count > 0 ? this.SelectedTab.Analysis[option.AnalysisIndex] : null;
            this.SelectedProjectChartContent = this.SelectedAnalysis != null ? this.SelectedAnalysis.ProjectChartContent : null;
            this.SelectedProjectResultContent = this.SelectedAnalysis != null ? this.SelectedAnalysis.ProjectResutContent : new ResultView(this.SelectedTab.Sensors, option.ModelPath);
        }

        /// <summary>
        /// Event for when click rename in context menu
        /// </summary>
        /// <param name="parameter">Object parameter</param>
        private async void ClickInRenameActionAsync(object parameter)
        {
            var result = await this.dialogCoordinator.ShowInputAsync(this, "Rename project", "Enter Project Name");

            // User pressed cancel
            if (result == null)
            {
                return;
            }

            ProjectItem currentProject = (ProjectItem)parameter;
            
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
            ProjectItem currentProject = (ProjectItem)parameter;
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
                this.SelectedProjectChartContent = null;

                this.SelectedProjectResultContent = null;
            }
            else
            {
                i = (i + (this.ProjectItems.Count - 1)) % this.ProjectItems.Count;

                this.SelectedProjectItem = this.ProjectItems[i];
                this.SelectedTabCategory = this.ProjectItems[i].Tabs;               
                this.SelectedTab = this.SelectedTabCategory[this.tabIndex];  // Select the tab item as Draw-In or Adjustment               
                this.SelectedAnalysis = this.SelectedTab.Analysis.Count > 0 ? this.SelectedTab.Analysis[0] : null;             // Selected analysis               
                this.SelectedProjectChartContent = this.SelectedAnalysis != null ? this.SelectedAnalysis.ProjectChartContent : null;  // Select the project content as the tab index chart graph
                this.SelectedProjectResultContent = this.SelectedAnalysis != null ? this.SelectedAnalysis.ProjectResutContent : new ResultView(this.SelectedTab.Sensors, this.SelectedProjectItem.ModelPath);
            }
        }

        /// <summary>
        /// Event for when click in one of the Tabs 
        /// </summary>
        /// <param name="parameter">Object parameter</param>
        private void ClickInTabCategoryAction(object parameter)
        {
            var textBlock = ((MouseButtonEventArgs)parameter).Source as TextBlock;
            var dc = (TabCategory)textBlock.DataContext;

            this.SelectedProjectChartContent = dc.Analysis.Count > 0 ? dc.Analysis[0].ProjectChartContent : null;
            this.SelectedProjectResultContent = dc.Analysis.Count > 0 ? dc.Analysis[0].ProjectResutContent : new ResultView(this.SelectedTab.Sensors, this.SelectedProjectItem.ModelPath);

            if (dc.Name == "Draw-In")
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
        /// <param name="parameter">Object parameter</param>
        private void BrowseFileAction(object parameter)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Text (*.txt)|*.txt|All Files(*.*)|*.*";

            bool? result = dialog.ShowDialog(); // Show file dialog

            // If user select some file
            if (result.HasValue && result.Value)    
            {
                this.fileSensorsPath = dialog.FileName; // Get the path 
               
                int counter = 1;

                // For each line in file
                foreach (string line in System.IO.File.ReadAllLines(this.fileSensorsPath))         
                {
                    string[] data = line.Split(' ');

                    // Create a sensor with file data
                    Sensor sensor = new Sensor("Sensor Name " + counter++, Convert.ToDouble(data[0]), Convert.ToDouble(data[1]), Convert.ToDouble(data[2]));
             
                    // Add sensor in sensor list
                    this.SelectedTab.Sensors.Add(sensor);
                }
              
                ((ResultView)this.SelectedProjectResultContent).ResultViewModel.LoadSensorsInModel(this.SelectedTab.Sensors.Where(a => a.Visibility == true));
            }
        }

        /// <summary>
        /// Delete sensor 
        /// </summary>
        /// <param name="parameter">Object Parameter</param>
        private void DeleteSensorAction(object parameter)
        {
            var sensor = parameter as Sensor;           

            sensor.Visibility = false;

            //TODO: OPTMIZE

            ((ResultView)this.SelectedProjectResultContent).ResultViewModel.LoadSensorsInModel(this.SelectedTab.Sensors.Where(a => a.Visibility == true));
        }

        /// <summary>
        /// Delete Analysis
        /// </summary>
        /// <param name="parameter">Object parameter</param>
        private void DeleteAnalysisAction(object parameter)
        {
            var analysis = parameter as Analysis;

            this.SelectedTab.Analysis.Remove(analysis);

            // If there is no analysis
            if (this.SelectedTab.Analysis.Count == 0)
            {
                this.SelectedProjectItem.AnalysisIndex = 0;
                this.SelectedAnalysis = null;
                this.SelectedProjectChartContent = null;
                this.SelectedProjectResultContent = new ResultView(this.SelectedTab.Sensors, this.SelectedProjectItem.ModelPath);
            }
            else
            {
                this.SelectedProjectItem.AnalysisIndex = (this.SelectedProjectItem.AnalysisIndex + (this.SelectedTab.Analysis.Count - 1)) % this.SelectedTab.Analysis.Count;
                this.SelectedAnalysis = this.SelectedTab.Analysis[this.SelectedProjectItem.AnalysisIndex];
                this.SelectedProjectChartContent = this.SelectedAnalysis.ProjectChartContent;
                this.SelectedProjectResultContent = this.SelectedAnalysis.ProjectResutContent;
            }
        }

        /// <summary>
        /// DClick in analysis list item
        /// </summary>
        /// <param name="parameter">Object parameter</param>
        private void ClickInAnalysisAction(object parameter)
        {
            Analysis analysis = null;

            if (((MouseButtonEventArgs)parameter).Source is Border src)
            {
                analysis = ((Border)src).DataContext as Analysis;
            }
            else if (((MouseButtonEventArgs)parameter).Source is TextBlock tb)
            {
                analysis = ((TextBlock)tb).DataContext as Analysis;
            }
              
            this.SelectedAnalysis = analysis;

            this.SelectedProjectItem.AnalysisIndex = this.SelectedProjectItem.Tabs[this.tabIndex].Analysis.IndexOf(analysis);

            this.SelectedProjectChartContent = this.SelectedAnalysis.ProjectChartContent;
            this.SelectedProjectResultContent = this.SelectedAnalysis.ProjectResutContent;
        }

        #endregion

        /// <summary>
        /// Event for when receive a mqtt message
        /// </summary>
        /// <param name="sender">Object sender</param>
        /// <param name="ea">Event arguments</param>
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
            double dif = (DateTime.Now - this.lastMessageReceivedTime).TotalMilliseconds;

            if (dif >= 60)
            {
                int index = jsonData.viewer == "drawin" ? 0 : 1;

                this.CreateAnalysis(index);
            }

            // For each sensors sensor values received
            foreach (List<string> data in jsonData.values)
            {
                string sensorName = data[0];                        // Get the sensor name (sensor id)
                long timestamp = Convert.ToInt64(data[1]);          // Get the timestamp                
                double.TryParse(data[2], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double value);   // Get the sensor value             
                string parameter = data[3];                         // Get the parameter
                string status = data[4];                            // Get the sensor status

                Sensor sensor = new Sensor(sensorName);             // Create the sensor with name and parameter

                string dateTime = this.UnixTimeStampToDateTime(timestamp);

                SensorValue sv = new SensorValue(value, dateTime, parameter, this.SelectedAnalysis.Name);

                sensor.Values.Add(sv);                                                                                        // Add the value in the created sensor                
                ((OpticalSensorView)this.SelectedProjectChartContent).OpticalSensorViewModel.AddValue(sensorName, value, sv); // Add value in sensor by name               
                ((OpticalSensorView)this.SelectedProjectChartContent).OpticalSensorViewModel.AddSensorLogData(sensor);        // Add in the sensor log

                this.AddValueInSensorListTab(sv, sensorName);
            }

            this.lastMessageReceivedTime = DateTime.Now;
        }

        /// <summary>
        /// Index of tab (if is drawin or adjustment)
        /// </summary>
        /// <param name="index">Index of tab</param>
        private void CreateAnalysis(int index)
        {            
            Analysis newAnalysis = new Analysis("Analysis " + DateTime.Now.ToString("HH:mm:ss.fff"), 
                                                DateTime.Now.ToString("dd/MM/yyy"), DateTime.Now.ToString("HH:mm:ss.fff"), 
                                                this.SelectedProjectItem.ModelPath, this.SelectedTab.Sensors);
            
            // Set the list sensor of the graph the same as the sensors list tab
            foreach (Sensor s in this.SelectedTab.Sensors)
            {
                if (s.Visibility == true)
                {
                    Sensor newSensor = new Sensor(s.SensorName, s.X, s.Y, s.Z);
                    newSensor.Id = s.Id;

                    newAnalysis.ProjectChartContent.OpticalSensorViewModel.AddSensorToGraph(newSensor);
                    newAnalysis.SensorsIds.Add(newSensor.Id);
                }
            }

            //newAnalysis.ProjectResutContent.ResultViewModel.LoadSensorsInModel(this.SelectedTab.Sensors.Where(a => a.Visibility == true));

            // Select the tab item as Draw-In or Adjustment
            this.SelectedAnalysis = newAnalysis;
            this.SelectedProjectItem.Tabs[index].Analysis.Add(newAnalysis);
            this.SelectedTab = this.SelectedProjectItem.Tabs[index];
            this.SelectedProjectChartContent = newAnalysis.ProjectChartContent;
            this.SelectedProjectResultContent = newAnalysis.ProjectResutContent;
        }

        /// <summary>
        /// Initialize left bar menu
        /// </summary>
        private void InitializeMenu()
        {
            ////this.tabCategory = new ObservableCollection<TabCategory>();
            ////ObservableCollection<TabCategory> tabCat2 = new ObservableCollection<TabCategory>();

            ////TabCategory p = new TabCategory { Name = "Draw-In" };
            ////TabCategory p2 = new TabCategory { Name = "Adjustment" };

            ////TabCategory t = new TabCategory { Name = "Draw-In" };
            ////TabCategory t2 = new TabCategory { Name = "Adjustment" };

            ////Sensor asd = new Sensor("Sensor 1", 10, 11, 0);
            ////Sensor asd2 = new Sensor("Sensor 2", 5, 24, 0);
            ////Sensor asd3 = new Sensor("Sensor 3", 7, 3, 0);

            ////p.Sensors.Add(asd);
            ////p.Sensors.Add(asd2);
            ////p.Sensors.Add(asd3);

            ////Analysis an = new Analysis("Analysis 1", "3 FEV 2018", "10:10:01");
            ////Analysis an2 = new Analysis("Analysis 2", "3 FEV 2018", "10:20:47");

            ////p.Analysis.Add(an);
            ////p.Analysis.Add(an2);

            ////p2.Sensors.Add(asd3);
            ////p2.Analysis.Add(an);

            ////p.ProjectChartContent = new OpticalSensorView();
            ////p2.ProjectChartContent = new OpticalSensorView();

            ////this.SelectedProjectChartContent = p.ProjectChartContent;
            ////this.SelectedSensorList = ((OpticalSensorView)p.ProjectChartContent).OpticalSensorViewModel.SensorList;

            ////((OpticalSensorView)p.ProjectChartContent).OpticalSensorViewModel.AddSensorToGraph(asd);
            ////((OpticalSensorView)SelectedProjectChartContent).OpticalSensorViewModel.AddValue("Sensor 1", 1.0);

            ////this.TabCategory.Add(p);
            ////this.TabCategory.Add(p2);

            ////tabCat2.Add(t);
            ////tabCat2.Add(t2);

            ////ProjectItem opt = new ProjectItem();
            ////ProjectItem opt2 = new ProjectItem();

            ////opt.Name = "Project 1";
            ////opt.Tabs = this.tabCategory;

            ////this.SelectedTab = opt.Tabs[0];

            ////opt2.Name = "Project 2";
            ////opt2.Tabs = tabCat2;

            ////XmlSerialization.WriteToXmlFile<ProjectItem>(@"C:\Users\heitor.araujo\source\repos\SensorViewer\SensorsViewer\bin\Debug\optionVm.txt", opt);

            ////this.SelectedTabCategory = this.tabCategory;

            ////this.ProjectItems.Add(opt);
            ////this.ProjectItems.Add(opt2);

            ////XmlSerialization.WriteToXmlFile<ObservableCollection<ProjectItem>>(@"C:\Users\heitor.araujo\source\repos\SensorViewer\SensorsViewer\bin\Debug\optionVm.txt", this.ProjectItems);

            ////this.ResultContent = new ResultView();
        }

        /// <summary>
        /// Convert timestamp to Datetime
        /// </summary>
        /// <param name="unixTimeStamp">timestamp value</param>
        /// <returns>Date time</returns>
        private string UnixTimeStampToDateTime(long unixTimeStamp)
        {
            System.DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            dateTime = dateTime.AddMilliseconds(unixTimeStamp);
            return dateTime.ToString("dd/MM/yyy HH:mm:ss.fff");
        }

        ////private string GetNextCheckAnalysisName(int index)
        ////{
        ////    foreach (Analysis analysis in this.SelectedProjectItem.Tabs[index].Analysis)

        ////    return "Analysis " + (this.SelectedProjectItem.Tabs[index].Analysis.Count + 1);
        ////}

        /// <summary>
        /// Add value in tab sensor list
        /// </summary>
        /// <param name="sv">Sensor value</param>
        /// <param name="sensorName">Sensor name</param>
        private void AddValueInSensorListTab(SensorValue sv, string sensorName)
        {
            for (int i = 0; i < this.SelectedTab.Sensors.Count; i++)
            {
                if (this.SelectedTab.Sensors[i].SensorName == sensorName)
                {
                    this.SelectedTab.Sensors[i].Values.Add(sv);
                }
            }
        }
    }
}
