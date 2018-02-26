// <copyright file="HomeViewModel.cs" company="GM">
//     gm.com. All rights reserved.
// </copyright>

namespace SensorsViewer.Home
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows.Controls;
    using System.Windows.Media;
    using SensorsViewer.Connection;
    using SensorsViewer.ProjectB;
    using SensorsViewer.SensorOption;

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
        private ObservableCollection<OptionVm> ProjectItems;

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
        private ObservableCollection<UserControl> sensorsContent;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeViewModel"/> class
        /// </summary>
        public HomeViewModel()
        {
            this.proc = new MqttConnection("localhost", 5672, "userTest", "userTest", "hello");
            this.proc.Connect();

            this.InitializeLeftBarMenu();

            //Set the B project content for OpticalSensorView
            this.projectBContent = (UserControl)(new OpticalSensorView());

            this.CloseWindowCommand = new RelayCommand(WindowClosingAction);
            this.CreateNewProjectCommand = new RelayCommand(CreateNewProjectAction);

            this.tabCategory = new ObservableCollection<ProjectGroupVm>();

            ProjectGroupVm p = new ProjectGroupVm { Name = "Draw-In", ProjectOptions = new ObservableCollection<ProjectOptions>() };
            ProjectGroupVm p2 = new ProjectGroupVm { Name = "Adjustment", ProjectOptions = new ObservableCollection<ProjectOptions>() };

            UserControl asd = ((UserControl) new SensorOptionView("aaaaaaaaaaa", "10", "11", "0", Colors.WhiteSmoke));
            UserControl asd2 = ((UserControl)new SensorOptionView("eeeeeeeeeee", "5", "24", "0", Colors.WhiteSmoke));
            UserControl asd3 = ((UserControl)new SensorOptionView("iiiiiiiiiii", "07", "03", "0", Colors.WhiteSmoke));

            ProjectOptions po = new ProjectOptions() { OptionName = "Sensors" };
            po.Content.Add(asd);
            po.Content.Add(asd2);
            po.Content.Add(asd3);

            ProjectOptions po2 = new ProjectOptions() { OptionName = "Analysis" };
            po2.Content.Add(asd);

            p.ProjectOptions.Add(po);
            p.ProjectOptions.Add(po2);

            this.TabCategory.Add(p);
            this.TabCategory.Add(p2);
        }

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
        /// Gets or sets sensors content
        /// </summary>
        public ObservableCollection<UserControl> SensorsContent
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
                return this.ProjectItems;
            }

            set
            {
                this.ProjectItems = value;
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
            this.AnalysisItems = new ObservableCollection<OptionVm> { new OptionVm { Title = "test" }, new OptionVm { Title = "test2" } };
        }

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
            AnalysisItems.Add(new OptionVm("whatr"));
        }
    }
}
