using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Animation.Editor.Models
{
    public sealed class Frame
    {
        
        public Frame() { }
        public Frame(string path, int delay,int index) {
            Path = path;
            Delay = delay;
            Index = index;
        }
        public Frame(string path, int delay,  int index,List<string> keys)
        {
            Path = path;
            Delay = delay;
            Index = index;
            if (keys != null)
                Keys = keys;
        }
        public Frame(string path, int delay, int index, Point point, List<string> keys)
        {
            Path = path;
            Delay = delay;
            Index = index;
            if (keys != null)
                Keys = keys;

            Cursor = point;
        }

        /// <summary>
        /// The frame image full path
        /// </summary>
        public string Path { get; set; }

        public int Index { get; set; }
        /// <summary>
        /// The delay of the frame.
        /// </summary>
        public int Delay { get; set; }

        /// <summary>
        /// Cursor position
        /// </summary>
        public Point? Cursor { get; set; }


        public List<string> Keys { get;set;}
    }
}
