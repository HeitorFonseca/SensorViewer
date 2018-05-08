// <copyright file="ConnectionSettings.xaml.cs" company="GM">
//     gm.com. All rights reserved.
// </copyright>

namespace SensorsViewer.Home
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
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
    using System.Windows.Shapes;
    using MahApps.Metro.Controls;
    using MahApps.Metro.Controls.Dialogs;
    using SensorsViewer.Connection;

    /// <summary>
    /// Interaction logic for ConnectionSettings.xaml
    /// </summary>
    public partial class ConnectionSettings : MetroWindow, INotifyPropertyChanged, IDataErrorInfo
    {
        /// <summary>
        /// Hostname string
        /// </summary>
        private string hostName;

        /// <summary>
        /// Port number
        /// </summary>
        private string portNumber;

        /// <summary>
        /// Username string
        /// </summary>
        private string username;

        /// <summary>
        /// Password 
        /// </summary>
        private string password;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionSettings"/> class
        /// </summary>
        public ConnectionSettings(MqttConnection conn)
        {
            InitializeComponent();

            this.DataContext = this;

            this.Username = conn.Username;
            this.Password = conn.Password;
            this.PortNumber = conn.Port.ToString();
            this.HostName = conn.HostName;
        }

        /// <summary>
        /// Event for when change property
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///  Gets or sets Close window command
        /// </summary>
        public ICommand EnterKeyDownCommand { get; set; }

        /// <summary>
        /// Gets or sets hostname
        /// </summary>
        public string HostName
        {
            get
            {
                return this.hostName;
            }

            set
            {
                this.hostName = value;
                this.OnPropertyChanged("HostName");
            }
        }

        /// <summary>
        /// Gets or sets port number
        /// </summary>
        public string PortNumber
        {
            get
            {
                return this.portNumber;
            }

            set
            {
                this.portNumber = value;
                this.OnPropertyChanged("PortNumber");
            }
        }

        /// <summary>
        /// Gets or sets username
        /// </summary>
        public string Username
        {
            get
            {
                return this.username;
            }

            set
            {
                this.username = value;
                this.OnPropertyChanged("Username");
            }
        }

        /// <summary>
        /// Gets or sets password
        /// </summary>
        public string Password
        {
            get
            {
                return this.password;
            }

            set
            {
                this.password = value;
                this.OnPropertyChanged("Password");
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

        public string Error { get { return string.Empty; } }

        string IDataErrorInfo.this[string columnName]
        {
            get
            {
                if (columnName == "PortNumber" && !IsNumber(this.PortNumber))
                {
                    return "Is not a number";
                }

                return null;
            }
        }

        /// <summary>
        /// Event for when click to create the project
        /// </summary>
        /// <param name="sender">Object Sender</param>
        /// <param name="e">Event e</param>
        private void OkBtnClickAsync(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        /// <summary>
        /// Event for when the user click in cancel button
        /// </summary>
        /// <param name="sender">Object Sender</param>
        /// <param name="e">Event e</param>
        private void CancelBtnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        public static bool IsNumber(string aNumber)
        {
            int i;
            bool bNum = int.TryParse(aNumber, out i);

            return bNum;
        }
    }   
}
