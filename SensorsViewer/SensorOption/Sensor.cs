// <copyright file="Sensors.cs" company="GM">
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
        /// Initializes a new instance of the class <see cref="Sensor"/> class
        /// </summary>
        public Sensor()
        {          
        }

        /// <summary>
        /// Initializes a new instance of the class <see cref="Sensor"/> class
        /// </summary>
        public Sensor(string text, string x, string y, string z)
        {
            this.Sensorid = text;
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

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
    }
}
