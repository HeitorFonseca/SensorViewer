// <copyright file="ProjectItem.cs" company="GM">
//     gm.com. All rights reserved.
// </copyright>

namespace SensorsViewer.Home
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    /// <summary>
    /// Option for left menu bar class
    /// </summary>
    public class ProjectItem : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets os sets idCount
        /// </summary>
        private static int idCount;

        /// <summary>
        /// Gets os sets name
        /// </summary>
        private string name;

        /// <summary>
        /// Gets os sets projects
        /// </summary>
        private ObservableCollection<TabCategory> tabs;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectItem"/> class
        /// </summary>
        public ProjectItem()
        {
            this.Id = idCount++;
            this.Tabs = new ObservableCollection<TabCategory>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectItem"/> class
        /// </summary>
        /// <param name="name">Option name</param>
        /// <param name="modelPath">Path of the model</param>
        public ProjectItem(string name, string modelPath)
        {
            this.Id = idCount++;
            this.Name = name;
            this.ModelPath = modelPath;
            this.AnalysisIndex = 0;

            this.Tabs = new ObservableCollection<TabCategory>
            {
                 new TabCategory(modelPath) { Name = "Draw-In" },
                 new TabCategory(modelPath) { Name = "Adjustment" }
            };

            ////Tabs[0].Analysis.Add(new SensorOption.Analysis("Analysis", "", ""));
            ////Tabs[1].Analysis.Add(new SensorOption.Analysis("Analysis", "", ""));

            ////Tabs[0].ProjectChartContent = new OpticalSensorView();
            ////Tabs[1].ProjectChartContent = new OpticalSensorView();

            ////Tabs[0].ProjectResutContent = new Result.ResultView(ModelPath);
            ////Tabs[1].ProjectResutContent = new Result.ResultView(ModelPath);
        }

        /// <summary>
        /// Event for when change property
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets Id
        /// </summary>
        public int QtdAnalysis { get; set; }

        /// <summary>
        /// Gets or sets Name
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            } 
            
            set
            {
                this.name = value;
                this.OnPropertyChanged("Name");
            }
        }

        /// <summary>
        /// Gets or sets ModelPath
        /// </summary>
        public string ModelPath { get; set; }

        /// <summary>
        /// Gets or sets Selected analysis index
        /// </summary>
        public int AnalysisIndex { get; set; }

        /// <summary>
        /// Gets or sets projects collection
        /// </summary>
        public ObservableCollection<TabCategory> Tabs
        {
            get
            {
                return this.tabs;
            }

            set
            {
                this.tabs = value;
                this.OnPropertyChanged("Tabs");
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
    }
}
