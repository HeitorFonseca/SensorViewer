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
        /// <summary>
        /// Array size
        /// </summary>
        private const int ARRAYSIZE = 20;

        /// <summary>
        /// Indicate which parameter is not going to be interpolated
        /// </summary>
        private string parameterString = "direction";

        /// <summary>
        /// Sensor size in z dimension
        /// </summary>
        private double sizeZ = 0.005;

        /// <summary>
        /// Arrow head size
        /// </summary>
        private int arrowHeadSize = 3;

        /// <summary>
        /// Arrow Diameter size
        /// </summary>
        private int arrowDiameterSize = 6;

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
        /// Sensor geometry model
        /// </summary>
        private List<GeometryModel3D>[] sensorModelArray;

        /// <summary>
        /// Interpolation vertex
        /// </summary>
        private Vertex[][] vertices;

        /// <summary>
        /// Bool array to indicate if texture was saved
        /// </summary>
        private bool[] savedVertices;

        /// <summary>
        /// Bitmap array of texture images
        /// </summary>
        private System.Drawing.Bitmap[] textureImages;

        /// <summary>
        /// Interpolation object
        /// </summary>
        private Interpolation interpolation;

        /// <summary>
        /// Number of interpolation already calculated
        /// </summary>
        private int dataCounter;

        /// <summary>
        /// Max slider value
        /// </summary>
        private int maxSlider;

        /// <summary>
        /// Current slider value
        /// </summary>
        private int slider = 0;

        /// <summary>
        /// Texture id
        /// </summary>
        private uint[] textures = new uint[1];

        /// <summary>
        /// Bool to render texture
        /// </summary>
        private bool changeImage = false;

        /// <summary>
        /// Bool to indicate if is a old or new analysis
        /// </summary>
        private bool oldAnalysis = false;

        /// <summary>
        /// Variable to control images to be displayed and saved
        /// </summary>
        private int displayInterp = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultViewModel"/> class
        /// </summary>
        public ResultViewModel()
        {
            this.ViewMode = true;
            this.slider = 0;
            this.dataCounter = 0;
            this.maxSlider = 0;

            this.vertices = new Vertex[ARRAYSIZE][];
            this.sensorModelArray = new List<GeometryModel3D>[ARRAYSIZE];
            this.savedVertices = new bool[ARRAYSIZE];
            this.textureImages = new System.Drawing.Bitmap[ARRAYSIZE];

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

            this.vertices = new Vertex[ARRAYSIZE][];
            this.sensorModelArray = new List<GeometryModel3D>[ARRAYSIZE];
            this.savedVertices = new bool[ARRAYSIZE];
            this.textureImages = new System.Drawing.Bitmap[ARRAYSIZE];

            this.ViewPort3d = new HelixViewport3D();
            this.device3D = new ModelVisual3D();
            this.OnCheckedModeViewButtonCommand = new RelayCommand(this.OnCheckedModeViewButtonAction);
            this.OnUnCheckedModeViewButtonCommand = new RelayCommand(this.OnUnCheckedModeViewButtonAction);
            this.OnSliderValueChanged = new RelayCommand(this.OnUnSliderValueChangedAction);
            this.OpenGLInitializedCommand = new RelayCommand(this.OpenGLControl_OpenGLInitialized);
            this.OpenGLDraw = new RelayCommand(this.OpenGLControl_OpenGLDraw);
            this.OpenGLResized = new RelayCommand(this.OpenGLControl_Resized);

            this.LoadStlModel(path);

            ////this.interpolation = new Interpolation(modelMesh, sensors);

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

            this.sensorModelArray = new List<GeometryModel3D>[ARRAYSIZE];
            this.vertices = new Vertex[ARRAYSIZE][];
            this.savedVertices = new bool[ARRAYSIZE];
            this.textureImages = new System.Drawing.Bitmap[ARRAYSIZE];

            this.ViewPort3d = new HelixViewport3D();
            this.device3D = new ModelVisual3D();
            this.OnCheckedModeViewButtonCommand = new RelayCommand(this.OnCheckedModeViewButtonAction);
            this.OnUnCheckedModeViewButtonCommand = new RelayCommand(this.OnUnCheckedModeViewButtonAction);
            this.OnSliderValueChanged = new RelayCommand(this.OnUnSliderValueChangedAction);
            this.OpenGLInitializedCommand = new RelayCommand(this.OpenGLControl_OpenGLInitialized);
            this.OpenGLDraw = new RelayCommand(this.OpenGLControl_OpenGLDraw);
            this.OpenGLResized = new RelayCommand(this.OpenGLControl_Resized);

            this.LoadStlModel(path);

            this.interpolation = new Interpolation(this.modelMesh, sensors);

            this.SensorsVisibility = Visibility.Hidden;
            this.InterpVisibility = Visibility.Visible;

            this.analysisFolderPath = System.IO.Directory.GetCurrentDirectory() + @"\Resources\Analysis\" + analysisName.Replace(':', '.');

            if (!this.CreateAnalysisFolder(this.analysisFolderPath))
            {
                this.LoadSensorsInModel2(sensors, analysisName);

                this.oldAnalysis = true;
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
            // this.ViewPort3d.Children.Remove(this.device3D);
            this.sensorGroupModel.Children.Clear();
            this.sensorGroupModel.Children.Add(this.stlModel);

            this.sensorModelArray[this.dataCounter] = new List<GeometryModel3D>();

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
                            if (gp.Key == this.parameterString)
                            {
                                double cte = gp.Last().Value * Math.PI / 180;

                                Point3D newPoint = new Point3D(sensor.X + (4 * sensor.Size * Math.Cos(cte)), sensor.Y + (4 * sensor.Size * Math.Sin(cte)), sensor.Z);
                                meshBuilder.AddArrow(new Point3D(sensor.X, sensor.Y, sensor.Z), newPoint, this.arrowDiameterSize, this.arrowHeadSize);
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

                this.sensorModelArray[this.dataCounter].Add(sensorModel);

                this.sensorGroupModel.Children.Add(sensorModel);
            }

            this.GroupModel = this.sensorGroupModel;
            this.device3D.Content = this.groupModel;

            if (!this.ViewPort3d.Children.Contains(this.device3D))
            {
                this.ViewPort3d.Children.Add(this.device3D);
            }
        }

        /// <summary>
        /// Load sensors in model
        /// </summary>
        /// <param name="sensors">List of sensors</param>
        /// <param name="analysisName">Analysis name</param>
        public void LoadSensorsInModel2(IEnumerable<Sensor> sensors, string analysisName)
        {
            // this.ViewPort3d.Children.Remove(this.device3D);
            this.sensorGroupModel.Children.Clear();
            this.sensorGroupModel.Children.Add(this.stlModel);

            this.sensorModelArray[this.dataCounter] = new List<GeometryModel3D>();

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

                        int[] indexs = grouped[0].Key == this.parameterString ? new int[] { 0, 1 } : new int[] { 1, 0 };

                        for (int i = 0; i < grouped[indexs[1]].Count(); i++)
                        {
                            if (this.sensorModelArray[i] == null)
                            {
                                this.sensorModelArray[i] = new List<GeometryModel3D>();
                            }

                            meshBuilder = new MeshBuilder();

                            color = Interpolation.GetHeatMapColor(grouped[indexs[1]].ElementAt(i).Value, -1, +1);
                            meshBuilder.AddBox(new Point3D(sensor.X, sensor.Y, sensor.Z), sensor.Size, sensor.Size, this.sizeZ);

                            if (grouped.Count > 1)
                            {
                                double cte = grouped[indexs[0]].ElementAt(i).Value * Math.PI / 180;

                                Point3D newPoint = new Point3D(sensor.X + (4 * sensor.Size * Math.Cos(cte)), sensor.Y + (4 * sensor.Size * Math.Sin(cte)), sensor.Z);
                                meshBuilder.AddArrow(new Point3D(sensor.X, sensor.Y, sensor.Z), newPoint, this.arrowDiameterSize, this.arrowHeadSize);
                            }

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

            // TODO
            // foreach (var model3d in this.sensorModelArray[this.sensorModelArray.Length - 1])
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
        }
        
        /// <summary>
        /// Load sensors in model
        /// </summary>
        /// <param name="sensors">List of sensors</param>
        public void LoadSensorsValuesInModel(IEnumerable<Sensor> sensors)
        {
            this.sensorGroupModel.Children.Clear();
            this.sensorGroupModel.Children.Add(this.stlModel);

            this.sensorModelArray[this.dataCounter] = new List<GeometryModel3D>();

            foreach (Sensor sensor in sensors)
            {
                MeshBuilder meshBuilder = new MeshBuilder();

                IEnumerable<IGrouping<string, SensorValue>> asd  = sensor.Values.GroupBy(a => a.Parameter);

                double value = 0;

                foreach (IGrouping<string, SensorValue> gp in asd)
                {
                    if (gp.Key == this.parameterString)
                    {
                        double cte = gp.Last().Value * Math.PI / 180;

                        Point3D newPoint = new Point3D(sensor.X + (4 * sensor.Size * Math.Cos(cte)), sensor.Y + (4 * sensor.Size * Math.Sin(cte)), sensor.Z);
                        meshBuilder.AddArrow(new Point3D(sensor.X, sensor.Y, sensor.Z), newPoint, this.arrowDiameterSize, this.arrowHeadSize);                       
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
                this.sensorModelArray[this.dataCounter].Add(sensorModel);
                this.sensorGroupModel.Children.Add(sensorModel);
            }

            if (this.modelMesh != null)
            {
                this.vertices[this.dataCounter++] = this.interpolation.Interpolate2(this.modelMesh, this.SensorsNotParameter(sensors));
                this.MaxSlider = this.dataCounter - 1;
                this.Slider = this.MaxSlider;
            }

            this.GroupModel = this.sensorGroupModel;
            this.device3D.Content = this.groupModel;

            if (!this.ViewPort3d.Children.Contains(this.device3D))
            {
                this.ViewPort3d.Children.Add(this.device3D);
            }
        }

        /// <summary>
        /// Dispose all images from analysis to delete directory
        /// </summary>
        public void FreeTextureImages()
        {
            for (int i = 0; i < this.textureImages.Count(); i++)
            {
                if (this.textureImages[i] != null)
                {
                    this.textureImages[i].Dispose();
                    this.textureImages[i] = null;
                }
            }
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
        /// Function to returns sensor with only interpolation parameters
        /// </summary>
        /// <param name="sensors">Sensor list</param>
        /// <returns>Returns sensors with only interpolation parameters </returns>
        private List<Sensor> SensorsNotParameter(IEnumerable<Sensor> sensors)
        {
            List<Sensor> asd = new List<Sensor>();

            foreach (Sensor sensor in sensors)
            {
                Sensor newSensor = new Sensor(sensor.SensorName, sensor.X, sensor.Y, sensor.Z)
                {
                    Values = sensor.Values.Where(a => a.Parameter != this.parameterString).ToList()
                };

                asd.Add(newSensor);
            }

            return asd;
        }

        /// <summary>
        /// Event when sharpgl initialize
        /// </summary>
        /// <param name="parameter">object parameter</param>
        private void OpenGLControl_OpenGLInitialized(object parameter)
        {
            OpenGL gl = ((OpenGLEventArgs)parameter).OpenGL;

            gl.ClearColor(255, 255, 255, 255);

            gl.Flush();
        }

        /// <summary>
        /// Event to sharpgl draw
        /// </summary>
        /// <param name="parameter">object parameter</param>
        private void OpenGLControl_OpenGLDraw(object parameter)
        {
            OpenGL gl = gl = ((OpenGLEventArgs)parameter).OpenGL;
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.LoadIdentity();
            gl.Flush();

            if (!this.oldAnalysis && (this.vertices == null || this.vertices[this.Slider] == null))
            {
                return;
            }

            if (!this.oldAnalysis)
            {
                //// Clear the color and depth buffers.
                ////gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

                //// Reset the modelview matrix.
                ////gl.LoadIdentity();

                // Move the geometry into a fairly central position.
                gl.Translate(0.0f, 0.0f, -20.0f);

                gl.PointSize(1.0f);

                gl.Begin(OpenGL.GL_TRIANGLES);

                int id = this.displayInterp < this.Slider ? this.displayInterp : this.Slider;

                for (int i = 0; i < this.vertices[id].Count(); i++)
                {
                    Color asd = Interpolation.GetHeatMapColor(this.vertices[id][i].Z, -1, +1);
                    gl.Color(asd.R / (float)255, asd.G / (float)255, asd.B / (float)255);
                    gl.Vertex(this.vertices[id][i].X / 100, this.vertices[id][i].Y / 100, 0.0f);
                }
     
                gl.End();

                // Flush OpenGL.
                ////gl.Flush();

                if (!this.savedVertices[id])
                {
                    this.TakeScreenshot(gl, id);
                    this.savedVertices[id] = true;
                }

                this.displayInterp++;
            }
            else
            {                
                if (!this.changeImage && this.textureImages[this.Slider] != null)
                {
                    this.SelectTexture(gl, this.Slider);
                    this.changeImage = true;
                }

                ////gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
                ////gl.LoadIdentity();

                gl.Translate(-0.12f, -0.052f, -3.5f);
                
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, this.textures[0]);

                gl.Begin(OpenGL.GL_QUADS);

                // Front Face
                gl.TexCoord(0.0f, 0.0f);
                gl.Vertex(-1.0f, -1.0f, 1.0f); // Bottom Left Of The Texture and Quad
                gl.TexCoord(1.0f, 0.0f);
                gl.Vertex(1.0f, -1.0f, 1.0f);  // Bottom Right Of The Texture and Quad
                gl.TexCoord(1.0f, 1.0f);
                gl.Vertex(1.0f, 1.0f, 1.0f);   // Top Right Of The Texture and Quad
                gl.TexCoord(0.0f, 1.0f);
                gl.Vertex(-1.0f, 1.0f, 1.0f);  // Top Left Of The Texture and Quad

                gl.End();

                ////gl.Flush();
            }

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
            catch (Exception)
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

        /// <summary>
        /// Save screenshot
        /// </summary>
        /// <param name="gl">Opengl object</param>
        /// <param name="index">Index of the image</param>
        private void TakeScreenshot(OpenGL gl, int index)
        {
            int w = 512; // gl.RenderContextProvider.Width;
            int h = 512; // gl.RenderContextProvider.Height;
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(w, h);
            System.Drawing.Imaging.BitmapData data =
                bmp.LockBits(new System.Drawing.Rectangle(0, 0, 512, 512), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            
            gl.ReadPixels(0, 0, w, h, OpenGL.GL_BGR, OpenGL.GL_UNSIGNED_BYTE, data.Scan0);

            bmp.UnlockBits(data);

            bmp.Save(this.analysisFolderPath + "\\Img" + index + ".bmp");
        }

        /// <summary>
        /// Load analysis screenshot
        /// </summary>
        private void LoadScreenshot()
        {
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(this.analysisFolderPath);

            var files = System.IO.Directory.GetFiles(this.analysisFolderPath, "*", System.IO.SearchOption.TopDirectoryOnly);

            int count = 0;
            foreach (string file in files)
            {
                Regex asd = new Regex("Img[0-9]+");

                Match dsa = asd.Match(file);

                if (dsa.Captures.Count == 0)
                {
                    continue;
                }


                string[] ret = dsa.Captures[0].Value.Split(new[] { "Img" }, StringSplitOptions.None);

                int index = Convert.ToInt32(ret[1]);

                this.textureImages[index] = new System.Drawing.Bitmap(this.analysisFolderPath + "\\Img" + index + ".bmp");

                count++;
            }

            this.MaxSlider = count - 1;
            this.Slider = this.MaxSlider;
        }

        /// <summary>
        /// Select the textude to be render
        /// </summary>
        /// <param name="gl">Opengl object</param>
        /// <param name="index">Index of the texture</param>
        private void SelectTexture(OpenGL gl, int index)
        {          
            // A bit of extra initialisation here, we have to enable textures.
            gl.Enable(OpenGL.GL_TEXTURE_2D);

            // Get one texture id, and stick it into the textures array.
            gl.GenTextures(1, this.textures);

            // Bind the texture.
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, this.textures[0]);

            System.Drawing.Imaging.BitmapData data = this.textureImages[index].LockBits(new System.Drawing.Rectangle(0, 0, 512, 512), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            // Tell OpenGL where the texture data is.
            gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, 3, 512, 512, 0, OpenGL.GL_BGR, OpenGL.GL_UNSIGNED_BYTE, data.Scan0);

            this.textureImages[index].UnlockBits(data);

            // Specify linear filtering.
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);            
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

            this.sensorGroupModel.Children.Clear();
            this.sensorGroupModel.Children.Add(this.stlModel);

            // TODO
            foreach (var model3d in this.sensorModelArray[newValue])
            {
                this.sensorGroupModel.Children.Add(model3d);
            }

            this.GroupModel = this.sensorGroupModel;
            this.device3D.Content = this.groupModel;

            this.changeImage = false;
        }

        /// <summary>
        /// Create analysis directory
        /// </summary>
        /// <param name="path">Directory path</param>
        /// <returns>If the folder already exists</returns>
        private bool CreateAnalysisFolder(string path)
        {
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);

                return true;
            }

            return false;
        }
    }
}
