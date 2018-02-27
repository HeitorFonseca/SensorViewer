// <copyright file="ProjectGroupVm.cs" company="GM">
//     gm.com. All rights reserved.
// </copyright>

namespace SensorsViewer.Home
{
    using SensorsViewer.SensorOption;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Project group left bar
    /// </summary>
    public class ProjectGroupVm
    {
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

        public ProjectGroupVm()
        {
            Sensors = new ObservableCollection<Sensor>();
            Analysis = new ObservableCollection<Analysis>();

        }
    }
}