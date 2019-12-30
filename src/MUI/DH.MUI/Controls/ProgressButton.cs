using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace DH.MUI.Controls
{
    [StyleTypedProperty(Property = nameof(ProgressStyle), StyleTargetType = typeof(ProgressBar))]
    public class ProgressButton : ToggleButton
    {
        /// <summary>Identifies the <see cref="ProgressStyle"/> dependency property.</summary>
        public static readonly DependencyProperty ProgressStyleProperty = DependencyProperty.Register(
            nameof(ProgressStyle), typeof(Style), typeof(ProgressButton), new PropertyMetadata(default(Style)));

        public Style ProgressStyle
        {
            get => (Style)GetValue(ProgressStyleProperty);
            set => SetValue(ProgressStyleProperty, value);
        }

        /// <summary>Identifies the <see cref="Progress"/> dependency property.</summary>
        public static readonly DependencyProperty ProgressProperty = DependencyProperty.Register(
            nameof(Progress), typeof(double), typeof(ProgressButton), new PropertyMetadata(0.0));

        public double Progress
        {
            get => (double)GetValue(ProgressProperty);
            set => SetValue(ProgressProperty, value);
        }
    }
}