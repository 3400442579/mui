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
        /// <summary>Identifies the <see cref="EllipseDiameter"/> dependency property.</summary>
        public static readonly DependencyProperty EllipseDiameterProperty = DependencyProperty.Register(nameof(EllipseDiameter), typeof(double), typeof(ModernButton), new PropertyMetadata(22D));

        /// <summary>Identifies the <see cref="EllipseStrokeThickness"/> dependency property.</summary>
        public static readonly DependencyProperty EllipseStrokeThicknessProperty = DependencyProperty.Register(nameof(EllipseStrokeThickness), typeof(double), typeof(ModernButton), new PropertyMetadata(1D));

        /// <summary>Identifies the <see cref="IconData"/> dependency property.</summary>
        public static readonly DependencyProperty IconDataProperty = DependencyProperty.Register(nameof(IconData), typeof(Geometry), typeof(ModernButton));

        /// <summary>Identifies the <see cref="IconHeight"/> dependency property.</summary>
        public static readonly DependencyProperty IconHeightProperty = DependencyProperty.Register(nameof(IconHeight), typeof(double), typeof(ModernButton), new PropertyMetadata(12D));

        /// <summary>Identifies the <see cref="IconWidth"/> dependency property.</summary>
        public static readonly DependencyProperty IconWidthProperty = DependencyProperty.Register(nameof(IconWidth), typeof(double), typeof(ModernButton), new PropertyMetadata(12D));

        /// <summary>
        /// Initializes a new instance of the <see cref="ModernButton"/> class.
        /// </summary>
        public ModernButton()
        {
            this.DefaultStyleKey = typeof(ModernButton);
        }

        /// <summary>
        /// Gets or sets the ellipse diameter.
        /// </summary>
        public double EllipseDiameter
        {
            get { return (double)GetValue(EllipseDiameterProperty); }
            set { SetValue(EllipseDiameterProperty, value); }
        }

        /// <summary>
        /// Gets or sets the ellipse stroke thickness.
        /// </summary>
        public double EllipseStrokeThickness
        {
            get { return (double)GetValue(EllipseStrokeThicknessProperty); }
            set { SetValue(EllipseStrokeThicknessProperty, value); }
        }

        /// <summary>
        /// Gets or sets the icon path data.
        /// </summary>
        /// <value>
        /// The icon path data.
        /// </value>
        public Geometry IconData
        {
            get { return (Geometry)GetValue(IconDataProperty); }
            set { SetValue(IconDataProperty, value); }
        }

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

        /// <summary>
        /// Gets or sets the icon width.
        /// </summary>
        /// <value>
        /// The icon width.
        /// </value>
        public double IconWidth
        {
            get { return (double)GetValue(IconWidthProperty); }
            set { SetValue(IconWidthProperty, value); }
        }
    }
}
