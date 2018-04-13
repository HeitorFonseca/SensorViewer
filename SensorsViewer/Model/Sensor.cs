﻿// <copyright file="Sensor.cs" company="GM">
//     gm.com. All rights reserved.
// </copyright>

namespace SensorsViewer.SensorOption
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Class of sensors
    /// </summary>
    public class Sensor : INotifyPropertyChanged
    {
        /// <summary>
        /// Sensor visibility
        /// </summary>
        private bool visibility = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="Sensor"/> class
        /// </summary>
        public Sensor()
        {
            this.Values = new List<SensorValue>();
            this.Id = this.GenerateID();
            this.Visibility = true;
            this.Size = 10.0;
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

            this.Size = 10.0;
            this.Values = new List<SensorValue>();
            this.Id = this.GenerateID();
            this.Visibility = true;
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
            this.Visibility = true;
            this.Size = 10.0;
        }

        /// <summary>
        /// Event for when change property
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

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
        /// Gets or sets Sensor Size
        /// </summary>
        public double Size { get; set; }

        /// <summary>
        /// Gets or sets Values
        /// </summary>
        public List<SensorValue> Values { get; set; }

        /// <summary>
        /// Gets maximum value
        /// </summary>
        public string Max
        {
            get
            {
                if (this.Values.Count > 0)
                {
                    return this.Values.Max(a => a.Value).ToString();
                }
                
                 return "-";                
            }
        }

        /// <summary>
        /// Gets minimum value
        /// </summary>
        public string Min
        {
            get
            {
                if (this.Values.Count > 0)
                {
                    return this.Values.Min(a => a.Value).ToString();
                }
               
                 return "-";                
            }
        }

        /// <summary>
        /// Gets Integral of values
        /// </summary>
        public string Integral
        {
            get
            {
                if (this.Values.Count > 0)
                {
                    return this.CalculateIntegral().ToString();
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether sensor was removed
        /// </summary>
        public bool Visibility
        {
            get
            {
                return this.visibility;
            }

            set
            {
                this.visibility = value;
                this.OnPropertyChanged("Visibility");
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

        /// <summary>
        /// Calculate Integral of values
        /// </summary>
        /// <returns>Returns the integral of value</returns>
        public double CalculateIntegral()
        {
            double sum = 0;
            for (int i = 0; i < this.Values.Count - 1; i++)
            {
                DateTime x1 = this.Values[i].Timestamp; ////DateTime.ParseExact(this.Values[i].Timestamp, "dd/MM/yyyy HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);

                DateTime x2 = this.Values[i + 1].Timestamp; ////DateTime.ParseExact(this.Values[i + 1].Timestamp, "dd/MM/yyyy HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);

                double dx = (x2 - x1).TotalMilliseconds;
                double funcValue = this.Values[i].Value;
                double rectangleArea = funcValue * dx;
                sum += rectangleArea;
            }

            return sum;
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
        public SensorValue(double value, DateTime timestamp, string parameter, string analysis)
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
        public DateTime Timestamp { get; set; }

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
