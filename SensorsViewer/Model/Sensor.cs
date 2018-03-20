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
            this.Values = new List<SensorValue>();
            this.Id = this.GenerateID();
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

            this.Values = new List<SensorValue>();
            this.Id = this.GenerateID();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sensor"/> class
        /// </summary>
        /// <param name="sensorName">Sensor Name</param>
        public Sensor(string sensorName)
        {
            this.SensorName = sensorName;
            this.Values = new List<SensorValue>();
            this.Id = this.GenerateID();
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
        public List<SensorValue> Values { get; set; }

        public double Max
        {
            get
            {
                return this.Values.Max(a => a.Value);
            }
        }

        public double Min
        {
            get
            {
                return this.Values.Min(a => a.Value);
            }
        }

        /// <summary>
        /// Generate unique Id
        /// </summary>
        /// <returns>generated id</returns>
        public string GenerateID()
        {
            return Guid.NewGuid().ToString();
        }
    }

    /// <summary>
    /// Class of sensor value
    /// </summary>
    public class SensorValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SensorValue"/> class
        /// </summary>
        public SensorValue()
        {           
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SensorValue"/> class
        /// </summary>
        /// <param name="value">Sensor value</param>
        /// <param name="timestamp">Value timestamp</param>
        /// <param name="parameter">Value parameter</param>
        /// <param name="analysis">Value Analysis</param>
        public SensorValue(double value, string timestamp, string parameter, string analysis)
        {
            this.Value = value;
            this.Timestamp = timestamp;
            this.AnalysisName = analysis;
            this.Parameter = parameter;
        }
        
        /// <summary>
        /// Gets or sets double value of the sensor
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Gets or sets Double value of the sensor
        /// </summary>
        public string Timestamp { get; set; }

        /// <summary>
        /// Gets or sets Parameter
        /// </summary>
        public string Parameter { get; set; }

        /// <summary>
        /// Gets or sets which analysis the value belongs to
        /// </summary>
        public string AnalysisName { get; set; }
    }
}
