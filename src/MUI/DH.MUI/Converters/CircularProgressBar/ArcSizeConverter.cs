using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DH.MUI.Converters
{
    public sealed class ArcSizeConverter : IValueConverter
    {
        public static ArcSizeConverter Default { get; } = new ArcSizeConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double && ((double)value > 0.0))
            {
                return new Size((double)value / 2, (double)value / 2);
            }

            return new Point();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}