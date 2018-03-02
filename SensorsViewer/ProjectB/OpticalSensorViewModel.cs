// <copyright file="OpticalSensorViewModel.cs" company="GM">
//     gm.com. All rights reserved.
// </copyright>

namespace SensorsViewer.ProjectB
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using LiveCharts;
    using LiveCharts.Wpf;

    /// <summary>
    /// Optical Sensor View Model
    /// </summary>
    public class OpticalSensorViewModel
    {       
        /// <summary>
        /// Initializes a new instance of the <see cref="OpticalSensorViewModel"/> class
        /// </summary>
        public OpticalSensorViewModel()
        {
            this.SeriesCollection = new SeriesCollection();
            this.SensorList = new ObservableCollection<SensorOption.Sensor>();
            this.SensorsLog = new ObservableCollection<SensorOption.Sensor>();
        }

        /// <summary>
        /// Gets or sets sensor list
        /// </summary>
        public ObservableCollection<SensorOption.Sensor> SensorList { get; set; }

        /// <summary>
        /// Gets or sets series collection
        /// </summary>
        public SeriesCollection SeriesCollection { get; set; }
 
        /// <summary>
        /// Gets or sets series sensor log list
        /// </summary>
        public ObservableCollection<SensorOption.Sensor> SensorsLog { get; set; }

        /// <summary>
        /// Add sensor in linesgraph
        /// </summary>
        /// <param name="sensor">sensor to add</param>
        public void AddSensorToGraph(SensorOption.Sensor sensor)
        {
            LineSeries newLs = new LineSeries
            {
                Title = sensor.SensorName,
                Values = new ChartValues<double>(),
                Tag = sensor.Id
            };

            if (sensor.Values != null)
            {
                foreach (double value in sensor.Values)
                {
                    newLs.Values.Add(new LiveCharts.Defaults.ObservableValue(value));
                }
            }

            this.SensorList.Add(sensor);
            this.SeriesCollection.Add(newLs);
        }

        public void RemoveSensorFromGraph(SensorOption.Sensor sensor)
        {
            this.SensorList.Remove(sensor);

            LineSeries ls = null;

            for (int i = 0; i < this.SeriesCollection.Count; i++)
            {
                LineSeries aux = (LineSeries)this.SeriesCollection[i];

                if (aux.Tag.ToString() == sensor.Id)
                {
                    ls = aux;
                    break;
                }
            }

            if (ls != null)
            {
                this.SeriesCollection.Remove(ls);
            }

        }

        /// <summary>
        /// Add value for sensor
        /// </summary>
        /// <param name="sensorid">sensor id</param>
        /// <param name="value">value to add</param>
        public void AddValue(string sensorid, double value)
        {            
            // If exist a line series with sensor with sensorid
            if (this.SeriesCollection.FirstOrDefault(a => a.Title == sensorid) is LineSeries ls)
            {
                LiveCharts.Defaults.ObservableValue obsValue = new LiveCharts.Defaults.ObservableValue(value);
                ls.Values.Add(value);
            }
        }

        /// <summary>
        /// Adds in drawInSensorData list 
        /// </summary>
        /// <param name="sensor">sensor to be added in sensor log</param>
        public void AddSensorLogData(SensorOption.Sensor sensor)
        {
            if (sensor != null)
            {
                this.SensorsLog.Add(sensor);
            }
        }
    }
}
