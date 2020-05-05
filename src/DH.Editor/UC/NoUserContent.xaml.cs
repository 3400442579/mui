using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using HandyControl.Data;
using HandyControl.Tools;


namespace DH.Editor.UC
{
    public partial class NoUserContent
    {
        public NoUserContent()
        {
            InitializeComponent();
        }

        private void ButtonLangs_OnClick(object sender, RoutedEventArgs e)
        {
            //if (e.OriginalSource is Button button && button.Tag is string tag)
            //{
            //    PopupConfig.IsOpen = false;
            //    if (tag.Equals(GlobalData.Config.Lang)) return;
            //    ConfigHelper.Instance.SetLang(tag);
            //    LangProvider.Culture = new CultureInfo(tag);
            //    Messenger.Default.Send<object>(null, "LangUpdated");

            //    GlobalData.Config.Lang = tag;
            //    GlobalData.Save();
            //}
        }

        //private void ButtonConfig_OnClick(object sender, RoutedEventArgs e) => PopupConfig.IsOpen = true;

        private void ButtonSkins_OnClick(object sender, RoutedEventArgs e)
        {
            //if (e.OriginalSource is Button button && button.Tag is SkinType tag)
            //{
            //    PopupConfig.IsOpen = false;
            //    if (tag.Equals(GlobalData.Config.Skin)) return;
            //    GlobalData.Config.Skin = tag;
            //    GlobalData.Save();
            //    ((App)Application.Current).UpdateSkin(tag);
            //}
        }

        private void MenuAbout_OnClick(object sender, RoutedEventArgs e)
        {
            //new AboutWindow
            //{
            //    Owner = Application.Current.MainWindow
            //}.ShowDialog();
        }
    }
}
