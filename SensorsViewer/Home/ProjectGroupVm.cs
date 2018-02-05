using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorsViewer.Home
{
    class ProjectGroupVm
    {
        public string Name { get; set; }
        public IEnumerable<OptionVm> Items { get; set; }
    }
}
    