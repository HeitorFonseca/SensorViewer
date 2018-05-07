// <copyright file="SplashScreen.xaml.cs" company="GM">
//     gm.com. All rights reserved.
// </copyright>

namespace SensorsViewer
{
    using System;
    using System.Collections.Generic;
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

    /// <summary>
    /// Splash Screen interface
    /// </summary>
    public interface ISplashScreen
    {
        /// <summary>
        /// Add mensage do splash screen
        /// </summary>
        /// <param name="message">Splash Screen message</param>
        void AddMessage(string message);

        /// <summary>
        /// Finish splash screen
        /// </summary>
        void LoadComplete();
    }

    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window, ISplashScreen
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SplashScreen"/> class
        /// </summary>
        public SplashScreen()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Add message to splash screen
        /// </summary>
        /// <param name="message">Splash screen message</param>
        public void AddMessage(string message)
        {
            Dispatcher.Invoke((Action)delegate ()
            {
                this.UpdateMessageTextBox.Text = message;
            });
        }

        /// <summary>
        /// Finish splash screen
        /// </summary>
        public void LoadComplete()
        {
            Dispatcher.InvokeShutdown();
        }
    }
}
