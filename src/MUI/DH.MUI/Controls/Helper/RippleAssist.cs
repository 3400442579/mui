using System.Windows;
using System.Windows.Media;

namespace DH.MUI.Controls
{
    public static class RippleAssist
    {
        #region ClipToBound

        public static readonly DependencyProperty ClipToBoundsProperty = DependencyProperty.RegisterAttached(
            "ClipToBounds", typeof(bool), typeof(RippleAssist), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>Helper for setting <see cref="ClipToBoundsProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="ClipToBoundsProperty"/> on.</param>
        /// <param name="value">ClipToBounds property value.</param>
        public static void SetClipToBounds(DependencyObject element, bool value)
        {
            element.SetValue(ClipToBoundsProperty, value);
        }

        /// <summary>Helper for getting <see cref="ClipToBoundsProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="ClipToBoundsProperty"/> from.</param>
        /// <returns>ClipToBounds property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        public static bool GetClipToBounds(DependencyObject element)
        {
            return (bool)element.GetValue(ClipToBoundsProperty);
        }

        #endregion

        #region StayOnCenter

        /// <summary>
        /// Set to <c>true</c> to cause the ripple to originate from the centre of the 
        /// content.  Otherwise the effect will originate from the mouse down position.        
        /// </summary>
        public static readonly DependencyProperty IsCenteredProperty = DependencyProperty.RegisterAttached(
            "IsCentered", typeof(bool), typeof(RippleAssist), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));


        /// <summary>Helper for setting <see cref="IsCenteredProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="IsCenteredProperty"/> on.</param>
        /// <param name="value">IsCentered property value.</param>
        public static void SetIsCentered(DependencyObject element, bool value)
        {
            element.SetValue(IsCenteredProperty, value);
        }


        /// <summary>Helper for getting <see cref="IsCenteredProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="IsCenteredProperty"/> from.</param>
        /// <returns>IsCentered property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        public static bool GetIsCentered(DependencyObject element)
        {
            return (bool)element.GetValue(IsCenteredProperty);
        }

        #endregion

        #region IsDisabled

        /// <summary>
        /// Set to <c>True</c> to disable ripple effect
        /// </summary>
        public static readonly DependencyProperty IsDisabledProperty = DependencyProperty.RegisterAttached(
            "IsDisabled", typeof(bool), typeof(RippleAssist), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));



        /// <summary>Helper for setting <see cref="IsDisabledProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="IsDisabledProperty"/> on.</param>
        /// <param name="value">IsDisabled property value.</param>
        public static void SetIsDisabled(DependencyObject element, bool value)
        {
            element.SetValue(IsDisabledProperty, value);
        }


        /// <summary>Helper for getting <see cref="IsDisabledProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="IsDisabledProperty"/> from.</param>
        /// <returns>IsDisabled property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        public static bool GetIsDisabled(DependencyObject element)
        {
            return (bool)element.GetValue(IsDisabledProperty);
        }

        #endregion

        #region RippleSizeMultiplier

        public static readonly DependencyProperty RippleSizeMultiplierProperty = DependencyProperty.RegisterAttached(
            "RippleSizeMultiplier", typeof(double), typeof(RippleAssist), new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>Helper for setting <see cref="RippleSizeMultiplierProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="RippleSizeMultiplierProperty"/> on.</param>
        /// <param name="value">RippleSizeMultiplier property value.</param>
        public static void SetRippleSizeMultiplier(DependencyObject element, double value)
        {
            element.SetValue(RippleSizeMultiplierProperty, value);
        }

        /// <summary>Helper for getting <see cref="RippleSizeMultiplierProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="RippleSizeMultiplierProperty"/> from.</param>
        /// <returns>RippleSizeMultiplier property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        public static double GetRippleSizeMultiplier(DependencyObject element)
        {
            return (double)element.GetValue(RippleSizeMultiplierProperty);
        }

        #endregion

        #region Feedback

        public static readonly DependencyProperty FeedbackProperty = DependencyProperty.RegisterAttached(
            "Feedback", typeof(Brush), typeof(RippleAssist), new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>Helper for setting <see cref="FeedbackProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="FeedbackProperty"/> on.</param>
        /// <param name="value">Feedback property value.</param>
        public static void SetFeedback(DependencyObject element, Brush value)
        {
            element.SetValue(FeedbackProperty, value);
        }

        /// <summary>Helper for getting <see cref="FeedbackProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="FeedbackProperty"/> from.</param>
        /// <returns>Feedback property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        public static Brush GetFeedback(DependencyObject element)
        {
            return (Brush)element.GetValue(FeedbackProperty);
        }

        #endregion

        #region RippleOnTop

        public static readonly DependencyProperty RippleOnTopProperty = DependencyProperty.RegisterAttached(
            "RippleOnTop", typeof(bool), typeof(RippleAssist),
            new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>Helper for setting <see cref="RippleOnTopProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="RippleOnTopProperty"/> on.</param>
        /// <param name="value">RippleOnTop property value.</param>
        public static void SetRippleOnTop(DependencyObject element, bool value)
        {
            element.SetValue(RippleOnTopProperty, value);
        }

        /// <summary>Helper for getting <see cref="RippleOnTopProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="RippleOnTopProperty"/> from.</param>
        /// <returns>RippleOnTop property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        public static bool GetRippleOnTop(DependencyObject element)
        {
            return (bool)element.GetValue(RippleOnTopProperty);
        }

        #endregion

    }
}

