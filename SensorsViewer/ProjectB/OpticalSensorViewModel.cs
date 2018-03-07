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
    using System.Windows.Input;
    using System.Windows.Media;
    using LiveCharts;
    using LiveCharts.Wpf;

    /// <summary>
    /// Optical Sensor View Model
    /// </summary>
    public class OpticalSensorViewModel
    {
        private int currentSeriesIndex = 0;
        private ColorsCollection SeriesColors = new ColorsCollection();

        private static Random Randomizer { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="OpticalSensorViewModel"/> class
        /// </summary>
        public OpticalSensorViewModel()
        {
            this.InitializeSeriesColors();
            this.SeriesCollection = new SeriesCollection();
            this.SensorList = new ObservableCollection<SensorOption.Sensor>();
            this.SensorsLog = new ObservableCollection<SensorOption.Sensor>();
            this.LoadedWindowCommand = new RelayCommand(WindowLoadedAction);
        }

        /// <summary>
        ///  Gets or sets Close window command
        /// </summary>
        public ICommand LoadedWindowCommand { get; set; }

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
        /// Event when close window
        /// </summary>
        private void WindowLoadedAction(object parameter)
        {
            var a = 1;

        }


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
                Tag = sensor.Id,
                Fill = new SolidColorBrush(this.GetNextDefaultColor()) { Opacity = 1d }
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

        public Color GetNextDefaultColor()
        {
            if (this.currentSeriesIndex == int.MaxValue) this.currentSeriesIndex = 0;
            var i = this.currentSeriesIndex;
            this.currentSeriesIndex++;


            return SeriesColors[i % SeriesColors.Count];
        }

        private void InitializeSeriesColors()
        {
            SeriesColors.Add(new Color() { A = 255, R = 45, G = 137, B = 239 }); //blue
            SeriesColors.Add(new Color() { A = 255, R = 238, G = 17, B = 17 });  //red
            SeriesColors.Add(new Color() { A = 255, R = 255, G = 196, B = 13 }); //yellow
            SeriesColors.Add(new Color() { A = 255, R = 0, G = 171, B = 169 });  //green blue
            SeriesColors.Add(new Color() { A = 255, R = 255, G = 0, B = 151 });  //pink
            SeriesColors.Add(new Color() { A = 255, R = 0, G = 163, B = 0 });    //green
            SeriesColors.Add(new Color() { A = 255, R = 218, G = 83, B = 44 });  //orange
            SeriesColors.Add(new Color() { A = 255, R = 43, G = 87, B = 151 });  //blue
            SeriesColors.Add(new Color() { A = 255, R = 109, G =0, B = 172 });   //purple
            SeriesColors.Add(new Color() { A = 255, R = 118, G = 59, B = 29 });  //brown
            SeriesColors.Add(new Color() { A = 255, R = 33, G = 53, B = 23 });   //dark green
            SeriesColors.Add(new Color() { A = 255, R = 26, G = 31, B = 55 });   //dark blue
            SeriesColors.Add(new Color() { A = 255, R = 98, G = 98, B = 98 });   //gray

        }
    }
}
