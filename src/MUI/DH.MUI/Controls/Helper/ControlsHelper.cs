using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace DH.MUI.Controls
{
    /// <summary>
    /// A helper class that provides various controls.
    /// </summary>
    public static class ControlsHelper
    {
        public static readonly DependencyProperty DisabledVisualElementVisibilityProperty = DependencyProperty.RegisterAttached("DisabledVisualElementVisibility", typeof(Visibility), typeof(ControlsHelper), new FrameworkPropertyMetadata(Visibility.Visible, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets the value to handle the visibility of the DisabledVisualElement in the template.
        /// </summary>
        [Category(AppName.MuiApps)]
        [AttachedPropertyBrowsableForType(typeof(TextBoxBase))]
        [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
        //[AttachedPropertyBrowsableForType(typeof(NumericUpDown))]
        public static Visibility GetDisabledVisualElementVisibility(UIElement element)
        {
            return (Visibility)element?.GetValue(DisabledVisualElementVisibilityProperty);
        }

        /// <summary>
        /// Sets the value to handle the visibility of the DisabledVisualElement in the template.
        /// </summary>
        /// <param name="element"><see cref="UIElement"/> to set <see cref="DisabledVisualElementVisibilityProperty"/> on.</param>
        public static void SetDisabledVisualElementVisibility(UIElement element, Visibility value)
        {
            element?.SetCurrentValue(DisabledVisualElementVisibilityProperty, value);
        }

        /// <summary>
        /// The DependencyProperty for the CharacterCasing property.
        /// Controls whether or not content is converted to upper or lower case
        /// </summary>
        public static readonly DependencyProperty ContentCharacterCasingProperty = DependencyProperty.RegisterAttached(
                "ContentCharacterCasing",
                typeof(CharacterCasing),
                typeof(ControlsHelper),
                new FrameworkPropertyMetadata(CharacterCasing.Normal, FrameworkPropertyMetadataOptions.AffectsMeasure),
                new ValidateValueCallback(value => CharacterCasing.Normal <= (CharacterCasing)value && (CharacterCasing)value <= CharacterCasing.Upper));

        /// <summary>
        /// Gets the character casing of the control
        /// </summary>
        [Category(AppName.MuiApps)]
        [AttachedPropertyBrowsableForType(typeof(ContentControl))]
        //[AttachedPropertyBrowsableForType(typeof(DropDownButton))]
        //[AttachedPropertyBrowsableForType(typeof(SplitButton))]
        [AttachedPropertyBrowsableForType(typeof(WindowCommands))]
        public static CharacterCasing GetContentCharacterCasing(UIElement element)
        {
            return (CharacterCasing)element?.GetValue(ContentCharacterCasingProperty);
        }

        /// <summary>
        /// Sets the character casing of the control
        /// </summary>
        public static void SetContentCharacterCasing(UIElement element, CharacterCasing value)
        {
            element?.SetCurrentValue(ContentCharacterCasingProperty, value);
        }



        public static readonly DependencyProperty FocusBorderBrushProperty = DependencyProperty.RegisterAttached("FocusBorderBrush",
                                                  typeof(Brush),
                                                  typeof(ControlsHelper),
                                                  new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the brush used to draw the focus border.
        /// </summary>
        [Category(AppName.MuiApps)]
        [AttachedPropertyBrowsableForType(typeof(TextBox))]
        [AttachedPropertyBrowsableForType(typeof(DatePicker))]
        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        [AttachedPropertyBrowsableForType(typeof(ButtonBase))]
        public static Brush GetFocusBorderBrush(DependencyObject element)
        {
            return (Brush)element?.GetValue(FocusBorderBrushProperty);
        }

        /// <summary>
        /// Sets the brush used to draw the focus border.
        /// </summary>
        [Category(AppName.MuiApps)]
        [AttachedPropertyBrowsableForType(typeof(TextBox))]
        [AttachedPropertyBrowsableForType(typeof(DatePicker))]
        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        [AttachedPropertyBrowsableForType(typeof(ButtonBase))]
        public static void SetFocusBorderBrush(DependencyObject element, Brush value)
        {
            element?.SetCurrentValue(FocusBorderBrushProperty, value);
        }

        public static readonly DependencyProperty FocusBorderThicknessProperty
            = DependencyProperty.RegisterAttached("FocusBorderThickness",
                                                  typeof(Thickness),
                                                  typeof(ControlsHelper),
                                                  new FrameworkPropertyMetadata(default(Thickness), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the brush used to draw the focus border.
        /// </summary>
        [Category(AppName.MuiApps)]
        [AttachedPropertyBrowsableForType(typeof(TextBox))]
        [AttachedPropertyBrowsableForType(typeof(DatePicker))]
        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        [AttachedPropertyBrowsableForType(typeof(ButtonBase))]
        public static Thickness GetFocusBorderThickness(DependencyObject element)
        {
            return (Thickness)element?.GetValue(FocusBorderThicknessProperty);
        }

        /// <summary>
        /// Sets the brush used to draw the focus border.
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject"/> to set <see cref="FocusBorderThicknessProperty"/> on.</param>
        [Category(AppName.MuiApps)]
        [AttachedPropertyBrowsableForType(typeof(TextBox))]
        [AttachedPropertyBrowsableForType(typeof(DatePicker))]
        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        [AttachedPropertyBrowsableForType(typeof(ButtonBase))]
        public static void SetFocusBorderThickness(DependencyObject element, Thickness value)
        {
            element?.SetCurrentValue(FocusBorderThicknessProperty, value);
        }

        public static readonly DependencyProperty MouseOverBorderBrushProperty
            = DependencyProperty.RegisterAttached("MouseOverBorderBrush",
                                                  typeof(Brush),
                                                  typeof(ControlsHelper),
                                                  new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the brush used to draw the mouse over brush.
        /// </summary>
        [Category(AppName.MuiApps)]
        [AttachedPropertyBrowsableForType(typeof(TextBox))]
        [AttachedPropertyBrowsableForType(typeof(CheckBox))]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        [AttachedPropertyBrowsableForType(typeof(DatePicker))]
        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        //[AttachedPropertyBrowsableForType(typeof(Tile))]
        public static Brush GetMouseOverBorderBrush(DependencyObject element)
        {
            return (Brush)element?.GetValue(MouseOverBorderBrushProperty);
        }


        [Category(AppName.MuiApps)]
        [AttachedPropertyBrowsableForType(typeof(TextBox))]
        [AttachedPropertyBrowsableForType(typeof(CheckBox))]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        [AttachedPropertyBrowsableForType(typeof(DatePicker))]
        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        //[AttachedPropertyBrowsableForType(typeof(Tile))]
        public static void SetMouseOverBorderBrush(DependencyObject element, Brush value)
        {
            element?.SetCurrentValue(MouseOverBorderBrushProperty, value);
        }


        public static readonly DependencyProperty CornerRadiusProperty
            = DependencyProperty.RegisterAttached("CornerRadius", typeof(CornerRadius), typeof(ControlsHelper),
                                                  new FrameworkPropertyMetadata(
                                                  new CornerRadius(),
                                                  FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary> 
        /// The CornerRadius property allows users to control the roundness of the button corners independently by 
        /// setting a radius value for each corner. Radius values that are too large are scaled so that they
        /// smoothly blend from corner to corner. (Can be used e.g. at MetroButton style)
        /// Description taken from original Microsoft description :-D
        /// </summary>
        [Category(AppName.MuiApps)]
        public static CornerRadius GetCornerRadius(UIElement element)
        {
            return (CornerRadius)element?.GetValue(CornerRadiusProperty);
        }

        public static void SetCornerRadius(UIElement element, CornerRadius value)
        {
            element?.SetCurrentValue(CornerRadiusProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the child contents of the control are not editable.
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.RegisterAttached("IsReadOnly",
                                                  typeof(bool),
                                                  typeof(ControlsHelper),
                                                  new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Gets a value indicating whether the child contents of the control are not editable.
        /// </summary>
        /// <returns>IsReadOnly property value.</returns>
        public static bool GetIsReadOnly(UIElement element)
        {
            return (bool)element?.GetValue(IsReadOnlyProperty);
        }

        /// <summary>
        /// Sets a value indicating whether the child contents of the control are not editable.
        /// </summary>
        public static void SetIsReadOnly(UIElement element, bool value)
        {
            element?.SetCurrentValue(IsReadOnlyProperty, value);
        }
    }
}