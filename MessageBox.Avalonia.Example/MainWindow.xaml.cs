using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;
using System.Collections.Generic;
using MessageBoxAvaloniaEnums = MessageBox.Avalonia.Enums;

namespace MessageBox.Avalonia.Example
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            this.FindControl<Button>("btnClick").Click += MainWindow_Click;
        }

        private async void MainWindow_Click(object sender, RoutedEventArgs e)
        {
            //var msBoxStandardWindow = MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow(new MessageBoxStandardParams
            //{
            //    ButtonDefinitions = MessageBoxAvaloniaEnums.ButtonEnum.OkAbort,
            //    ContentTitle = "Title",
            //    ContentMessage = "Message",
            //    Icon = MessageBoxAvaloniaEnums.Icon.Plus,
            //    Style = MessageBoxAvaloniaEnums.Style.DarkMode,
            //    WindowStartupLocation=WindowStartupLocation.CenterOwner
            //});
            //var ss = await msBoxStandardWindow.ShowDialog(this);


            var messageBoxCustomWindow = MessageBox.Avalonia.MessageBoxManager.GetMessageBoxCustomWindow(new MessageBoxCustomParams
            {
                ButtonDefinitions = new List<Models.ButtonDefinition> { new Models.ButtonDefinition() { IsDefault = true, Name = "aa", Type = ButtonType.Default }, new Models.ButtonDefinition() { Name = "cc", Type = ButtonType.Colored } },
                ContentTitle = "Title",
                ContentMessage = "Message",
                Icon = MessageBoxAvaloniaEnums.Icon.Plus,
                Style = MessageBoxAvaloniaEnums.Style.DarkMode,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            }); ;
            var ss1 = await messageBoxCustomWindow.ShowDialog(this);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
