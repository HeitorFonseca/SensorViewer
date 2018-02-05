using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SensorsViewer.Home
{
    class HomeViewModel
    {
        private UserControl _content;
        private ObservableCollection<TabItem> _tabs;
        private IEnumerable<ProjectGroupVm> _ProjectAMenu;
        private IEnumerable<ProjectGroupVm> _ProjectBMenu;

        private readonly IEnumerable<ProjectGroupVm> _dataSource;

        public HomeViewModel()
        {
            _dataSource = new[]
            {
                new ProjectGroupVm
                {
                    Name = "A Project",
                    Items = new[]
                    {
                        new OptionVm("Load stl model"),
                        new OptionVm("Load sensors data"),
                        new OptionVm("Material Design"),
                        new OptionVm("Solid Color")
                    }
                }
            };

            _ProjectAMenu = _dataSource;

            _dataSource = new[]
            {
                new ProjectGroupVm
                {
                    Name = "B Project",
                    Items = new[]
                    {
                        new OptionVm("Save data"),
                        new OptionVm("Play"),
                        new OptionVm("Stop"),
                        new OptionVm("Exit")
                    }
                }
            };

            _ProjectBMenu = _dataSource;
        }

        public IEnumerable<ProjectGroupVm> ProjectAMenu
        {
            get { return _ProjectAMenu; }
            set
            {
                _ProjectAMenu = value;
                OnPropertyChanged("ProjectAMenu");
            }
        }

        public IEnumerable<ProjectGroupVm> ProjectBMenu
        {
            get { return _ProjectBMenu; }
            set
            {
                _ProjectAMenu = value;
                OnPropertyChanged("ProjectBMenu");
            }
        }

        public UserControl Content
        {
            get { return _content; }
            set
            {
                _content = value;
                OnPropertyChanged("Content");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
