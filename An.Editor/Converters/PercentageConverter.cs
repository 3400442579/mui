using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace An.Editor.Converters
{
    public class PercentageConverter : Avalonia.Data.Converters.IValueConverter
    {
        public static PercentageConverter Default { get; } = new PercentageConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int n)
            {
                return n / 100f;
            }

            return 0f;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is float n)
            {
                return  n * 100;
            }
            return 0;
        }
    }
}
