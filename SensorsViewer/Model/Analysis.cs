// <copyright file="Analysis.cs" company="GM">
//     gm.com. All rights reserved.
// </copyright>

namespace SensorsViewer.SensorOption
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Serialization;
    using SensorsViewer.ProjectB;
    using SensorsViewer.Result;

    /// <summary>
    /// Class of Analysis
    /// </summary>
    public class Analysis : INotifyPropertyChanged
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
        /// Initializes a new instance of the <see cref="Analysis"/> class
        /// </summary>
        public Analysis()
        {
            this.ProjectChartContent = new OpticalSensorView();
            this.SensorsIds = new ObservableCollection<string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Analysis"/> class
        /// </summary>
        /// <param name="name">Analysis name</param>
        /// <param name="date">Analysis date</param>
        /// <param name="time">Analysis time</param>
        /// <param name="path">Analysis Model path</param>
        public Analysis(string name, string date, string time, string path, IEnumerable<Sensor> sensors)
        {
            this.Name = name;
            this.Date = date;
            this.Time = time;
            this.SensorsIds = new ObservableCollection<string>();
            this.ProjectChartContent = new OpticalSensorView();
            this.ProjectResutContent = new ResultView(sensors, path);
        }

        /// <summary>
        /// Event for when change property
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets analysis name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets Y position
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// Gets or sets Z position
        /// </summary>
        public string Time { get; set; }

        public ObservableCollection<string> SensorsIds { get; set; }

        /// <summary>
        /// Gets or sets project B User control content
        /// </summary>
        [XmlIgnore]
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

        /// <summary>
        /// Gets or sets project B User control content
        /// </summary>
        [XmlIgnore]
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
