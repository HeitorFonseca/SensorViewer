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
    using Microsoft.Win32;
    using Newtonsoft.Json;
    using RabbitMQ.Client.Events;
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
        /// private selected sensor list
        /// </summary>
        private ObservableCollection<Sensor> selectedSensorList;

        /// <summary>
        /// Path of sensors file
        /// </summary>
        private string fileSensorsPath;

        /// <summary>
        /// Event for test
        /// </summary>
        private System.Windows.Threading.DispatcherTimer dispatcherTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeViewModel"/> class
        /// </summary>
        public HomeViewModel()
        {
            this.proc = new MqttConnection("localhost", 5672, "userTest", "userTest", "hello");
            this.proc.Connect();
            this.proc.ReadDataEvnt(WhenMessageReceived);

            this.InitializeLeftBarMenu();

            this.CloseWindowCommand = new RelayCommand(this.WindowClosingAction);
            this.CreateNewProjectCommand = new RelayCommand(this.CreateNewProjectAction);
            this.DeleteSensorCommand = new DeleteItemCommand(this);
            this.AddNewSensorCommand = new AddSensorCommand(this);
            this.ClickInOptionVmCommand = new ClickInOptionCommand(this);
            this.EditSensorDataCommand = new ChangeSensorDataCommand(this);
            this.BrowseFileCommand = new RelayCommand(this.BrowseFileAction);

            this.tabCategory = new ObservableCollection<ProjectGroupVm>();

            ProjectGroupVm p = new ProjectGroupVm { Name = "Draw-In" };
            ProjectGroupVm p2 = new ProjectGroupVm { Name = "Adjustment" };

            Sensor asd = new Sensor("Sensor 1", "10", "11", "0");
            Sensor asd2 = new Sensor("Sensor 2", "5", "24", "0");
            Sensor asd3 = new Sensor("Sensor 3", "07", "03", "0");

            Analysis an = new Analysis("Analysis 1", "3 FEV 2018", "10:10:01");
            Analysis an2 = new Analysis("Analysis 2", "3 FEV 2018", "10:20:47");

            p.Sensors.Add(asd);
            p.Sensors.Add(asd2);
            p.Sensors.Add(asd3);

            p.Analysis.Add(an);
            p.Analysis.Add(an2);

            p2.Sensors.Add(asd3);
            p2.Analysis.Add(an);            

            p.ProjectContent = (UserControl)(new OpticalSensorView());

            this.SelectedProjectContent = p.ProjectContent;
            this.SelectedSensorList = ((OpticalSensorView)p.ProjectContent).OpticalSensorViewModel.SensorList;

            ((OpticalSensorView)p.ProjectContent).OpticalSensorViewModel.AddSensorToGraph(asd);
            ((OpticalSensorView)SelectedProjectContent).OpticalSensorViewModel.AddValue("Sensor 1", 1.0);

            this.TabCategory.Add(p);
            this.TabCategory.Add(p2);


            //  DispatcherTimer setup
            //dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            //dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
            //dispatcherTimer.Interval = new TimeSpan(0, 0, 5);
            //dispatcherTimer.Start();

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
        ///  Gets or sets browse sensor file
        /// </summary>
        public ICommand BrowseFileCommand { get; set; }

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
        private void WindowClosingAction(object parameter)
        {
            this.proc.Disconnect();
        }

        /// <summary>
        /// Event to create new project
        /// </summary>
        private void CreateNewProjectAction(object parameter)
        {
            this.AnalysisItems.Add(new OptionVm("whatr"));
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

                textBox.Text = System.IO.Path.GetFileName(this.fileSensorsPath);

                string[] lines = System.IO.File.ReadAllLines(this.fileSensorsPath);

                int counter = 1;

                foreach (string line in lines)
                {
                    string[] data = line.Split(' ');

                    Sensor sensor = new Sensor("Sensor Name " + counter, data[0], data[1], data[2]);

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
    }
}
