// <copyright file="OpacityConverter.cs" company="GM">
//     gm.com. All rights reserved.
// </copyright>

namespace SensorsViewer.ProjectB
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// Opacity converter
    /// </summary>
    public class OpacityConverter : IValueConverter
    {
        /// <summary>
        /// Convert value
        /// </summary>
        /// <param name="value">Parameter value</param>
        /// <param name="targetType">Target type</param>
        /// <param name="parameter">Object Parameter</param>
        /// <param name="culture">Culture Info</param>
        /// <returns>Converted value</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Visibility)value == Visibility.Visible ? 1d : .3d;
        }

        /// <summary>
        /// Convert back
        /// </summary>
        /// <param name="value">Parameter value</param>
        /// <param name="targetType">Target type</param>
        /// <param name="parameter">Object Parameter</param>
        /// <param name="culture">Culture Info</param>
        /// <returns>Converted back value</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
