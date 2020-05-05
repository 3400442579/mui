using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;

namespace DH.Editor.ViewModels
{
    public class AppViewModel : ReactiveUI.ReactiveObject
    {
        private bool isOpenSettin = false;
        public bool IsOpenSetting {
            get { return isOpenSettin; }
            set { this.RaiseAndSetIfChanged(ref isOpenSettin, value); }
        }

    }
}
