
using Ani.IMG;
using Ani.IMG.APNG;
using Ani.IMG.GIF;
using Ani.IMG.Webp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SkiaSharp;
using System;
using System.IO;
using System.Linq;
using ImageMagick;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // WebDecoding();

            //GifDecoding();


            //ApngDecoding();
            //ApngEncoder();


            webp();
            //ss();
        }


        static void WebDecoding()
        {

            WebpDecoder webpDecoder = new WebpDecoder(@"test\world-cup-2014-42.webp");
            for (int i = 0; i < webpDecoder.FrameCount; i++)
                webpDecoder.GetFrame(i, $@"test\webp\webp_{i}.webp");
        }

        static void GifDecoding()
        {
            //using FileStream stream = new FileStream(@"test\world-cup-2014-42.gif", FileMode.Open, FileAccess.Read);
            GifDecoder decoder = new GifDecoder(@"test\world-cup-2014-42.gif");

            string pathTemp = @"test\gif";
            for (int i = 0; i < decoder.FrameCount; i++)
                decoder.GetFrame(i, $@"{pathTemp}\{i}.webp");
        }

        static void ApngDecoding()
        {
            string pathTemp = @"test\png";
            using var stream = new FileStream(@"test\aaa.png", FileMode.Open);
            var apng = new Apng();
            apng.Load(stream);


            if (!apng.IsAnimated)
                return;

            SKBitmap baseFrame = null;

            var fullSize = new SKSize((int)apng.Width, (int)apng.Height);
            for (int index = 0; index < apng.FrameCount; index++)
            {
                SKBitmap rawFrame = apng.ToBitmap(index, out Ani.IMG.APNG.Frame frame);

                var bitmapSource = Render.MakeFrame(fullSize, rawFrame, frame, baseFrame);
                switch (frame.DisposeOp)
                {
                    case DisposeOperation.NONE:
                        baseFrame = bitmapSource.Copy();
                        break;
                    case DisposeOperation.BACKGROUND:
                        baseFrame = Render.IsFullFrame(frame, fullSize) ? null : Render.ClearArea(bitmapSource, frame);
                        break;
                    case DisposeOperation.PREVIOUS:
                        //Reuse same base frame.
                        break;
                }

                string fn = Path.Combine(pathTemp, $"{index}.png");
                using var output = new FileStream(fn, FileMode.Create);
                bitmapSource.Encode(SKEncodedImageFormat.Png, 100).SaveTo(output);
            }
        }

        static void ApngEncoder()
        {
            string pathTemp = @"test\png";

            string[] ss = Directory.GetFiles(pathTemp).OrderBy(o => Convert.ToInt32(Path.GetFileName(o).Replace(Path.GetExtension(o), ""))).ToArray();

            //Apng apng = APNGAssembler.AssembleAPNG(ss, true);//1.png
            //using var stream = apng.ToStream();

            using var stream = new MemoryStream();//2.png
            Encoder encoder = new Encoder(stream, ss.Length, 0);
            foreach (var s in ss)
                encoder.AddFrame(s, new SKRect(0, 0, 547, 200));
            
            encoder.Finish();
            using var fileStream = new FileStream(@"test\png\aaa.png", FileMode.Create, FileAccess.Write, FileShare.None, 4096);
            stream.WriteTo(fileStream);
        }



        static void ss()
        {
            //using SKBitmap bitmap = SKBitmap.Decode(@"E:\T\0.png");
            //using SKBitmap bitm = Util.PixelAnalysis(bitmap, new Ani.IMG.Frame { Path = @"E:\T\1.png", Rect = new SKRectI { Left = 0, Top = 0, Bottom = 230, Right = 230 } });
            //using FileStream stream = new FileStream(@"E:\T\1_1.png", FileMode.Create);
            //bitm.Encode(SKEncodedImageFormat.Png, 100).SaveTo(stream);



            var bg = Image.Load<Rgba32>(@"E:\T\0.png");
            using Image img = Util.Ss(bg, new Ani.IMG.Frame { Path = @"E:\T\1.png", Rect = new SKRectI { Left = 0, Top = 0, Bottom = 230, Right = 230 } });
            using FileStream stream = new FileStream(@"E:\T\1_1.png", FileMode.Create);
            img.SaveAsPng(stream);
            
            using FileStream stream2 = new FileStream(@"E:\T\0_1.png", FileMode.Create);
            bg.SaveAsPng(stream2);



           // Util.Alpha()
        }


        static void webp() {

            //ImageMagick.MagickImageCollection images = new MagickImageCollection();
           // MagickImage magick = new MagickImage("");
            

            using var collection = new MagickImageCollection(@"C:\Users\jxw\source\repos\mui\ConsoleApp\bin\Debug\netcoreapp3.1\test\world-cup-2014-42.gif");

            collection.Add("Snakeware.png");
            collection[0].AnimationDelay = 100; // in this example delay is 1000ms/1sec
           
            // Add second image, set the animation delay (in 1/100th of a second) and flip the image
            collection.Add("Snakeware.png");
            collection[1].AnimationDelay = 100; // in this example delay is 1000ms/1sec
            //collection[1].Flip();


            collection.Coalesce();
            collection.Write(@"C:\Users\jxw\source\repos\mui\ConsoleApp\bin\Debug\netcoreapp3.1\test\test.webp", MagickFormat.WebP);
        }


    }



}
