using ImageMagick;
using ImageMagick.Defines;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ani.IMG.WEBP
{
    public class WebpEncoder : IDisposable
    {
        readonly int repeatCount;
        private MagickImageCollection collection;

        public WebpEncoder(int repeatCount)
        {
            collection = new MagickImageCollection(@"C:\Users\jxw\source\repos\mui\ConsoleApp\bin\Debug\netcoreapp3.1\test\world-cup-2014-42.gif");
            this.repeatCount = repeatCount;
        }

        public void AddFrame(string path, ushort delay = 66)
        {
            collection.Add(path);
            collection[0].AnimationDelay = 66; // in this example delay is 1000ms/1sec
            


           
           
        }

        public void Save(string save) {
            collection.Optimize();
            //WebpEncoder encoder = new WebpEncoder(repeatCount);
            //encoder.AddFrame();
            
            //encoder.Save(save);
            collection.Write(save,MagickFormat.WebP);
        }

        public void Dispose()
        {
            collection.Dispose();
        }
    }
}
