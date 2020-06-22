using System;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using System.Collections.Generic;
using An.Editor.Controls;
using An.Editor.Models;

namespace An.Editor.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            this.WhenAnyValue(o => o.ScaleValue).Subscribe(o =>
            {
                Scale = (int)o;
            });

            Project = new Project { Width = 500, Height = 400, Name = "aa"  };
            Project.Frames.Add(new Frame { Index = 0, Delay = 100, Source = @"C:\Users\jxw\Desktop\17\1.png" });
            Project.Frames.Add(new Frame { Index = 1, Delay = 110, Source = @"C:\Users\jxw\Desktop\17\2.png" });
        }

        public string Greeting => "Welcome to Avalonia!";


        public List<string> Greetings => new List<string> { "a", "b" };


        private int scale = 100;
        public int Scale
        {
            set
            {
                if (scale != value)
                {
                    scale = value;
                    this.RaisePropertyChanged(nameof(Scale));
                }
            }
            get
            {
                return scale;
            }
        }


        private object scaleValue = 100;
        public object ScaleValue
        {
            set
            {
                if (scaleValue != value)
                {
                    scaleValue = value;
                    this.RaisePropertyChanged(nameof(ScaleValue));
                }
            }
            get
            {
                return scaleValue;
            }
        }


        private Project project;
        public Project Project
        {
            set
            {
                project = value;
                this.RaisePropertyChanged(nameof(Project));
            }
            get
            {
                return project;
            }
        }

        private Frame selectedFrame;
        public Frame SelectedFrame
        {
            set
            {
                selectedFrame = value;
                this.RaisePropertyChanged(nameof(SelectedFrame));
            }
            get
            {
                return selectedFrame;
            }
        }
    }
}
