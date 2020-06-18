using An.Editor.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace An.Editor.Models
{
    public class Project : ReactiveObject
    {
        public static Project Instance = new Project();

        private int width;
        public int Width
        {
            get { return width; }
            set
            {
                if (width != value)
                {
                    width = value;
                    this.RaisePropertyChanged(nameof(Width));
                }
            }
        }

        private int height;
        public int Height
        {
            get { return height; }
            set
            {
                if (height != value)
                {
                    height = value;
                    this.RaisePropertyChanged(nameof(Height));
                }
            }
        }

        public ObservableCollection<Frame> Frames { get; } = new ObservableCollection<Frame>();

        public string Name { get; set; }
        public DateTime Createtime { get; set; }
    }
}
