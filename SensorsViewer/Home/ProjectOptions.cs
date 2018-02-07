using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorsViewer.Home
{
    public class ProjectOptions
    {
        /// <summary>
        /// Gets or sets Option name
        /// </summary>
        public string OptionName { get; set; }

        /// <summary>
        /// Gets or sets Sensors
        /// </summary>
        public ObservableCollection<Sensors> Sensors { get; set; }

        /// <summary>
        /// Gets or sets Analysis
        /// </summary>
        public ObservableCollection<Analysis> Analysis { get; set; }

        public ProjectOptions()
        {
            Sensors = new ObservableCollection<Home.Sensors>();
            Analysis = new ObservableCollection<Home.Analysis>();
        }
    }
}
