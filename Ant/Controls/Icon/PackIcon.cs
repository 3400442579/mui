using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Ant.Wpf.Controls
{
    public class PackIcon : Control
    {
        private static readonly Lazy<IDictionary<PackIconKind, string>> _dataIndex
            = new Lazy<IDictionary<PackIconKind, string>>(PackIconDataFactory.Create);

        static PackIcon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PackIcon), new FrameworkPropertyMetadata(typeof(PackIcon)));
            OpacityProperty.OverrideMetadata(typeof(PackIcon), new UIPropertyMetadata(1d, (d, e) => { d.CoerceValue(SpinProperty); }));
            VisibilityProperty.OverrideMetadata(typeof(PackIcon), new UIPropertyMetadata(Visibility.Visible, (d, e) => { d.CoerceValue(SpinProperty); }));
        }

        private static readonly DependencyPropertyKey DataPropertyKey
           = DependencyProperty.RegisterReadOnly(nameof(Data), typeof(string), typeof(PackIcon), new PropertyMetadata(""));

        // ReSharper disable once StaticMemberInGenericType
        /// <summary>Identifies the <see cref="Data"/> dependency property.</summary>
        public static readonly DependencyProperty DataProperty = DataPropertyKey.DependencyProperty;


        /// <summary>
        /// Gets the path data for the current icon kind.
        /// </summary>
        [TypeConverter(typeof(GeometryConverter))]
        public string Data
        {
            get { return (string)GetValue(DataProperty); }
            protected set { SetValue(DataPropertyKey, value); }
        }

        /// <summary>Identifies the <see cref="Icon"/> dependency property.</summary>
        public static readonly DependencyProperty IconProperty
            = DependencyProperty.Register(nameof(Icon), typeof(string), typeof(PackIcon), new PropertyMetadata("", OnIconChanged));

        /// <summary>
        /// Gets or sets the icon to display.
        /// </summary>
        public string Icon
        {
            get => (string)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        /// <summary>Identifies the <see cref="Kind"/> dependency property.</summary>
        public static readonly DependencyProperty KindProperty
            = DependencyProperty.Register(nameof(Kind), typeof(PackIconKind), typeof(PackIcon), new PropertyMetadata(default(PackIconKind), OnKindChanged));

        private static void OnKindChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                ((PackIcon)dependencyObject).UpdateData();
            }
        }

        /// <summary>
        /// Gets or sets the icon to display.
        /// </summary>
        public PackIconKind Kind
        {
            get { return (PackIconKind)GetValue(KindProperty); }
            set { SetValue(KindProperty, value); }
        }

        /// <summary>Identifies the <see cref="Flip"/> dependency property.</summary>
        public static readonly DependencyProperty FlipProperty
            = DependencyProperty.Register(
                nameof(Flip),
                typeof(PackIconFlipOrientation),
                typeof(PackIcon),
                new PropertyMetadata(PackIconFlipOrientation.Normal));

        /// <summary>
        /// Gets or sets the flip orientation.
        /// </summary>
        public PackIconFlipOrientation Flip
        {
            get { return (PackIconFlipOrientation)this.GetValue(FlipProperty); }
            set { this.SetValue(FlipProperty, value); }
        }

        /// <summary>Identifies the <see cref="RotationAngle"/> dependency property.</summary>
        public static readonly DependencyProperty RotationAngleProperty
            = DependencyProperty.Register(
                nameof(RotationAngle),
                typeof(double),
                typeof(PackIcon),
                new PropertyMetadata(0d, null, (dependencyObject, value) =>
                {
                    var val = (double)value;
                    return val < 0 ? 0d : (val > 360 ? 360d : value);
                }));


        /// <summary>
        /// Gets or sets the rotation (angle).
        /// </summary>
        /// <value>The rotation.</value>
        public double RotationAngle
        {
            get { return (double)this.GetValue(RotationAngleProperty); }
            set { this.SetValue(RotationAngleProperty, value); }
        }

        /// <summary>Identifies the <see cref="Spin"/> dependency property.</summary>
        public static readonly DependencyProperty SpinProperty
            = DependencyProperty.Register(
                nameof(Spin),
                typeof(bool),
                typeof(PackIcon),
                new PropertyMetadata(default(bool), OnSpinChanged, CoerceSpin));

        private static object CoerceSpin(DependencyObject dependencyObject, object value)
        {
            if (dependencyObject is PackIcon packIcon && (!packIcon.IsVisible || packIcon.Opacity <= 0 || packIcon.SpinDuration <= 0.0))
            {
                return false;
            }
            return value;
        }

        private static void OnSpinChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is PackIcon packIcon && e.OldValue != e.NewValue && e.NewValue is bool)
            {
                packIcon.ToggleSpinAnimation((bool)e.NewValue);
            }
        }


        /// <summary>
        /// Gets or sets a value indicating whether the inner icon is spinning.
        /// </summary>
        /// <value><c>true</c> if spin; otherwise, <c>false</c>.</value>
        public bool Spin
        {
            get { return (bool)this.GetValue(SpinProperty); }
            set { this.SetValue(SpinProperty, value); }
        }

        private void ToggleSpinAnimation(bool spin)
        {
            if (spin)
            {
                this.BeginSpinAnimation();
            }
            else
            {
                this.StopSpinAnimation();
            }
        }

        private Storyboard spinningStoryboard;
        private FrameworkElement _innerGrid;
        private FrameworkElement InnerGrid => this._innerGrid ?? (this._innerGrid = this.GetTemplateChild("PART_InnerGrid") as FrameworkElement);

        private void BeginSpinAnimation()
        {
            var element = this.InnerGrid;
            if (null == element)
            {
                return;
            }
            var transformGroup = element.RenderTransform as TransformGroup ?? new TransformGroup();
            var rotateTransform = transformGroup.Children.OfType<RotateTransform>().LastOrDefault();

            if (rotateTransform != null)
            {
                rotateTransform.Angle = 0;
            }
            else
            {
                transformGroup.Children.Add(new RotateTransform());
                element.RenderTransform = transformGroup;
            }

            var animation = new DoubleAnimation
            {
                From = 0,
                To = 360,
                AutoReverse = this.SpinAutoReverse,
                EasingFunction = this.SpinEasingFunction,
                RepeatBehavior = RepeatBehavior.Forever,
                Duration = new Duration(TimeSpan.FromSeconds(this.SpinDuration))
            };

            var storyboard = new Storyboard();
            storyboard.Children.Add(animation);
            Storyboard.SetTarget(animation, element);


            Storyboard.SetTargetProperty(animation, new PropertyPath($"(0).(1)[{transformGroup.Children.Count - 1}].(2)", RenderTransformProperty, TransformGroup.ChildrenProperty, RotateTransform.AngleProperty));

            spinningStoryboard = storyboard;
            storyboard.Begin();
        }

        private void StopSpinAnimation()
        {
            var storyboard = spinningStoryboard;
            storyboard?.Stop();
            spinningStoryboard = null;
        }

        /// <summary>Identifies the <see cref="SpinDuration"/> dependency property.</summary>
        public static readonly DependencyProperty SpinDurationProperty
            = DependencyProperty.Register(
                nameof(SpinDuration),
                typeof(double),
                typeof(PackIcon),

                new PropertyMetadata(1d, OnSpinDurationChanged));

        private static void OnSpinDurationChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is PackIcon packIcon && e.OldValue != e.NewValue && packIcon.Spin && e.NewValue is double)
            {
                packIcon.StopSpinAnimation();
                packIcon.BeginSpinAnimation();
            }
        }


        /// <summary>
        /// Gets or sets the duration of the spinning animation (in seconds). This will also restart the spin animation.
        /// </summary>
        /// <value>The duration of the spin in seconds.</value>
        public double SpinDuration
        {
            get { return (double)this.GetValue(SpinDurationProperty); }
            set { this.SetValue(SpinDurationProperty, value); }
        }

        /// <summary>Identifies the <see cref="SpinEasingFunction"/> dependency property.</summary>
        public static readonly DependencyProperty SpinEasingFunctionProperty
            = DependencyProperty.Register(
                nameof(SpinEasingFunction),

                typeof(IEasingFunction),
                typeof(PackIcon),
                new PropertyMetadata(null, OnSpinEasingFunctionChanged));

        private static void OnSpinEasingFunctionChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is PackIcon packIcon && e.OldValue != e.NewValue && packIcon.Spin)
            {
                packIcon.StopSpinAnimation();
                packIcon.BeginSpinAnimation();
            }
        }


        public IEasingFunction SpinEasingFunction
        {
            get { return (IEasingFunction)this.GetValue(SpinEasingFunctionProperty); }
            set { this.SetValue(SpinEasingFunctionProperty, value); }
        }

        /// <summary>Identifies the <see cref="SpinAutoReverse"/> dependency property.</summary>
        public static readonly DependencyProperty SpinAutoReverseProperty
            = DependencyProperty.Register(
                nameof(SpinAutoReverse),
                typeof(bool),
                typeof(PackIcon),
                new PropertyMetadata(default(bool), OnSpinAutoReverseChanged));

        private static void OnSpinAutoReverseChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is PackIcon packIcon && e.OldValue != e.NewValue && packIcon.Spin && e.NewValue is bool)
            {
                packIcon.StopSpinAnimation();
                packIcon.BeginSpinAnimation();
            }
        }

        /// <summary>
        /// Gets or sets the AutoReverse of the spinning animation. This will also restart the spin animation.
        /// </summary>
        /// <value><c>true</c> if [spin automatic reverse]; otherwise, <c>false</c>.</value>
        public bool SpinAutoReverse
        {
            get { return (bool)this.GetValue(SpinAutoReverseProperty); }
            set { this.SetValue(SpinAutoReverseProperty, value); }
        }




      


        protected void UpdateData()
        {
            if (string.IsNullOrWhiteSpace(Data))
            {
                if (Kind != default)
                {
                    string data = null;
                    _dataIndex.Value?.TryGetValue(Kind, out data);

                    this.Data = data;
                }
                else
                {
                    this.Data = null;
                }
            }
        }

        protected void UpdateData(object v)
        {
            if (v is string s)
                this.Data = s;
        }

        private static void OnIconChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                ((PackIcon)dependencyObject).UpdateData(e.NewValue);
            }
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            //this.UpdateData();

            this.CoerceValue(SpinProperty);

            if (this.Spin)
            {
                this.StopSpinAnimation();
                this.BeginSpinAnimation();
            }
        }


    }
}
