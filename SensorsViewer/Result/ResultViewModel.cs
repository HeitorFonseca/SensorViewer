// <copyright file="ResultViewModel.cs" company="GM">
//     gm.com. All rights reserved.
// </copyright>

namespace SensorsViewer.Result
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
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
    public class ResultViewModel : INotifyPropertyChanged
    {
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
        /// Group Model
        /// </summary>
        private Model3DGroup sensorGroupModel = new Model3DGroup();

        /// <summary>
        /// Group Model
        /// </summary>
        private Model3DGroup interpGroupModel = new Model3DGroup();

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
        private IEnumerable<Sensor> Sensors;

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
        public ResultViewModel(IEnumerable<Sensor> sensors)
        {
            this.ViewMode = false;
            this.Sensors = sensors;

            this.ViewPort3d = new HelixViewport3D();
            this.device3D = new ModelVisual3D();
            this.OnCheckedModeViewButtonCommand = new RelayCommand(this.OnCheckedModeViewButtonAction);
            this.OnUnCheckedModeViewButtonCommand = new RelayCommand(this.OnUnCheckedModeViewButtonAction);

            this.LoadSensorsInModel(sensors, "");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultViewModel"/> class
        /// </summary>
        public ResultViewModel(IEnumerable<Sensor> sensors, string analysisName)
        {
            this.ViewMode = false;
            this.Sensors = sensors;

            this.ViewPort3d = new HelixViewport3D();
            this.device3D = new ModelVisual3D();
            this.OnCheckedModeViewButtonCommand = new RelayCommand(this.OnCheckedModeViewButtonAction);
            this.OnUnCheckedModeViewButtonCommand = new RelayCommand(this.OnUnCheckedModeViewButtonAction);

            this.LoadSensorsInModel(sensors, analysisName);
        }

        #region PropertyDeclaration

        /// <summary>
        /// Event for when change property
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

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
                this.OnPropertyChanged("GroupModel");
            }
        }

        /// <summary>
        /// Gets or sets View Mode
        /// </summary>
        public bool ViewMode { get; set; }

        /// <summary>
        /// Gets or Sets Model X dimension
        /// </summary>
        public string ModelXSize { get; set; }

        /// <summary>
        /// Gets or Sets Model Y dimension
        /// </summary>
        public string ModelYSize { get; set; }

        /// <summary>
        /// Gets or Sets Model Z dimension
        /// </summary>
        public string ModelZSize { get; set; }

        #endregion

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

        /// <summary>
        /// Load stl model
        /// </summary>
        /// <param name="stlFile">Stl model path</param>
        public void LoadStlModel(string stlFile)
        {
            this.stlFilePath = stlFile;
            Model3D stlModel = this.Display3d(this.stlFilePath);

            this.ModelXSize = stlModel.Bounds.SizeX.ToString();
            this.ModelYSize = stlModel.Bounds.SizeY.ToString();
            this.ModelZSize = stlModel.Bounds.SizeZ.ToString();

            //this.groupModel.Children.Add(stlModel);
            this.sensorGroupModel.Children.Add(stlModel); // Add in sensor group model
            this.interpGroupModel.Children.Add(stlModel); // Add in interpolation group model

            this.device3D.Content = this.groupModel;           
        }

        /// <summary>
        /// Load sensors in model
        /// </summary>
        /// <param name="Sensors">List of sensors</param>
        public void LoadSensorsInModel(IEnumerable<Sensor> sensors, string analysisName)
        {
            this.ViewPort3d.Children.Remove(this.device3D);

            foreach (GeometryModel3D model in this.sensorModelList)
            {
                this.groupModel.Children.Remove(model);
            }

            this.sensorModelList.Clear();

            foreach (Sensor sensor in sensors)
            {
                Color color;

                if (sensor.Values.Count != 0 && !string.IsNullOrEmpty(analysisName))
                {
                    IEnumerable<SensorValue> svc = from values in sensor.Values where values.AnalysisName == analysisName select values;
                    color = Interpolation.GetHeatMapColor(svc.Last().Value, -1, 1);
                }
                else
                {
                    color = new Color() { A = 255, R = 255, G = 255, B = 0 };
                }

                MeshBuilder meshBuilder = new MeshBuilder();
                
                meshBuilder.AddBox(new Point3D(sensor.X, sensor.Y, sensor.Z), sensor.Size, sensor.Size, sizeZ);               

                GeometryModel3D sensorModel = new GeometryModel3D(meshBuilder.ToMesh(), MaterialHelper.CreateMaterial(color));

                this.sensorModelList.Add(sensorModel);
                this.sensorGroupModel.Children.Add(sensorModel);
            }

            this.GroupModel = sensorGroupModel;
            this.device3D.Content = groupModel;
            this.ViewPort3d.Children.Add(this.device3D);
        }

        /// <summary>
        /// Load sensors in model
        /// </summary>
        /// <param name="Sensors">List of sensors</param>
        public void LoadSensorsValuesInModel(IEnumerable<Sensor> sensors)
        {
            this.ViewPort3d.Children.Remove(this.device3D);

            //foreach (GeometryModel3D model in this.sensorModelList)
            //{
            //    this.groupModel.Children.Remove(model);
            //}

            this.sensorModelList.Clear();

            foreach (Sensor sensor in sensors)
            {
                MeshBuilder meshBuilder = new MeshBuilder();

                meshBuilder.AddBox(new Point3D(sensor.X, sensor.Y, sensor.Z), sensor.Size, sensor.Size, sizeZ);

                Color color;

                // If sensor does not receive any value, receives yellow as collor
                if (sensor.Values.Count != 0)
                {
                    color = Interpolation.GetHeatMapColor(sensor.Values.Last().Value, -1, 1);
                }
                else
                {
                    color = new Color() { A = 255, R = 255, G = 255, B = 0 };
                }

                GeometryModel3D sensorModel = new GeometryModel3D(meshBuilder.ToMesh(), MaterialHelper.CreateMaterial(color));

                this.sensorModelList.Add(sensorModel);
                this.sensorGroupModel.Children.Add(sensorModel);
            }

            this.GroupModel = this.sensorGroupModel;
            this.device3D.Content = groupModel;
            this.ViewPort3d.Children.Add(this.device3D);
        }

        ///  Event when checked toggle button
        /// </summary>
        /// <param name="parameter">Object parameter</param>
        private void OnCheckedModeViewButtonAction(object parameter)
        {
            this.ViewPort3d.Children.Remove(this.device3D);
            //this.groupModel.Children.Clear();

            this.GroupModel = this.interpGroupModel;
            //foreach (GeometryModel3D model in this.sensorModelList)
            //{
            //    this.groupModel.Children.Remove(model);

            //}

            this.device3D.Content = this.groupModel;
            this.ViewPort3d.Children.Add(this.device3D);
        }

        ///  Event when close window
        /// </summary>
        /// <param name="parameter">Object parameter</param>
        private void OnUnCheckedModeViewButtonAction(object parameter)
        {
            this.ViewPort3d.Children.Remove(this.device3D);
            //this.groupModel.Children.Clear();

            this.GroupModel = this.sensorGroupModel;
            //foreach (GeometryModel3D model in this.sensorModelList)
            //{
            //    this.groupModel.Children.Add(model);

            //}

            this.device3D.Content = this.groupModel;
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

                Material material = new DiffuseMaterial(new SolidColorBrush(Colors.DarkSlateGray));
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
