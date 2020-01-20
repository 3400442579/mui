using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace DH.Editor.ViewModels
{
    public class RootViewModel : ObservableObject
    {
       // public Window Window { get; set; }

        private static RelayCommand m_OpenSetting;
        public RelayCommand OpenSettingCommand => m_OpenSetting ?? (m_OpenSetting = new RelayCommand(() =>
        {
            SettingWindow settingWindow = new SettingWindow();
            settingWindow.ShowDialog();
        }));
    }
}
