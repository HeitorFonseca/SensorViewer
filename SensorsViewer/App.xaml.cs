// <copyright file="App.xaml.cs" company="GM">
//     gm.com. All rights reserved.
// </copyright>

namespace SensorsViewer
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Splash screen interface object
        /// </summary>
        public static ISplashScreen SplashScreen;

        /// <summary>
        /// Reset event
        /// </summary>
        private ManualResetEvent resetSplashCreated;

        /// <summary>
        /// Splash screen thread
        /// </summary>
        private Thread splashThread;

        /// <summary>
        /// On application start up
        /// </summary>
        /// <param name="e">Start up event</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            // ManualResetEvent acts as a block.  It waits for a signal to be set.
            this.resetSplashCreated = new ManualResetEvent(false);

            // Create a new thread for the splash screen to run on
            this.splashThread = new Thread(this.ShowSplash);
            this.splashThread.SetApartmentState(ApartmentState.STA);
            this.splashThread.IsBackground = true;
            this.splashThread.Name = "Splash Screen";
            this.splashThread.Start();

            // Wait for the blocker to be signaled before continuing. This is essentially the same as: while(ResetSplashCreated.NotSet) {}
            this.resetSplashCreated.WaitOne();

            base.OnStartup(e);
        }

        /// <summary>
        /// Display splash screen
        /// </summary>
        private void ShowSplash()
        {
            // Create the window
            SplashScreen splashScreenWindow = new SplashScreen();
            SplashScreen = splashScreenWindow;

            // Show it
            splashScreenWindow.Show();

            // Now that the window is created, allow the rest of the startup to run
            this.resetSplashCreated.Set();
            System.Windows.Threading.Dispatcher.Run();
        }
    }
}
