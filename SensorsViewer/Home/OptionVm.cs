using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorsViewer.Home
{
    class OptionVm
    {
        public int Id { get; private set; }
        public string Title { get; set; }
        public Type Content { get; set; }
        public string Tags { get; set; }

        private static int _idCount;

        public OptionVm()
        {
            this.Id = _idCount++;
            this.Tags = string.Empty;
        }

        public OptionVm(string title)
        {
            this.Id = _idCount++;
            this.Title = title;
            this.Tags = string.Empty;
        }

        public OptionVm(string title, Type content, string tags = "")
        {
            this.Id = _idCount++;
            this.Title = title;
            this.Content = content;
            this.Tags = tags;
        }
    }
}
