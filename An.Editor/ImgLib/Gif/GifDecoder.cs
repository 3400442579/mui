using SixLabors.ImageSharp;
using System.Drawing.Imaging;

namespace An.Image.Gif
{
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



        ~GifDecoder()
        {
            gifImage.Dispose();
        }
    }

    public class GifDecoder2
    {
        private readonly SixLabors.ImageSharp.Image gifImage;

        public GifDecoder2(string path)
        {
            gifImage = SixLabors.ImageSharp.Image.Load(path);

            FrameCount = gifImage.Frames.Count;
        }

        public int FrameCount { get; }


        public void GetFrame(int index, string outfile)
        {
            using var frameImage = gifImage.Frames.CloneFrame(index);
            frameImage.Save(outfile);
        }


        ~GifDecoder2()
        {
            gifImage.Dispose();
        }

    }
}
