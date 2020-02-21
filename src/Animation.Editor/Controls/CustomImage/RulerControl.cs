using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Animation.Editor.Controls
{
    #region Enumerations
    public enum Orientation { Horizontal, Vertical }
    #endregion

    [TemplatePart(Name = "trackLine", Type = typeof(Line))]
    [TemplatePart(Name = "verticalTrackLine", Type = typeof(Line))]
    [TemplatePart(Name = "horizontalTrackLine", Type = typeof(Line))]
    public class RulerControl : Control
    {
        //#region MouseMoveRoutedEvent
        //public static readonly RoutedEvent MouseMoveEvent = EventManager.RegisterRoutedEvent("MouseMove", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RulerControl));

        //public event RoutedEventHandler MouseMove
        //{
        //    add { AddHandler(MouseMoveEvent, value); }
        //    remove { RemoveHandler(MouseMoveEvent, value); }
        //}
        //#endregion

        //readonly DrawingGroup backingStore = new DrawingGroup();


        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(RulerControl),
            new UIPropertyMetadata(Orientation.Horizontal));

        public Orientation Orientation
        {
            get { return (Orientation)base.GetValue(OrientationProperty); }
            set { this.SetValue(OrientationProperty, value); }
        }
        
        public static readonly DependencyProperty ZoomProperty = 
            DependencyProperty.Register("Zoom", typeof(double),typeof(RulerControl), new UIPropertyMetadata(1d));
        public double Zoom
        {
            get { return (double)base.GetValue(ZoomProperty); }
            set { this.SetValue(ZoomProperty, value); }
        }

        public static readonly DependencyProperty ImageWidthProperty =
            DependencyProperty.Register("ImageWidth", typeof(double), typeof(RulerControl),
            new UIPropertyMetadata(0d));

        public double ImageWidth
        {
            get { return (double)GetValue(ImageWidthProperty); }
            set { this.SetValue(ImageWidthProperty, value); }
        }

       


        public static readonly DependencyProperty ImageHeightProperty =
            DependencyProperty.Register("ImageHeight", typeof(double), typeof(RulerControl),
            new UIPropertyMetadata(0d));

        public double ImageHeight
        {
            get { return (double)GetValue(ImageHeightProperty); }
            set { this.SetValue(ImageHeightProperty, value); }
        }
        //private static void UpdateView(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    var s = e.NewValue;
        //    //CustomImage ctrl = (CustomImage)d;
        //}

        //Point mousePosition;
        //Pen mouseTrackPen = new Pen(new SolidColorBrush(Colors.Black), 1);
        Line mouseVerticalTrackLine;
        Line mouseHorizontalTrackLine;

        static RulerControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RulerControl), new FrameworkPropertyMetadata(typeof(RulerControl)));
        }

        private static int lh = 14;//行高
        private static int maxlh = 5;//最长线起点
        private static int mlh = 8;//中间线起点
        private static int minlh = 10;//最短线起点
        protected override void OnRender(DrawingContext drawingContext)
        {
            //base.OnRender(drawingContext);

            RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);

            Typeface typeface = new Typeface("Verdana");
            Brush foreBrush = this.Foreground;
            foreBrush.Opacity = 20;
            Pen line_Pen = new Pen(foreBrush, 1);
            double pixelsPerDip = VisualTreeHelper.GetDpi(this).PixelsPerDip;

            int h = Convert.ToInt32(this.ActualHeight);
            int w = Convert.ToInt32(this.ActualWidth);

            int spx = 10;
            int sh = Convert.ToInt32((h - ImageHeight * Zoom) / 2);
            int sw = Convert.ToInt32((w - ImageWidth * Zoom) / 2);

            StreamGeometry strgeo = new StreamGeometry();
            using StreamGeometryContext ctx = strgeo.Open();
            if (Orientation == Orientation.Horizontal)
            {
                int index = 0;
                int x = sw;
                do  {
                    if (index % 10 == 0)
                    {
                        if (index != 0)
                        {
                            FormattedText font = new FormattedText((index * 10).ToString(), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, 9, foreBrush, pixelsPerDip);
                            drawingContext.DrawText(font, new Point(x + 1, 0));
                        }
                        ctx.BeginFigure(new Point(x, maxlh), false, false);
                    }
                    else
                        ctx.BeginFigure(new Point(x, minlh), false, false);
                    ctx.LineTo(new Point(x, lh), true, false);
                    index += 1;
                    x -= spx;
                } while (x > 0);

                index = 0;
                x = sw;
                do
                {
                    if (index % 10 == 0)
                    {
                        FormattedText font = new FormattedText((index * 10).ToString(), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, 9, foreBrush, pixelsPerDip);
                        drawingContext.DrawText(font, new Point(x + 2, 0));
                        ctx.BeginFigure(new Point(x, maxlh), false, false);
                    }
                    else
                        ctx.BeginFigure(new Point(x, minlh), false, false);
                    ctx.LineTo(new Point(x, lh), true, false);
                    index += 1;
                    x += spx;
                } while (x < w);
              
            }
            else
            {
                int index = 0;
                int y = sh;
                do
                {
                    if (index % 10 == 0)
                    {
                        char[] chars = (index * 10).ToString().ToCharArray();
                        for (int i = 0; i < chars.Length; i++)
                        {
                            FormattedText font = new FormattedText(chars[i].ToString(), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, 10, foreBrush, pixelsPerDip);
                            drawingContext.DrawText(font, new Point(3, y + (font.Height - 3) * i));
                        }
                        ctx.BeginFigure(new Point(maxlh, y), false, false);
                    }
                    else
                        ctx.BeginFigure(new Point(minlh, y), false, false);
                    ctx.LineTo(new Point(lh, y), true, false);
                    index += 1;
                    y -= spx;
                } while (y > 0);

                index = 0;
                y = sh;
                do
                {
                    if (index % 10 == 0)
                    {
                        char[] chars = (index * 10).ToString().ToCharArray();
                        for (int i = 0; i < chars.Length; i++)
                        {
                            FormattedText font = new FormattedText(chars[i].ToString(), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, 10, foreBrush, pixelsPerDip);
                            drawingContext.DrawText(font, new Point(3, y  + (font.Height-3) * i));
                        }
                        ctx.BeginFigure(new Point(maxlh, y), false, false);
                    }
                    else
                        ctx.BeginFigure(new Point(minlh, y), false, false);
                    ctx.LineTo(new Point(lh, y), true, false);

                    index += 1;
                    y += spx;
                } while (y < h);
            }

            drawingContext.DrawGeometry(foreBrush, line_Pen, strgeo);

        }

       
        public void RaiseHorizontalRulerMoveEvent(MouseEventArgs e)
        {
            Point mousePoint = e.GetPosition(this);
            mouseHorizontalTrackLine.SetCurrentValue(Line.X1Property, mousePoint.X);
            mouseHorizontalTrackLine.SetCurrentValue(Line.X2Property, mousePoint.X);
        }
        public void RaiseVerticalRulerMoveEvent(MouseEventArgs e)
        {
            Point mousePoint = e.GetPosition(this);
            mouseVerticalTrackLine.SetCurrentValue(Line.Y1Property, mousePoint.Y);
            mouseVerticalTrackLine.SetCurrentValue(Line.Y2Property, mousePoint.Y);
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            //mouseVerticalTrackLine = GetTemplateChild("verticalTrackLine") as Line;
            //mouseHorizontalTrackLine = GetTemplateChild("horizontalTrackLine") as Line;
            //mouseVerticalTrackLine.SetCurrentValue(VisibilityProperty, Visibility.Visible);
            //mouseHorizontalTrackLine.SetCurrentValue(VisibilityProperty, Visibility.Visible);
        }
    }
}
