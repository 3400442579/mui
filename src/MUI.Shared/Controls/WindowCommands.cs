using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ControlzEx;

namespace DH.MUI.Controls
{
    [StyleTypedProperty(Property = nameof(ItemContainerStyle), StyleTargetType = typeof(WindowCommands))]
    public class WindowCommands : ToolBar
    {
        /// <summary>Identifies the <see cref="ShowSeparators"/> dependency property.</summary>
        public static readonly DependencyProperty ShowSeparatorsProperty = DependencyProperty.Register(
                nameof(ShowSeparators),
                typeof(bool),
                typeof(WindowCommands),
                new FrameworkPropertyMetadata(true,
                                              FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                                              OnShowSeparatorsChanged));

        private static void OnShowSeparatorsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                ((WindowCommands)d).ResetSeparators();
            }
        }

        /// <summary>
        /// Gets or sets the value indicating whether to show the separators or not.
        /// </summary>
        public bool ShowSeparators
        {
            get { return (bool)this.GetValue(ShowSeparatorsProperty); }
            set { this.SetValue(ShowSeparatorsProperty, value); }
        }

        /// <summary>Identifies the <see cref="ShowLastSeparator"/> dependency property.</summary>
        public static readonly DependencyProperty ShowLastSeparatorProperty = DependencyProperty.Register(
                nameof(ShowLastSeparator),
                typeof(bool),
                typeof(WindowCommands),
                new FrameworkPropertyMetadata(true,
                                              FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                                              OnShowLastSeparatorChanged));

        private static void OnShowLastSeparatorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                ((WindowCommands)d).ResetSeparators(false);
            }
        }

        /// <summary>
        /// Gets or sets the value indicating whether to show the last separator or not.
        /// </summary>
        public bool ShowLastSeparator
        {
            get { return (bool)this.GetValue(ShowLastSeparatorProperty); }
            set { this.SetValue(ShowLastSeparatorProperty, value); }
        }

        /// <summary>Identifies the <see cref="SeparatorHeight"/> dependency property.</summary>
        public static readonly DependencyProperty SeparatorHeightProperty = DependencyProperty.Register(
                nameof(SeparatorHeight),
                typeof(double),
                typeof(WindowCommands),
                new FrameworkPropertyMetadata(15d, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets the value indicating the height of the separators.
        /// </summary>
        [TypeConverter(typeof(LengthConverter))]
        public double SeparatorHeight
        {
            get { return (double)this.GetValue(SeparatorHeightProperty); }
            set { this.SetValue(SeparatorHeightProperty, value); }
        }

        /// <summary>Identifies the <see cref="ParentWindow"/> dependency property.</summary>
        public static readonly DependencyPropertyKey ParentWindowPropertyKey =  DependencyProperty.RegisterReadOnly(
                nameof(ParentWindow),
                typeof(Window),
                typeof(WindowCommands),
                new PropertyMetadata(null));

        /// <summary>Identifies the <see cref="ParentWindow"/> dependency property.</summary>
        public static readonly DependencyProperty ParentWindowProperty = ParentWindowPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the window.
        /// </summary>
        public Window ParentWindow
        {
            get { return (Window)this.GetValue(ParentWindowProperty); }
            private set { this.SetValue(ParentWindowPropertyKey, value); }
        }

        static WindowCommands()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowCommands), new FrameworkPropertyMetadata(typeof(WindowCommands)));
        }

        public WindowCommands()
        {
            this.Loaded += this.WindowCommandsLoaded;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new WindowCommandsItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is WindowCommandsItem;
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            if (!(element is WindowCommandsItem windowCommandsItem))
            {
                return;
            }

            if (!(item is FrameworkElement frameworkElement))
            {
                windowCommandsItem.ApplyTemplate();
                frameworkElement = windowCommandsItem.ContentTemplate?.LoadContent() as FrameworkElement;
            }

            frameworkElement?.SetBinding(ControlsHelper.ContentCharacterCasingProperty,
                                         new Binding { Source = this, Path = new PropertyPath(ControlsHelper.ContentCharacterCasingProperty) });

            this.AttachVisibilityHandler(windowCommandsItem, item as UIElement);
            this.ResetSeparators();
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);

            if (item is FrameworkElement frameworkElement)
            {
                BindingOperations.ClearBinding(frameworkElement, ControlsHelper.ContentCharacterCasingProperty);
            }

            this.DetachVisibilityHandler(element as WindowCommandsItem);
            this.ResetSeparators(false);
        }

        private void AttachVisibilityHandler(WindowCommandsItem container, UIElement item)
        {
            if (container != null)
            {
                if (null == item)
                {
                    // if item is not a UIElement then maybe the ItemsSource binds to a collection of objects
                    // and an ItemTemplate is set, so let's try to solve this
                    container.ApplyTemplate();
                    if (!(container.ContentTemplate?.LoadContent() is UIElement content))
                    {
                        // no UIElement was found, so don't show this container
                        container.SetCurrentValue(VisibilityProperty, Visibility.Collapsed);
                    }

                    return;
                }

                container.SetCurrentValue(VisibilityProperty, item.Visibility);
                var isVisibilityNotifier = new PropertyChangeNotifier(item, VisibilityProperty);
                isVisibilityNotifier.ValueChanged += this.VisibilityPropertyChanged;
                container.VisibilityPropertyChangeNotifier = isVisibilityNotifier;
            }
        }

        private void DetachVisibilityHandler(WindowCommandsItem container)
        {
            if (container != null)
            {
                container.VisibilityPropertyChangeNotifier = null;
            }
        }

        private void VisibilityPropertyChanged(object sender, EventArgs e)
        {
            if (sender is UIElement item)
            {
                var container = this.GetWindowCommandsItem(item);
                if (container != null)
                {
                    container.Visibility = item.Visibility;
                    this.ResetSeparators();
                }
            }
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            this.ResetSeparators();
        }

        private void ResetSeparators(bool reset = true)
        {
            if (this.Items.Count == 0)
            {
                return;
            }

            var windowCommandsItems = this.GetWindowCommandsItems().ToList();

            if (reset)
            {
                foreach (var windowCommandsItem in windowCommandsItems)
                {
                    windowCommandsItem.IsSeparatorVisible = this.ShowSeparators;
                }
            }

            var lastContainer = windowCommandsItems.LastOrDefault(i => i.IsVisible);
            if (lastContainer != null)
            {
                lastContainer.IsSeparatorVisible = this.ShowSeparators && this.ShowLastSeparator;
            }
        }

        private WindowCommandsItem GetWindowCommandsItem(object item)
        {
            if (item is WindowCommandsItem windowCommandsItem)
            {
                return windowCommandsItem;
            }

            return (WindowCommandsItem)this.ItemContainerGenerator.ContainerFromItem(item);
        }

        private IEnumerable<WindowCommandsItem> GetWindowCommandsItems()
        {
            foreach (var item in this.Items)
            {
                var windowCommandsItem = this.GetWindowCommandsItem(item);
                if (windowCommandsItem != null)
                {
                    yield return windowCommandsItem;
                }
            }
        }

        private void WindowCommandsLoaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= this.WindowCommandsLoaded;

            var contentPresenter = this.TryFindParent<ContentPresenter>();
            if (contentPresenter != null)
            {
                this.SetCurrentValue(DockPanel.DockProperty, contentPresenter.GetValue(DockPanel.DockProperty));
            }

            if (null == this.ParentWindow)
            {
                var window = this.TryFindParent<Window>();
                this.SetValue(ParentWindowPropertyKey, window);
            }
        }
    }
}