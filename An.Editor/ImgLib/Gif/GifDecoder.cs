using SkiaSharp;
using System.IO;

namespace An.Image.Gif
{
    //public class GifDecoder
    //{
    //    private readonly System.Drawing.Image gifImage;
    //    private readonly FrameDimension dimension;
    //    private int currentFrame = -1;
    //    private int step = 1;


    //    public GifDecoder(string path)
    //    {
    //        gifImage = System.Drawing.Image.FromFile(path);
    //        dimension = new FrameDimension(gifImage.FrameDimensionsList[0]); // gets the GUID
    //        FrameCount = gifImage.GetFrameCount(dimension); // total frames in the animation
    //    }


    //    public bool ReverseAtEnd { get; set; }

    //    public int FrameCount { get; }

    //    public System.Drawing.Image GetNextFrame()
    //    {
    //        currentFrame += step;

    //        // if the animation reaches a boundary
    //        if (currentFrame >= FrameCount || currentFrame < 1)
    //        {
    //            if (ReverseAtEnd)
    //            {
    //                step *= -1; // reverse the count
    //                currentFrame += step; // apply it
    //            }
    //            else
    //            {
    //                currentFrame = 0; // or start over
    //            }
    //        }
    //        return GetFrame(currentFrame);
    //    }

    //    public System.Drawing.Image GetFrame(int index)
    //    {
    //        gifImage.SelectActiveFrame(dimension, index); // find the frame
    //        return (System.Drawing.Image)gifImage.Clone(); // return a copy
    //    }



    //    ~GifDecoder()
    //    {
    //        gifImage.Dispose();
    //    }
    //}

    //public class GifDecoder2
    //{
    //    private readonly SixLabors.ImageSharp.Image gifImage;

    //    public GifDecoder2(string path)
    //    {
    //        gifImage = SixLabors.ImageSharp.Image.Load(path);

    //        FrameCount = gifImage.Frames.Count;
    //    }

    //    public int FrameCount { get; }


    //    public void GetFrame(int index, string outfile)
    //    {
    //        using var frameImage = gifImage.Frames.CloneFrame(index);
    //        frameImage.Save(outfile);
    //    }


    //    ~GifDecoder2()
    //    {
    //        gifImage.Dispose();
    //    }

    //}

    public class GifDecoder
    {
        private readonly SKCodec codec = null;
        private SKImageInfo info = SKImageInfo.Empty;
        private readonly SKBitmap bitmap = null;
        private readonly SKCodecFrameInfo[] frames;

        public GifDecoder(string gif)
        {

            codec = SKCodec.Create(gif);
            frames = codec.FrameInfo;

            info = codec.Info;
            info = new SKImageInfo(info.Width, info.Height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);

            bitmap = new SKBitmap(info);
        }

        ~GifDecoder()
        {
            bitmap.Dispose();
            codec.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="save"></param>
        /// <returns> Gets or sets the number of milliseconds to show this frame.</returns>
        public int GetFrame(int index, string save)
        {
            if (index >= codec.FrameCount)
                throw new System.Exception("Index超出FrameCount");

            var opts = new SKCodecOptions(index);
            if (codec?.GetPixels(info, bitmap.GetPixels(), opts) == SKCodecResult.Success)
            {
                bitmap.NotifyPixelsChanged();

                using FileStream stream = new System.IO.FileStream(save, FileMode.Create);
                using SKPixmap pixmap = new SKPixmap(bitmap.Info, bitmap.GetPixels());

                switch (System.IO.Path.GetExtension(save).ToLower()) {
                    case ".png":
                        pixmap.Encode(SKEncodedImageFormat.Png, 100).SaveTo(stream);
                        break;
                    case ".jpg":
                    case ".jpeg":
                        pixmap.Encode(SKEncodedImageFormat.Jpeg, 100).SaveTo(stream);
                        break;

                    case ".webp":
                        pixmap.Encode(SKEncodedImageFormat.Webp, 100).SaveTo(stream);
                        break;
                    default:
                        pixmap.Encode(SKEncodedImageFormat.Bmp, 100).SaveTo(stream);
                        break;
                }
               
                //bitmap.PeekPixels().Encode(SKEncodedImageFormat.Png, 80).SaveTo(stream);
            }

            return frames[index].Duration;
        }


        public int FrameCount => codec.FrameCount;

    }
}
