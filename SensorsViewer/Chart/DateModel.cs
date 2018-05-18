// <copyright file="DateModel.cs" company="GM">
//     gm.com. All rights reserved.
// </copyright>

namespace SensorsViewer.Chart
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Class of date model chart
    /// </summary>
    public class DateModel
    {
        /// <summary>
        /// Gets or sets datetime
        /// </summary>
        public System.DateTime DateTime { get; set; }

        /// <summary>
        /// Gets or sets sensor value
        /// </summary>
        public double Value { get; set; }
    }
}
