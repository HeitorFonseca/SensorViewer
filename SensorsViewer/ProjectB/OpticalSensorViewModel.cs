// <copyright file="OpticalSensorViewModel.cs" company="GM">
//     gm.com. All rights reserved.
// </copyright>

namespace SensorsViewer.ProjectB
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using System.Windows.Media;
    using LiveCharts;
    using LiveCharts.Wpf;

    /// <summary>
    /// Optical Sensor View Model
    /// </summary>
    public class OpticalSensorViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Current Series index
        /// </summary>
        private int currentSeriesIndex = 0;

        /// <summary>
        /// Collection of colors to be used in graph
        /// </summary>
        private ColorsCollection SeriesColors = new ColorsCollection();

        /// <summary>
        /// File path of the sensor
        /// </summary>
        private string sensorsFilePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpticalSensorViewModel"/> class
        /// </summary>
        public OpticalSensorViewModel()
        {
            this.InitializeSeriesColors();
            this.SeriesCollection = new SeriesCollection();
            this.SensorList = new ObservableCollection<SensorOption.Sensor>();
            this.SensorsLog = new ObservableCollection<SensorOption.Sensor>();
        }

        /// <summary>
        /// Event for when change property
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

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
        /// Gets or sets sensors file path
        /// </summary>
        public string SensorsFilePath
        {
            get
            {
                return this.sensorsFilePath;
            }

            set
            {
                this.sensorsFilePath = value;
                this.OnPropertyChanged("SensorsFilePath");
            }
        }

        /// <summary>
        /// Add sensor in linesgraph
        /// </summary>
        /// <param name="sensor">sensor to add</param>
        public void AddSensorToGraph(SensorOption.Sensor sensor)
        {
            Color nextColor = this.GetNextDefaultColor();

            LineSeries newLs = new LineSeries
            {
                Title = sensor.SensorName,
                Values = new ChartValues<double>(),
                Tag = sensor.Id,
                Fill = new SolidColorBrush(nextColor) { Opacity = 0.15d }
            };

            Brush textBrush = newLs.Fill.Clone();
            textBrush.Opacity = 1d;

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

        public void ShowLoadedSensors()
        {
            foreach (SensorOption.Sensor sensor in this.SensorList)
            {
                Color nextColor = this.GetNextDefaultColor();

                LineSeries newLs = new LineSeries
                {
                    Title = sensor.SensorName,
                    Values = new ChartValues<double>(),
                    Tag = sensor.Id,
                    Fill = new SolidColorBrush(nextColor) { Opacity = 0.15d }
                };

                Brush textBrush = newLs.Fill.Clone();
                textBrush.Opacity = 1d;

                if (sensor.Values != null)
                {
                    foreach (double value in sensor.Values)
                    {
                        newLs.Values.Add(new LiveCharts.Defaults.ObservableValue(value));
                    }
                }

                this.SeriesCollection.Add(newLs);
            }
        }

        /// <summary>
        /// Get next color for graph
        /// </summary>
        /// <returns></returns>
        private Color GetNextDefaultColor()
        {
            if (this.currentSeriesIndex == int.MaxValue) this.currentSeriesIndex = 0;
            var i = this.currentSeriesIndex;
            this.currentSeriesIndex++;


            return SeriesColors[i % SeriesColors.Count];
        }

        private void InitializeSeriesColors()
        {
           this.SeriesColors.Add(new Color() { A = 255, R = 45, G = 137, B = 239 }); // blue
           this.SeriesColors.Add(new Color() { A = 255, R = 238, G = 17, B = 17 });  // red
           this.SeriesColors.Add(new Color() { A = 255, R = 255, G = 196, B = 13 }); // yellow
           this.SeriesColors.Add(new Color() { A = 255, R = 0, G = 171, B = 169 });  // green blue
           this.SeriesColors.Add(new Color() { A = 255, R = 255, G = 0, B = 151 });  // pink
           this.SeriesColors.Add(new Color() { A = 255, R = 0, G = 163, B = 0 });    // green
           this.SeriesColors.Add(new Color() { A = 255, R = 218, G = 83, B = 44 });  // orange
           this.SeriesColors.Add(new Color() { A = 255, R = 43, G = 87, B = 151 });  // blue
           this.SeriesColors.Add(new Color() { A = 255, R = 109, G =0, B = 172 });   // purple
           this.SeriesColors.Add(new Color() { A = 255, R = 118, G = 59, B = 29 });  // brown
           this.SeriesColors.Add(new Color() { A = 255, R = 33, G = 53, B = 23 });   // dark green
           this.SeriesColors.Add(new Color() { A = 255, R = 26, G = 31, B = 55 });   // dark blue
           this.SeriesColors.Add(new Color() { A = 255, R = 98, G = 98, B = 98 });   // gray

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
}
