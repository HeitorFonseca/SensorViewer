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

        private string textInBox;

        /// <summary>
        /// Gets or sets series collection
        /// </summary>
        public ObservableCollection<string> SeriesCollection { get; set; }

        public SensorOptionViewModel()
        {
        }

        public string TextInBox
        {
            get
            {
                return this.textInBox;
            }

            set
            {
                this.textInBox = value;
                this.OnPropertyChanged("TextInBox");
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
    }
}
