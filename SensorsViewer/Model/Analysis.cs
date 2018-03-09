// <copyright file="Analysis.cs" company="GM">
//     gm.com. All rights reserved.
// </copyright>

namespace SensorsViewer.SensorOption
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Class of Analysis
    /// </summary>
    public class Analysis
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Analysis"/> class
        /// </summary>
        public Analysis()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Analysis"/> class
        /// </summary>
        /// <param name="name">analysis name</param>
        /// <param name="date">analysis date</param>
        /// <param name="time">analysis time</param>
        public Analysis(string name, string date, string time)
        {
            this.Name = name;
            this.Date = date;
            this.Time = time;
        }

        /// <summary>
        /// Gets or sets analysis name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets Y position
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// Gets or sets Z position
        /// </summary>
        public string Time { get; set; }
    }
}
