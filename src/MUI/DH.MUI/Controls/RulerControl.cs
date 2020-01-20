using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DH.MUI.Controls
{
    #region Enumerations
    public enum Orientation { Horizontal, Vertical }
    #endregion

    [TemplatePart(Name = "trackLine", Type = typeof(Line))]
    [TemplatePart(Name = "verticalTrackLine", Type = typeof(Line))]
    [TemplatePart(Name = "horizontalTrackLine", Type = typeof(Line))]
    public class RulerControl : Control
    {
        #region MouseMoveRoutedEvent

        public static readonly RoutedEvent MouseMoveEvent = EventManager.RegisterRoutedEvent("MouseMove", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RulerControl));

        public event RoutedEventHandler MouseMove
        {
            add { AddHandler(MouseMoveEvent, value); }
            remove { RemoveHandler(MouseMoveEvent, value); }
        }
        #endregion

        #region DepencyProperty OrientationProperty
        /// <summary>Identifies the <see cref="Orientation"/> dependency property.</summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(RulerControl),
            new UIPropertyMetadata(Orientation.Horizontal));

        public Orientation Orientation
        {
            get { return (Orientation)base.GetValue(OrientationProperty); }
            set { this.SetValue(OrientationProperty, value); }
        }
        #endregion
        #region DepencyProperty MajorIntervalProperty
        /// <summary>Identifies the <see cref="MajorInterval"/> dependency property.</summary>
        public static readonly DependencyProperty MajorIntervalProperty =  DependencyProperty.Register("MajorInterval", typeof(int), typeof(RulerControl),  new UIPropertyMetadata(100));

        public int MajorInterval
        {
            get { return (int)base.GetValue(MajorIntervalProperty); }
            set { this.SetValue(MajorIntervalProperty, value); }
        }
        #endregion
        #region DepencyProperty MarkLengthProperty
        /// <summary>Identifies the <see cref="MarkLength"/> dependency property.</summary>
        public static readonly DependencyProperty MarkLengthProperty =
            DependencyProperty.Register("MarkLength", typeof(int), typeof(RulerControl), new UIPropertyMetadata(20));

        public int MarkLength
        {
            get { return (int)base.GetValue(MarkLengthProperty); }
            set { this.SetValue(MarkLengthProperty, value); }
        }
        #endregion
        #region DepencyProperty MiddleMarkLengthProperty
        /// <summary>Identifies the <see cref="MiddleMarkLength"/> dependency property.</summary>
        public static readonly DependencyProperty MiddleMarkLengthProperty =
            DependencyProperty.Register("MiddleMarkLength", typeof(int), typeof(RulerControl),
            new UIPropertyMetadata(10));

        public int MiddleMarkLength
        {
            get { return (int)base.GetValue(MiddleMarkLengthProperty); }
            set { this.SetValue(MiddleMarkLengthProperty, value); }
        }
        #endregion
        #region DepencyProperty LittleMarkLengthProperty
        /// <summary>Identifies the <see cref="LittleMarkLength"/> dependency property.</summary>
        public static readonly DependencyProperty LittleMarkLengthProperty =
            DependencyProperty.Register("LittleMarkLength", typeof(int), typeof(RulerControl),
            new UIPropertyMetadata(5));

        public int LittleMarkLength
        {
            get { return (int)base.GetValue(LittleMarkLengthProperty); }
            set { this.SetValue(LittleMarkLengthProperty, value); }
        }
        #endregion
        #region DepencyProperty StartValueProperty
        /// <summary>Identifies the <see cref="StartValue"/> dependency property.</summary>
        public static readonly DependencyProperty StartValueProperty =
            DependencyProperty.Register("StartValue", typeof(double), typeof(RulerControl),
            new UIPropertyMetadata(0.0));

        public double StartValue
        {
            get { return (double)base.GetValue(StartValueProperty); }
            set { this.SetValue(StartValueProperty, value); }
        }
        #endregion
        //Point mousePosition;
        //Pen mouseTrackPen = new Pen(new SolidColorBrush(Colors.Black), 1);
        Line mouseVerticalTrackLine;
        Line mouseHorizontalTrackLine;

        static RulerControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RulerControl), new FrameworkPropertyMetadata(typeof(RulerControl)));
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
            double psuedoStartValue = StartValue;
            #region Horizontal Ruler
            if (this.Orientation == Orientation.Horizontal)
            {
                for (int i = 0; i < this.ActualWidth / MajorInterval; i++)
                {
                    var ft = new FormattedText((psuedoStartValue * MajorInterval).ToString(), System.Globalization.CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Tahoma"), 10, Brushes.Black);
                    drawingContext.DrawText(ft, new Point(i * MajorInterval, 0));
                    drawingContext.DrawLine(new Pen(new SolidColorBrush(Colors.Red), 1), new Point(i * MajorInterval, MarkLength), new Point(i * MajorInterval, 0));
                    drawingContext.DrawLine(new Pen(new SolidColorBrush(Colors.Green), 1),
                        new Point(i * MajorInterval + (MajorInterval / 2), MiddleMarkLength),
                        new Point(i * MajorInterval + (MajorInterval / 2), 0));
                    for (int j = 1; j < 10; j++)
                    {
                        if (j == 5)
                        {
                            continue;
                        }
                        drawingContext.DrawLine(new Pen(new SolidColorBrush(Colors.Blue), 1),
                        new Point(i * MajorInterval + (((MajorInterval * j) / 10)), LittleMarkLength),
                        new Point(i * MajorInterval + (((MajorInterval * j) / 10)), 0));
                    }
                    psuedoStartValue++;
                }
            }
            #endregion
            #region Vertical Ruler
            else
            {
                psuedoStartValue = StartValue;
                for (int i = 0; i < this.ActualHeight / MajorInterval; i++)
                {
                    var ft = new FormattedText((psuedoStartValue * MajorInterval).ToString(), System.Globalization.CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Tahoma"), 10, Brushes.Black);
                    drawingContext.DrawText(ft, new Point(0, i * MajorInterval));
                    drawingContext.DrawLine(new Pen(new SolidColorBrush(Colors.Red), 1), new Point(MarkLength, i * MajorInterval), new Point(0, i * MajorInterval));
                    drawingContext.DrawLine(new Pen(new SolidColorBrush(Colors.Red), 1), new Point(MarkLength, i * MajorInterval), new Point(0, i * MajorInterval));
                    drawingContext.DrawLine(new Pen(new SolidColorBrush(Colors.Green), 1),
                        new Point(MiddleMarkLength, i * MajorInterval + (MajorInterval / 2)),
                        new Point(0, i * MajorInterval + (MajorInterval / 2)));
                    for (int j = 1; j < 10; j++)
                    {
                        if (j == 5)
                        {
                            continue;
                        }
                        drawingContext.DrawLine(new Pen(new SolidColorBrush(Colors.Blue), 1),
                        new Point(LittleMarkLength, i * MajorInterval + (((MajorInterval * j) / 10))),
                        new Point(0, i * MajorInterval + (((MajorInterval * j) / 10))));
                    }
                    psuedoStartValue++;
                }
            }
            #endregion

        }
        //protected override void OnMouseMove(MouseEventArgs e)
        //{

        //}
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
            mouseVerticalTrackLine = GetTemplateChild("verticalTrackLine") as Line;
            mouseHorizontalTrackLine = GetTemplateChild("horizontalTrackLine") as Line;
            mouseVerticalTrackLine.SetCurrentValue(VisibilityProperty, Visibility.Visible);
            mouseHorizontalTrackLine.SetCurrentValue(VisibilityProperty, Visibility.Visible);

        }
    }
}
