using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace DH.MUI.Controls
{
    public enum ShadowDepth
    {
        Depth0,
        Depth1,
        Depth2,
        Depth3,
        Depth4,
        Depth5
    }

    [Flags]
    public enum ShadowEdges
    {
        None = 0,
        Left = 1,
        Top = 2,
        Right = 4,
        Bottom = 8,
        All = Left | Top | Right | Bottom
    }

    internal class ShadowLocalInfo
    {
        public ShadowLocalInfo(double standardOpacity)
        {
            StandardOpacity = standardOpacity;
        }

        public double StandardOpacity { get; }
    }

    public static class ShadowAssist
    {
        public static readonly DependencyProperty ShadowDepthProperty = DependencyProperty.RegisterAttached(
            "ShadowDepth", typeof (ShadowDepth), typeof (ShadowAssist), new FrameworkPropertyMetadata(default(ShadowDepth), FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>Helper for setting <see cref="ShadowDepthProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="ShadowDepthProperty"/> on.</param>
        /// <param name="value">ShadowDepth property value.</param>
        public static void SetShadowDepth(DependencyObject element, ShadowDepth value)
        {
            element.SetValue(ShadowDepthProperty, value);
        }

        /// <summary>Helper for getting <see cref="ShadowDepthProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="ShadowDepthProperty"/> from.</param>
        /// <returns>ShadowDepth property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        public static ShadowDepth GetShadowDepth(DependencyObject element)
        {
            return (ShadowDepth) element.GetValue(ShadowDepthProperty);
        }

        private static readonly DependencyPropertyKey LocalInfoPropertyKey = DependencyProperty.RegisterAttachedReadOnly(
            "LocalInfo", typeof (ShadowLocalInfo), typeof (ShadowAssist), new PropertyMetadata(default(ShadowLocalInfo)));

        private static void SetLocalInfo(DependencyObject element, ShadowLocalInfo value)
        {
            element.SetValue(LocalInfoPropertyKey, value);
        }

        private static ShadowLocalInfo GetLocalInfo(DependencyObject element)
        {
            return (ShadowLocalInfo) element.GetValue(LocalInfoPropertyKey.DependencyProperty);
        }

        public static readonly DependencyProperty DarkenProperty = DependencyProperty.RegisterAttached(
            "Darken", typeof (bool), typeof (ShadowAssist), new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.AffectsRender, OnDarkenChanged));

        private static void OnDarkenChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var uiElement = dependencyObject as UIElement;

            if (!(uiElement?.Effect is DropShadowEffect dropShadowEffect)) return;

            if ((bool) dependencyPropertyChangedEventArgs.NewValue)
            {
                SetLocalInfo(dependencyObject, new ShadowLocalInfo(dropShadowEffect.Opacity));

                var doubleAnimation = new DoubleAnimation(1, new Duration(TimeSpan.FromMilliseconds(350)))
                {
                    FillBehavior = FillBehavior.HoldEnd
                };
                dropShadowEffect.BeginAnimation(DropShadowEffect.OpacityProperty, doubleAnimation);                
            }
            else
            {
                var shadowLocalInfo = GetLocalInfo(dependencyObject);
                if (shadowLocalInfo == null) return;

                var doubleAnimation = new DoubleAnimation(shadowLocalInfo.StandardOpacity, new Duration(TimeSpan.FromMilliseconds(350)))
                {
                    FillBehavior = FillBehavior.HoldEnd
                };
                dropShadowEffect.BeginAnimation(DropShadowEffect.OpacityProperty, doubleAnimation);
            }
        }

        /// <summary>Helper for setting <see cref="DarkenProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="DarkenProperty"/> on.</param>
        /// <param name="value">Darken property value.</param>
        public static void SetDarken(DependencyObject element, bool value)
        {
            element.SetValue(DarkenProperty, value);
        }

        /// <summary>Helper for getting <see cref="DarkenProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="DarkenProperty"/> from.</param>
        /// <returns>Darken property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        public static bool GetDarken(DependencyObject element)
        {
            return (bool) element.GetValue(DarkenProperty);
        }

        public static readonly DependencyProperty CacheModeProperty = DependencyProperty.RegisterAttached(
            "CacheMode", typeof(CacheMode), typeof(ShadowAssist), new FrameworkPropertyMetadata(new BitmapCache { EnableClearType = true, SnapsToDevicePixels = true }, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>Helper for setting <see cref="CacheModeProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="CacheModeProperty"/> on.</param>
        /// <param name="value">CacheMode property value.</param>
        public static void SetCacheMode(DependencyObject element, CacheMode value)
        {
            element.SetValue(CacheModeProperty, value);
        }

        /// <summary>Helper for getting <see cref="CacheModeProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="CacheModeProperty"/> from.</param>
        /// <returns>CacheMode property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        public static CacheMode GetCacheMode(DependencyObject element)
        {
            return (CacheMode)element.GetValue(CacheModeProperty);
        }

        public static readonly DependencyProperty ShadowEdgesProperty = DependencyProperty.RegisterAttached(
            "ShadowEdges", typeof(ShadowEdges), typeof(ShadowAssist), new PropertyMetadata(ShadowEdges.All));

        /// <summary>Helper for setting <see cref="ShadowEdgesProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="ShadowEdgesProperty"/> on.</param>
        /// <param name="value">ShadowEdges property value.</param>
        public static void SetShadowEdges(DependencyObject element, ShadowEdges value)
        {
            element.SetValue(ShadowEdgesProperty, value);
        }

        /// <summary>Helper for getting <see cref="ShadowEdgesProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="ShadowEdgesProperty"/> from.</param>
        /// <returns>ShadowEdges property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        public static ShadowEdges GetShadowEdges(DependencyObject element)
        {
            return (ShadowEdges) element.GetValue(ShadowEdgesProperty);
        }
    }
}