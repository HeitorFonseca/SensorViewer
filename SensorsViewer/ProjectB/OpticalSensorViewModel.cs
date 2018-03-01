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
            ////this.SeriesCollection = new SeriesCollection
            ////{
            ////    new LineSeries
            ////    {
            ////        Title = "Series 1",
            ////        Values = new ChartValues<double> { 4, 6, 5, 2 ,7 }
            ////    },
            ////    new LineSeries
            ////    {
            ////        Title = "Series 2",
            ////        Values = new ChartValues<double> { 6, 7, 3, 4 ,6 }
            ////    }
            ////};

            this.SeriesCollection = new SeriesCollection();
            this.SensorList = new ObservableCollection<SensorOption.Sensor>();
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
            LineSeries asd = this.SeriesCollection.FirstOrDefault(a => a.Title == sensorid) as LineSeries;
            
            asd.Values.Add(new LiveCharts.Defaults.ObservableValue(value)); 
        }
    }
}
