// <copyright file="ResultViewModel.cs" company="GM">
//     gm.com. All rights reserved.
// </copyright>

namespace SensorsViewer.Result
{
    using HelixToolkit.Wpf;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Class for result view model
    /// </summary>
    public class ResultViewModel
    {
        /// <summary>
        /// 3D device
        /// </summary>
        private ModelVisual3D device3D;

        /// <summary>
        /// Group Model
        /// </summary>
        Model3DGroup groupModel = new Model3DGroup();

        /// <summary>
        /// Stl model mesh
        /// </summary>
        MeshGeometry3D modelMesh = new MeshGeometry3D();

        /// <summary>
        /// Stl file path
        /// </summary>
        private string stlFilePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultViewModel"/> class
        /// </summary>
        public ResultViewModel(string stlFile)
        {
            this.stlFilePath = stlFile;
            this.viewPort3d = new HelixViewport3D();
            this.device3D = new ModelVisual3D();
        }

        public HelixViewport3D viewPort3d { get; set; }

        /// <summary>
        /// 
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

        public void LoadStlModel()
        {
            Model3D stlModel = Display3d(this.stlFilePath);

            groupModel.Children.Add(stlModel);
            device3D.Content = groupModel;           
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
                //Adding a gesture here
                viewPort3d.RotateGesture = new MouseGesture(MouseAction.LeftClick);

                //Import 3D model file
                ModelImporter import = new ModelImporter();

                Material material = new DiffuseMaterial(new SolidColorBrush(Colors.Black));
                import.DefaultMaterial = material;

                //Load the 3D model file
                device = import.Load(model);

                Action<GeometryModel3D, Transform3D> nameAction = ((geometryModel, transform) =>
                {
                    modelMesh = (MeshGeometry3D)geometryModel.Geometry;
                });

                device.Traverse(nameAction);
            }
            catch (Exception e)
            {
                // Handle exception in case can not find the 3D model file
                throw new Exception("not find 3d model file");
            }

            return device;
        }
    }
}
