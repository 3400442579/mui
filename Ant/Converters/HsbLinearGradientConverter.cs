using Ant.Wpf.Common;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Ant.Wpf.Converters
{
    public sealed class HsbLinearGradientConverter : IValueConverter
    {
        public static HsbLinearGradientConverter Default { get; } = new HsbLinearGradientConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var v = (double)value;

            return new LinearGradientBrush(Colors.White, new Hsb(v, 1, 1).ToColor(), 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
