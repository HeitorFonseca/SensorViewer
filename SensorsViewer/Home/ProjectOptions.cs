using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

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
        public ObservableCollection<UserControl> Content { get; set; }

        public ProjectOptions()
        {
            Content = new ObservableCollection<UserControl>();
        }
    }
}
