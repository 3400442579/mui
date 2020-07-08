
using Ani.IMG.APNG;
using Ani.IMG.GIF;
using Ani.IMG.Webp;
using SkiaSharp;
using System;
using System.IO;
using System.Linq;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // WebDecoding();

            //GifDecoding();


            ApngDecoding();
            //ApngEncoder();
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
                SKBitmap rawFrame = apng.ToBitmap(index, out Frame frame);

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

    }



}
