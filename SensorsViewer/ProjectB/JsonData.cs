// <copyright file="JsonData.cs" company="GM">
//     gm.com. All rights reserved.
// </copyright>

namespace SensorsViewer.ProjectB
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Class for read JsonData
    /// </summary>
    public class JsonData
    {
        /// <summary>
        /// Gets or sets viewer
        /// </summary>
        public string viewer { get; set; }

        /// <summary>
        /// Gets or sets list of values list
        /// </summary>
        public List<List<string>> values { get; set; }
    }
}
