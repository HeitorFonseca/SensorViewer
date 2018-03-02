﻿// <copyright file="Sensor.cs" company="GM">
//     gm.com. All rights reserved.
// </copyright>

namespace SensorsViewer.SensorOption
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Class of sensors
    /// </summary>
    public class Sensor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Sensor"/> class
        /// </summary>
        public Sensor()
        {
            this.Id = this.generateID();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sensor"/> class
        /// </summary>
        /// <param name="text"> sensor name </param>
        /// <param name="x">x parameter</param>
        /// <param name="y">y parameter</param>
        /// <param name="z">z parameter</param>
        public Sensor(string text, string x, string y, string z)
        {
            this.SensorName = text;
            this.X = x;
            this.Y = y;
            this.Z = z;

            this.Id = this.generateID();
        }

        /// <summary>
        /// Gets or sets Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets X position
        /// </summary>
        public string X { get; set; }

        /// <summary>
        /// Gets or sets Y position
        /// </summary>
        public string Y { get; set; }

        /// <summary>
        /// Gets or sets Z position
        /// </summary>
        public string Z { get; set; }

        /// <summary>
        /// Gets or sets status
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets Sensor Name
        /// </summary>
        public string SensorName { get; set; }

        /// <summary>
        /// Gets or sets Values
        /// </summary>
        public List<double> Values { get; set; }

        /// <summary>
        /// Gets or sets TimeStamp
        /// </summary>
        public List<double> TimeStamp { get; set; }

        /// <summary>
        /// Gets or sets Parameter
        /// </summary>
        public List<double> Parameter { get; set; }

        /// <summary>
        /// Generate unique Id
        /// </summary>
        /// <returns>generated id</returns>
        public string generateID()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
