//using SixLabors.ImageSharp;
//using SixLabors.ImageSharp.Formats;
//using SixLabors.ImageSharp.PixelFormats;
//using SixLabors.ImageSharp.Processing;
using SkiaSharp;

namespace Ani.IMG
{
    public class Util
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bgBitmap"></param>
        /// <param name="frame"></param>
        /// <param name="minAlpha">最小透明度</param>
        /// <returns></returns>
        public static SKBitmap PixelAnalysis(SKBitmap bgBitmap, Frame frame, int minAlpha = 0)
        {
            bool change = false;
            int x1, y1;

            using var cur = SKBitmap.Decode(frame.Path);

            SKBitmap sK = new SKBitmap(new SKImageInfo(cur.Width, cur.Height));

            for (int x = frame.Rect.Left; x < frame.Rect.Right; x++)
            {
                for (int y = frame.Rect.Top; y < frame.Rect.Bottom; y++)
                {
                    x1 = x - frame.Rect.Left;
                    y1 = y - frame.Rect.Top;

                    SKColor c1 = bgBitmap.GetPixel(x, y);
                    SKColor c2 = cur.GetPixel(x1, y1);

                    if (c2.Alpha <= minAlpha ||
                        c1.Red == c2.Red && c1.Blue == c2.Blue && c1.Green == c2.Green)
                    {
                        //cur.SetPixel(x1, y1, SKColors.Empty);
                        if (!change)
                            change = true;
                    }
                    else
                        sK.SetPixel(x1, y1, c2);
                }
            }

            if (change)
            {
                using SKCanvas canvas = new SKCanvas(bgBitmap);
                canvas.DrawBitmap(cur, frame.Rect);
            }


            // SKBitmap sK = new SKBitmap(new SKImageInfo(cur.Width, cur.Height));
            //using SKCanvas canvas1 = new SKCanvas(sK);
            //canvas1.Clear(SKColors.Transparent);
            //canvas1.DrawBitmap(SKBitmap.Decode(@"C:\Users\jxw\Desktop\警报.png"), 0,0);
            //canvas1.DrawBitmap(SKBitmap.Decode(@"E:\T\1_2.png"), frame.Rect);


            return sK;
        }


        //public static Image<Rgba32> Ss(Image<Rgba32> bgBitmap, Frame frame, int minAlpha = 0)
        //{
        //    bool change = false;
        //    int x1, y1;

        //    var cur = Image.Load<Rgba32>(frame.Path);


        //    for (int x = frame.Rect.Left; x < frame.Rect.Right; x++)
        //    {
        //        for (int y = frame.Rect.Top; y < frame.Rect.Bottom; y++)
        //        {
        //            x1 = x - frame.Rect.Left;
        //            y1 = y - frame.Rect.Top;

        //            var c1 = bgBitmap[x, y];
        //            var c2 = cur[x1, y1];

        //            if (c2.A <= minAlpha ||
        //                c1.R == c2.R && c1.B == c2.B && c1.G == c2.G)
        //            {
        //                c2.A = 0;
        //                cur[x1, y1] = c2;
        //                if (!change)
        //                    change = true;
        //            }

        //        }
        //    }

        //    if (change)
        //    {
        //        bgBitmap.Mutate(o => o.DrawImage(cur, new Point(frame.Rect.Left, frame.Rect.Top), 1f));
        //    }

        //    return cur;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bgBitmap"></param>
        /// <param name="minAlpha">最小透明度</param>
        public static SKBitmap SetAlpha(SKBitmap bgBitmap, int minAlpha)
        {
            if (bgBitmap.AlphaType == SKAlphaType.Opaque)
                return bgBitmap;

            if (bgBitmap.ColorType != SKColorType.Bgra8888 && bgBitmap.ColorType != SKColorType.Rgba8888 && bgBitmap.ColorType != SKColorType.Unknown)
                return bgBitmap;


            SKBitmap skbitmap = new SKBitmap(new SKImageInfo(bgBitmap.Width, bgBitmap.Height));
           
            for (int x = 0; x < bgBitmap.Width; x++)
            {
                for (int y = 0; y < bgBitmap.Height; y++)
                {
                    SKColor c1 = bgBitmap.GetPixel(x, y);

                    if (c1.Alpha <= minAlpha)
                        skbitmap.SetPixel(x, y, SKColors.Transparent);
                    else
                        skbitmap.SetPixel(x, y, c1);
                }
            }

            return skbitmap;
        }


        //public static void Alpha(Image<Rgba32> img, int minAlpha)
        //{
        //    //if (img.PixelType.)
        //    //    return;

        //    //if (bgBitmap.ColorType != SKColorType.Bgra8888 && bgBitmap.ColorType != SKColorType.Rgba8888 && bgBitmap.ColorType != SKColorType.Unknown)
        //    //    return;

        //    for (int x = 0; x < img.Width; x++)
        //    {
        //        for (int y = 0; y < img.Height; y++)
        //        {
        //            var c1 = img[x, y];

        //            if (c1.A <= minAlpha)
        //                img[x, y]=Color.Transparent;
        //        }
        //    }


        //}
    }
}
