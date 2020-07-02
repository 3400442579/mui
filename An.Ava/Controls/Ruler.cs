using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;

namespace An.Ava.Controls
{
    #region Enumerations
    // public enum OrientationEnum { Horizontal, Vertical }

    //单位
    public enum UnitEnum { Pixel/*像素*/, Inch/*英寸*/, CM/*厘米*/ }

    #endregion
    public class Ruler : TemplatedControl
    {
        private const int startv = 20;

        private static readonly List<ScaleParm> PixelScaleParms = new List<ScaleParm>() {
            new ScaleParm{ Scale=2, Interval=5000, Pixel=100, Num=10  },
            new ScaleParm{ Scale=3, Interval=5000, Pixel=150, Num=10  },
            new ScaleParm{ Scale=4, Interval=2000, Pixel=80, Num=10  },
            new ScaleParm{ Scale=5, Interval=2000, Pixel=100, Num=10  },
            new ScaleParm{ Scale=6, Interval=1000, Pixel=60, Num=5  },
            new ScaleParm{ Scale=7, Interval=1000, Pixel=70, Num=5  },
            new ScaleParm{ Scale=8, Interval=1000, Pixel=80, Num=10  },
            new ScaleParm{ Scale=10, Interval=1000, Pixel=100, Num=10  },
            new ScaleParm{ Scale=13, Interval=500, Pixel=65, Num=5  },
            new ScaleParm{ Scale=17, Interval=500, Pixel=85, Num=5  },
            new ScaleParm{ Scale=20, Interval=500, Pixel=100, Num=10  },
            new ScaleParm{ Scale=25, Interval=500, Pixel=120, Num=10  },
            new ScaleParm{ Scale=35, Interval=200, Pixel=70, Num=5  },
            new ScaleParm{ Scale=50, Interval=200, Pixel=100, Num=10  },
            new ScaleParm{ Scale=65, Interval=100, Pixel=65, Num=5  },
            new ScaleParm{ Scale=80, Interval=100, Pixel=80, Num=5  },
            new ScaleParm{ Scale=100, Interval=100, Pixel=100, Num=10  },
            new ScaleParm{ Scale=150, Interval=50, Pixel=75, Num=5  },
            new ScaleParm{ Scale=200, Interval=50, Pixel=100, Num=10  },
            new ScaleParm{ Scale=300, Interval=20, Pixel=60, Num=5  },
            new ScaleParm{ Scale=400, Interval=20, Pixel=80, Num=10  },
            new ScaleParm{ Scale=500, Interval=20, Pixel=100, Num=10  },
            new ScaleParm{ Scale=600, Interval=10, Pixel=60, Num=5  },
            new ScaleParm{ Scale=800, Interval=10, Pixel=80, Num=5  },
            new ScaleParm{ Scale=1000, Interval=10, Pixel=100, Num=10  },
            new ScaleParm{ Scale=1200, Interval=5, Pixel=60, Num=5  },
            new ScaleParm{ Scale=1400, Interval=5, Pixel=70, Num=5  },
            new ScaleParm{ Scale=1600, Interval=5, Pixel=80, Num=5  },
            new ScaleParm{ Scale=2000, Interval=5, Pixel=100, Num=10  },
            new ScaleParm{ Scale=2400, Interval=5, Pixel=120, Num=10  },
            new ScaleParm{ Scale=2800, Interval=5, Pixel=140, Num=10  },
            new ScaleParm{ Scale=3200, Interval=2, Pixel=64, Num=2  },
            new ScaleParm{ Scale=4000, Interval=2, Pixel=80, Num=2  },
            new ScaleParm{ Scale=4800, Interval=2, Pixel=96, Num=2  },
            new ScaleParm{ Scale=5600, Interval=2, Pixel=112, Num=2  },
            new ScaleParm{ Scale=6400, Interval=2, Pixel=128, Num=2  },
        };

        public static List<int> Scales => PixelScaleParms.Select(o => o.Scale).ToList();

