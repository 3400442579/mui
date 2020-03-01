﻿using Ant.Wpf.Core;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Ant.Wpf.Controls
{
    /// <summary>
    /// Represents a Modern UI styled dialog window.
    /// </summary>
    public class ModernDialog : DpiAwareWindow
    {
        /// <summary>
        /// Identifies the BackgroundContent dependency property.
        /// </summary>
        public static readonly DependencyProperty BackgroundContentProperty = DependencyProperty.Register("BackgroundContent", typeof(object), typeof(ModernDialog));
        /// <summary>
        /// Identifies the Buttons dependency property.
        /// </summary>
        public static readonly DependencyProperty ButtonsProperty = DependencyProperty.Register("Buttons", typeof(IEnumerable<Button>), typeof(ModernDialog));
        private Button okButton;
        private Button cancelButton;
        private Button yesButton;
        private Button noButton;
        private Button closeButton;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModernDialog"/> class.
        /// </summary>
        public ModernDialog()
        {
            this.DefaultStyleKey = typeof(ModernDialog);
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            this.CloseCommand = new RelayCommand(o => {
                var result = o as MessageBoxResult?;
                if (result.HasValue) {
                    this.MessageBoxResult = result.Value;

                    // sets the Window.DialogResult as well
                    if (result.Value == MessageBoxResult.OK || result.Value == MessageBoxResult.Yes) {
                        this.DialogResult = true;
                    }
                    else if (result.Value == MessageBoxResult.Cancel || result.Value == MessageBoxResult.No){
                        this.DialogResult = false;
                    }
                    else{
                        this.DialogResult = null;
                    }
                }
                Close();
            });

            this.Buttons = new Button[] { this.CloseButton };

            // set the default owner to the app main window (if possible)
            if (Application.Current != null && Application.Current.MainWindow != this) {
                this.Owner = Application.Current.MainWindow;
            }
        }

        private Button CreateCloseDialogButton(string content, bool isDefault, bool isCancel, MessageBoxResult result)
        {
            return new Button {
                Content = content,
                Command = this.CloseCommand,
                CommandParameter = result,
                IsDefault = isDefault,
                IsCancel = isCancel,
                MinHeight = 21,
                MinWidth = 65,
                Margin = new Thickness(4, 0, 0, 0)
            };
        }

        /// <summary>
        /// Gets the close window command.
        /// </summary>
        public ICommand CloseCommand { get; }

        public Button CreateButton(MessageBoxResult messageBoxResult, string content, bool isDefault = false, bool isCancel = false)
        {
            return CreateCloseDialogButton(content, isDefault, isCancel, messageBoxResult);
        }

        /// <summary>
        /// Gets the Ok button.
        /// </summary>
        public Button OkButton
        {
            get
            {
                if (this.okButton == null) {
                    this.okButton = CreateCloseDialogButton("确定", true, false, MessageBoxResult.OK);
                }
                return this.okButton;
            }
        }

        /// <summary>
        /// Gets the Cancel button.
        /// </summary>
        public Button CancelButton
        {
            get
            {
                if (this.cancelButton == null) {
                    this.cancelButton = CreateCloseDialogButton("取消", false, true, MessageBoxResult.Cancel);
                }
                return this.cancelButton;
            }
        }

        /// <summary>
        /// Gets the Yes button.
        /// </summary>
        public Button YesButton
        {
            get
            {
                if (this.yesButton == null) {
                    this.yesButton = CreateCloseDialogButton("是", true, false, MessageBoxResult.Yes);
                }
                return this.yesButton;
            }
        }

        /// <summary>
        /// Gets the No button.
        /// </summary>
        public Button NoButton
        {
            get
            {
                if (this.noButton == null) {
                    this.noButton = CreateCloseDialogButton("否", false, true, MessageBoxResult.No);
                }
                return this.noButton;
            }
        }

        /// <summary>
        /// Gets the Close button.
        /// </summary>
        public Button CloseButton
        {
            get
            {
                if (this.closeButton == null) {
                    this.closeButton = CreateCloseDialogButton("Close", true, false, MessageBoxResult.None);
                }
                return this.closeButton;
            }
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
        /// Gets or sets the dialog buttons.
        /// </summary>
        public IEnumerable<Button> Buttons
        {
            get { return (IEnumerable<Button>)GetValue(ButtonsProperty); }
            set { SetValue(ButtonsProperty, value); }
        }

        /// <summary>
        /// Gets the message box result.
        /// </summary>
        /// <value>
        /// The message box result.
        /// </value>
        public MessageBoxResult MessageBoxResult { get; private set; } = MessageBoxResult.None;

        /// <summary>
        /// Displays a messagebox.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="title">The title.</param>
        /// <param name="button">The button.</param>
        /// <param name="owner">The window owning the messagebox. The messagebox will be located at the center of the owner.</param>
        /// <returns></returns>
        public static MessageBoxResult ShowMessage(string text, string title, MessageBoxButton button,Dictionary<string,string> buttonTexts=null, Window owner = null)
        {
            var dlg = new ModernDialog {
                Title = title,
                Content = text,// new BBCodeBlock { BBCode = text, Margin = new Thickness(0, 0, 0, 8) },
                MinHeight = 0,
                MinWidth = 0,
                MaxHeight = 480,
                MaxWidth = 640,
            };
            if (owner != null) {
                dlg.Owner = owner;
            }

            dlg.Buttons = GetButtons(dlg, button, buttonTexts);

            dlg.ShowDialog();
            return dlg.MessageBoxResult;
        }

        private static IEnumerable<Button> GetButtons(ModernDialog owner, MessageBoxButton button,Dictionary<string,string> buttontexts=null)
        {
            if (button == MessageBoxButton.OK)
            {
                if (buttontexts != null && buttontexts.ContainsKey("Ok"))
                    owner.OkButton.Content = buttontexts["Ok"];
                yield return owner.OkButton;
            }
            else if (button == MessageBoxButton.OKCancel)
            {
                if (buttontexts != null)
                {
                    if (buttontexts.ContainsKey("Ok"))
                        owner.OkButton.Content = buttontexts["Ok"];
                    if (buttontexts.ContainsKey("Cancel"))
                        owner.CancelButton.Content = buttontexts["Cancel"];
                }
                yield return owner.OkButton;
                yield return owner.CancelButton;
            }
            else if (button == MessageBoxButton.YesNo)
            {
                if (buttontexts != null)
                {
                    if (buttontexts.ContainsKey("Yes"))
                        owner.YesButton.Content = buttontexts["Yes"];
                    if (buttontexts.ContainsKey("No"))
                        owner.NoButton.Content = buttontexts["No"];
                }
                yield return owner.YesButton;
                yield return owner.NoButton;
            }
            else if (button == MessageBoxButton.YesNoCancel)
            {
                if (buttontexts != null)
                {
                    if (buttontexts.ContainsKey("Yes"))
                        owner.YesButton.Content = buttontexts["Yes"];
                    if (buttontexts.ContainsKey("No"))
                        owner.NoButton.Content = buttontexts["No"];
                    if (buttontexts.ContainsKey("Cancel"))
                        owner.CancelButton.Content = buttontexts["Cancel"];
                }
                    yield return owner.YesButton;
                yield return owner.NoButton;
                yield return owner.CancelButton;
            }
        }
    }
}
