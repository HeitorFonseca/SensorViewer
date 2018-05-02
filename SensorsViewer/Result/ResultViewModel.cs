﻿// <copyright file="ResultViewModel.cs" company="GM">
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
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;
    using HelixToolkit.Wpf;
    using SensorsViewer.SensorOption;
    using SharpGL;
    using SharpGL.SceneGraph;

    /// <summary>
    /// Class for result view model
    /// </summary>
    public class ResultViewModel : INotifyPropertyChanged
    {
        private string parameterString = "direction";

        /// <summary>
        /// Sensor size in z dimension
        /// </summary>
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
        /// Stl model mesh
        /// </summary>
        private MeshGeometry3D modelMesh = null;

        /// <summary>
        /// STL model 3D
        /// </summary>
        private Model3D stlModel;

        /// <summary>
        /// Bool to display sensors
        /// </summary>
        private Visibility sensorsVibility;

        /// <summary>
        /// Bool to display interpolation
        /// </summary>
        private Visibility interpVibility;

        /// <summary>
        /// Stl file path
        /// </summary>
        private string stlFilePath;

        /// <summary>
        /// sensors from analysis
        /// </summary>
        private IEnumerable<Sensor> Sensors;

        /// <summary>
        /// Sensor geometry model
        /// </summary>
        private List<GeometryModel3D>[] sensorModelArray;

        /// <summary>
        /// Interpolation vertex
        /// </summary>
        private Vertex[][] vertices;

        private Interpolation interpolation;

        private int dataCounter;

        private int maxSlider;

        private int slider;

        private int vectorSize = 11;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultViewModel"/> class
        /// </summary>
        public ResultViewModel()
        {
            this.ViewMode = true;
            this.slider = 0;
            this.dataCounter = 0;
            this.maxSlider = 0;

            this.vertices = new Vertex[vectorSize][];
            this.sensorModelArray = new List<GeometryModel3D>[vectorSize];

            this.ViewPort3d = new HelixViewport3D();
            this.device3D = new ModelVisual3D();
            this.OnCheckedModeViewButtonCommand = new RelayCommand(this.OnCheckedModeViewButtonAction);
            this.OnUnCheckedModeViewButtonCommand = new RelayCommand(this.OnUnCheckedModeViewButtonAction);
            this.OnSliderValueChanged = new RelayCommand(this.OnUnSliderValueChangedAction);
            this.OpenGLInitializedCommand = new RelayCommand(this.OpenGLControl_OpenGLInitialized);
            this.OpenGLDraw = new RelayCommand(this.OpenGLControl_OpenGLDraw);
            this.OpenGLResized = new RelayCommand(this.OpenGLControl_Resized);
            this.SensorsVisibility = Visibility.Hidden;
            this.InterpVisibility = Visibility.Visible;            
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref="ResultViewModel"/> class
        /// </summary>
        /// <param name="sensors">Model sensors</param>
        /// <param name="path">Model path</param>
        public ResultViewModel(IEnumerable<Sensor> sensors, string path)
        {
            this.ViewMode = true;
            this.slider = 0;
            this.dataCounter = 0;
            this.maxSlider = 0;

            this.vertices = new Vertex[vectorSize][];
            this.sensorModelArray = new List<GeometryModel3D>[vectorSize];

            this.Sensors = sensors;
            this.ViewPort3d = new HelixViewport3D();
            this.device3D = new ModelVisual3D();
            this.OnCheckedModeViewButtonCommand = new RelayCommand(this.OnCheckedModeViewButtonAction);
            this.OnUnCheckedModeViewButtonCommand = new RelayCommand(this.OnUnCheckedModeViewButtonAction);
            this.OnSliderValueChanged = new RelayCommand(this.OnUnSliderValueChangedAction);
            this.OpenGLInitializedCommand = new RelayCommand(this.OpenGLControl_OpenGLInitialized);
            this.OpenGLDraw = new RelayCommand(this.OpenGLControl_OpenGLDraw);
            this.OpenGLResized = new RelayCommand(this.OpenGLControl_Resized);

            this.LoadStlModel(path);

            this.interpolation = new Interpolation(modelMesh, sensors);

            this.LoadSensorsInModel(sensors, string.Empty);

            this.SensorsVisibility = Visibility.Hidden;
            this.InterpVisibility = Visibility.Visible;

        }

        /// <summary>
        ///  Initializes a new instance of the <see cref="ResultViewModel"/> class
        /// </summary>
        /// <param name="sensors">Model sensors</param>
        /// <param name="path">Model path</param>
        /// <param name="analysisName">Analysis name</param>
        public ResultViewModel(IEnumerable<Sensor> sensors, string path, string analysisName)
        {
            this.ViewMode = true;
            this.slider = 0;
            this.dataCounter = 0;
            this.maxSlider = 0;

            this.Sensors = sensors;

            this.sensorModelArray = new List<GeometryModel3D>[vectorSize];
            this.vertices = new Vertex[vectorSize][];


            this.ViewPort3d = new HelixViewport3D();
            this.device3D = new ModelVisual3D();
            this.OnCheckedModeViewButtonCommand = new RelayCommand(this.OnCheckedModeViewButtonAction);
            this.OnUnCheckedModeViewButtonCommand = new RelayCommand(this.OnUnCheckedModeViewButtonAction);
            this.OnSliderValueChanged = new RelayCommand(this.OnUnSliderValueChangedAction);
            this.OpenGLInitializedCommand = new RelayCommand(this.OpenGLControl_OpenGLInitialized);
            this.OpenGLDraw = new RelayCommand(this.OpenGLControl_OpenGLDraw);
            this.OpenGLResized = new RelayCommand(this.OpenGLControl_Resized);

            this.LoadStlModel(path);

            this.interpolation = new Interpolation(modelMesh, sensors);

            this.LoadSensorsInModel(sensors, analysisName);

            this.SensorsVisibility = Visibility.Hidden;
            this.InterpVisibility = Visibility.Visible;
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
        ///  Gets or sets click mode view command
        /// </summary>
        public ICommand OnSliderValueChanged { get; set; }

        /// <summary>
        ///  Gets or sets Loaded window command
        /// </summary>
        public ICommand OpenGLInitializedCommand { get; set; }

        /// <summary>
        ///  Gets or sets Loaded window command
        /// </summary>
        public ICommand OpenGLDraw { get; set; }

        /// <summary>
        ///  Gets or sets Loaded window command
        /// </summary>
        public ICommand OpenGLResized { get; set; }

        /// <summary>
        /// Gets or sets Hellix view port 3d
        /// </summary>
        public HelixViewport3D ViewPort3d { get; set; }

        /// <summary>
        /// Gets or sets group model
        /// </summary>
        public Visibility SensorsVisibility
        {
            get
            {
                return this.sensorsVibility;
            }

            set
            {
                this.sensorsVibility = value;
                this.OnPropertyChanged("SensorsVisibility");
            }
        }

        /// <summary>
        /// Gets or sets group model
        /// </summary>
        public Visibility InterpVisibility
        {
            get
            {
                return this.interpVibility;
            }

            set
            {
                this.interpVibility = value;
                this.OnPropertyChanged("InterpVisibility");
            }
        }

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
        /// Gets or sets a value indicating whether View Mode
        /// </summary>
        public bool ViewMode { get; set; }

        /// <summary>
        /// Gets or sets a slider value
        /// </summary>
        public int Slider
        {
            get
            {
                return this.slider;
            }

            set
            {
                this.slider = value;
                this.OnPropertyChanged("Slider");
            }

        }

        /// <summary>
        /// Gets or sets a slider value
        /// </summary>
        public int MaxSlider
        {
            get
            {                
                return this.maxSlider;
            }

            set
            {
                this.maxSlider = value;
                this.OnPropertyChanged("MaxSlider");

            }
        }

        /// <summary>
        /// Gets or sets Model X dimension
        /// </summary>
        public string ModelXSize { get; set; }

        /// <summary>
        /// Gets or setsModel Y dimension
        /// </summary>
        public string ModelYSize { get; set; }

        /// <summary>
        /// Gets or sets Model Z dimension
        /// </summary>
        public string ModelZSize { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Load stl model
        /// </summary>
        /// <param name="stlFile">Stl model path</param>
        public void LoadStlModel(string stlFile)
        {
            this.stlFilePath = stlFile;
            this.stlModel = this.Display3d(this.stlFilePath);

            this.ModelXSize = this.stlModel.Bounds.SizeX.ToString();
            this.ModelYSize = this.stlModel.Bounds.SizeY.ToString();
            this.ModelZSize = this.stlModel.Bounds.SizeZ.ToString();

            this.sensorGroupModel.Children.Add(this.stlModel); // Add in sensor group model

            this.device3D.Content = this.groupModel;             
        }

        /// <summary>
        /// Load sensors in model
        /// </summary>
        /// <param name="sensors">List of sensors</param>
        /// <param name="analysisName">Analysis name</param>
        public void LoadSensorsInModel(IEnumerable<Sensor> sensors, string analysisName)
        {
            this.ViewPort3d.Children.Remove(this.device3D);

            this.sensorGroupModel.Children.Clear();
            this.sensorGroupModel.Children.Add(this.stlModel);

            this.sensorModelArray[dataCounter] = new List<GeometryModel3D>();

            foreach (Sensor sensor in sensors)
            {
                MeshBuilder meshBuilder = new MeshBuilder();

                Color color = new Color() { A = 255, R = 255, G = 255, B = 0 };

                if (sensor.Values.Count != 0 && !string.IsNullOrEmpty(analysisName))
                {
                    // Get data from analysis 
                    IEnumerable<SensorValue> svc = from values in sensor.Values where values.AnalysisName == analysisName select values;

                    if (svc.Count() > 0)
                    {
                        // Group by parameters
                        IEnumerable<IGrouping<string, SensorValue>> asd = svc.GroupBy(a => a.Parameter);
                        foreach (IGrouping<string, SensorValue> gp in asd)
                        {
                            // If parameter is direction then add arrow with the value
                            if (gp.Key == parameterString)
                            {
                                double cte = gp.Last().Value * Math.PI / 180;

                                Point3D newPoint = new Point3D(sensor.X + 4 * sensor.Size * Math.Cos(cte), sensor.Y + 4 * sensor.Size * Math.Sin(cte), sensor.Z);
                                meshBuilder.AddArrow(new Point3D(sensor.X, sensor.Y, sensor.Z), newPoint, 3);
                            }
                            else
                            {
                                // With the respective color
                                color = Interpolation.GetHeatMapColor(gp.Last().Value, -1, +1);
                            }
                        }
                    }
                    else
                    {
                        color = new Color() { A = 255, R = 255, G = 255, B = 0 };
                    }
                }
              
                
                meshBuilder.AddBox(new Point3D(sensor.X, sensor.Y, sensor.Z), sensor.Size, sensor.Size, this.sizeZ);               

                GeometryModel3D sensorModel = new GeometryModel3D(meshBuilder.ToMesh(), MaterialHelper.CreateMaterial(color));

                this.sensorModelArray[dataCounter].Add(sensorModel);

                this.sensorGroupModel.Children.Add(sensorModel);
            }

            if (this.modelMesh != null)
            {
                this.vertices[this.dataCounter++] = this.interpolation.Interpolate2(modelMesh, SensorsNotParameter(sensors));
                this.MaxSlider = dataCounter - 1;
                this.Slider = this.MaxSlider;
            }

            this.GroupModel = this.sensorGroupModel;
            this.device3D.Content = this.groupModel;
            this.ViewPort3d.Children.Add(this.device3D);
        }

        /// <summary>
        /// Load sensors in model
        /// </summary>
        /// <param name="sensors">List of sensors</param>
        public void LoadSensorsValuesInModel(IEnumerable<Sensor> sensors)
        {
            this.ViewPort3d.Children.Remove(this.device3D);
            this.sensorGroupModel.Children.Clear();
            this.sensorGroupModel.Children.Add(this.stlModel);

            this.sensorModelArray[dataCounter] = new List<GeometryModel3D>();

            foreach (Sensor sensor in sensors)
            {
                MeshBuilder meshBuilder = new MeshBuilder();

                IEnumerable<IGrouping<string, SensorValue>> asd  = sensor.Values.GroupBy(a => a.Parameter);

                double value = 0;

                foreach (IGrouping<string, SensorValue> gp in asd)
                {
                    if (gp.Key == parameterString)
                    {
                        double cte = gp.Last().Value * Math.PI / 180;

                        Point3D newPoint = new Point3D(sensor.X + 4 * sensor.Size * Math.Cos(cte), sensor.Y + 4 * sensor.Size * Math.Sin(cte), sensor.Z);
                        meshBuilder.AddArrow(new Point3D(sensor.X, sensor.Y, sensor.Z), newPoint, 3);                       
                    }
                    else
                    {
                        value = gp.Last().Value;
                        meshBuilder.AddBox(new Point3D(sensor.X, sensor.Y, sensor.Z), sensor.Size, sensor.Size, this.sizeZ);
                    }                    
                }

                Color color;

                // If sensor does not receive any value, receives yellow as collor
                if (sensor.Values.Count != 0)
                {
                    color = Interpolation.GetHeatMapColor(value, -1, +1);
                }
                else
                {
                    color = new Color() { A = 255, R = 255, G = 255, B = 0 };
                }

                GeometryModel3D sensorModel = new GeometryModel3D(meshBuilder.ToMesh(), MaterialHelper.CreateMaterial(color));
                this.sensorModelArray[dataCounter].Add(sensorModel);
                this.sensorGroupModel.Children.Add(sensorModel);
            }

            if (this.modelMesh != null)
            {

                this.vertices[this.dataCounter++] = this.interpolation.Interpolate2(modelMesh, SensorsNotParameter(sensors));
                this.MaxSlider = dataCounter - 1;
                this.Slider = this.MaxSlider;
            }

            this.GroupModel = this.sensorGroupModel;
            this.device3D.Content = this.groupModel;
            this.ViewPort3d.Children.Add(this.device3D);
        }

        private List<Sensor> SensorsNotParameter(IEnumerable<Sensor> sensors)
        {
            List<Sensor> asd = new List<Sensor>();

            foreach (Sensor sensor in sensors)
            {
                //List<Sensor> s = sensors.Where(a => a.Values.Where(b => b.Parameter != parameterString));

                Sensor dsa = new Sensor(sensor.SensorName, sensor.X, sensor.Y, sensor.Z);

                dsa.Values = sensor.Values.Where(a => a.Parameter != parameterString).ToList();

                asd.Add(dsa);

            }

            return asd;
        }

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
        /// Event when sharpgl initialize
        /// </summary>
        /// <param name="parameter">object parameter</param>
        private void OpenGLControl_OpenGLInitialized(object parameter)
        {
            OpenGL gl = ((OpenGLEventArgs)parameter).OpenGL;

            gl.ClearColor(255, 255, 255, 255);
        }

        /// <summary>
        /// Event to sharpgl draw
        /// </summary>
        /// <param name="parameter">object parameter</param>
        private void OpenGLControl_OpenGLDraw(object parameter)
        {
            if (this.vertices == null || this.vertices[Slider] == null)
            {
                return;
            }

            OpenGL gl = ((OpenGLEventArgs)parameter).OpenGL;

            // Clear the color and depth buffers.
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            // Reset the modelview matrix.
            gl.LoadIdentity();

            // Move the geometry into a fairly central position.
            gl.Translate(0.0f, 0.0f, -20.0f);

            // Draw a pyramid. First, rotate the modelview matrix.
            ////gl.Rotate(rotatePyramid, 0.0f, 1.0f, 0.0f);

            gl.PointSize(1.0f);

            gl.Begin(OpenGL.GL_TRIANGLES);

            for (int i = 0; i < this.vertices[Slider].Count(); i++)
            {
                Color asd = Interpolation.GetHeatMapColor(this.vertices[Slider][i].Z, -1, +1);

                gl.Color(asd.R / (float)255, asd.G / (float)255, asd.B / (float)255);
                ////gl.Color(0.5f, 0.5f, 0.5f);
                gl.Vertex(this.vertices[Slider][i].X/100, this.vertices[Slider][i].Y/100, 0.0f);
            }

            gl.End();

            // Flush OpenGL.
            gl.Flush();
        }

        /// <summary>
        /// When window resize
        /// </summary>
        /// <param name="parameter">Object parameter</param>
        private void OpenGLControl_Resized(object parameter)
        {
            // Get the OpenGL instance.
            OpenGL gl = ((OpenGLEventArgs)parameter).OpenGL;

            // Load and clear the projection matrix.
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();

            // Calculate The Aspect Ratio Of The Window
            gl.Perspective(45.0f, (float)gl.RenderContextProvider.Width / (float)gl.RenderContextProvider.Height, 0.1f, 100.0f);

            // Load the modelview.
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }

        /// <summary>
        /// Event when checked toggle button
        /// </summary>
        /// <param name="parameter">Object parameter</param>
        private void OnCheckedModeViewButtonAction(object parameter)
        {
            this.SensorsVisibility = Visibility.Hidden;
            this.InterpVisibility = Visibility.Visible;

            this.ViewMode = true;          
        }

        /// <summary>
        /// Event when close window
        /// </summary>
        /// <param name="parameter">Object parameter</param>
        private void OnUnCheckedModeViewButtonAction(object parameter)
        {
            this.SensorsVisibility = Visibility.Visible;
            this.InterpVisibility = Visibility.Hidden;

            this.ViewMode = true;        
        }

        /// <summary>
        /// Event when close window
        /// </summary>
        /// <param name="parameter">Object parameter</param>
        private void OnUnSliderValueChangedAction(object parameter)
        {
           
            int newValue = Convert.ToInt32(((RoutedPropertyChangedEventArgs<double>)parameter).NewValue);

            this.ViewPort3d.Children.Remove(this.device3D);
            this.sensorGroupModel.Children.Clear();
            this.sensorGroupModel.Children.Add(this.stlModel);

            foreach (var model3d in this.sensorModelArray[newValue])
            {
                this.sensorGroupModel.Children.Add(model3d);
            }

            this.GroupModel = this.sensorGroupModel;
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

                Material material = new DiffuseMaterial(new SolidColorBrush(Colors.LightSlateGray));
                import.DefaultMaterial = material;

                // Load the 3D model file
                device = import.Load(model);

                Action<GeometryModel3D, Transform3D> nameAction = (geometryModel, transform) =>
                {
                    modelMesh = (MeshGeometry3D)geometryModel.Geometry;
                };               

                device.Traverse(nameAction);             

            }
            catch (Exception e)
            {
                // Handle exception in case can not find the 3D model file
                throw new Exception("not find 3d model file");
            }
            
            return device;
        }

        /// <summary>
        /// Get sensors from analysis name
        /// </summary>
        /// <param name="tabSensor">Sensors from tab</param>
        /// <param name="sensorsId">Sensors Ids</param>
        /// <returns>Sensors from analysis</returns>
        private ObservableCollection<Sensor> GetSensorsFromAnalysis(IEnumerable<Sensor> tabSensor, ObservableCollection<string> sensorsId)
        {
            ObservableCollection<Sensor> analysisSensors = new ObservableCollection<Sensor>();

            foreach (Sensor s in tabSensor)
            {
                if (sensorsId.Contains(s.Id))
                {
                    analysisSensors.Add(s);
                }
            }

            return analysisSensors;
        }
    }
}
