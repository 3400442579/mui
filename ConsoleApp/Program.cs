
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using six = SixLabors.ImageSharp;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //AnimatedGif animatedGif = new AnimatedGif(@"C:\Users\jxw\Desktop\NGif.gif");
            //for (int i = 0; i < animatedGif.FrameCount; i++)
            //    animatedGif.GetFrame(i).Save($@"E:\T\{i}.png");


            var image = six.Image.Load(@"C:\Users\jxw\Desktop\NGif.gif");
            for (int i = 0; i < image.Frames.Count; i++)
            {
                using var frameImage = image.Frames.CloneFrame(i);
                //frameImage.Mutate(x => x.Quantize());
                frameImage.Save($@"E:\T\a\{i}.png");// we include all metadata from the original image;
            }
        }




    }


    public class GifDecoder
    {
        private readonly System.Drawing.Image gifImage;
        private readonly FrameDimension dimension;
        private int currentFrame = -1;
        private int step = 1;


        public GifDecoder(string path)
        {
            gifImage = System.Drawing.Image.FromFile(path);
            dimension = new FrameDimension(gifImage.FrameDimensionsList[0]); // gets the GUID
            FrameCount = gifImage.GetFrameCount(dimension); // total frames in the animation
        }


        public bool ReverseAtEnd { get; set; }

        public int FrameCount { get; }

        public System.Drawing.Image GetNextFrame()
        {
            currentFrame += step;

            // if the animation reaches a boundary
            if (currentFrame >= FrameCount || currentFrame < 1)
            {
                if (ReverseAtEnd)
                {
                    step *= -1; // reverse the count
                    currentFrame += step; // apply it
                }
                else
                {
                    currentFrame = 0; // or start over
                }
            }
            return GetFrame(currentFrame);
        }

        public System.Drawing.Image GetFrame(int index)
        {
            gifImage.SelectActiveFrame(dimension, index); // find the frame
            return (System.Drawing.Image)gifImage.Clone(); // return a copy
        }



        public void aa(string file) {
            var image = SixLabors.ImageSharp.Image.Load(file);
            for (int i = 0; i < image.Frames.Count; i++)
            {
                using var frameImage = image.Frames.CloneFrame(i);
                frameImage.Save($@"E:\T\a\{i}.png");
            }

            image.Dispose();
        }
    }
}
