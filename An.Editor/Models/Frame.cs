using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace An.Editor.ViewModels
{
    public class Frame : ReactiveObject
    {
        private string souce;
        public string Souce
        {
            get { return souce; }
            set
            {
                if (souce != value)
                {
                    souce = value;
                    this.RaisePropertyChanged(nameof(Souce));
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
