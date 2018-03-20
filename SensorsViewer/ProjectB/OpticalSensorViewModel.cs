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
        private ColorsCollection seriesColors = new ColorsCollection();

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
            
            this.SensorList.Add(sensor);
            this.SeriesCollection.Add(newLs);
        }

        /// <summary>
        /// Remove sensor from chart graph
        /// </summary>
        /// <param name="sensor">Which sensor is going to be removed</param>
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
        /// <param name="sensorName">Sensor id</param>
        /// <param name="value">Value to add</param>
        /// <param name="sv">Sensor value</param>
        public void AddValue(string sensorName, double value, SensorOption.SensorValue sv)
        {            
            // If exist a line series with sensor with sensorid
            if (this.SeriesCollection.FirstOrDefault(a => a.Title == sensorName) is LineSeries ls)
            {
                LiveCharts.Defaults.ObservableValue obsValue = new LiveCharts.Defaults.ObservableValue(value);
                ls.Values.Add(value);

                for (int i = 0; i < this.SensorList.Count; i++)
                {
                    if (this.SensorList[i].SensorName == sensorName)
                    {
                        this.SensorList[i].Values.Add(sv);
                    }
                }
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

        /// <summary>
        /// Show loaded sensors when load windows
        /// </summary>
        /// <param name="sensorList">Sensor list</param>
        /// <param name="analysisName">Analysis name</param>
        public void ShowLoadedSensors(ObservableCollection<SensorOption.Sensor> sensorList, string analysisName)
        {
            foreach (SensorOption.Sensor sensor in sensorList)
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

                bool enter = false;
                if (sensor.Values != null)
                {
                    foreach (SensorOption.SensorValue v in sensor.Values)
                    {
                        if (v.AnalysisName == analysisName)
                        {
                            newLs.Values.Add(v.Value);
                            enter = true;
                        }
                        else if (enter == true)
                        {
                            break;
                        }
                    }
                }

                this.SeriesCollection.Add(newLs);
            }
        }

        /// <summary>
        /// Show sensor log when load window
        /// </summary>
        /// <param name="sensorList">Sensor list</param>
        /// <param name="analysisName">Analysis name</param>
        public void ShowSensorsLog(ObservableCollection<SensorOption.Sensor> sensorList, string analysisName)
        {
            ObservableCollection<SensorOption.Sensor> list = new ObservableCollection<SensorOption.Sensor>();

            foreach (SensorOption.Sensor sensor in sensorList)
            {
                foreach (SensorOption.SensorValue values in sensor.Values)
                {
                    if (values.AnalysisName == analysisName)
                    {
                        SensorOption.Sensor s = new SensorOption.Sensor(sensor.SensorName);
                        s.Values.Add(values);
                        list.Add(s);
                    }
                }              
            }

            list = new ObservableCollection<SensorOption.Sensor>(list.OrderBy(a => a.Values[0].Timestamp));

            this.SensorsLog = list;
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

        /// <summary>
        /// Get next color for graph
        /// </summary>
        /// <returns>Returns the next default color</returns>
        private Color GetNextDefaultColor()
        {
            if (this.currentSeriesIndex == int.MaxValue)
            {
                this.currentSeriesIndex = 0;
            }

            var i = this.currentSeriesIndex;
            this.currentSeriesIndex++;

            return this.seriesColors[i % this.seriesColors.Count];
        }

        /// <summary>
        /// Initialize defaults color
        /// </summary>
        private void InitializeSeriesColors()
        {
           this.seriesColors.Add(new Color() { A = 255, R = 45, G = 137, B = 239 }); // blue
           this.seriesColors.Add(new Color() { A = 255, R = 238, G = 17, B = 17 });  // red
           this.seriesColors.Add(new Color() { A = 255, R = 255, G = 196, B = 13 }); // yellow
           this.seriesColors.Add(new Color() { A = 255, R = 0, G = 171, B = 169 });  // green blue
           this.seriesColors.Add(new Color() { A = 255, R = 255, G = 0, B = 151 });  // pink
           this.seriesColors.Add(new Color() { A = 255, R = 0, G = 163, B = 0 });    // green
           this.seriesColors.Add(new Color() { A = 255, R = 218, G = 83, B = 44 });  // orange
           this.seriesColors.Add(new Color() { A = 255, R = 43, G = 87, B = 151 });  // blue
           this.seriesColors.Add(new Color() { A = 255, R = 109, G = 0, B = 172 });   // purple
           this.seriesColors.Add(new Color() { A = 255, R = 118, G = 59, B = 29 });  // brown
           this.seriesColors.Add(new Color() { A = 255, R = 33, G = 53, B = 23 });   // dark green
           this.seriesColors.Add(new Color() { A = 255, R = 26, G = 31, B = 55 });   // dark blue
           this.seriesColors.Add(new Color() { A = 255, R = 98, G = 98, B = 98 });   // gray
        }        
    }
}
