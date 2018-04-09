// <copyright file="ResultView.xaml.cs" company="GM">
//     gm.com. All rights reserved.
// </copyright>

namespace SensorsViewer.Result
{
    using SensorsViewer.SensorOption;
    using SharpGL;
    using SharpGL.SceneGraph;
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

    using SharpDx = HelixToolkit.Wpf.SharpDX;

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

            this.ViewPort3DX.Camera = new SharpDx.PerspectiveCamera
            {
                Position = new Point3D(0, 0, 0),
                LookDirection = new Vector3D(0, 0, -1),
                //UpDirection = new Vector3D(0, 1, 0),
                FarPlaneDistance = 1000
            };

            func();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultView"/> class
        /// </summary>
        /// <param name="path">Model path</param>
        public ResultView(IEnumerable<Sensor> sensors, string path)
        {
            this.InitializeComponent();

            this.ResultViewModel = new ResultViewModel(sensors, path);        
           
            this.viewPort3d.ZoomExtents();
            this.viewPort3d.ZoomExtentsWhenLoaded = true;
           

            this.DataContext = this.ResultViewModel;

            this.ViewPort3DX.Camera = new SharpDx.PerspectiveCamera
            {
                Position = new Point3D(0, 0, 0),
                LookDirection = new Vector3D(0, 0, -1),
                //UpDirection = new Vector3D(0, 1, 0),
                FarPlaneDistance = 1000
            };

            func();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultView"/> class
        /// </summary>
        /// <param name="path">Model path</param>
        public ResultView(IEnumerable<Sensor> sensors, string path, string analysisName)
        {
            this.InitializeComponent();

            this.ResultViewModel = new ResultViewModel(sensors, path, analysisName);
                 
            this.viewPort3d.ZoomExtents();
            this.viewPort3d.ZoomExtentsWhenLoaded = true;

            this.ViewPort3DX.ZoomExtents();
            this.ViewPort3DX.ZoomExtentsWhenLoaded = true;

            this.DataContext = this.ResultViewModel;

            this.ViewPort3DX.Camera = new SharpDx.PerspectiveCamera
            {
                Position = new Point3D(0, 0, 0),
                LookDirection = new Vector3D(0, 0, -1),
                //UpDirection = new Vector3D(0, 1, 0),
                FarPlaneDistance = 1000
            };

            func();
        }


        public void func()
        {
            SharpDx.PointGeometryModel3D PointModel = new SharpDx.PointGeometryModel3D();

            SharpDx.PointGeometry3D Points = new SharpDx.PointGeometry3D();
            var ptPos = new SharpDx.Core.Vector3Collection();
            var ptIdx = new SharpDx.Core.IntCollection();
            var colr = new SharpDx.Core.Color4Collection();

            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    for (int z = 0; z < 10; z++)
                    {
                        ptIdx.Add(ptPos.Count);
                        ptPos.Add(new SharpDX.Vector3(x, y, z));
                        colr.Add(new SharpDX.Color4(100, 255, 10, 1));
                    }
                }
            }
            Points.Positions = ptPos;
            Points.Indices = ptIdx;
            Points.Colors = colr;

            PointModel.Geometry = Points;
            PointModel.Size = new System.Windows.Size(10, 10);
            PointModel.Color = new SharpDX.Color(0, 0, 0);

           // this.pegaCarai.Children.Add(PointModel);
        }
        /// <summary>
        /// Gets or sets optical sensor view model
        /// </summary>
        public ResultViewModel ResultViewModel { get; set; }

        private void OpenGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            //  Get the OpenGL instance that's been passed to us.
            OpenGL gl = args.OpenGL;

            gl.ClearColor(255, 255, 255, 255);

        }
    }
}
