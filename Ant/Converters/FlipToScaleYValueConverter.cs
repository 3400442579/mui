using Ant.Wpf.Controls;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Ant.Wpf.Converters
{
   
    [MarkupExtensionReturnType(typeof(FlipToScaleYValueConverter))]
    public sealed class FlipToScaleYValueConverter : IValueConverter
    {
        public static FlipToScaleYValueConverter Default { get; } = new FlipToScaleYValueConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is PackIconFlipOrientation flip)
            {
                var scaleY = flip == PackIconFlipOrientation.Vertical || flip == PackIconFlipOrientation.Both ? -1 : 1;
                return scaleY;
            }

            return 1;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}