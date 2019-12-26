using System.Windows;
using System.Windows.Controls;

namespace DH.MUI.Controls
{
    public class ContentControlEx : ContentControl
    {
        
        /// <summary>Identifies the <see cref="ContentCharacterCasing"/> dependency property.</summary>
        public static readonly DependencyProperty ContentCharacterCasingProperty =
            DependencyProperty.Register(nameof(ContentCharacterCasing),
                                        typeof(CharacterCasing),
                                        typeof(ContentControlEx),
                                        new FrameworkPropertyMetadata(CharacterCasing.Normal, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure),
                                        value => CharacterCasing.Normal <= (CharacterCasing)value && (CharacterCasing)value <= CharacterCasing.Upper);

        /// <summary> 
        /// The DependencyProperty for the CharacterCasing property.
        /// Controls whether or not content is converted to upper or lower case
        /// Default Value: CharacterCasing.Normal
        /// </summary> 
        public CharacterCasing ContentCharacterCasing
        {
            get { return (CharacterCasing)GetValue(ContentCharacterCasingProperty); }
            set { SetValue(ContentCharacterCasingProperty, value); }
        }

        /// <summary>Identifies the <see cref="RecognizesAccessKey"/> dependency property.</summary> 
        public static readonly DependencyProperty RecognizesAccessKeyProperty =
            DependencyProperty.Register(nameof(RecognizesAccessKey),
                                        typeof(bool),
                                        typeof(ContentControlEx),
                                        new FrameworkPropertyMetadata(false));

        /// <summary> 
        /// Determine if the inner ContentPresenter should use AccessText in its style
        /// </summary> 
        public bool RecognizesAccessKey
        {
            get { return (bool)GetValue(RecognizesAccessKeyProperty); }
            set { SetValue(RecognizesAccessKeyProperty, value); }
        }

        static ContentControlEx()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ContentControlEx), new FrameworkPropertyMetadata(typeof(ContentControlEx)));
        }
    }
}