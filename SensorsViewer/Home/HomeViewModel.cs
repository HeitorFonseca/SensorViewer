// <copyright file="HomeViewModel.cs" company="GM">
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
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using MahApps.Metro.Controls.Dialogs;
    using Microsoft.Win32;
    using Newtonsoft.Json;
    using RabbitMQ.Client.Events;
    using SensorsViewer.Connection;
    using SensorsViewer.Home.Commands;
    using SensorsViewer.Chart;
    using SensorsViewer.Result;
    using SensorsViewer.SensorOption;
    using Winforms = System.Windows.Forms;

    /// <summary>
    /// Home view model class
    /// </summary>
    public class HomeViewModel : INotifyPropertyChanged
    {
        #region Private Variables
        /// <summary>
        /// Home view object
        /// </summary>
        private HomeView homeView;

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
        /// Time difference to create analysis
        /// </summary>
        private int difTimeToCreateAnalysisInMs = 3000;

        /// <summary>
        /// Time for last message received
        /// </summary>
        private DateTime lastMessageReceivedTime;

        /// <summary>
        /// Dialog coordinator for show dialog
        /// </summary>
        private IDialogCoordinator dialogCoordinator;

        /// <summary>
        /// String to current directory
        /// </summary>
        private string currentDirectory;

        /// <summary>
        /// Parameter to indicate which is not going to be interpolated
        /// </summary>
        private string parameterString = "direction";

        #endregion

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

            this.ProjectItems = new ObservableCollection<ProjectItem>();

            this.CloseWindowCommand = new RelayCommand(this.WindowClosingAction);
            this.LoadedWindowCommand = new RelayCommand(this.WindowLoadedActionAsync);
            this.ClickInRenameContextMenu = new RelayCommand(this.ClickInRenameActionAsync);
            this.ClickInDeleteContextMenu = new RelayCommand(this.ClickInDeleteAction);

            this.CreateProjectCommand = new RelayCommand(this.CreateProjectAction);
            this.SelectProjectCommand = new RelayCommand(this.SelectProjectAction);
            this.DeleteSensorCommand = new RelayCommand(this.DeleteSensorAction);
            this.DeleteAnalysisCommand = new RelayCommand(this.DeleteAnalysisAction);

            this.ConnectionSettingsCommand = new RelayCommand(this.ConnectionSettingsActionAsync);

            this.AddNewSensorCommand = new AddSensorCommand(this);
            this.ClickInTabCategoryCommand = new RelayCommand(this.ClickInTabCategoryAction);
            this.EditSensorDataCommand = new ChangeSensorDataCommand(this);
            this.BrowseFileCommand = new RelayCommand(this.BrowseFileAction);

            this.ClickInAnalysisItem = new RelayCommand(this.ClickInAnalysisAction);
            this.ClickInExportToTxtCommand = new RelayCommand(this.ClickInExportToTxtAction);

            this.currentDirectory = System.IO.Directory.GetCurrentDirectory();

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
        ///  Gets or sets Close window command
        /// </summary>
        public ICommand ConnectionSettingsCommand { get; set; }

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
        ///  Gets or sets export to txt command
        /// </summary>
        public ICommand ClickInExportToTxtCommand { get; set; }

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
            foreach (ProjectItem opt in this.ProjectItems)
            {
                foreach (TabCategory tab in opt.Tabs)
                {
                    // Each Tab has its chart and values
                    for (int i = 0; i < tab.Analysis.Count; i++)
                    {
                        if (tab.Analysis[i].NewAnalysis)
                        {
                            tab.Analysis[i].ProjectChartContent.TakeTheChart(tab.Analysis[i].FolderPath);
                        }

                        tab.Analysis[i].NewAnalysis = false;
                    }
                }
            }

            XmlSerialization.WriteToXmlFile<ObservableCollection<ProjectItem>>(this.currentDirectory + @"\Resources\META-INF\persistence.txt", this.ProjectItems);

            // Disconect connection
            if (this.proc != null)
            {
                this.proc.Disconnect();
            }
        }

        /// <summary>
        ///  Event when load window
        /// </summary>
        /// <param name="parameter">Object parameter</param>
        private async void WindowLoadedActionAsync(object parameter)
        {
            try
            {             
                CreateAnalysisFolder(this.currentDirectory + @"\Resources\Analysis");

                this.ProjectItems = XmlSerialization.ReadFromXmlFile<ObservableCollection<ProjectItem>>(this.currentDirectory + @"\Resources\META-INF\persistence.txt");

                App.SplashScreen.AddMessage("Loading application");

                foreach (ProjectItem opt in this.ProjectItems)
                {
                    foreach (TabCategory tab in opt.Tabs)
                    {
                        // Each Tab has its chart and values
                        for (int i = 0; i < tab.Analysis.Count; i++)
                        {
                            App.SplashScreen.AddMessage("Loading " + tab.Analysis[i].Name + " Sensors");
                            ObservableCollection<Sensor> analysisSensors = this.GetSensorsFromAnalysis(tab.Sensors, tab.Analysis[i].SensorsIds);
                            App.SplashScreen.AddMessage("Loading " + tab.Analysis[i].Name + " Chart");
                            tab.Analysis[i].ProjectChartContent.ChartViewModel.ShowLoadedSensors(analysisSensors, tab.Analysis[i].Name);
                            App.SplashScreen.AddMessage("Loading " + tab.Analysis[i].Name + " Model");
                            tab.Analysis[i].ProjectResutContent = new ResultView(analysisSensors, opt.ModelPath, tab.Analysis[i].Name);
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
                    ((ResultView)this.SelectedProjectResultContent).ResultViewModel.LoadSensorsInModel(this.SelectedTab.Sensors.Where(a => a.Visibility == true), string.Empty);
                }
            }
            catch (Exception e)
            {
               ////throw new Exception("Error when load xml file");
            }
            finally
            {
                App.SplashScreen.LoadComplete();                
            }

            //Check if rabbitmq server is installed 
            if (!GetInstalledApps("RabbitMq"))
            {
                this.homeView = (HomeView)((RoutedEventArgs)parameter).Source;

                var mySettings = new MetroDialogSettings()
                {
                    //ColorScheme = MetroDialogOptions.ColorScheme,
                    DialogTitleFontSize = 13,
                    DialogMessageFontSize = 17,
                };

                MessageDialogResult result = await this.homeView.ShowMessageAsync("Error!", "RabbitMQ Server is missing", MessageDialogStyle.Affirmative, mySettings);
            }
            else
            {
                // Create connection
                this.proc = new MqttConnection("localhost", 5672, "userTest", "userTest", "GMTestqueue");
                this.proc.Connect();
                this.proc.ReadDataEvnt(this.WhenMessageReceived);
            }
        }

        /// <summary>
        /// Get sensors from analysis name
        /// </summary>
        /// <param name="tabSensor">Sensors from tab</param>
        /// <param name="sensorsId">Sensors Ids</param>
        /// <returns>Sensors from analysis</returns>
        private ObservableCollection<Sensor> GetSensorsFromAnalysis(ObservableCollection<Sensor> tabSensor, ObservableCollection<string> sensorsId)
        {
            ObservableCollection<Sensor> analysisSensors = new ObservableCollection<Sensor>();

            foreach (Sensor s in tabSensor)
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
        ///  Event to connection settings
        /// </summary>
        /// <param name="parameter">Object parameter</param>
        private async void ConnectionSettingsActionAsync(object parameter)
        {
            if (this.proc == null)
            {
                //Check if rabbitmq server is installed 
                if (GetInstalledApps("RabbitMq"))
                {

                    var mySettings = new MetroDialogSettings()
                    {
                        //ColorScheme = MetroDialogOptions.ColorScheme,
                        DialogTitleFontSize = 13,
                        DialogMessageFontSize = 17,
                    };

                    MessageDialogResult result = await this.homeView.ShowMessageAsync("RabbitMQ Server is missing!", "Can not configure connection", MessageDialogStyle.Affirmative, mySettings);
                }
            }
            else
            {
                ConnectionSettings stt = new ConnectionSettings(this.proc);
                stt.ShowDialog();

                if (stt.DialogResult.HasValue && stt.DialogResult.Value)
                {
                    if (stt.HostName != this.proc.HostName || stt.PortNumber != this.proc.Port.ToString())
                    {
                        this.proc.Disconnect();

                        this.proc = new MqttConnection(stt.HostName, Convert.ToInt32(stt.PortNumber), stt.Username, stt.Password, "GMTestqueue");
                        this.proc.Connect();
                        this.proc.ReadDataEvnt(this.WhenMessageReceived);
                    }
                }
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
            int id = this.SelectedTab.Analysis.Count >= option.AnalysisIndex ? 0 : this.SelectedTab.Analysis.Count-1;

            this.SelectedAnalysis = this.SelectedTab.Analysis.Count > 0 ? this.SelectedTab.Analysis[id] : null;
            this.SelectedProjectChartContent = this.SelectedAnalysis != null ? this.SelectedAnalysis.ProjectChartContent : null;
            this.SelectedProjectResultContent = this.SelectedAnalysis != null ? this.SelectedAnalysis.ProjectResutContent : new ResultView(this.SelectedTab.Sensors.Where(a => a.Visibility == true), option.ModelPath);
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
                this.SelectedProjectResultContent = this.SelectedAnalysis != null ? this.SelectedAnalysis.ProjectResutContent : new ResultView(this.SelectedTab.Sensors.Where(a => a.Visibility == true), this.SelectedProjectItem.ModelPath);
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
            this.SelectedProjectResultContent = dc.Analysis.Count > 0 ? dc.Analysis[0].ProjectResutContent : new ResultView(dc.Sensors.Where(a => a.Visibility == true), this.SelectedProjectItem.ModelPath);

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
              
                ((ResultView)this.SelectedProjectResultContent).ResultViewModel.LoadSensorsInModel(this.SelectedTab.Sensors.Where(a => a.Visibility == true), string.Empty);
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

            // TODO: OPTMIZE
            ((ResultView)this.SelectedProjectResultContent).ResultViewModel.LoadSensorsInModel(this.SelectedTab.Sensors.Where(a => a.Visibility == true), string.Empty);
        }

        /// <summary>
        /// Delete Analysis
        /// </summary>
        /// <param name="parameter">Object parameter</param>
        private void DeleteAnalysisAction(object parameter)
        {
            var analysis = parameter as Analysis;

            // Remove sensor values from this analysis 
            for (int i = 0; i < this.SelectedTab.Sensors.Count; i++) 
            {
                List<SensorValue> sensorValues = this.SelectedTab.Sensors[i].Values.Where(a => a.AnalysisName == analysis.Name).ToList();

                for (int j = 0; j < sensorValues.Count; j++) 
                {
                    this.SelectedTab.Sensors[i].Values.Remove(sensorValues[j]);
                }
            }
            
            // Free interpolation images to after delete the folder
            ((ResultView)analysis.ProjectResutContent).ResultViewModel.FreeTextureImages();

            // Remove the folder with interpolation images
            if (Directory.Exists(analysis.FolderPath))
            {
                Directory.Delete(analysis.FolderPath, true);
            }

            this.SelectedTab.Analysis.Remove(analysis);

            // If there is no analysis
            if (this.SelectedTab.Analysis.Count == 0)
            {
                this.SelectedProjectItem.AnalysisIndex = 0;
                this.SelectedAnalysis = null;
                this.SelectedProjectChartContent = null;
                this.SelectedProjectResultContent = new ResultView(this.SelectedTab.Sensors.Where(a => a.Visibility == true), this.SelectedProjectItem.ModelPath);
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
        /// Click in analysis list item
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

        /// <summary>
        /// Click to export to Txt
        /// </summary>
        /// <param name="parameter">Object parameter</param>
        private void ClickInExportToTxtAction(object parameter)
        {
            var analysis = parameter as Analysis;

            Winforms.FolderBrowserDialog folderDialog = new Winforms.FolderBrowserDialog();
            folderDialog.ShowNewFolderButton = false;
            folderDialog.SelectedPath = System.AppDomain.CurrentDomain.BaseDirectory;

            Winforms.DialogResult result = folderDialog.ShowDialog();
                  
            // If user select some file
            if (result == Winforms.DialogResult.OK)
            {
                string sensorLogPath = folderDialog.SelectedPath  + "\\" + analysis.Name.Replace(':', '-') + ".txt";
                string resultAnalysisPath = folderDialog.SelectedPath + "\\" + "Result " + analysis.Name.Replace(':', '-') + ".txt";

                string sensorLog = analysis.Name + "\n";           
                string resultAnalysisLog = analysis.Name + "\n";

                sensorLog += "Sensor name  X  Y  Z  Value  Timestamp  Parameter\n";
                foreach (Sensor sensor in analysis.ProjectChartContent.ChartViewModel.SensorsLog)
                {
                    sensorLog += sensor.SensorName + " " + sensor.X + " " + sensor.Y + " " + sensor.Z + " " + sensor.Values[0].Value + " " + sensor.Values[0].TimestampStr + " " + sensor.Values[0].Parameter + "\n";
                }

                resultAnalysisLog += "Sensor name  Min  Max  Integral\n";
                foreach (Sensor sensor in analysis.ProjectChartContent.ChartViewModel.SensorList)
                {
                    resultAnalysisLog += sensor.SensorName + " " + sensor.Min + " " + sensor.Max + " " + sensor.Integral + "\n";
                }

                if (File.Exists(sensorLogPath))
                {
                    File.Delete(sensorLogPath);
                }

                File.Create(sensorLogPath).Dispose();

                using (StreamWriter tw = new StreamWriter(sensorLogPath, true))
                {
                    tw.WriteLine(sensorLog);
                }

                if (File.Exists(resultAnalysisPath))
                {
                    File.Delete(resultAnalysisPath);
                }

                File.Create(resultAnalysisPath).Dispose();

                using (StreamWriter tw = new StreamWriter(resultAnalysisPath))
                {
                    tw.WriteLine(resultAnalysisLog);
                }
            }
        }

        #endregion

        #region Private Methods
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

            if (dif >= this.difTimeToCreateAnalysisInMs)
            {
                // Check if the message is addressed to drawin or adjustment
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

                DateTime dateTime = this.UnixTimeStampToDateTime(timestamp);

                SensorValue sv = new SensorValue(value, dateTime, parameter, this.SelectedAnalysis.Name);

                sensor.Values.Add(sv);                                                                                        // Add the value in the created sensor                
                if (sv.Parameter != this.parameterString)
                {
                    ((ChartView)this.SelectedProjectChartContent).ChartViewModel.AddValue(sensorName, value, sv); // Add value in sensor by name               
                }

                ((ChartView)this.SelectedProjectChartContent).ChartViewModel.AddSensorLogData(sensor);        // Add in the sensor log                

                this.AddValueInSensorListTab(sv, sensorName);                                                                 // Add value in tab sensor list
            }

            ((ResultView)this.SelectedProjectResultContent).ResultViewModel.LoadSensorsValuesInModel(this.SelectedTab.Sensors.Where(a => a.Visibility == true && a.Values.Count > 0));

            this.lastMessageReceivedTime = DateTime.Now;
        }

        /// <summary>
        /// Index of tab (if is drawin or adjustment)
        /// </summary>
        /// <param name="index">Index of tab</param>
        private void CreateAnalysis(int index)
        {            
            Analysis newAnalysis = new Analysis(
                                                "Analysis " + DateTime.Now.ToString("HH:mm:ss.fff"), 
                                                DateTime.Now.ToString("dd/MM/yyy"), 
                                                DateTime.Now.ToString("HH:mm:ss.fff"), 
                                                this.SelectedProjectItem.ModelPath, 
                                                this.SelectedTab.Sensors.Where(a => a.Visibility == true));
            
            // Set the list sensor of the graph the same as the sensors list tab
            foreach (Sensor s in this.SelectedTab.Sensors)
            {
                if (s.Visibility == true)
                {
                    Sensor newSensor = new Sensor(s.SensorName, s.X, s.Y, s.Z);
                    newSensor.Id = s.Id;

                    newAnalysis.ProjectChartContent.ChartViewModel.AddSensorToGraph(newSensor);
                    newAnalysis.SensorsIds.Add(newSensor.Id);
                }
            }

            // Select the tab item as Draw-In or Adjustment
            this.SelectedAnalysis = newAnalysis;
            this.SelectedProjectItem.Tabs[index].Analysis.Add(newAnalysis);
            this.SelectedTab = this.SelectedProjectItem.Tabs[index];
            this.SelectedProjectChartContent = newAnalysis.ProjectChartContent;
            this.SelectedProjectResultContent = newAnalysis.ProjectResutContent;
        }

        /// <summary>
        /// Convert timestamp to Datetime
        /// </summary>
        /// <param name="unixTimeStamp">timestamp value</param>
        /// <returns>Date time</returns>
        private DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            System.DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            dateTime = dateTime.AddMilliseconds(unixTimeStamp);
            return dateTime;
        }

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

        /// <summary>
        /// Create analysis directory
        /// </summary>
        /// <param name="path">Directory path</param>
        /// <returns>If the folder already exists</returns>
        private bool CreateAnalysisFolder(string path)
        {
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);

                return true;
            }

            return false;
        }

        private bool GetInstalledApps(string app)
        {
            string uninstallKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(uninstallKey))
            {
                foreach (string skName in rk.GetSubKeyNames())
                {
                    using (RegistryKey sk = rk.OpenSubKey(skName))
                    {
                        try
                        {
                            object ret = sk.GetValue("DisplayName");

                            if (ret.ToString().ToLower().Contains(app.ToLower()))
                            {
                                return true;
                            }
                        }
                        catch (Exception ex)
                        { }
                    }
                }
            }

            return false;
        }
        #endregion
    }
}
