using System;
using System.Globalization;
using System.Windows.Data;

namespace DH.MUI.Converters
{
    public sealed class RotateTransformCentreConverter : IValueConverter
    {
        public static RotateTransformCentreConverter Default { get; } = new RotateTransformCentreConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //value == actual width
            return (double) value/2;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}