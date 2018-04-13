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
    using LiveCharts.Configurations;
    using LiveCharts.Wpf;
    using SensorsViewer.SensorOption;

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

        private double _axisMax;
        private double _axisMin;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpticalSensorViewModel"/> class
        /// </summary>
        public OpticalSensorViewModel()
        {
            var dayConfig = Mappers.Xy<DateModel>()
                .X(dayModel => (double)dayModel.DateTime.Ticks)
                .Y(dayModel => dayModel.Value);

            //lets save the mapper globally.
            Charting.For<DateModel>(dayConfig);

            this.InitializeSeriesColors();
            this.SeriesCollection = new SeriesCollection(dayConfig);
            this.SensorList = new ObservableCollection<SensorOption.Sensor>();
            this.SensorsLog = new ObservableCollection<SensorOption.Sensor>();

            XFormatter = this.EOQ;//val => new DateTime((long)val).ToString("HH: mm:ss.fff");
            YFormatter = this.YUKE;//val => val.ToString("N");

            //AxisStep forces the distance between each separator in the X axis
            AxisStep = TimeSpan.FromSeconds(1).Ticks;

            //AxisUnit forces lets the axis know that we are plotting seconds
            //this is not always necessary, but it can prevent wrong labeling
            AxisUnit = TimeSpan.TicksPerSecond;
        }

        private void SetAxisLimits(DateTime now)
        {
            AxisMax = now.Ticks; // + TimeSpan.FromSeconds(1).Ticks; // lets force the axis to be 1 second ahead
            AxisMin = now.Ticks - TimeSpan.FromSeconds(10).Ticks; // and 8 seconds behind
        }

        public double AxisStep { get; set; }
        public double AxisUnit { get; set; }

        public double AxisMax
        {
            get { return _axisMax; }
            set
            {
                _axisMax = value;
                OnPropertyChanged("AxisMax");
            }
        }
        public double AxisMin
        {
            get { return _axisMin; }
            set
            {
                _axisMin = value;
                OnPropertyChanged("AxisMin");
            }
        }
        /// <summary>
        /// Event for when change property
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        public Func<double, string> XFormatter { get; set; }
        public Func<double, string> YFormatter { get; set; }

        public string EOQ(double val)
        {
            string asd = new DateTime((long)val).ToString("mm:ss.fff");

            return asd;
        }

        public string YUKE(double val)
        {
            string asd = val.ToString("N");

            return asd;
        }

        public long MinValue { get; set; }

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
                Values = new ChartValues<DateModel>(),
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
                DateModel dm = new DateModel() { DateTime = sv.Timestamp, Value = value };
                ls.Values.Add(dm);

                for (int i = 0; i < this.SensorList.Count; i++)
                {
                    if (this.SensorList[i].SensorName == sensorName)
                    {
                        this.SensorList[i].Values.Add(sv);
                        SetAxisLimits(sv.Timestamp);
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
            ObservableCollection<SensorOption.Sensor> list = new ObservableCollection<SensorOption.Sensor>();

            foreach (SensorOption.Sensor sensor in sensorList)
            {
                Color nextColor = this.GetNextDefaultColor();

                LineSeries newLs = new LineSeries
                {
                    Title = sensor.SensorName,
                    Values = new ChartValues<DateModel>(),
                    Tag = sensor.Id,
                    Fill = new SolidColorBrush(nextColor) { Opacity = 0.15d }
                };

                Brush textBrush = newLs.Fill.Clone();
                textBrush.Opacity = 1d;

                SensorOption.Sensor newSensor = new SensorOption.Sensor(sensor.SensorName, sensor.X, sensor.Y, sensor.Z);
                newSensor.Id = sensor.Id;

                this.SensorList.Add(newSensor);

                int index = this.SensorList.IndexOf(newSensor);

                if (sensor.Values != null)
                {
                    ObservableCollection<SensorOption.Sensor> asd = new ObservableCollection<SensorOption.Sensor>();

                    foreach (SensorOption.SensorValue v in sensor.Values)
                    {
                        if (v.AnalysisName == analysisName)
                        {
                            DateModel dm = new DateModel() { DateTime = v.Timestamp, Value = v.Value };
                            // Chart
                            newLs.Values.Add(dm);

                            // Update Log
                            SensorOption.Sensor s = new SensorOption.Sensor(sensor.SensorName);
                            s.Values.Add(v);
                            list.Add(s);

                            // Sensor List                           
                            this.SensorList.ElementAt(index).Values.Add(v);
                        }
                    }
                }

                if (newLs.Values.Count > 0)
                {
                    AxisMin = ((DateModel)newLs.Values[0]).DateTime.Ticks;
                    AxisMax = ((DateModel)newLs.Values[newLs.Values.Count - 1]).DateTime.Ticks;
                }

                list = new ObservableCollection<SensorOption.Sensor>(list.OrderBy(a => a.Values[0].Timestamp));
                this.SensorsLog = list;
                this.SeriesCollection.Add(newLs);
            }
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
