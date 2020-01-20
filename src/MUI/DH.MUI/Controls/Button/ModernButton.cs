using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DH.MUI.Controls
{
    /// <summary>
    /// Adds icon content to a standard button.
    /// </summary>
    public class ModernButton : Button
    {
      
        /// <summary>
        /// Initializes a new instance of the <see cref="ModernButton"/> class.
        /// </summary>
        public ModernButton()
        {
            this.DefaultStyleKey = typeof(ModernButton);
        }

        /// <summary>Identifies the <see cref="Icon"/> dependency property.</summary>
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(nameof(Icon), typeof(PackIcon), typeof(ModernButton));

        /// <summary>
        /// Gets or sets the icon path data.
        /// </summary>
        /// <value>
        /// The icon path data.
        /// </value>
        public PackIcon Icon
        {
            get { return (PackIcon)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        /// <summary>Identifies the <see cref="IconHeight"/> dependency property.</summary>
        public static readonly DependencyProperty IconHeightProperty = DependencyProperty.Register(nameof(IconHeight), typeof(double), typeof(ModernButton), new PropertyMetadata(12D));

        /// <summary>
        /// Gets or sets the icon height.
        /// </summary>
        /// <value>
        /// The icon height.
        /// </value>
        public double IconHeight
        {
            get { return (double)GetValue(IconHeightProperty); }
            set { SetValue(IconHeightProperty, value); }
        }




        /// <summary>Identifies the <see cref="IconWidth"/> dependency property.</summary>
        public static readonly DependencyProperty IconWidthProperty = DependencyProperty.Register(nameof(IconWidth), typeof(double), typeof(ModernButton), new PropertyMetadata(12D));

        public double IconWidth
        {
            get { return (double)GetValue(IconWidthProperty); }
            set { SetValue(IconWidthProperty, value); }
        }


        /// <summary>Identifies the <see cref="Orientation"/> dependency property.</summary>
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(  nameof(Orientation),  typeof(System.Windows.Controls.Orientation), typeof(ModernButton),
                new FrameworkPropertyMetadata(System.Windows.Controls.Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public System.Windows.Controls.Orientation Orientation
        {
            get { return (System.Windows.Controls.Orientation)this.GetValue(OrientationProperty); }
            set { this.SetValue(OrientationProperty, value); }
        }
    }
}
