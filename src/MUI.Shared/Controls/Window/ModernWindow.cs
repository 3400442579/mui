using DH.MUI.Presentation;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace DH.MUI.Controls
{
    [TemplatePart(Name = "WindowBorder", Type = typeof(Border))]
    /// <summary>
    /// Represents a Modern UI styled window.
    /// </summary>
    public class ModernWindow : DpiAwareWindow
    {
        /// <summary>Identifies the <see cref="BackgroundContent"/> dependency property.</summary>
        public static readonly DependencyProperty BackgroundContentProperty = DependencyProperty.Register(nameof(BackgroundContent), typeof(object), typeof(ModernWindow));
        /// <summary>Identifies the <see cref="IsTitleVisible"/> dependency property.</summary>
        public static readonly DependencyProperty IsTitleVisibleProperty = DependencyProperty.Register(nameof(IsTitleVisible), typeof(bool), typeof(ModernWindow), new PropertyMetadata(false));

        public static readonly DependencyProperty LeftWindowCommandsProperty = DependencyProperty.Register("LeftWindowCommands", typeof(WindowCommands), typeof(ModernWindow), new PropertyMetadata(null, UpdateLogicalChilds));
        public static readonly DependencyProperty RightWindowCommandsProperty = DependencyProperty.Register("RightWindowCommands", typeof(WindowCommands), typeof(ModernWindow), new PropertyMetadata(null, UpdateLogicalChilds));
       

        private Storyboard backgroundAnimation;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModernWindow"/> class.
        /// </summary>
        public ModernWindow()
        {
            this.DefaultStyleKey = typeof(ModernWindow);

            // associate window commands with this instance
            this.CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, OnCloseWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, OnMaximizeWindow, OnCanResizeWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, OnMinimizeWindow, OnCanMinimizeWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, OnRestoreWindow, OnCanResizeWindow));

            // listen for theme changes
            AppearanceManager.Current.PropertyChanged += OnAppearanceManagerPropertyChanged;
        }

      

        /// <summary>
        /// Gets or sets the background content of this window instance.
        /// </summary>
        public object BackgroundContent
        {
            get { return GetValue(BackgroundContentProperty); }
            set { SetValue(BackgroundContentProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the window title is visible in the UI.
        /// </summary>
        public bool IsTitleVisible
        {
            get { return (bool)GetValue(IsTitleVisibleProperty); }
            set { SetValue(IsTitleVisibleProperty, value); }
        }
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call System.Windows.FrameworkElement.ApplyTemplate().
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // retrieve BackgroundAnimation storyboard
            if (GetTemplateChild("WindowBorder") is Border border)
            {
                this.backgroundAnimation = border.Resources["BackgroundAnimation"] as Storyboard;

                if (this.backgroundAnimation != null)
                {
                    this.backgroundAnimation.Begin();
                }
            }
        }

        private static void UpdateLogicalChilds(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (!(dependencyObject is ModernWindow window))
            {
                return;
            }
            if (e.OldValue is FrameworkElement oldChild)
            {
                window.RemoveLogicalChild(oldChild);
            }
            if (e.NewValue is FrameworkElement newChild)
            {
                window.AddLogicalChild(newChild);
                newChild.DataContext = window.DataContext;
            }
        }


        /// <summary>
        /// Raises the System.Windows.Window.Closed event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // detach event handler
            AppearanceManager.Current.PropertyChanged -= OnAppearanceManagerPropertyChanged;
        }

        private void OnAppearanceManagerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // start background animation if theme has changed
            if (e.PropertyName == "ThemeSource" && this.backgroundAnimation != null) {
                this.backgroundAnimation.Begin();
            }
        }

        private void OnCanResizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ResizeMode == ResizeMode.CanResize || this.ResizeMode == ResizeMode.CanResizeWithGrip;
        }

        private void OnCanMinimizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ResizeMode != ResizeMode.NoResize;
        }

        private void OnCloseWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void OnMaximizeWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(this);
        }

        private void OnMinimizeWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void OnRestoreWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(this);
        }


    }
}
