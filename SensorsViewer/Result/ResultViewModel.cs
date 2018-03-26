// <copyright file="ResultViewModel.cs" company="GM">
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
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;
    using HelixToolkit.Wpf;
    using SensorsViewer.SensorOption;

    /// <summary>
    /// Class for result view model
    /// </summary>
    public class ResultViewModel
    {
        private double sizeX = 10;
        private double sizeY = 10;
        private double sizeZ = 0.005;

        /// <summary>
        /// 3D device
        /// </summary>
        private ModelVisual3D device3D;

        /// <summary>
        /// Group Model
        /// </summary>
        private Model3DGroup groupModel = new Model3DGroup();

        /// <summary>
        /// Stl model mesh
        /// </summary>
        private MeshGeometry3D modelMesh = new MeshGeometry3D();

        /// <summary>
        /// Stl file path
        /// </summary>
        private string stlFilePath;

        /// <summary>
        /// Sensors from analysis
        /// </summary>
        private ObservableCollection<Sensor> Sensors;

        /// <summary>
        /// Sensor geometry model
        /// </summary>
        private List<GeometryModel3D> sensorModelList = new List<GeometryModel3D>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultViewModel"/> class
        /// </summary>
        public ResultViewModel()
        {
            this.ViewMode = false;

            this.ViewPort3d = new HelixViewport3D();
            this.device3D = new ModelVisual3D();
            this.OnCheckedModeViewButtonCommand = new RelayCommand(this.OnCheckedModeViewButtonAction);
            this.OnUnCheckedModeViewButtonCommand = new RelayCommand(this.OnUnCheckedModeViewButtonAction);
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="ResultViewModel"/> class
        /// </summary>
        public ResultViewModel(ObservableCollection<Sensor> sensors)
        {
            this.ViewMode = false;
            this.Sensors = sensors;

            this.ViewPort3d = new HelixViewport3D();
            this.device3D = new ModelVisual3D();
            this.OnCheckedModeViewButtonCommand = new RelayCommand(this.OnCheckedModeViewButtonAction);
            this.OnUnCheckedModeViewButtonCommand = new RelayCommand(this.OnUnCheckedModeViewButtonAction);
        }

        /// <summary>
        ///  Gets or sets click mode view command
        /// </summary>
        public ICommand OnCheckedModeViewButtonCommand { get; set; }

        /// <summary>
        ///  Gets or sets click mode view command
        /// </summary>
        public ICommand OnUnCheckedModeViewButtonCommand { get; set; }

        /// <summary>
        /// Gets or sets Hellix view port 3d
        /// </summary>
        public HelixViewport3D ViewPort3d { get; set; }

        /// <summary>
        /// Gets or sets group model
        /// </summary>
        public Model3DGroup GroupModel
        {
            get
            {
                return this.groupModel;
            }

            set
            {
                this.groupModel = value;
            }
        }

        /// <summary>
        /// Gets or sets View Mode
        /// </summary>
        public bool ViewMode { get; set; }

        ///  Event when checked toggle button
        /// </summary>
        /// <param name="parameter">Object parameter</param>
        private void OnCheckedModeViewButtonAction(object parameter)
        {
            this.ViewPort3d.Children.Remove(this.device3D);

            foreach (GeometryModel3D model in this.sensorModelList)
            {
                this.groupModel.Children.Remove(model);

            }

            this.device3D.Content = this.groupModel;
            this.ViewPort3d.Children.Add(this.device3D);
        }

        ///  Event when close window
        /// </summary>
        /// <param name="parameter">Object parameter</param>
        private void OnUnCheckedModeViewButtonAction(object parameter)
        {
            this.ViewPort3d.Children.Remove(this.device3D);

            foreach (GeometryModel3D model in this.sensorModelList)
            {
                this.groupModel.Children.Add(model);

            }

            this.device3D.Content = this.groupModel;
            this.ViewPort3d.Children.Add(this.device3D);
        }

        /// <summary>
        /// Load stl model
        /// </summary>
        /// <param name="stlFile">Stl model path</param>
        public void LoadStlModel(string stlFile)
        {
            this.stlFilePath = stlFile;
            Model3D stlModel = this.Display3d(this.stlFilePath);

            this.groupModel.Children.Add(stlModel);
            this.device3D.Content = this.groupModel;           
        }

        /// <summary>
        /// Load sensors in model
        /// </summary>
        /// <param name="Sensors">List of sensors</param>
        public void LoadSensorsInModel(ObservableCollection<Sensor> sensors)
        {
            this.ViewPort3d.Children.Remove(this.device3D);

            foreach (GeometryModel3D model in this.sensorModelList)
            {
                this.groupModel.Children.Remove(model);
            }

            this.sensorModelList.Clear();

            foreach (Sensor sensor in sensors)
            {
                MeshBuilder meshBuilder = new MeshBuilder();
                
                meshBuilder.AddBox(new Point3D(sensor.X, sensor.Y, sensor.Z), sizeX, sizeY, sizeZ);

                GeometryModel3D sensorModel = new GeometryModel3D(meshBuilder.ToMesh(), MaterialHelper.CreateMaterial(Brushes.Yellow));

                this.sensorModelList.Add(sensorModel);
                this.groupModel.Children.Add(sensorModel);
            }

            this.device3D.Content = groupModel;
            this.ViewPort3d.Children.Add(this.device3D);
        }

        /// <summary>
        /// Display 3D Model
        /// </summary>
        /// <param name="model">Path to the Model file</param>
        /// <returns>3D Model Content</returns>
        private Model3D Display3d(string model)
        {
            Model3D device = null;
            try
            {
                // Adding a gesture here
                this.ViewPort3d.RotateGesture = new MouseGesture(MouseAction.LeftClick);

                // Import 3D model file
                ModelImporter import = new ModelImporter();

                Material material = new DiffuseMaterial(new SolidColorBrush(Colors.Black));
                import.DefaultMaterial = material;

                // Load the 3D model file
                device = import.Load(model);

                Action<GeometryModel3D, Transform3D> nameAction = (geometryModel, transform) =>
                {
                    modelMesh = (MeshGeometry3D)geometryModel.Geometry;
                };

                device.Traverse(nameAction);
            }
            catch (Exception)
            {
                // Handle exception in case can not find the 3D model file
                throw new Exception("not find 3d model file");
            }

            return device;
        }        
    }
}
