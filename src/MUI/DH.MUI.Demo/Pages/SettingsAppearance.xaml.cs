using System.Windows;
using DH.MUI.Controls;
using DH.MUI.Demo.ViewModels;

namespace DH.MUI.Demo.Pages
{
    /// <summary>
    /// SettingsAppearance.xaml 的交互逻辑
    /// </summary>
    public partial class SettingsAppearance : ModernWindow
    {
        public SettingsAppearance()
        {
            InitializeComponent();
            this.DataContext = new SettingsAppearanceViewModel();
        }
    }
}
