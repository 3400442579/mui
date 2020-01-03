using System.Windows;

namespace DH.MUI.Controls
{
    /// <summary>
    /// Helper properties for working with for make round corner.
    /// </summary>
    public static class ButtonAssist
    {
        /// <summary>
        /// Controls the corner radius of the surrounding box.
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.RegisterAttached(
            "CornerRadius", typeof(CornerRadius), typeof(ButtonAssist), new PropertyMetadata(new CornerRadius(2.0)));

        /// <summary>Helper for setting <see cref="CornerRadiusProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="CornerRadiusProperty"/> on.</param>
        /// <param name="value">CornerRadius property value.</param>
        public static void SetCornerRadius(DependencyObject element, CornerRadius value)
        {
            element.SetValue(CornerRadiusProperty, value);
        }

        /// <summary>Helper for getting <see cref="CornerRadiusProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="CornerRadiusProperty"/> from.</param>
        /// <returns>CornerRadius property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        public static CornerRadius GetCornerRadius(DependencyObject element)
        {
            return (CornerRadius)element.GetValue(CornerRadiusProperty);
        }
    }
}
