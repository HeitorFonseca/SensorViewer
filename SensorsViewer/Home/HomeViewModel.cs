// <copyright file="HomeViewModel.cs" company="GM">
//     gm.com. All rights reserved.
// </copyright>

namespace SensorsViewer.Home
{
    using SensorsViewer.ProjectB;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows.Controls;

    /// <summary>
    /// Home view model class
    /// </summary>
    public class HomeViewModel
    {
        /// <summary>
        /// Private enumerable datasource
        /// </summary>
        private readonly IEnumerable<ProjectGroupVm> dataSource;

        /// <summary>
        /// Project A User control content
        /// </summary>
        private UserControl projectAContent;

        /// <summary>
        /// Project B User control content
        /// </summary>
        private UserControl projectBContent;

        /// <summary>
        /// Private project A menu left bar
        /// </summary>
        private IEnumerable<ProjectGroupVm> projectAMenu;

        /// <summary>
        /// Private project b menu left bar
        /// </summary>
        private IEnumerable<ProjectGroupVm> projectBMenu;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeViewModel"/> class
        /// </summary>
        public HomeViewModel()
        {
            InitializeLeftBarMenu();

            //Set the B project content for OpticalSensorView
            this.projectBContent = (UserControl)(new OpticalSensorView());
        }

        /// <summary>
        /// Event for when change property
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets project A menu left bar
        /// </summary>
        public IEnumerable<ProjectGroupVm> ProjectAMenu
        {
            get
            {
                return this.projectAMenu;
            }

            set
            {
                this.projectAMenu = value;
                this.OnPropertyChanged("ProjectAMenu");
            }
        }

        /// <summary>
        /// Gets or sets project B menu left bar
        /// </summary>
        public IEnumerable<ProjectGroupVm> ProjectBMenu
        {
            get
            {
                return this.projectBMenu;
            }

            set
            {
                this.projectAMenu = value;
                this.OnPropertyChanged("ProjectBMenu");
            }
        }

        /// <summary>
        /// Gets or sets user control content
        /// </summary>
        public UserControl ProjectBContent
        {
            get
            {
                return this.projectBContent;
            }

            set
            {
                this.projectBContent = value;
                this.OnPropertyChanged("Content");
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

        private void InitializeLeftBarMenu()
        {
            this.projectAMenu = new[]
            {
                new ProjectGroupVm
                {
                    Name = "A Project",
                    Items = new[]
                    {
                        new OptionVm("Load STL Model"), new OptionVm("Load Sensors"), new OptionVm("Export csv"), new OptionVm("Export txt")
                    }
                }
            };

            this.projectBMenu = new[]
            {
                new ProjectGroupVm
                {
                    Name = "B Project",
                    Items = new[]
                    {
                        new OptionVm("Export csv"), new OptionVm("Export txt")
                    }
                }
            };           
        }
    }
}
