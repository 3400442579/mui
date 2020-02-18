using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animation.Editor.ViewModel
{
    public class CapturaViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string[] splitButtonNames=new string[] { "全屏","窗口","区域" };
        public string[] SplitButtonNames
        {
            get => splitButtonNames;
           private set
            {
                splitButtonNames = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SplitButtonNames)));

                // Update a command status
                // DoSomethingCommand.RaiseCanExecuteChanged();
            }
        }

        private string[] ssNames = new string[] { "开始", "停止", "暂停" };
        public string[] SSNames
        {
            get => ssNames;
            private set
            {
                ssNames = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ssNames)));

                // Update a command status
                // DoSomethingCommand.RaiseCanExecuteChanged();
            }
        }
    }
}
