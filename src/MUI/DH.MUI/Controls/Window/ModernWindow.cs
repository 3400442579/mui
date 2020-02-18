using DH.MUI.Core;
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
        private Storyboard backgroundAnimation;

        /// <summary>Identifies the <see cref="BackgroundContent"/> dependency property.</summary>
        public static readonly DependencyProperty BackgroundContentProperty = DependencyProperty.Register(nameof(BackgroundContent), typeof(object), typeof(ModernWindow));

        /// <summary>Identifies the <see cref="TitleBarHeight"/> dependency property.</summary>
        public static readonly DependencyProperty TitleBarHeightProperty = DependencyProperty.Register(nameof(TitleBarHeight), typeof(int), typeof(ModernWindow), new PropertyMetadata(20));
        /// <summary>Identifies the <see cref="TitleAlignment"/> dependency property.</summary>
        public static readonly DependencyProperty TitleAlignmentProperty = DependencyProperty.Register(nameof(TitleAlignment), typeof(HorizontalAlignment), typeof(ModernWindow));
        /// <summary>Identifies the <see cref="ShowTitle"/> dependency property.</summary>
        public static readonly DependencyProperty ShowTitleProperty = DependencyProperty.Register(nameof(ShowTitle), typeof(bool), typeof(ModernWindow), new PropertyMetadata(true));

        /// <summary>Identifies the <see cref="ShowIcon"/> dependency property.</summary>
        public static readonly DependencyProperty ShowIconProperty = DependencyProperty.Register(nameof(ShowIcon), typeof(bool), typeof(ModernWindow), new PropertyMetadata(true));

        /// <summary>Identifies the <see cref="UseNoneWindowStyle"/> dependency property.</summary>
        public static readonly DependencyProperty UseNoneWindowStyleProperty = DependencyProperty.Register("UseNoneWindowStyle", typeof(bool), typeof(ModernWindow), new PropertyMetadata(false));

        //public static readonly DependencyProperty ShowTitleBarProperty = DependencyProperty.Register("ShowTitleBar", typeof(bool), typeof(ModernWindow), new PropertyMetadata(true, OnShowTitleBarPropertyChangedCallback, OnShowTitleBarCoerceValueCallback));


        /// <summary>Identifies the <see cref="LeftWindowCommands"/> dependency property.</summary>
        public static readonly DependencyProperty LeftWindowCommandsProperty = DependencyProperty.Register(nameof(LeftWindowCommands), typeof(WindowCommands), typeof(ModernWindow), new PropertyMetadata(null, UpdateLogicalChilds));
        /// <summary>Identifies the <see cref="RightWindowCommands"/> dependency property.</summary>
        public static readonly DependencyProperty RightWindowCommandsProperty = DependencyProperty.Register(nameof(RightWindowCommands), typeof(WindowCommands), typeof(ModernWindow), new PropertyMetadata(null, UpdateLogicalChilds));

        /// <summary>Identifies the <see cref="ShowMinButton"/> dependency property.</summary>
        public static readonly DependencyProperty ShowMinButtonProperty = DependencyProperty.Register(nameof(ShowMinButton), typeof(bool), typeof(ModernWindow), new PropertyMetadata(true));
        /// <summary>Identifies the <see cref="ShowMaxRestoreButton"/> dependency property.</summary>
        public static readonly DependencyProperty ShowMaxRestoreButtonProperty = DependencyProperty.Register(nameof(ShowMaxRestoreButton), typeof(bool), typeof(ModernWindow), new PropertyMetadata(true));
        /// <summary>Identifies the <see cref="ShowCloseButton"/> dependency property.</summary>
        public static readonly DependencyProperty ShowCloseButtonProperty = DependencyProperty.Register(nameof(ShowCloseButton), typeof(bool), typeof(ModernWindow), new PropertyMetadata(true));


      

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
        /// Gets/sets the TitleBar's height.
        /// </summary>
        public int TitleBarHeight
        {
            get { return (int)GetValue(TitleBarHeightProperty); }
            set { SetValue(TitleBarHeightProperty, value); }
        }

        /// <summary>
        /// Gets/sets the title horizontal alignment.
        /// </summary>
        public HorizontalAlignment TitleAlignment
        {
            get { return (HorizontalAlignment)GetValue(TitleAlignmentProperty); }
            set { SetValue(TitleAlignmentProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the window title is visible in the UI.
        /// </summary>
        public bool ShowTitle
        {
            get { return (bool)GetValue(ShowTitleProperty); }
            set { SetValue(ShowTitleProperty, value); }
        }
        //public bool ShowTitleBar
        //{
        //    get { return (bool)GetValue(ShowTitleBarProperty); }
        //    set { SetValue(ShowTitleBarProperty, value); }
        //}

        public bool UseNoneWindowStyle
        {
            get { return (bool)GetValue(UseNoneWindowStyleProperty); }
            set { SetValue(UseNoneWindowStyleProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool ShowIcon
        {
            get { return (bool)GetValue(ShowIconProperty); }
            set { SetValue(ShowIconProperty, value); }
        }

    

        /// <summary>
        /// Gets or sets whether if the minimize button is visible and the minimize system menu is enabled.
        /// </summary>
        public bool ShowMinButton
        {
            get { return (bool)GetValue(ShowMinButtonProperty); }
            set { SetValue(ShowMinButtonProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether if the maximize/restore button is visible and the maximize/restore system menu is enabled.
        /// </summary>
        public bool ShowMaxRestoreButton
        {
            get { return (bool)GetValue(ShowMaxRestoreButtonProperty); }
            set { SetValue(ShowMaxRestoreButtonProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether if the close button is visible.
        /// </summary>
        public bool ShowCloseButton
        {
            get { return (bool)GetValue(ShowCloseButtonProperty); }
            set { SetValue(ShowCloseButtonProperty, value); }
        }

       
        /// <summary>
        /// Gets/sets the left window commands that hosts the user commands.
        /// </summary>
        public WindowCommands LeftWindowCommands
        {
            get { return (WindowCommands)GetValue(LeftWindowCommandsProperty); }
            set { SetValue(LeftWindowCommandsProperty, value); }
        }


        /// <summary>
        /// Gets/sets the right window commands that hosts the user commands.
        /// </summary>
        public WindowCommands RightWindowCommands
        {
            get { return (WindowCommands)GetValue(RightWindowCommandsProperty); }
            set { SetValue(RightWindowCommandsProperty, value); }
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