        //private readonly List<ScaleParm> InchScaleParms = new List<ScaleParm>() {
        //    new ScaleParm{ Scale=2, Interval=5000, Pixel=100, Num=10  },
        //    new ScaleParm{ Scale=3, Interval=5000, Pixel=150, Num=10  },
        //    new ScaleParm{ Scale=4, Interval=2000, Pixel=80, Num=10  },
        //    new ScaleParm{ Scale=5, Interval=2000, Pixel=100, Num=10  },
        //    new ScaleParm{ Scale=6, Interval=1000, Pixel=60, Num=5  },
        //    new ScaleParm{ Scale=7, Interval=1000, Pixel=70, Num=5  },
        //    new ScaleParm{ Scale=8, Interval=1000, Pixel=80, Num=10  },
        //    new ScaleParm{ Scale=10, Interval=1000, Pixel=100, Num=10  },
        //    new ScaleParm{ Scale=13, Interval=500, Pixel=65, Num=5  },
        //    new ScaleParm{ Scale=17, Interval=500, Pixel=85, Num=10  },
        //    new ScaleParm{ Scale=20, Interval=500, Pixel=100, Num=10  },
        //    new ScaleParm{ Scale=25, Interval=500, Pixel=120, Num=10  },
        //    new ScaleParm{ Scale=33, Interval=200, Pixel=66, Num=5  },
        //    new ScaleParm{ Scale=50, Interval=200, Pixel=100, Num=10  },
        //    new ScaleParm{ Scale=67, Interval=100, Pixel=67, Num=5  },
        //    new ScaleParm{ Scale=100, Interval=1, Pixel=96, Num=10  },
        //    new ScaleParm{ Scale=150, Interval=1, Pixel=75, Num=5  },
        //    new ScaleParm{ Scale=200, Interval=1, Pixel=100, Num=10  },
        //    new ScaleParm{ Scale=300, Interval=20, Pixel=60, Num=5  },
        //    new ScaleParm{ Scale=400, Interval=20, Pixel=80, Num=10  },
        //    new ScaleParm{ Scale=500, Interval=20, Pixel=100, Num=10  },
        //    new ScaleParm{ Scale=600, Interval=10, Pixel=60, Num=5  },
        //    new ScaleParm{ Scale=800, Interval=10, Pixel=80, Num=5  },
        //    new ScaleParm{ Scale=1000, Interval=10, Pixel=100, Num=10  },
        //    new ScaleParm{ Scale=1200, Interval=5, Pixel=60, Num=5  },
        //    new ScaleParm{ Scale=1400, Interval=5, Pixel=70, Num=5  },
        //    new ScaleParm{ Scale=1600, Interval=5, Pixel=80, Num=5  },
        //    new ScaleParm{ Scale=2000, Interval=5, Pixel=100, Num=10  },
        //    new ScaleParm{ Scale=2400, Interval=5, Pixel=120, Num=10  },
        //    new ScaleParm{ Scale=2800, Interval=5, Pixel=140, Num=10  },
        //    new ScaleParm{ Scale=3200, Interval=2, Pixel=64, Num=2  },
        //    new ScaleParm{ Scale=4000, Interval=2, Pixel=80, Num=2  },
        //    new ScaleParm{ Scale=4800, Interval=2, Pixel=96, Num=2  },
        //    new ScaleParm{ Scale=5600, Interval=2, Pixel=112, Num=2  },
        //    new ScaleParm{ Scale=6400, Interval=2, Pixel=128, Num=2  },
        //};

