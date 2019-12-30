using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DH.MUI.Converters
{
    /// <summary>
    /// Converts a null value to Visibility.Visible and any other value to Visibility.Collapsed
    /// </summary>
    [ValueConversion(typeof(string), typeof(Visibility))]
    public sealed class IsNullConverter : IValueConverter
    {
        /// <summary> Gets the default instance </summary>
        public static IsNullConverter Default { get; } = new IsNullConverter();

        public static IsNullConverter Inverse { get; } = new IsNullConverter { IsInverse = true };


        public bool IsInverse { get; set; }

        public IsNullConverter()
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
            var flag = value == null;
            if (value is string)
            {
                flag = string.IsNullOrEmpty((string)value);
            }
            if (IsInverse)
            {
                return (flag ? Visibility.Collapsed : Visibility.Visible);
            }
            else
            {
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
