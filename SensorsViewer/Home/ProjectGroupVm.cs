// <copyright file="ProjectGroupVm.cs" company="GM">
//     gm.com. All rights reserved.
// </copyright>

namespace SensorsViewer.Home
{
    using System;
    using System.Collections.Generic;

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
        /// Gets or sets Sensors
        /// </summary>
        public IList<Sensors> Sensors { get; set; }

        /// <summary>
        /// Gets or sets Analysis
        /// </summary>
        public IList<Analysis> Analysis { get; set; }
    }
}