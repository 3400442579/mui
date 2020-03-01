﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Ant.Wpf.Converters
{
    /// <summary>
    /// Converts boolean to visibility values.
    /// </summary>
    [ValueConversion(typeof(bool?), typeof(Visibility))]
    public sealed class BoolToVisibilityConverter : IValueConverter
    {
        /// <summary> Gets the default instance </summary>
        public static BoolToVisibilityConverter Default { get; } = new BoolToVisibilityConverter();

        public static BoolToVisibilityConverter Inverse { get; } = new BoolToVisibilityConverter { IsInverse = true };

        public bool IsInverse { get; set; }

        public BoolToVisibilityConverter()
        {
            IsInverse = false;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool flag = false;
            if (value is bool) {
                flag = (bool)value;
            }
            else if (value is bool?) {
                bool? nullable = (bool?)value;
                flag = nullable ?? false;
            }

            //bool inverse = (parameter as string) == "inverse";

            if (IsInverse) {
                return (flag ? Visibility.Collapsed : Visibility.Visible);
            }
            else {
                return (flag ? Visibility.Visible : Visibility.Collapsed);
            }
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
