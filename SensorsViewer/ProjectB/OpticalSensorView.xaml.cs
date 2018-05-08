// <copyright file="OpticalSensorView.xaml.cs" company="GM">
//     gm.com. All rights reserved.
// </copyright>

namespace SensorsViewer.ProjectB
{
    using System;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using LiveCharts;

    /// <summary>
    /// Interaction logic for OpticalSensorView.xaml
    /// </summary>
    public partial class OpticalSensorView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpticalSensorView"/> class
        /// </summary>
        public OpticalSensorView()
        {
            this.InitializeComponent();

            this.OpticalSensorViewModel = new OpticalSensorViewModel();

            this.DataContext = this.OpticalSensorViewModel;            
        }

        /// <summary>
        /// Gets or sets optical sensor view model
        /// </summary>
        public OpticalSensorViewModel OpticalSensorViewModel { get; set; }

        /// <summary>
        /// Set the visibility of the sensor in chart
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">event e</param>
        private void ListBox_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var item = ItemsControl.ContainerFromElement(ListBox, (DependencyObject)e.OriginalSource) as ListBoxItem;
            if (item == null)
            {
                return;
            }

            var series = (LiveCharts.Wpf.LineSeries)item.Content;

            series.Visibility = series.Visibility == Visibility.Visible
                ? Visibility.Hidden
                : Visibility.Visible;
        }

        public void TakeTheChart()
        {

            //var newChart = new LiveCharts.Wpf.CartesianChart
            //{
            //    Width = this.ChartElement.Width,
            //    Height = this.ChartElement.Height,

            //    AxisX = this.ChartElement.AxisX,
            //    AxisY = this.ChartElement.AxisY,

            //    Background = this.ChartElement.Background,
            //    Style = this.ChartElement.Style,

            //    Series = this.ChartElement.Series,
            //    SeriesColors = this.ChartElement.SeriesColors,

            //    VisualElements = this.ChartElement.VisualElements,

            //    Template = this.ChartElement.Template,

            //};

            //var asd = new LiveCharts.Wpf.CartesianChart();

            LiveCharts.Wpf.ColorsCollection newCol = this.ChartElement.SeriesColors;

            SeriesCollection newSerCol = this.ChartElement.Series;


            Canvas canvas = (Canvas)this.ChartElement.Parent;

            canvas.Children.Remove(this.ChartElement);

            this.ChartElement.Series = newSerCol;

            var viewbox = new Viewbox();
            viewbox.Child = this.ChartElement;
            viewbox.Measure(this.ChartElement.RenderSize);
            viewbox.Arrange(new Rect(new Point(0, 0), this.ChartElement.RenderSize));
            this.ChartElement.Update(true, true); //force chart redraw
            viewbox.UpdateLayout();

            SaveToPng(this.ChartElement, @"C:\Users\heitor.araujo\source\repos\SensorViewer\SensorsViewer\Resources\Chart.png");

            viewbox.Child = null;

            canvas.Children.Add(this.ChartElement);

            //png file was created at the root directory.
        }

        private void SaveToPng(FrameworkElement visual, string fileName)
        {
            var encoder = new PngBitmapEncoder();
            EncodeVisual(visual, fileName, encoder);
        }

        private static void EncodeVisual(FrameworkElement visual, string fileName, BitmapEncoder encoder)
        {
            var bitmap = new RenderTargetBitmap((int)visual.ActualWidth, (int)visual.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(visual);
            var frame = BitmapFrame.Create(bitmap);
            encoder.Frames.Add(frame);
            using (var stream = File.Create(fileName)) encoder.Save(stream);
        }
    }
}
