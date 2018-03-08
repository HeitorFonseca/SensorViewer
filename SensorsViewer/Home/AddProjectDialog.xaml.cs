using MahApps.Metro.Controls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace SensorsViewer.Home
{
    /// <summary>
    /// Interaction logic for AddProjectDialog.xaml
    /// </summary>
    public partial class AddProjectDialog : MetroWindow, INotifyPropertyChanged
    {
        private string projectName;
        private string modelPath;

        public AddProjectDialog()
        {
            InitializeComponent();

            DataContext = this;
        }

        /// <summary>
        /// Event for when change property
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        public string ProjectName
        {
            get
            {
                return this.projectName;
            }

            set
            {
                this.projectName = value;
                this.OnPropertyChanged("ProjectName");

            }
        }

        public string ModelPath
        {
            get
            {
                return this.modelPath;
            }

            set
            {
                this.modelPath = value;
                this.OnPropertyChanged("ModelPath");

            }
        }

        private async void OkBtnClickAsync(object sender, RoutedEventArgs e)
        {
            var mySettings = new MetroDialogSettings()
            {
                ColorScheme = MetroDialogOptions.ColorScheme, 
                DialogTitleFontSize = 12,
                DialogMessageFontSize = 17,                
            };

            if (string.IsNullOrEmpty(this.projectName))
            {                
                MessageDialogResult result = await this.ShowMessageAsync("Error!", "Empty project name", MessageDialogStyle.Affirmative, mySettings);
            }
            else if (string.IsNullOrEmpty(this.modelPath))
            {
                MessageDialogResult result = await this.ShowMessageAsync("Error!", "Empty model path", MessageDialogStyle.Affirmative, mySettings);
            }
            else if (!File.Exists(this.modelPath))
            {
                MessageDialogResult result = await this.ShowMessageAsync("Error!", "Model path does not exist", MessageDialogStyle.Affirmative, mySettings);
            }
            else
            {
                DialogResult = true;
            }
        }

        private void CancelBtnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void FileDialogBtnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog()
            {
                Filter = "STL (*.stl)|*.stl"
            };

            if (fileDialog.ShowDialog() == true)
            {
                this.ModelPath = fileDialog.FileName;
            }
        }

        /// <summary>
        /// When changes property
        /// </summary>
        /// <param name="propertyName">Property name</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
