using System.Windows;
using System.Windows.Media;

namespace DH.MUI.Controls
{
    public static class ButtonProgressAssist
    {
        public static readonly DependencyProperty MinimumProperty = DependencyProperty.RegisterAttached(
            "Minimum", typeof(double), typeof(ButtonProgressAssist), new FrameworkPropertyMetadata(default(double)));

        /// <summary>Helper for setting <see cref="MinimumProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="MinimumProperty"/> on.</param>
        /// <param name="value">Minimum property value.</param>
        public static void SetMinimum(DependencyObject element, double value)
        {
            element.SetValue(MinimumProperty, value);
        }

        /// <summary>Helper for getting <see cref="MinimumProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="MinimumProperty"/> from.</param>
        /// <returns>Minimum property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        public static double GetMinimum(DependencyObject element)
        {
            return (double)element.GetValue(MinimumProperty);
        }

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.RegisterAttached(
            "Maximum", typeof(double), typeof(ButtonProgressAssist), new FrameworkPropertyMetadata(100.0));

        /// <summary>Helper for setting <see cref="MaximumProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="MaximumProperty"/> on.</param>
        /// <param name="value">Maximum property value.</param>
        public static void SetMaximum(DependencyObject element, double value)
        {
            element.SetValue(MaximumProperty, value);
        }

        /// <summary>Helper for getting <see cref="MaximumProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="MaximumProperty"/> from.</param>
        /// <returns>Maximum property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        public static double GetMaximum(DependencyObject element)
        {
            return (double)element.GetValue(MaximumProperty);
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.RegisterAttached(
            "Value", typeof(double), typeof(ButtonProgressAssist), new FrameworkPropertyMetadata(default(double)));

        /// <summary>Helper for setting <see cref="ValueProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="ValueProperty"/> on.</param>
        /// <param name="value">Value property value.</param>
        public static void SetValue(DependencyObject element, double value)
        {
            element.SetValue(ValueProperty, value);
        }

        /// <summary>Helper for getting <see cref="ValueProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="ValueProperty"/> from.</param>
        /// <returns>Value property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        public static double GetValue(DependencyObject element)
        {
            return (double)element.GetValue(ValueProperty);
        }

        public static readonly DependencyProperty IsIndeterminateProperty = DependencyProperty.RegisterAttached(
            "IsIndeterminate", typeof(bool), typeof(ButtonProgressAssist), new FrameworkPropertyMetadata(default(bool)));

        /// <summary>Helper for setting <see cref="IsIndeterminateProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="IsIndeterminateProperty"/> on.</param>
        /// <param name="isIndeterminate">IsIndeterminate property value.</param>
        public static void SetIsIndeterminate(DependencyObject element, bool isIndeterminate)
        {
            element.SetValue(IsIndeterminateProperty, isIndeterminate);
        }

        /// <summary>Helper for getting <see cref="IsIndeterminateProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="IsIndeterminateProperty"/> from.</param>
        /// <returns>IsIndeterminate property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        public static bool GetIsIndeterminate(DependencyObject element)
        {
            return (bool)element.GetValue(IsIndeterminateProperty);
        }

        public static readonly DependencyProperty IndicatorForegroundProperty = DependencyProperty.RegisterAttached(
            "IndicatorForeground", typeof(Brush), typeof(ButtonProgressAssist), new FrameworkPropertyMetadata(default(Brush)));

        /// <summary>Helper for setting <see cref="IndicatorForegroundProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="IndicatorForegroundProperty"/> on.</param>
        /// <param name="indicatorForeground">IndicatorForeground property value.</param>
        public static void SetIndicatorForeground(DependencyObject element, Brush indicatorForeground)
        {
            element.SetValue(IndicatorForegroundProperty, indicatorForeground);
        }

        /// <summary>Helper for getting <see cref="IndicatorForegroundProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="IndicatorForegroundProperty"/> from.</param>
        /// <returns>IndicatorForeground property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        public static Brush GetIndicatorForeground(DependencyObject element)
        {
            return (Brush)element.GetValue(IndicatorForegroundProperty);
        }

        public static readonly DependencyProperty IndicatorBackgroundProperty = DependencyProperty.RegisterAttached(
            "IndicatorBackground", typeof(Brush), typeof(ButtonProgressAssist), new FrameworkPropertyMetadata(default(Brush)));

        /// <summary>Helper for setting <see cref="IndicatorBackgroundProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="IndicatorBackgroundProperty"/> on.</param>
        /// <param name="indicatorBackground">IndicatorBackground property value.</param>
        public static void SetIndicatorBackground(DependencyObject element, Brush indicatorBackground)
        {
            element.SetValue(IndicatorBackgroundProperty, indicatorBackground);
        }

        /// <summary>Helper for getting <see cref="IndicatorBackgroundProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="IndicatorBackgroundProperty"/> from.</param>
        /// <returns>IndicatorBackground property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        public static Brush GetIndicatorBackground(DependencyObject element)
        {
            return (Brush)element.GetValue(IndicatorBackgroundProperty);
        }

        public static readonly DependencyProperty IsIndicatorVisibleProperty = DependencyProperty.RegisterAttached(
            "IsIndicatorVisible", typeof(bool), typeof(ButtonProgressAssist), new FrameworkPropertyMetadata(default(bool)));

        /// <summary>Helper for setting <see cref="IsIndicatorVisibleProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="IsIndicatorVisibleProperty"/> on.</param>
        /// <param name="isIndicatorVisible">IsIndicatorVisible property value.</param>
        public static void SetIsIndicatorVisible(DependencyObject element, bool isIndicatorVisible)
        {
            element.SetValue(IsIndicatorVisibleProperty, isIndicatorVisible);
        }

        /// <summary>Helper for getting <see cref="IsIndicatorVisibleProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="IsIndicatorVisibleProperty"/> from.</param>
        /// <returns>IsIndicatorVisible property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        public static bool GetIsIndicatorVisible(DependencyObject element)
        {
            return (bool)element.GetValue(IsIndicatorVisibleProperty);
        }

        public static readonly DependencyProperty OpacityProperty = DependencyProperty.RegisterAttached(
            "Opacity", typeof(double), typeof(ButtonProgressAssist), new FrameworkPropertyMetadata(default(double)));

        /// <summary>Helper for setting <see cref="OpacityProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="OpacityProperty"/> on.</param>
        /// <param name="opacity">Opacity property value.</param>
        public static void SetOpacity(DependencyObject element, double opacity)
        {
            element.SetValue(OpacityProperty, opacity);
        }

        /// <summary>Helper for getting <see cref="OpacityProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="OpacityProperty"/> from.</param>
        /// <returns>Opacity property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        public static double GetOpacity(DependencyObject element)
        {
            return (double)element.GetValue(OpacityProperty);
        }
    }
}
