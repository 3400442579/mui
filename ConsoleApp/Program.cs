
using Ani.IMG.APNG;
using SkiaSharp;
using System.IO;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string pathTemp = @"test\png";

            string[] ss = Directory.GetFiles(pathTemp);
            using var stream = new MemoryStream();
            using Encoder encoder = new Encoder(stream, ss.Length, 1);
            foreach (var s in ss)
            {
                encoder.AddFrame(s, new SKRect(0, 0, 547, 200));
            }

            using var fileStream = new FileStream(@"test\png\aaa.png", FileMode.Create, FileAccess.Write, FileShare.None, 4096);
            stream.WriteTo(fileStream);



            //using var stream = new FileStream(@"test\aaa.png", FileMode.Open);
            //var apng = new Apng();
            //apng.Load(stream);



            //if (!apng.IsAnimated)
            //    return;

            //SKBitmap baseFrame = null;

            //var fullSize = new SKSize((int)apng.Width, (int)apng.Height);
            //for (int index = 0; index < apng.FrameCount; index++)
            //{
            //    SKBitmap rawFrame = apng.ToBitmap(index, out Frame frame);


            //    var bitmapSource = Render.MakeFrame(fullSize, rawFrame, frame, baseFrame);
            //    switch (frame.DisposeOp)
            //    {
            //        case DisposeOperation.NONE:
            //            baseFrame = bitmapSource.Copy();
            //            break;
            //        case DisposeOperation.BACKGROUND:
            //            baseFrame = Render.IsFullFrame(frame, fullSize) ? null : Render.ClearArea(bitmapSource, frame);
            //            break;
            //        case DisposeOperation.PREVIOUS:
            //            //Reuse same base frame.
            //            break;
            //    }


            //    string fn = Path.Combine(pathTemp, $"{index}.png");
            //    using var output = new FileStream(fn, FileMode.Create);
            //    bitmapSource.Encode(SKEncodedImageFormat.Png, 100).SaveTo(output);
            //}

           

        }

    }



}
