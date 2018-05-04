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
    using System.Text.RegularExpressions;
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
        /// Stl file path
        /// </summary>
        private string analysisFolderPath;

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

        private bool[] savedVertices;

        private System.Drawing.Bitmap[] textureImages;

        private Interpolation interpolation;

        private int dataCounter;

        private int maxSlider;

        private int slider = 0;

        private int vectorSize = 10;

        private uint[] textures = new uint[1];

        private bool initialized = false;

        private bool oldAnalysis = false;


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
            this.savedVertices = new bool[this.vectorSize];
            this.textureImages = new System.Drawing.Bitmap[this.vectorSize];

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
            this.savedVertices = new bool[this.vectorSize];
            this.textureImages = new System.Drawing.Bitmap[this.vectorSize];

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

            //this.interpolation = new Interpolation(modelMesh, sensors);

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
            this.savedVertices = new bool[this.vectorSize];
            this.textureImages = new System.Drawing.Bitmap[this.vectorSize];

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

            this.analysisFolderPath = System.IO.Directory.GetCurrentDirectory() + @"\..\..\Resources\Analysis\" + analysisName.Replace(':','.');

            if (!this.CreateAnalysisFolder(analysisFolderPath))
            {
                oldAnalysis = true;
                this.LoadScreenshot();
            }
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

            this.GroupModel = this.sensorGroupModel;
            this.device3D.Content = this.groupModel;

            if (!this.ViewPort3d.Children.Contains(this.device3D))
            {
                this.ViewPort3d.Children.Add(this.device3D);
            }
        }

        /*
        public void LoadSensorsInModel2(IEnumerable<Sensor> sensors, string analysisName)
        {
            this.ViewPort3d.Children.Remove(this.device3D);

            this.sensorGroupModel.Children.Clear();
            this.sensorGroupModel.Children.Add(this.stlModel);

            this.sensorModelArray[dataCounter] = new List<GeometryModel3D>();

            int pos = 0;
            foreach (Sensor sensor in sensors)
            {

                Color color = new Color() { A = 255, R = 255, G = 255, B = 0 };
                MeshBuilder meshBuilder;

                if (sensor.Values.Count != 0)
                {
                    // Get data from analysis 
                    IEnumerable<SensorValue> svc = from values in sensor.Values where values.AnalysisName == analysisName select values;

                    if (svc.Count() > 0)
                    {
                        // Group by parameters
                        List<IGrouping<string, SensorValue>> grouped = svc.GroupBy(a => a.Parameter).ToList();

                        int[] indexs = (grouped[0].Key == this.parameterString ? new int[] { 0, 1 } : new int[] { 1, 0 });

                        for (int i = 0; i < grouped[indexs[0]].Count(); i++)
                        {
                            if (this.sensorModelArray[i] == null)
                            {
                                this.sensorModelArray[i] = new List<GeometryModel3D>();
                            }

                            meshBuilder = new MeshBuilder();

                            double cte = grouped[indexs[0]].ElementAt(i).Value * Math.PI / 180;

                            Point3D newPoint = new Point3D(sensor.X + 4 * sensor.Size * Math.Cos(cte), sensor.Y + 4 * sensor.Size * Math.Sin(cte), sensor.Z);
                            meshBuilder.AddArrow(new Point3D(sensor.X, sensor.Y, sensor.Z), newPoint, 3);

                            color = Interpolation.GetHeatMapColor(grouped[indexs[1]].ElementAt(i).Value, -1, +1);
                            meshBuilder.AddBox(new Point3D(sensor.X, sensor.Y, sensor.Z), sensor.Size, sensor.Size, this.sizeZ);

                            GeometryModel3D sensorModel2 = new GeometryModel3D(meshBuilder.ToMesh(), MaterialHelper.CreateMaterial(color));

                            this.sensorModelArray[i].Add(sensorModel2);
                        }                       
                    }
                    else
                    {
                        meshBuilder = new MeshBuilder();

                        color = new Color() { A = 255, R = 255, G = 255, B = 0 };
                        meshBuilder.AddBox(new Point3D(sensor.X, sensor.Y, sensor.Z), sensor.Size, sensor.Size, this.sizeZ);

                        GeometryModel3D sensorModel2 = new GeometryModel3D(meshBuilder.ToMesh(), MaterialHelper.CreateMaterial(color));

                        this.sensorModelArray[pos].Add(sensorModel2);
                    }
                }            
            }

            foreach (var model3d in this.sensorModelArray[this.sensorModelArray.Length - 1])
            {
                this.sensorGroupModel.Children.Add(model3d);
            }

            this.GroupModel = this.sensorGroupModel;
            this.device3D.Content = this.groupModel;
            this.ViewPort3d.Children.Add(this.device3D);
        }
        */

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
            if (!this.ViewPort3d.Children.Contains(this.device3D))
            {
                this.ViewPort3d.Children.Add(this.device3D);
            }
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
            OpenGL gl = gl = ((OpenGLEventArgs)parameter).OpenGL;

            if (!oldAnalysis && (this.vertices == null || this.vertices[Slider] == null))
            {
                return;
            }

            if (!oldAnalysis)
            {
                // Clear the color and depth buffers.
                gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

                // Reset the modelview matrix.
                gl.LoadIdentity();

                // Move the geometry into a fairly central position.
                gl.Translate(0.0f, 0.0f, -20.0f);

                gl.PointSize(1.0f);

                gl.Begin(OpenGL.GL_TRIANGLES);

                for (int i = 0; i < this.vertices[Slider].Count(); i++)
                {
                    Color asd = Interpolation.GetHeatMapColor(this.vertices[Slider][i].Z, -1, +1);

                    gl.Color(asd.R / (float)255, asd.G / (float)255, asd.B / (float)255);

                    gl.Vertex(this.vertices[Slider][i].X / 100, this.vertices[Slider][i].Y / 100, 0.0f);
                }

                gl.End();

                // Flush OpenGL.
                gl.Flush();

                if (!this.savedVertices[Slider])
                {
                    this.TakeScreenshot(gl, Slider);
                    this.savedVertices[Slider] = true;
                }                
            }

            else
            {                
                if (!this.initialized && this.textureImages[Slider] != null)
                {
                    SelectTexture(gl, Slider);
                    this.initialized = true;
                }

                gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
                gl.LoadIdentity();
                gl.Translate(0.0f, 0.0f, -3.2f);

                gl.BindTexture(OpenGL.GL_TEXTURE_2D, textures[0]);

                gl.Begin(OpenGL.GL_QUADS);

                // Front Face
                gl.TexCoord(0.0f, 0.0f); gl.Vertex(-1.0f, -1.0f, 1.0f); // Bottom Left Of The Texture and Quad
                gl.TexCoord(1.0f, 0.0f); gl.Vertex(1.0f, -1.0f, 1.0f);  // Bottom Right Of The Texture and Quad
                gl.TexCoord(1.0f, 1.0f); gl.Vertex(1.0f, 1.0f, 1.0f);   // Top Right Of The Texture and Quad
                gl.TexCoord(0.0f, 1.0f); gl.Vertex(-1.0f, 1.0f, 1.0f);  // Top Left Of The Texture and Quad


                gl.End();

                gl.Flush();
            }
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

            //TODO
            foreach (var model3d in this.sensorModelArray[0])
            {
                this.sensorGroupModel.Children.Add(model3d);
            }

            this.GroupModel = this.sensorGroupModel;
            this.device3D.Content = this.groupModel;

            if (!this.ViewPort3d.Children.Contains(this.device3D))
            {
                this.ViewPort3d.Children.Add(this.device3D);
            }

            this.initialized = false;

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

        private void TakeScreenshot(OpenGL gl, int index)
        {
            int w = 512;// gl.RenderContextProvider.Width;
            int h = 512;// gl.RenderContextProvider.Height;
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(w, h);
            System.Drawing.Imaging.BitmapData data =
                bmp.LockBits(new System.Drawing.Rectangle(0, 0, 512, 512),
                System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            
            gl.ReadPixels(0, 0, w, h, OpenGL.GL_BGR, OpenGL.GL_UNSIGNED_BYTE, data.Scan0);

            bmp.UnlockBits(data);

            bmp.Save(this.analysisFolderPath + "\\Img" + index + ".bmp");
        }

        private void LoadScreenshot()
        {
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(this.analysisFolderPath);

            var files = System.IO.Directory.GetFiles(this.analysisFolderPath, "*", System.IO.SearchOption.TopDirectoryOnly);

            int count = 0;
            foreach (string file in files)
            {
                Regex asd = new Regex("Img[0-9]+");

                Match dsa = asd.Match(file);

                string[] ret = dsa.Captures[0].Value.Split(new[] { "Img" }, StringSplitOptions.None);

                int index = Convert.ToInt32(ret[1]);

                textureImages[index] = new System.Drawing.Bitmap(this.analysisFolderPath + "\\Img" + index + ".bmp");

                count++;
            }

            int fCount = System.IO.Directory.GetFiles(this.analysisFolderPath, "*", System.IO.SearchOption.TopDirectoryOnly).Length;

            this.MaxSlider = count - 1;
            this.Slider = MaxSlider;
        }

        private void SelectTexture(OpenGL gl, int index)
        {          
            //  A bit of extra initialisation here, we have to enable textures.
            gl.Enable(OpenGL.GL_TEXTURE_2D);

            //  Get one texture id, and stick it into the textures array.
            gl.GenTextures(1, textures);

            //  Bind the texture.
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, textures[0]);

            System.Drawing.Imaging.BitmapData data = textureImages[index].LockBits(new System.Drawing.Rectangle(0, 0, 512, 512),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            //  Tell OpenGL where the texture data is.
            gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, 3, 512, 512, 0, OpenGL.GL_BGR, OpenGL.GL_UNSIGNED_BYTE,
                data.Scan0);

            textureImages[index].UnlockBits(data);

            //  Specify linear filtering.
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);
            
        }

        private bool CreateAnalysisFolder(string path)
        {
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);

                return true;
            }

            return false;
        }

        //private void SaveVerticesInFile(int index)
        //{
        //    using (System.IO.StreamWriter file =
        //    new System.IO.StreamWriter(@"C:\Users\heitor.araujo\source\repos\SensorViewer\SensorsViewer\Resources\Analysis\WriteLines2.txt", true))
        //    {
        //        file.WriteLine("Vertex " + index + 1);

        //        file.Write("" + this.vertices[index][0].Z);

        //        for (int i = 1; i < this.vertices[index].Count(); i++)
        //        {
        //            file.Write(" " + this.vertices[index][i].Z);
        //        }
        //    }
        //}
    }
}
