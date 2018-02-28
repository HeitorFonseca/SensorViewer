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
    using SensorsViewer.SensorOption;

    /// <summary>
    /// Project group left bar
    /// </summary>
    public class ProjectGroupVm : INotifyPropertyChanged
    {
        /// <summary>
        /// Project content
        /// </summary>
        private UserControl projectContent;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectGroupVm"/> class
        /// </summary>
        public ProjectGroupVm()
        {
            this.Sensors = new ObservableCollection<Sensor>();
            this.Analysis = new ObservableCollection<Analysis>();
            this.ProjectContent = new UserControl();
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
        public ObservableCollection<Sensor> Sensors { get; set; }

        /// <summary>
        /// Gets or sets Sensors
        /// </summary>
        public ObservableCollection<Analysis> Analysis { get; set; }

        /// <summary>
        /// Gets or sets project B User control content
        /// </summary>
        public UserControl ProjectContent
        {
            get
            {
                return this.projectContent;
            }

            set
            {
                this.projectContent = value;
                this.OnPropertyChanged("ProjectContent");
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