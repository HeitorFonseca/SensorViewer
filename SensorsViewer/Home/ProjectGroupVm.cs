// <copyright file="ProjectGroupVm.cs" company="GM">
//     gm.com. All rights reserved.
// </copyright>

namespace SensorsViewer.Home
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Project group left bar
    /// </summary>
    public class ProjectGroupVm
    {
        /// <summary>
        /// Gets or sets name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets project options
        /// </summary>
        public ObservableCollection<ProjectOptions> ProjectOptions { get; set; }     
    }
}