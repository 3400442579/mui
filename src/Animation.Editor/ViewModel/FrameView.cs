using DH.MUI.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Animation.Editor.ViewModel
{
    public class FrameView : NotifyPropertyChanged
    {
        private int delay;
        public int Delay
        {
            get { return delay; }
            set { this.Set(ref delay, value); }
        }

        private int index;
        public int Index
        {
            get { return index; }
            set { this.Set(ref index, value); }
        }

        private string path;
        public string Path
        {
            get { return path; }
            set { this.Set(ref path, value); }
        }

        private bool selected;
        public bool Selected
        {
            get { return selected; }
            set { this.Set(ref selected, value); }
        }
    }
}