        //private readonly List<ScaleParm> CmScaleParms = new List<ScaleParm>() {
        //    new ScaleParm{ Scale=2, Interval=5000, Pixel=100, Num=10  },
        //    new ScaleParm{ Scale=3, Interval=5000, Pixel=150, Num=10  },
        //    new ScaleParm{ Scale=4, Interval=2000, Pixel=80, Num=10  },
        //    new ScaleParm{ Scale=5, Interval=2000, Pixel=100, Num=10  },
        //    new ScaleParm{ Scale=6, Interval=1000, Pixel=60, Num=5  },
        //    new ScaleParm{ Scale=7, Interval=1000, Pixel=70, Num=5  },
        //    new ScaleParm{ Scale=8, Interval=1000, Pixel=80, Num=10  },
        //    new ScaleParm{ Scale=10, Interval=1000, Pixel=100, Num=10  },
        //    new ScaleParm{ Scale=13, Interval=500, Pixel=65, Num=5  },
        //    new ScaleParm{ Scale=17, Interval=500, Pixel=85, Num=10  },
        //    new ScaleParm{ Scale=20, Interval=500, Pixel=100, Num=10  },
        //    new ScaleParm{ Scale=25, Interval=500, Pixel=120, Num=10  },
        //    new ScaleParm{ Scale=33, Interval=200, Pixel=66, Num=5  },
        //    new ScaleParm{ Scale=50, Interval=200, Pixel=100, Num=10  },
        //    new ScaleParm{ Scale=67, Interval=100, Pixel=67, Num=5  },
        //    new ScaleParm{ Scale=100, Interval=100, Pixel=100, Num=10  },
        //    new ScaleParm{ Scale=150, Interval=50, Pixel=75, Num=5  },
        //    new ScaleParm{ Scale=200, Interval=50, Pixel=100, Num=10  },
        //    new ScaleParm{ Scale=300, Interval=20, Pixel=60, Num=5  },
        //    new ScaleParm{ Scale=400, Interval=20, Pixel=80, Num=10  },
        //    new ScaleParm{ Scale=500, Interval=20, Pixel=100, Num=10  },
        //    new ScaleParm{ Scale=600, Interval=10, Pixel=60, Num=5  },
        //    new ScaleParm{ Scale=800, Interval=10, Pixel=80, Num=5  },
        //    new ScaleParm{ Scale=1000, Interval=10, Pixel=100, Num=10  },
        //    new ScaleParm{ Scale=1200, Interval=5, Pixel=60, Num=5  },
        //    new ScaleParm{ Scale=1400, Interval=5, Pixel=70, Num=5  },
        //    new ScaleParm{ Scale=1600, Interval=5, Pixel=80, Num=5  },
        //    new ScaleParm{ Scale=2000, Interval=5, Pixel=100, Num=10  },
        //    new ScaleParm{ Scale=2400, Interval=5, Pixel=120, Num=10  },
        //    new ScaleParm{ Scale=2800, Interval=5, Pixel=140, Num=10  },
        //    new ScaleParm{ Scale=3200, Interval=2, Pixel=64, Num=2  },
        //    new ScaleParm{ Scale=4000, Interval=2, Pixel=80, Num=2  },
        //    new ScaleParm{ Scale=4800, Interval=2, Pixel=96, Num=2  },
        //    new ScaleParm{ Scale=5600, Interval=2, Pixel=112, Num=2  },
        //    new ScaleParm{ Scale=6400, Interval=2, Pixel=128, Num=2  },
        //};



        public static readonly StyledProperty<double> ContWidthProperty =
            AvaloniaProperty.Register<Ruler, double>(nameof(ContWidth));

        public static readonly StyledProperty<double> ContHeightProperty =
            AvaloniaProperty.Register<Ruler, double>(nameof(ContHeight));

        public static readonly StyledProperty<double> ValueXProperty =
            AvaloniaProperty.Register<Ruler, double>(nameof(ValueX), defaultValue: 0d);

        public static readonly StyledProperty<double> ValueYProperty =
            AvaloniaProperty.Register<Ruler, double>(nameof(ValueY), defaultValue: 0d);

        public static readonly StyledProperty<int> ScaleProperty =
            AvaloniaProperty.Register<Ruler, int>(nameof(Scale), inherits: true, defaultValue: 100);

        public static readonly StyledProperty<IBrush> HighlightProperty = 
            AvaloniaProperty.Register<Ruler, IBrush>(nameof(Foreground), defaultValue: Brushes.Green);

        public static readonly StyledProperty<UnitEnum> UnitProperty =
            AvaloniaProperty.Register<Ruler, UnitEnum>(nameof(Unit), defaultValue: UnitEnum.Pixel);
        
        public static readonly StyledProperty<string> UnitNameProperty =
            AvaloniaProperty.Register<Ruler, string>(nameof(UnitName),defaultValue: "像素");

    
        public double ContWidth
        {
            get { return base.GetValue(ContWidthProperty); }
            set { this.SetValue(ContWidthProperty, value); }
        }

        public double ContHeight
        {
            get { return base.GetValue(ContHeightProperty); }
            set { this.SetValue(ContHeightProperty, value); }
        }

        public double ValueX
        {
            get { return base.GetValue(ValueXProperty); }
            set { this.SetValue(ValueXProperty, value); }
        }

        public double ValueY
        {
            get { return base.GetValue(ValueYProperty); }
            set { this.SetValue(ValueYProperty, value); }
        }

