﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Ant.Wpf.Controls
{
    public class ScrollViewer : System.Windows.Controls.ScrollViewer
    {
        private double _totalVerticalOffset;

        private double _totalHorizontalOffset;

        private bool _isRunning;

        /// <summary>
        ///     滚动方向
        /// </summary>
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
            nameof(Orientation), typeof(System.Windows.Controls.Orientation), typeof(ScrollViewer), new PropertyMetadata(System.Windows.Controls.Orientation.Vertical));

        /// <summary>
        ///     滚动方向
        /// </summary>
        public System.Windows.Controls.Orientation Orientation
        {
            get => (System.Windows.Controls.Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        /// <summary>
        ///     是否响应鼠标滚轮操作
        /// </summary>
        public static readonly DependencyProperty CanMouseWheelProperty = DependencyProperty.Register(
            nameof(CanMouseWheel), typeof(bool), typeof(ScrollViewer), new PropertyMetadata(true));

        /// <summary>
        ///     是否响应鼠标滚轮操作
        /// </summary>
        public bool CanMouseWheel
        {
            get => (bool)GetValue(CanMouseWheelProperty);
            set => SetValue(CanMouseWheelProperty, value);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (!CanMouseWheel) return;

            if (!IsEnableInertia)
            {
                if (Orientation == System.Windows.Controls.Orientation.Vertical)
                {
                    base.OnMouseWheel(e);
                }
                else
                {
                    _totalHorizontalOffset = HorizontalOffset;
                    SetCurrentValue(CurrentHorizontalOffsetProperty, HorizontalOffset);
                    _totalHorizontalOffset = Math.Min(Math.Max(0, _totalHorizontalOffset - e.Delta), ScrollableWidth);
                    SetCurrentValue(CurrentHorizontalOffsetProperty, _totalHorizontalOffset);
                }
                return;
            }
            e.Handled = true;

            if (Orientation == System.Windows.Controls.Orientation.Vertical)
            {
                if (!_isRunning)
                {
                    _totalVerticalOffset = VerticalOffset;
                    SetCurrentValue(CurrentVerticalOffsetProperty, VerticalOffset);
                }
                _totalVerticalOffset = Math.Min(Math.Max(0, _totalVerticalOffset - e.Delta), ScrollableHeight);
                ScrollToVerticalOffsetInternal(_totalVerticalOffset);
            }
            else
            {
                if (!_isRunning)
                {
                    _totalHorizontalOffset = HorizontalOffset;
                    SetCurrentValue(CurrentHorizontalOffsetProperty, HorizontalOffset);
                }
                _totalHorizontalOffset = Math.Min(Math.Max(0, _totalHorizontalOffset - e.Delta), ScrollableWidth);
                ScrollToHorizontalOffsetInternal(_totalHorizontalOffset);
            }
        }

        internal void ScrollToTopInternal(double milliseconds = 500)
        {
            if (!_isRunning)
            {
                _totalVerticalOffset = VerticalOffset;
                SetCurrentValue(CurrentVerticalOffsetProperty, VerticalOffset);
            }
            ScrollToVerticalOffsetInternal(0, milliseconds);
        }

        internal void ScrollToVerticalOffsetInternal(double offset, double milliseconds = 500)
        {
            var animation = AnimationHelper.CreateAnimation(offset, milliseconds);
            animation.EasingFunction = new CubicEase
            {
                EasingMode = EasingMode.EaseOut
            };
            animation.FillBehavior = FillBehavior.Stop;
            animation.Completed += (s, e1) =>
            {
                SetCurrentValue(CurrentVerticalOffsetProperty, offset);
                _isRunning = false;
            };
            _isRunning = true;

            BeginAnimation(CurrentVerticalOffsetProperty, animation, HandoffBehavior.Compose);
        }

        internal void ScrollToHorizontalOffsetInternal(double offset, double milliseconds = 500)
        {
            var animation = AnimationHelper.CreateAnimation(offset, milliseconds);
            animation.EasingFunction = new CubicEase
            {
                EasingMode = EasingMode.EaseOut
            };
            animation.FillBehavior = FillBehavior.Stop;
            animation.Completed += (s, e1) =>
            {
                SetCurrentValue(CurrentHorizontalOffsetProperty, offset);
                _isRunning = false;
            };
            _isRunning = true;

            BeginAnimation(CurrentHorizontalOffsetProperty, animation, HandoffBehavior.Compose);
        }

        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters) =>
            IsPenetrating ? null : base.HitTestCore(hitTestParameters);

        /// <summary>
        ///     是否支持惯性
        /// </summary>
        public static readonly DependencyProperty IsEnableInertiaProperty = DependencyProperty.RegisterAttached(
            nameof(IsEnableInertia), typeof(bool), typeof(ScrollViewer), new PropertyMetadata(false));

        public static void SetIsEnableInertia(DependencyObject element, bool value)
        {
            element.SetValue(IsEnableInertiaProperty, value);
        }

        public static bool GetIsEnableInertia(DependencyObject element)
        {
            return (bool)element.GetValue(IsEnableInertiaProperty);
        }

        /// <summary>
        ///     是否支持惯性
        /// </summary>
        public bool IsEnableInertia
        {
            get => (bool)GetValue(IsEnableInertiaProperty);
            set => SetValue(IsEnableInertiaProperty, value);
        }

        /// <summary>
        ///     控件是否可以穿透点击
        /// </summary>
        public static readonly DependencyProperty IsPenetratingProperty = DependencyProperty.RegisterAttached(
            nameof(IsPenetrating), typeof(bool), typeof(ScrollViewer), new PropertyMetadata(false));

        /// <summary>
        ///     控件是否可以穿透点击
        /// </summary>
        public bool IsPenetrating
        {
            get => (bool)GetValue(IsPenetratingProperty);
            set => SetValue(IsPenetratingProperty, value);
        }

        public static void SetIsPenetrating(DependencyObject element, bool value)
        {
            element.SetValue(IsPenetratingProperty, value);
        }

        public static bool GetIsPenetrating(DependencyObject element)
        {
            return (bool)element.GetValue(IsPenetratingProperty);
        }

        /// <summary>
        ///     当前垂直滚动偏移
        /// </summary>
        internal static readonly DependencyProperty CurrentVerticalOffsetProperty = DependencyProperty.Register(
            nameof(CurrentVerticalOffset), typeof(double), typeof(ScrollViewer), new PropertyMetadata(0.0, OnCurrentVerticalOffsetChanged));

        private static void OnCurrentVerticalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScrollViewer ctl && e.NewValue is double v)
            {
                ctl.ScrollToVerticalOffset(v);
            }
        }

        /// <summary>
        ///     当前垂直滚动偏移
        /// </summary>
        internal double CurrentVerticalOffset
        {
            // ReSharper disable once UnusedMember.Local
            get => (double)GetValue(CurrentVerticalOffsetProperty);
            set => SetValue(CurrentVerticalOffsetProperty, value);
        }

        /// <summary>
        ///     当前水平滚动偏移
        /// </summary>
        internal static readonly DependencyProperty CurrentHorizontalOffsetProperty = DependencyProperty.Register(
            nameof(CurrentHorizontalOffset), typeof(double), typeof(ScrollViewer), new PropertyMetadata(0.0, OnCurrentHorizontalOffsetChanged));

        private static void OnCurrentHorizontalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScrollViewer ctl && e.NewValue is double v)
            {
                ctl.ScrollToHorizontalOffset(v);
            }
        }

        /// <summary>
        ///     当前水平滚动偏移
        /// </summary>
        internal double CurrentHorizontalOffset
        {
            get => (double)GetValue(CurrentHorizontalOffsetProperty);
            set => SetValue(CurrentHorizontalOffsetProperty, value);
        }
    }
}