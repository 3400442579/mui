using SkiaSharp;
using System.IO;

namespace Ani.IMG.Webp
{



    public class WebpDecoder
    {
        private readonly SKCodec codec = null;
        private SKImageInfo info = SKImageInfo.Empty;
        //private readonly SKBitmap bitmap = null;
        private readonly SKCodecFrameInfo[] frames;

        public WebpDecoder(string gif)
        {
            codec = SKCodec.Create(gif);
        }

        ~WebpDecoder()
        {
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

            SKCodecOptions opts = new SKCodecOptions(index);
           
            using var bitmap = new SKBitmap(codec.Info);
 
            if (codec?.GetPixels(codec.Info, bitmap.GetPixels(), opts) == SKCodecResult.Success)
            {
                bitmap.NotifyPixelsChanged();

                using FileStream stream = new System.IO.FileStream(save, FileMode.Create);
                using SKPixmap pixmap = new SKPixmap(bitmap.Info, bitmap.GetPixels());

                switch (System.IO.Path.GetExtension(save).ToLower())
                {
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
                        pixmap.Encode(SKEncodedImageFormat.Webp, 100).SaveTo(stream);
                        break;
                }
            }
            return codec.FrameInfo[index].Duration;
        }


        public int FrameCount => codec.FrameCount;

    }
}