        public int Scale
        {
            get { return GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        public UnitEnum Unit
        {
            get { return base.GetValue(UnitProperty); }
            set { this.SetValue(UnitProperty, value); }
        }

        public string UnitName
        {
            get { return base.GetValue(UnitNameProperty); }
            set { this.SetValue(UnitNameProperty, value); }
        }


        public IBrush Highlight
        {
            get { return GetValue(HighlightProperty); }
            set { SetValue(HighlightProperty, value); }
        }

        //Line mouseVerticalTrackLine;
        //Line mouseHorizontalTrackLine;

        static Ruler()
        {
            AffectsRender<Ruler>(ScaleProperty);
        }

        public Ruler()
        {
            this.PointerMoved += RulerControl_PointerMoved;
            this.UseLayoutRounding = true;
            
        }

        private void RulerControl_PointerMoved(object sender, PointerEventArgs e)
        {
            var point = e.GetPosition(this);
            ValueX = point.X > 20 ? point.X : 20;
            ValueY = point.Y > 20 ? point.Y : 20;
        }


        private void TitleBarColor(DrawingContext drawingContext, UnitEnum Unit)
        {
            drawingContext.FillRectangle(new SolidColorBrush(Colors.White), new Rect(new Point(0, 0), new Point(20, 20)));
            //drawingContext.DrawText(Brushes.Black,
            //     new Point(0, 2),
            //     new FormattedText()
            //     {
            //         Text = Unit == UnitEnum.Pixel ? "像素" : (Unit == UnitEnum.Inch ? "英寸" : "厘米"),
            //         Typeface = new Typeface("宋体", 10),
            //         TextAlignment = Avalonia.Media.TextAlignment.Center
            //     });

            drawingContext.FillRectangle(Brushes.White, new Rect(new Point(20, 0), new Point(this.Bounds.Width, 20)));
            drawingContext.FillRectangle(Brushes.White, new Rect(new Point(0, 20), new Point(20, this.Bounds.Height)));
        }


        public override void Render(DrawingContext drawingContext)
        {
            //const double minPixel = 100;       //长黑标线最小像素间距
            //string _unit = "mm";             //记录尺度单位
            //double interval;                   //长黑标线实际距离
            //double intervalPixel;              //长黑标线实际像素距离
            //int intervalNum;

            float scale = Scale / 100f;
        

            TitleBarColor(drawingContext, Unit);

            var scaleParm = PixelScaleParms.FirstOrDefault(o => o.Scale == Scale);
            if (scaleParm == null)
                return;

            double interval = scaleParm.Interval;//长黑标线实际距离
            double intervalPixel = scaleParm.Pixel; //长黑标线实际像素距离
            int intervalNum = scaleParm.Num;
            
            var ftext = new FormattedText() { Typeface = new Typeface("Arial", 10), TextAlignment = TextAlignment.Left };

            
            //水平
            double _width = this.Bounds.Width;
            double _pixelDistence = intervalPixel / intervalNum;     //单个小间隔像素距离
            double startx = Math.Round((_width - ContWidth * scale) / 2, 0) + 10;

            System.Diagnostics.Debug.WriteLine(_pixelDistence+","+Scale);

            StreamGeometry strgeo = new StreamGeometry();
            using StreamGeometryContext ctx = strgeo.Open();
            
            ctx.SetFillRule(FillRule.NonZero);
            
            int _lineIndex = 0;
            // > 0 (+)
            for (double i = startx; i < _width; i += _pixelDistence)
            {
                if (_lineIndex % intervalNum == 0)
                {
                    ftext.Text = (interval * (_lineIndex / intervalNum)).ToString();
                    drawingContext.DrawText(Foreground, new Point(i + 3, -1), ftext);
                    //drawingContext.FillRectangle(Brushes.Black, new Rect(new Point(i, 0), new Point(i + 1, 20)));
                    ctx.BeginFigure(new Point(i, 0), false);
                    ctx.LineTo(new Point(i, 20));
                    ctx.LineTo(new Point(i + 1, 20));
                    ctx.LineTo(new Point(i + 1, 0));
                }
                else
                {
                    //drawingContext.FillRectangle(new SolidColorBrush(Colors.Black, 0.8d), new Rect(new Point(i, 12), new Point(i + 1, 20)));
                    ctx.BeginFigure(new Point(i, 16), false);
                    ctx.LineTo(new Point(i, 20));
                    ctx.LineTo(new Point(i + 1, 20));
                    ctx.LineTo(new Point(i + 1, 16));
                    //ctx.EndFigure(true);
                }
                _lineIndex++;
            }

            _lineIndex = 0;
            // < 0 (-)
            for (double i = startx; i >= startv; i -= _pixelDistence)
            {
                if (_lineIndex % intervalNum == 0)
                {
                    if (_lineIndex != 0)
                    {
                        ftext.Text = (interval * (_lineIndex / intervalNum)).ToString("-0");
                        drawingContext.DrawText(Foreground, new Point(i + 3, -1), ftext);
                    }
                    //drawingContext.FillRectangle(Brushes.Black, new Rect(new Point(i, 0), new Point(i + 1, 20)));
                    ctx.BeginFigure(new Point(i, 0), false);
                    ctx.LineTo(new Point(i, 20));
                    ctx.LineTo(new Point(i + 1, 20));
                    ctx.LineTo(new Point(i + 1, 0));
                    //ctx.EndFigure(true);
                }
                else
                {
                    //drawingContext.FillRectangle(new SolidColorBrush(Colors.Black, 0.8d), new Rect(new Point(i, 12), new Point(i + 1, 20)));
                    ctx.BeginFigure(new Point(i, 16), false);
                    ctx.LineTo(new Point(i, 20));
                    ctx.LineTo(new Point(i + 1, 20));
                    ctx.LineTo(new Point(i + 1, 16));
                    //ctx.EndFigure(true);
                }
                _lineIndex++;
            }


            //垂直
            double _height = this.Bounds.Height;
            double starty = Math.Round((_height - ContHeight * scale) / 2, 0) + 10;
            using (drawingContext.PushPreTransform(new TranslateTransform(20, 0).Value))
            using (drawingContext.PushPreTransform(new RotateTransform(-270).Value))
            {
                // > 0 (+)
                _lineIndex = 0;
                for (double i = starty; i < _height; i += _pixelDistence)
                {
                    if (_lineIndex % intervalNum == 0)
                    {
                        ftext.Text = (interval * (_lineIndex / intervalNum)).ToString();
                        drawingContext.DrawText(Foreground, new Point(i + 3, 8), ftext);
                        //drawingContext.FillRectangle(Brushes.Black, new Rect(new Point(i, 0), new Point(i + 1, 20)));
                        ctx.BeginFigure(new Point(0, i), false);
                        ctx.LineTo(new Point(20, i));
                        ctx.LineTo(new Point(20, i + 1));
                        ctx.LineTo(new Point(0, i + 1));
                        //ctx.EndFigure(true);
                    }
                    else
                    {
                        //drawingContext.FillRectangle(new SolidColorBrush(Colors.Black, 0.8d), new Rect(new Point(i, 0), new Point(i + 1, 8)));
                        ctx.BeginFigure(new Point(16, i), false);
                        ctx.LineTo(new Point(20, i));
                        ctx.LineTo(new Point(20, i + 1));
                        ctx.LineTo(new Point(16, i + 1));
                    }
                    _lineIndex++;
                }

                // < 0 (-)
                _lineIndex = 0;
                for (double i = starty; i >= startv; i -= _pixelDistence)
                {
                    if (_lineIndex % intervalNum == 0)
                    {
                        if (_lineIndex != 0)
                        {
                            ftext.Text = (interval * (_lineIndex / intervalNum)).ToString("-0");
                            drawingContext.DrawText(Foreground, new Point(i + 3, 8), ftext);
                        }
                        // drawingContext.FillRectangle(Brushes.Black, new Rect(new Point(i, 0), new Point(i + 1, 20)));
                        ctx.BeginFigure(new Point(0, i), true);
                        ctx.LineTo(new Point(20, i));
                        ctx.LineTo(new Point(20, i + 1));
                        ctx.LineTo(new Point(0, i + 1));
                    }
                    else
                    {
                        // drawingContext.FillRectangle(new SolidColorBrush(Colors.Black, 0.8d), new Rect(new Point(i, 0), new Point(i + 1, 8)));
                        ctx.BeginFigure(new Point(16, i), false);
                        ctx.LineTo(new Point(20, i));
                        ctx.LineTo(new Point(20, i + 1));
                        ctx.LineTo(new Point(16, i + 1));
                    }
                    _lineIndex++;
                }
            }

            drawingContext.DrawGeometry(Foreground, null, strgeo);
        }



        //private (double Interval, double IntervalPixel, int IntervalNum) PixelRender(double scale)
        //{
        //    PixelScaleParms.FirstOrDefault(o => o.Scale == scale);


        //    const double minPixel = 30;       //长黑标线最小像素间距
        //    double interval = 100;                   //长黑标线实际距离
        //    double intervalPixel = 100;              //长黑标线实际像素距离
        //    int intervalNum = 10;



        //    double intervalTemp = interval / scale;





        //    if (intervalPixel >= 30 && intervalPixel <= 120) { 

        //    }




        //    ////用来判断实际有效数字及尺度
        //    //string[] strTemp = (minPixel * scale).ToString("E").Split('E');
        //    //double.TryParse(strTemp[0], out double scientificF);
        //    //int.TryParse(strTemp[1], out int scientificE);

        //    ////_scientificE =-2 0.0x, -1 0.x, 0 个位数，1十位数据 ，2百位
        //    //if (scientificE >= 2 || (scientificE >= 1 && scientificF >= 5))
        //    //{
        //    //    scientificE -= 3;
        //    //}

        //    ////将间隔标准化
        //    //if (scientificE < 2) //-2 + 4， -1+3， 0+2， 1+1 
        //    //{
        //    //    interval = scientificF * Math.Pow(10, -scientificE + 2);
        //    //}
        //    //else
        //    //{
        //    //    //将间隔标准化
        //    //    if (scientificF >= 5)
        //    //    {
        //    //        interval = 10 * Math.Pow(10, scientificE);
        //    //    }
        //    //    else if (scientificF >= 2.5)
        //    //    {
        //    //        interval = 5 * Math.Pow(10, scientificE);
        //    //    }
        //    //    else
        //    //    {
        //    //        interval = 2.5 * Math.Pow(10, scientificE);
        //    //    }
        //    //}



        //    return (interval, intervalPixel, intervalNum);
        //}

        //private void InchRender(DrawingContext drawingContext)
        //{

        //}

        //private void CMRender(DrawingContext drawingContext)
        //{

        //}

        //private (double Interval, double Distenced, int IntervalNum) GetPixelDistenced(double interval, double intervalPixel, UnitEnum unit, int scale)
        //{
        //    int jg = 0;
        //    double distenced = intervalPixel / 10;
        //    double intervald = interval;

        //    distenced = Math.Round((scale / 100d), 1) * distenced;
        //    intervald = interval * (scale / 100d);

        //    if (unit == UnitEnum.Pixel)
        //    {
        //        if (distenced < 2.5)
        //        {
        //            distenced = 2.5;
        //            jg = 4;
        //        }
        //        else if (distenced < 5)
        //        {
        //            distenced = 2.5;
        //            jg = 4;
        //        }
        //        else if (distenced < 10)
        //        {
        //            distenced = 5;
        //            jg = 5;
        //        }
        //        else
        //        {
        //            distenced = 10;
        //            jg = 10;
        //        }

        //    }
        //    else if (unit == UnitEnum.Inch)
        //    {

        //    }
        //    else
        //    { //cm

        //    }

        //    return (intervald, distenced, jg);
        //}

        //public void RaiseHorizontalRulerMoveEvent(MouseEventArgs e)
        //{
        //    Point mousePoint = e.GetPosition(this);
        //    mouseHorizontalTrackLine.X1 = mouseHorizontalTrackLine.X2 = mousePoint.X;
        //}
        //public void RaiseVerticalRulerMoveEvent(MouseEventArgs e)
        //{
        //    Point mousePoint = e.GetPosition(this);
        //    mouseVerticalTrackLine.Y1 = mouseVerticalTrackLine.Y2 = mousePoint.Y;
        //}

        //protected override void ApplyTemplate()
        //{
        //    //mouseVerticalTrackLine = this.FindControl<Line>("verticalTrackLine");
        //    //mouseHorizontalTrackLine = this.FindControl<Line>("horizontalTrackLine");
        //    //mouseVerticalTrackLine.IsVisible =true;
        //    //mouseHorizontalTrackLine.IsVisible = true;
        //}
    }

    internal class ScaleParm
    {
        public int Scale { get; set; }
        public double Interval { get; set; }           //长黑标线实际距离
        public double Pixel { get; set; }              //长黑标线实际像素距离
        public int Num { get; set; }

       
    }
}
