using DH.Editor.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DH.Editor.UC
{
    /// <summary>
    /// SettingUc.xaml 的交互逻辑
    /// </summary>
    public partial class SettingUc : ReactiveUserControl<SettingViewModel>
    {
        public SettingUc()
        {
            InitializeComponent();

            ViewModel = new SettingViewModel();

            this.WhenActivated(disposableRegistration =>
            {
                // Notice we don't have to provide a converter, on WPF a global converter is
                // registered which knows how to convert a boolean into visibility.
                
 
                this.OneWayBind(ViewModel,
                    viewModel => viewModel.AccentColors,
                    view => view.AccentColor.ItemsSource)
                    .DisposeWith(disposableRegistration);
            });
        }
    }
}
