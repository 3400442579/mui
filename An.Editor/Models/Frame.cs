using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace An.Editor.ViewModels
{
    public class Frame : ReactiveObject
    {
        private string source;
        public string Source
        {
            get { return source; }
            set
            {
                if (source != value)
                {
                    source = value;
                    this.RaisePropertyChanged(nameof(Source));
                }
            }
        }

        private int delay;
        public int Delay
        {
            get { return delay; }
            set
            {
                if (delay != value)
                {
                    delay = value;
                    this.RaisePropertyChanged(nameof(Delay));
                }
            }
        }

        private int index;
        public int Index
        {
            get { return index; }
            set
            {
                if (index != value)
                {
                    index = value;
                    this.RaisePropertyChanged(nameof(Index));
                }
            }
        }

    }
}
