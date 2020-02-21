using System.Windows;
using System.Windows.Controls;

namespace Animation.Editor.Controls
{

    public class CustomImage : Control
    {
        static CustomImage()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomImage), new FrameworkPropertyMetadata(typeof(CustomImage)));
        }

        public static readonly DependencyProperty SourceProperty = 
            DependencyProperty.Register(nameof(Source), 
               typeof(string), 
               typeof(CustomImage),
               new UIPropertyMetadata(null, new PropertyChangedCallback(OnSourceChanged)));

        public string Source
        {
            get => (string)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }
        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //CustomImage ctrl = (CustomImage)d;
        }


        /// <summary>
        /// 画布宽度
        /// </summary>
        public static readonly DependencyProperty CanvasWidthProperty
           = DependencyProperty.Register(nameof(CanvasWidth), typeof(int), typeof(CustomImage));
        public int CanvasWidth
        {
            get => (int)GetValue(CanvasWidthProperty);
            set => SetValue(CanvasWidthProperty, value);
        }

        /// <summary>
        /// 画布高度
        /// </summary>
        public static readonly DependencyProperty CanvasHeightProperty = 
            DependencyProperty.Register(nameof(CanvasHeight),
               typeof(int),typeof(CustomImage),
               new FrameworkPropertyMetadata(0));
        public int CanvasHeight
        {
            get => (int)GetValue(CanvasHeightProperty);
            set => SetValue(CanvasHeightProperty, value);
        }
       

        public static readonly DependencyProperty ZoomProperty =
           DependencyProperty.Register("Zoom", typeof(double), typeof(CustomImage), new UIPropertyMetadata(1d));
        public double Zoom
        {
            get { return (double)GetValue(ZoomProperty); }
            set { this.SetValue(ZoomProperty, value); }
        }
    }
}
