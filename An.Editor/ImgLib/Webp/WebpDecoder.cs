using SkiaSharp;
using System.IO;

namespace An.Image.Webp 
{
    
    public class WebpDecoder
    {
        private readonly SKCodec codec = null;
        private SKImageInfo info = SKImageInfo.Empty;
        private readonly SKBitmap bitmap = null;
        private readonly SKCodecFrameInfo[] frames;

        public WebpDecoder(string gif)
        {
 
            codec = SKCodec.Create(gif);
            frames = codec.FrameInfo;

            info = codec.Info;
            info = new SKImageInfo(info.Width, info.Height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);

            bitmap = new SKBitmap(info);
        }

        ~WebpDecoder()
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
            var re = codec?.GetPixels(info, bitmap.GetPixels(), opts);
            if (re == SKCodecResult.Success)
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
