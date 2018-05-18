// <copyright file="TabCategory.cs" company="GM">
//     gm.com. All rights reserved.
// </copyright>

namespace SensorsViewer.Home
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using SensorsViewer.SensorOption;

    /// <summary>
    /// Project group left bar
    /// </summary>
    public class TabCategory : INotifyPropertyChanged
    {                
        /// <summary>
        /// Collection of analysis
        /// </summary>
        private ObservableCollection<Analysis> analysis;

        /// <summary>
        /// Collection of sensors
        /// </summary>
        private ObservableCollection<Sensor> sensors;

        /// <summary>
        /// Initializes a new instance of the <see cref="TabCategory"/> class
        /// </summary>
        public TabCategory()
        {
            this.Analysis = new ObservableCollection<Analysis>();
            this.Sensors = new ObservableCollection<Sensor>();
        }
    
        /// <summary>
        /// Initializes a new instance of the <see cref="TabCategory"/> class
        /// </summary>
        /// <param name="path">Model path</param>
        public TabCategory(string path)
        {
            this.Analysis = new ObservableCollection<Analysis>();
            this.Sensors = new ObservableCollection<Sensor>();
        }

        /// <summary>
        /// Event for when change property
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #region Properties Declarations

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
                return this.sensors;
            }

            set
            {
                this.sensors = value;
                this.OnPropertyChanged("Sensors");             
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
                this.OnPropertyChanged("Analysis");
            }
        }

        /// <summary>
        /// Gets a value indicating whether to enable Sensors
        /// </summary>
        public bool EnableSensors
        {
            get
            {
                return this.Analysis.Count == 0 ? true : false;
            }
        }

        #endregion

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