using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;

namespace Animation.Editor.Core
{
    public class Dpi
    {
        public double X { get; set; }
        public double Y { get; set; }
        public Dpi(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static (double X, double Y) GetDpiFromVisual(Visual visual)
        {
            var source = PresentationSource.FromVisual(visual);

            var dpiX = 96.0;
            var dpiY = 96.0;

            if (source?.CompositionTarget != null)
            {
                dpiX = 96.0 * source.CompositionTarget.TransformToDevice.M11;
                dpiY = 96.0 * source.CompositionTarget.TransformToDevice.M22;
            }

            return (dpiX, dpiY);
        }
        public static (int X, int Y) GetDpiBySystemParameters(bool retunY = true)
        {
            const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Static;

            var dpiXProperty = typeof(SystemParameters).GetProperty("DpiX", bindingFlags);

            var dpiX = 96;

            if (dpiXProperty != null)
            {
                dpiX = (int)dpiXProperty.GetValue(null, null);
            }

            var dpiY = 0;
            if (retunY)
            {
                var dpiYProperty = typeof(SystemParameters).GetProperty("DpiY", bindingFlags);

                if (dpiYProperty != null)
                {
                    dpiY = (int)dpiYProperty.GetValue(null, null);
                }
                else
                    dpiY = 96;
            }

            return (dpiX, dpiY);
        }



        private const int LOGPIXELSX = 88;
        private const int LOGPIXELSY = 90;

        [DllImport("gdi32.dll")]
        private static extern int GetDeviceCaps(IntPtr hdc, int index);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDc);

        public static (double X, double Y) GetDpiByWin32()
        {
            var hDc = GetDC(IntPtr.Zero);

            var dpiX = GetDeviceCaps(hDc, LOGPIXELSX);
            var dpiY = GetDeviceCaps(hDc, LOGPIXELSY);

            ReleaseDC(IntPtr.Zero, hDc);
            return (dpiX, dpiY);
        }
    }
}
