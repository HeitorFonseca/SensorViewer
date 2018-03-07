// <copyright file="Sensor.cs" company="GM">
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
            this.TimeStamp = new List<string>();
            this.Values = new List<double>();
            this.Id = this.generateID();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sensor"/> class
        /// </summary>
        /// <param name="sensorName"> sensor name </param>
        /// <param name="x">x parameter</param>
        /// <param name="y">y parameter</param>
        /// <param name="z">z parameter</param>
        public Sensor(string sensorName, double x, double y, double z)
        {
            this.SensorName = sensorName;
            this.X = x;
            this.Y = y;
            this.Z = z;

            this.TimeStamp = new List<string>();
            this.Values = new List<double>();
            this.Id = this.generateID();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sensor"/> class
        /// </summary>
        /// <param name="sensorName">sensorName</param>
        /// <param name="parameter">parameter of the value</param>
        public Sensor(string sensorName, string parameter)
        {
            this.SensorName = sensorName;
            this.Parameter = parameter;

            this.TimeStamp = new List<string>();
            this.Values = new List<double>();
            this.Id = this.generateID();
        }

        /// <summary>
        /// Gets or sets Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets X position
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Gets or sets Y position
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Gets or sets Z position
        /// </summary>
        public double Z { get; set; }

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
        public List<string> TimeStamp { get; set; }

        /// <summary>
        /// Gets or sets Parameter
        /// </summary>
        public string Parameter { get; set; }

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
