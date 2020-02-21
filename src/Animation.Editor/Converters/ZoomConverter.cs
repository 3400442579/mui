using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;

namespace Animation.Editor.Converters
{
   public class ZoomConverter : IValueConverter
    {
        public static ZoomConverter Default { get; } = new ZoomConverter();

        static ZoomConverter()
        {
            Default = new ZoomConverter();
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double d = (double)value;

            return System.Convert.ToInt32(d * 100);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double d = (double)value;
            return d / 100;
        }

    }
}
