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
    public class OpticalSensor : SensorOption.Sensor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpticalSensor"/> class
        /// </summary>
        /// <param name="text">optical sensor id</param>
        /// <param name="x">sensor x</param>
        /// <param name="y">sensor y</param>
        /// <param name="z">sensor z</param>
        public OpticalSensor(string text, double x, double y, double z) : base(text, x, y, z)
        {
            this.Values = new List<double>();
            this.TimeStamp = new List<string>();
            this.LnSerie = new LineSeries { };
        }

        /// <summary>
        /// Gets or sets Line serie
        /// </summary>
        public LineSeries LnSerie { get; set; }
    }
}