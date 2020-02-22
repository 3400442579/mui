using DH.MUI.Core;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Windows;

namespace Animation.Editor.Models
{
    /// <summary>
    /// 帧
    /// </summary>
    public sealed class Fi : NotifyPropertyChanged
    { 
        //Fi
        public Fi() { }
        public Fi(string path, int delay) {
            Path = path;
            Delay = delay;
            //Index = index;
        }
        public Fi(string path, int delay,List<string> keys)
        {
            Path = path;
            Delay = delay;
            //Index = index;
            if (keys != null)
                Keys = keys;
        }
        public Fi(string path, int delay, Point? cursorPoint, List<string> keys, int index)
        {
            Path = path;
            Delay = delay;
            //Index = index;
            if (keys != null)
                Keys = keys;

            Cursor = cursorPoint;
        }

        private int delay;
        public int Delay
        {
            get { return delay; }
            set { this.Set(ref delay, value); }
        }

        public int Index { get; set; }
 
        public string Path { get; set; }
        
        /// <summary>
        /// 这个用在view中
        /// </summary>
        private bool selected;
        [JsonIgnore]
        public bool Selected
        {
            get { return selected; }
            set { Set(ref selected, value); }
        }

        /// <summary>
        /// Cursor position
        /// </summary>
        public Point? Cursor { get; set; }

        //
        public List<string> Keys { get;set;}
    }
}
