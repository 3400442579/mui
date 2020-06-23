using Avalonia;
using Avalonia.Media;
using ReactiveUI;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace An.Editor.ViewModels
{
    public class Frame : ReactiveObject
    {
        private string path;
        public string Path
        {
            get { return path; }
            set
            {
                if (path != value)
                {
                    path = value;
                    this.RaisePropertyChanged(nameof(Path));
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
        [JsonIgnore]
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


        public int? CurX { get; set; }
        public int? CurY { get; set; }
        public bool? Clicked { get; set; }
        public List<string> Keys { get; set; }


        /// <summary>
        /// The Rectangle of the frame.
        /// </summary>
        [JsonIgnore]
        public Rect Rect { get; set; }

        /// <summary>
        /// The color that will be treated as transparent on this frame.
        /// </summary>
        [JsonIgnore]
        public Color ColorKey { get; set; }

        /// <summary>
        /// True if the frame has area, width and height > 0.
        /// </summary>
        [JsonIgnore]
        public bool HasArea => Rect.Width > 0 && Rect.Height > 0;



        public Frame(string path, int delay)
        {
            this.path = path;
            this.delay = delay;
        }

        public Frame(string path, int delay, int? curX, int? curY, bool? clicked, List<string> keys, int index)
        {
            this.path = path;
            this.delay = delay;
            CurX = curX;
            CurY = curY;
            Clicked = clicked;
            Keys = keys;
            this.index = index;
        }
    }
}
