// <copyright file="ResultView.cs" company="GM">
//     gm.com. All rights reserved.
// </copyright>

namespace SensorsViewer.Result
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
    using System.Windows.Navigation;
    using System.Windows.Shapes;

    /// <summary>
    /// Interaction logic for ResultView.xaml
    /// </summary>
    public partial class ResultView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResultView"/> class
        /// </summary>
        public ResultView(string path)
        {
            InitializeComponent();

            this.ResultViewModel = new ResultViewModel(path);

            this.ResultViewModel.LoadStlModel();
            this.viewPort3d.ZoomExtents();
            this.viewPort3d.ZoomExtentsWhenLoaded = true;

            this.DataContext = this.ResultViewModel;
        }

        /// <summary>
        /// Gets or sets optical sensor view model
        /// </summary>
        public ResultViewModel ResultViewModel { get; set; }
    }
}
