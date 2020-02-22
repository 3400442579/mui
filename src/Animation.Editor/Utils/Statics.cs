using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Animation.Editor.Utils
{
    public static class Statics
    {
        internal static Rect Offset(this Rect rect, double offset)
        {
            return new Rect(Math.Round(rect.Left + offset, MidpointRounding.AwayFromZero), Math.Round(rect.Top + offset, MidpointRounding.AwayFromZero),
                Math.Round(rect.Width - (offset * 2d), MidpointRounding.AwayFromZero), Math.Round(rect.Height - (offset * 2d), MidpointRounding.AwayFromZero));

            //return new Rect(rect.Left + offset, rect.Top + offset, rect.Width - (offset * 2d), rect.Height - (offset * 2d));
        }

        internal static Rect Scale(this Rect rect, double scale)
        {
            return new Rect(Math.Round(rect.Left * scale, MidpointRounding.AwayFromZero), Math.Round(rect.Top * scale, MidpointRounding.AwayFromZero),
                Math.Round(rect.Width * scale, MidpointRounding.AwayFromZero), Math.Round(rect.Height * scale, MidpointRounding.AwayFromZero));
        }
        internal static Rect Limit(this Rect rect, double width, double height)
        {
            var newX = rect.X < 0 ? 0 : rect.X;
            var newY = rect.Y < 0 ? 0 : rect.Y;

            var newWidth = newX + rect.Width > width ? width - newX : rect.Width;
            var newHeight = newY + rect.Height > height ? height - newY : rect.Height;

            return new Rect(newX, newY, newWidth, newHeight);
        }

        internal static Size Scale(this Size size, double scale)
        {
            return new Size(Math.Round(size.Width * scale, MidpointRounding.AwayFromZero), Math.Round(size.Height * scale, MidpointRounding.AwayFromZero));
        }
        internal static Point Scale(this Point point, double scale)
        {
            return new Point(Math.Round(point.X * scale, MidpointRounding.AwayFromZero), Math.Round(point.Y * scale, MidpointRounding.AwayFromZero));
        }
        public static double RoundUpValue(double value, int decimalpoint = 0)
        {
            var result = Math.Round(value, decimalpoint);

            if (result < value)
                result += Math.Pow(10, -decimalpoint);

            return result;
        }
        /// <summary>
        /// Gets the scale of the current window.
        /// </summary>
        /// <param name="window">The Window.</param>
        /// <returns>The scale of the given Window.</returns>
        public static double Scale(this Visual window)
        {
            var source = PresentationSource.FromVisual(window);

            if (source?.CompositionTarget != null)
                return source.CompositionTarget.TransformToDevice.M11;

            return 1d;
        }

        public static bool Contains(this Int32Rect first, Int32Rect second)
        {
            if (first.IsEmpty || second.IsEmpty || (first.X > second.X || first.Y > second.Y) || first.X + first.Width < second.X + second.Width)
                return false;

            return first.Y + first.Height >= second.Y + second.Height;
        }

    }
}
