// <copyright file="ChartView.xaml.cs" company="GM">
//     gm.com. All rights reserved.
// </copyright>

namespace SensorsViewer.Chart
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
    /// Interaction logic for ChartView.xaml
    /// </summary>
    public partial class ChartView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartView"/> class
        /// </summary>
        public ChartView()
        {
            this.InitializeComponent();

            this.ChartViewModel = new ChartSensorViewModel();

            this.DataContext = this.ChartViewModel;            
        }

        /// <summary>
        /// Gets or sets chart sensor view model
        /// </summary>
        public ChartSensorViewModel ChartViewModel { get; set; }

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

        public void TakeTheChart(string path)
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

            //LiveCharts.Wpf.ColorsCollection newCol = this.ChartElement.SeriesColors;

            //SeriesCollection newSerCol = this.ChartElement.Series;

            //Canvas canvas = (Canvas)this.ChartElement.Parent;

            //canvas.Children.Remove(this.ChartElement);

            //this.ChartElement.Series = newSerCol;

            //var viewbox = new Viewbox();
            //viewbox.Child = this.ChartElement;
            //viewbox.Measure(this.ChartElement.RenderSize);
            //viewbox.Arrange(new Rect(new Point(0, 0), this.ChartElement.RenderSize));
            //this.ChartElement.Update(true, true); //force chart redraw
            //viewbox.UpdateLayout();

            var myChart = new LiveCharts.Wpf.CartesianChart
            {
                DisableAnimations = true,
                Width = 570,
                Height = 535,
                Series = this.ChartElement.Series,
                AxisX = this.ChartElement.AxisX,
            };

            DateModel dm = (DateModel)this.ChartElement.Series[0].Values[0];
            DateModel dm2 = (DateModel)this.ChartElement.Series[0].Values[this.ChartElement.Series[0].Values.Count-1];

            myChart.AxisX[0].LabelFormatter = XFormatterFunc;

            myChart.AxisX[0].MinValue = dm.DateTime.Ticks;
            myChart.AxisX[0].MaxValue = dm2.DateTime.Ticks;

            var viewbox = new Viewbox();
            viewbox.Child = myChart;
            viewbox.Measure(myChart.RenderSize);
            viewbox.Arrange(new Rect(new Point(0, 0), myChart.RenderSize));
            myChart.Update(true, true); //force chart redraw
            viewbox.UpdateLayout();

            SaveToPng(myChart, path + "\\Chart.png");

            //this.ChartElement.Update(true, true);

            //viewbox.Child = null;

            //canvas.Children.Add(this.ChartElement);

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

        /// <summary>
        /// X formater function
        /// </summary>
        /// <param name="val">Value to be formated</param>
        /// <returns>Formated value</returns>
        private string XFormatterFunc(double val)
        {
            string asd = new DateTime((long)val).ToString("mm:ss.fff");

            return asd;
        }

    }
}
