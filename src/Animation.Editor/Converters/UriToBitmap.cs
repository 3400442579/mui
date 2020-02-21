using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Animation.Editor.Converters
{
    public class UriToBitmap : IValueConverter
    {
        public static  UriToBitmap Default { get; } = new UriToBitmap();

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var stringValue = value as string;
            var size = parameter as string;

            if (string.IsNullOrEmpty(stringValue))
                return null;

            if (!File.Exists(stringValue))
                return null;

            if (!string.IsNullOrEmpty(size))
                return SourceFrom(stringValue, System.Convert.ToInt32(size));

            return SourceFrom(stringValue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Binding.DoNothing;
        }

        public static BitmapSource SourceFrom(string fileSource, int? size = null)
        {
            using var stream = new FileStream(fileSource, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;

            bitmapImage.StreamSource = stream;
            if (size.HasValue)
            {
                if (bitmapImage.DecodePixelHeight > bitmapImage.DecodePixelWidth)
                    bitmapImage.DecodePixelHeight = size.Value;
                else
                    bitmapImage.DecodePixelWidth = size.Value;
            }
            bitmapImage.EndInit();
            bitmapImage.Freeze(); //Just in case you want to load the image in another thread
            return bitmapImage;
        }

    }
}
