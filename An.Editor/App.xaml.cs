using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using An.Editor.ViewModels;
using An.Editor.Views;
using Avalonia.Styling;
using Avalonia.Markup.Xaml.Styling;
using System;
using An.Ava;
using An.Ava.Controls;

namespace An.Editor
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                //desktop.MainWindow = new MainWindow
                //{
                //    DataContext = new MainWindowViewModel(),
                //};

                //var theme = new StyleInclude(new Uri("resm:Styles?assembly=An.Ava"))
                //{
                //    Source = new Uri($"avares://An.Ava/Styles/Extended/MetroWindow.xaml?assembly=An.Ava")
                //    //Source = new Uri($"resm:{tempXamlPath}?assembly=Avalonia.ExtendedToolkit")
                //};
                desktop.MainWindow = new TestWindow();
                //desktop.MainWindow.Styles.Add(theme);

            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
