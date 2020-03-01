using System.Windows;

namespace Ant.Wpf.Controls
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

        /// <summary>
        /// Controls the Shape :Circle.
        /// </summary>
        public static readonly DependencyProperty ShapeProperty = DependencyProperty.RegisterAttached(
            "Shape", typeof(string), typeof(ButtonAssist), new PropertyMetadata(""));

        /// <summary>Helper for setting <see cref="ShapeProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="ShapeProperty"/> on.</param>
        /// <param name="value">CornerRadius property value.</param>
        public static void SetShape(DependencyObject element, string value)
        {
            element.SetValue(ShapeProperty, value);
        }

        /// <summary>Helper for getting <see cref="ShapeProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="ShapeProperty"/> from.</param>
        /// <returns>CornerRadius property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        public static string GetShape(DependencyObject element)
        {
            return (string)element.GetValue(ShapeProperty);
        }

        /// <summary>
        /// Controls the Shape :Circle.
        /// </summary>
        public static readonly DependencyProperty IconProperty = DependencyProperty.RegisterAttached(
            "Icon", typeof(string), typeof(ButtonAssist), new PropertyMetadata(""));

        /// <summary>Helper for setting <see cref="IconProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="IconProperty"/> on.</param>
        /// <param name="value">CornerRadius property value.</param>
        public static void SetIcon(DependencyObject element, string value)
        {
            element.SetValue(IconProperty, value);
        }

        /// <summary>Helper for getting <see cref="IconProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="IconProperty"/> from.</param>
        /// <returns>CornerRadius property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        public static string GetIcon(DependencyObject element)
        {
            return (string)element.GetValue(IconProperty);
        }


        /// <summary>
        /// Controls the Shape :Circle.
        /// </summary>
        public static readonly DependencyProperty SizeProperty = DependencyProperty.RegisterAttached(
            "Size", typeof(string), typeof(ButtonAssist), new PropertyMetadata(""));

        /// <summary>Helper for setting <see cref="IconProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="IconProperty"/> on.</param>
        /// <param name="value">CornerRadius property value.</param>
        public static void SetSize(DependencyObject element, string value)
        {
            element.SetValue(SizeProperty, value);
        }

        /// <summary>Helper for getting <see cref="SizeProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="SizeProperty"/> from.</param>
        /// <returns>CornerRadius property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        public static string GetSize(DependencyObject element)
        {
            return (string)element.GetValue(SizeProperty);
        }



        /// <summary>
        /// 危险按钮
        /// </summary>
        public static readonly DependencyProperty DangerProperty = DependencyProperty.RegisterAttached(
            "Danger", typeof(bool), typeof(ButtonAssist), new PropertyMetadata(false));

        /// <summary>Helper for setting <see cref="DangerProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="DangerProperty"/> on.</param>
        /// <param name="value">CornerRadius property value.</param>
        public static void SetDanger(DependencyObject element, bool value)
        {
            element.SetValue(DangerProperty, value);
        }

        /// <summary>Helper for getting <see cref="DangerProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="DangerProperty"/> from.</param>
        /// <returns>CornerRadius property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        public static bool GetDanger(DependencyObject element)
        {
            return (bool)element.GetValue(DangerProperty);
        }

    }
}
