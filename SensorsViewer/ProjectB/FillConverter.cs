// <copyright file="FillConverter.cs" company="GM">
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
    using System.Windows.Media;

    /// <summary>
    /// Class to fill converter
    /// </summary>
    public class FillConverter : IValueConverter
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
            Brush newValue = ((Brush)value).Clone();
            newValue.Opacity = 1d;

            return newValue;
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
