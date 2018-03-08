// <copyright file="OptionVm.cs" company="GM">
//     gm.com. All rights reserved.
// </copyright>

namespace SensorsViewer.Home
{
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
        private ObservableCollection<ProjectGroupVm> projects;

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionVm"/> class
        /// </summary>
        public OptionVm()
        {
            this.Id = idCount++;
            this.Projects = new ObservableCollection<ProjectGroupVm>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionVm"/> class
        /// </summary>
        /// <param name="title">Option Title</param>
        public OptionVm(string title, string modelPath)
        {
            this.Id = idCount++;
            this.Title = title;
            this.modelPath = modelPath;

            this.Projects = new ObservableCollection<ProjectGroupVm>();
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
        /// Gets or sets Title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Projects collection
        /// </summary>
        public ObservableCollection<ProjectGroupVm> Projects
        {
            get
            {
                return this.projects;
            }

            set
            {
                this.projects = value;
                this.OnPropertyChanged("Projects");
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
