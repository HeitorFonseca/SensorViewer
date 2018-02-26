// <copyright file="SensorOptionViewModel.cs" company="GM">
//     gm.com. All rights reserved.
// </copyright>

namespace SensorsViewer.SensorOption
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SensorOptionViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Private sensor id text
        /// </summary>
        private string sensorIdText;

        /// <summary>
        /// Private sensor x position
        /// </summary>
        private string xPosition;

        /// <summary>
        /// Private sensor y position
        /// </summary>
        private string yPosition;

        /// <summary>
        /// Private sensor z position
        /// </summary>
        private string zPosition;

        /// <summary>
        /// Gets or sets series collection
        /// </summary>
        public ObservableCollection<string> SeriesCollection { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SensorOptionViewModel"/> class
        /// </summary>
        public SensorOptionViewModel()
        {
            this.ClickOnSensorContent = new RelayCommand(ClickOnSensorContentEvent);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SensorOptionViewModel"/> class
        /// </summary>
        public SensorOptionViewModel(string text, string x, string y, string z)
        {
            this.sensorIdText = text;
            this.xPosition = x;
            this.yPosition = y;
            this.zPosition = z;

            this.ClickOnSensorContent = new RelayCommand(ClickOnSensorContentEvent);

        }

        /// <summary>
        ///  Gets or sets Create new project command
        /// </summary>
        public RelayCommand ClickOnSensorContent { get; set; }

        /// <summary>
        /// Gets or sets sensor id text
        /// </summary>
        public string SensorIdText
        {
            get
            {
                return this.sensorIdText;
            }

            set
            {
                this.sensorIdText = value;
                this.OnPropertyChanged("TextInBox");
            }
        }


        /// <summary>
        /// Gets or sets sensor x position
        /// </summary>
        public string XPosition
        {
            get
            {
                return this.xPosition;
            }

            set
            {
                this.xPosition = value;
                this.OnPropertyChanged("XPosition");
            }
        }

        /// <summary>
        /// Gets or sets sensor y position
        /// </summary>
        public string YPosition
        {
            get
            {
                return this.yPosition;
            }

            set
            {
                this.yPosition = value;
                this.OnPropertyChanged("YPosition");
            }
        }

        /// <summarz>
        /// Gets or sets sensor z position
        /// </summarz>
        public string ZPosition
        {
            get
            {
                return this.zPosition;
            }

            set
            {
                this.zPosition = value;
                this.OnPropertyChanged("ZPosition");
            }
        }

        /// <summary>
        /// Event for when change property
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

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
        /// Evento to create new project
        /// </summary>
        private void ClickOnSensorContentEvent()
        {
            var asdasd = 123;

            var asasdasd = 123123;
        }
    }
}
