using DH.Editor.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;

namespace DH.Editor.Views
{
    /// <summary>
    /// SettingView.xaml 的交互逻辑
    /// </summary>
    public partial class SettingView : ReactiveUserControl<SettingViewModel>
    {
        public SettingView()
        {
            InitializeComponent();

            ViewModel = new SettingViewModel();

            this.WhenActivated(disposableRegistration =>
            {
                // Notice we don't have to provide a converter, on WPF a global converter is
                // registered which knows how to convert a boolean into visibility.

                this.Bind(ViewModel,
                    viewModel => viewModel.SelectedColor,
                    view => view.AccentColor.SelectedItem)
                .DisposeWith(disposableRegistration);

                this.Bind(ViewModel,
                    viewModel => viewModel.SelectedTheme,
                    view => view.Threme.SelectedItem)
                .DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                    viewModel => viewModel.AccentColors,
                    view => view.AccentColor.ItemsSource)
                .DisposeWith(disposableRegistration);
            });
        }
    }
}
