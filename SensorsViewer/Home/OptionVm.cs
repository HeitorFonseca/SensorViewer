// <copyright file="OptionVm.cs" company="GM">
//     gm.com. All rights reserved.
// </copyright>

namespace SensorsViewer.Home
{
    using SensorsViewer.ProjectB;
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    /// <summary>
    /// Option for left menu bar class
    /// </summary>
    public class OptionVm : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets os sets idCount
        /// </summary>
        private static int idCount;

        /// <summary>
        /// Gets os sets modelPath
        /// </summary>
        private string modelPath;

        /// <summary>
        /// Gets os sets projects
        /// </summary>
        private ObservableCollection<ProjectGroupVm> tabs;

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionVm"/> class
        /// </summary>
        public OptionVm()
        {
            this.Id = idCount++;
            this.Tabs = new ObservableCollection<ProjectGroupVm>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionVm"/> class
        /// </summary>
        /// <param name="name">Option name</param>
        public OptionVm(string name, string modelPath)
        {
            this.Id = idCount++;
            this.Name = name;
            this.modelPath = modelPath;

            this.Tabs = new ObservableCollection<ProjectGroupVm>
            {
                 new ProjectGroupVm { Name = "Draw-In" },
                 new ProjectGroupVm { Name = "Adjustment" }
            };

            Tabs[0].ProjectChartContent = new OpticalSensorView();

        }

        /// <summary>
        /// Event for when change property
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets Id
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Gets or sets Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Projects collection
        /// </summary>
        public ObservableCollection<ProjectGroupVm> Tabs
        {
            get
            {
                return this.tabs;
            }

            set
            {
                this.tabs = value;
                this.OnPropertyChanged("Tabs");
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
    }
}
