// <copyright file="OpticalSensor.cs" company="GM">
//     gm.com. All rights reserved.
// </copyright>

namespace SensorsViewer.ProjectB
{
    using System;
    using System.Collections.Generic;
    using LiveCharts.Wpf;

    /// <summary>
    /// Optical Sensor Class
    /// </summary>
    public class OpticalSensor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpticalSensor"/> class
        /// </summary>
        public OpticalSensor()
        {
            this.Values = new List<double>();
            this.TimeStamp = new List<double>();
            LnSerie = new LineSeries { };
        }

        /// <summary>
        /// Gets or sets status
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets Sensorid
        /// </summary>
        public string Sensorid { get; set; }

        /// <summary>
        /// Gets or sets Values
        /// </summary>
        public List<double> Values { get; set; }

        /// <summary>
        /// Gets or sets TimeStamp
        /// </summary>
        public List<double> TimeStamp { get; set; }

        /// <summary>
        /// Gets or sets Line serie
        /// </summary>
        public LineSeries LnSerie { get; set; }
    }
}