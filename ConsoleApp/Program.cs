
using APNGLib;
using SkiaSharp;
using System.IO;


namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {

            //string pathTemp = @"test\png";
            //SKBitmap rawFrame = SKBitmap.Decode(@"C:\Users\jxw\Desktop\17\1.png");
            //SKBitmap baseFrame = SKBitmap.Decode(@"C:\Users\jxw\Downloads\gif问题\gif问题\点赞\1.png");

            //SKBitmap bitmap = new SKBitmap(new SKImageInfo((int)baseFrame.Width, (int)baseFrame.Height));
            //using SKCanvas canvas = new SKCanvas(bitmap);
            //canvas.Clear();

            ////var fullRect = new SKRect(0, 0, baseFrame.Width, baseFrame.Height);
            //canvas.DrawBitmap(baseFrame,0,0);


            //canvas.DrawBitmap(rawFrame, new SKRect(0, 0, (int)rawFrame.Width, (int)rawFrame.Height));
            //var fileName = Path.Combine(pathTemp, $"a.png");
            //using var output = new FileStream(fileName, FileMode.Create);
            //var encode = bitmap.Encode(SKEncodedImageFormat.Png, 100);
            //encode.SaveTo(output);

            //return;

            string pathTemp = @"test\png";

            using var stream = new FileStream(@"test\bounce.png", FileMode.Open);
            var apng = new APNG();
            apng.Load(stream);

            if (!apng.IsAnimated)
                return;

            var fullSize = new SKSize((int)apng.Width, (int)apng.Height);

            SKBitmap baseFrame = null;
            for (var index = 0; index < apng.FrameCount; index++)
            {
                var frame = apng.GetFrame(index);
                SKImage rawFrame = SKImage.FromEncodedData(apng.ToStream(index));

                // SKImage sKImage = rawFrame.Subset(new SKRectI(10,10, rawFrame.Width, rawFrame.Height));//.ToRasterImage(false);

              SKBitmap sss=  SKBitmap.Decode(@"C:\Users\jxw\Downloads\APNGManagement-master\APNGManagement-master\APNGViewer\bin\Debug\19.png");
                if (index > 0)
                    rawFrame = SKImage.FromEncodedData(@"C:\Users\jxw\Downloads\APNGManagement-master\APNGManagement-master\APNGViewer\bin\Debug\19.png");

               // using var stream1 = new FileStream(Path.Combine(pathTemp, $"fraem_{index}.png"), FileMode.Create);
               // rawFrame.Encode(SKEncodedImageFormat.Png, 100).SaveTo(stream1);


                using SKBitmap bitmap = new SKBitmap(new SKImageInfo((int)fullSize.Width, (int)fullSize.Height));
                //using SKBitmap bitmap = baseFrame != null ? baseFrame.Copy() : new SKBitmap(new SKImageInfo((int)fullSize.Width, (int)fullSize.Height));
                using SKCanvas canvas = new SKCanvas(bitmap);
                if (baseFrame != null)
                {
                    var fullRect = new SKRect(0, 0, fullSize.Width, fullSize.Height);
                    if (frame.BlendOp == Apng.BlendOps.Source)
                        canvas.DrawBitmap(Apng.ClearArea(baseFrame, frame), fullRect);
                    else
                        canvas.DrawBitmap(baseFrame, fullRect);
                }

                var rect = new SKRect((int)frame.Left, (int)frame.Top, (int)(frame.Left + frame.Width), (int)(frame.Top + frame.Height));
                //canvas.ClipRect(rect);
                SKPaint paint = new SKPaint { BlendMode = SKBlendMode.SrcOver };
                // canvas.DrawBitmap(SKBitmap.Decode(@"C:\Users\jxw\Desktop\17\1.png"), (int)frame.Left, (int)frame.Top);
                canvas.DrawImage(rawFrame, rect, paint);
                //canvas.DrawBitmap(SKBitmap.FromImage(rawFrame), (int)frame.Left, (int)frame.Top);

               
              
                switch (frame.DisposeOp)
                {
                    case Apng.DisposeOps.None: //No disposal is done on this frame before rendering the next; the contents of the output buffer are left as is.
                        baseFrame = bitmap.Copy();
                        break;
                    case Apng.DisposeOps.Background: //The frame's region of the output buffer is to be cleared to fully transparent black before rendering the next frame.
                        baseFrame = baseFrame == null || Apng.IsFullFrame(frame, fullSize) ? null : Apng.ClearArea(baseFrame, frame);
                        break;
                    case Apng.DisposeOps.Previous: //The frame's region of the output buffer is to be reverted to the previous contents before rendering the next frame.
                                                   //Reuse same base frame.
                        break;
                }
              
                string fn = Path.Combine(pathTemp, $"{index}.png");
                using var output = new FileStream(fn, FileMode.Create);
                bitmap.Encode(SKEncodedImageFormat.Png, 100).SaveTo(output);
            }
        }

    }



}
