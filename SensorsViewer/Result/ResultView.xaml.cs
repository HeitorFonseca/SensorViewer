// <copyright file="ResultView.xaml.cs" company="GM">
//     gm.com. All rights reserved.
// </copyright>

namespace SensorsViewer.Result
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
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
    using System.Windows.Media.Media3D;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using SensorsViewer.SensorOption;
    using SharpGL;
    using SharpGL.SceneGraph;

    /// <summary>
    /// Interaction logic for ResultView.xaml
    /// </summary>
    public partial class ResultView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResultView"/> class
        /// </summary>
        public ResultView()
        {
            this.InitializeComponent();
            this.ResultViewModel = new ResultViewModel();           
            this.DataContext = this.ResultViewModel;           
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultView"/> class
        /// </summary>
        /// <param name="sensors">Model sensors</param>
        /// <param name="path">Model path</param>
        public ResultView(IEnumerable<Sensor> sensors, string path)
        {
            this.InitializeComponent();
            this.ResultViewModel = new ResultViewModel(sensors, path);                   
            this.viewPort3d.ZoomExtents();
            this.viewPort3d.ZoomExtentsWhenLoaded = true;           
            this.DataContext = this.ResultViewModel;         
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultView"/> class
        /// </summary>
        /// <param name="sensors">Model sensors</param>
        /// <param name="path">Model path</param>
        /// <param name="analysisName">Analysis name</param>
        public ResultView(IEnumerable<Sensor> sensors, string path, string analysisName)
        {
            this.InitializeComponent();
            this.ResultViewModel = new ResultViewModel(sensors, path, analysisName);                 
            this.viewPort3d.ZoomExtents();
            this.viewPort3d.ZoomExtentsWhenLoaded = true;
            this.DataContext = this.ResultViewModel;          
        }

        /// <summary>
        /// Gets or sets optical sensor view model
        /// </summary>
        public ResultViewModel ResultViewModel { get; set; }

        /// <summary>
        /// Event when initialize sharpgl
        /// </summary>
        /// <param name="sender">Object sender</param>
        /// <param name="args">Event args</param>
        private void OpenGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            // Get the OpenGL instance that's been passed to us.
            OpenGL gl = args.OpenGL;
            gl.ClearColor(255, 255, 255, 255);

            gl.Flush();
        }
    }
}
