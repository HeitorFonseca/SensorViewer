// <copyright file="Analysis.cs" company="GM">
//     gm.com. All rights reserved.
// </copyright>

namespace SensorsViewer.SensorOption
{
    using SensorsViewer.ProjectB;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Serialization;

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
        /// Initializes a new instance of the <see cref="Analysis"/> class
        /// </summary>
        public Analysis()
        {
            this.ProjectChartContent = new OpticalSensorView();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Analysis"/> class
        /// </summary>
        /// <param name="name">analysis name</param>
        /// <param name="date">analysis date</param>
        /// <param name="time">analysis time</param>
        public Analysis(string name, string date, string time)
        {
            this.Name = name;
            this.Date = date;
            this.Time = time;

            this.ProjectChartContent = new OpticalSensorView();
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
