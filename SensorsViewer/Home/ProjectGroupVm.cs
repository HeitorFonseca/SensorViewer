// <copyright file="ProjectGroupVm.cs" company="GM">
//     gm.com. All rights reserved.
// </copyright>

namespace SensorsViewer.Home
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows.Controls;
    using System.Xml.Serialization;
    using SensorsViewer.ProjectB;
    using SensorsViewer.Result;
    using SensorsViewer.SensorOption;

    /// <summary>
    /// Project group left bar
    /// </summary>
    public class ProjectGroupVm : INotifyPropertyChanged
    {
        /// <summary>
        /// Project content
        /// </summary>
        private OpticalSensorView projectChartContent;

        /// <summary>
        /// Project content
        /// </summary>
        private ResultView projectResultContent;

        /// <summary>
        /// Collection of analysis
        /// </summary>
        public ObservableCollection<Analysis> analysis;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectGroupVm"/> class
        /// </summary>
        public ProjectGroupVm()
        {
            this.Analysis = new ObservableCollection<Analysis>();            
            this.ProjectChartContent = new OpticalSensorView();
            this.ProjectResutContent = new ResultView();
        }

        /// <summary>
        /// Event for when change property
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets Sensors
        /// </summary>
        public ObservableCollection<Sensor> Sensors
        {
            get
            {
                return this.ProjectChartContent.OpticalSensorViewModel.SensorList;
            }

            set
            {
                this.ProjectChartContent.OpticalSensorViewModel.SensorList = value;
                this.OnPropertyChanged("ProjectChartContent");
            }
        }

        /// <summary>
        /// Gets or sets Sensors
        /// </summary>
        public ObservableCollection<Analysis> Analysis
        {
            get
            {
                return this.analysis;
            }

            set
            {
                this.analysis = value;
            }
        }

        [XmlIgnore]
        /// <summary>
        /// Gets or sets project B User control content
        /// </summary>
        public OpticalSensorView ProjectChartContent
        {
            get
            {
                return this.projectChartContent;
            }

            set
            {
                this.projectChartContent = value;
                this.OnPropertyChanged("ProjectChartContent");
            }
        }

        [XmlIgnore]
        /// <summary>
        /// Gets or sets project B User control content
        /// </summary>
        public ResultView ProjectResutContent
        {
            get
            {
                return this.projectResultContent;
            }

            set
            {
                this.projectResultContent = value;
                this.OnPropertyChanged("ProjectResutContent");
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