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

            using var stream = new FileStream(@"test\world-cup-2014-42.png", FileMode.Open);
            var apng = new Apng(stream);

            if (!apng.ReadFrames())
                return;

            var fullSize = new SKSize((int)apng.Ihdr.Width, (int)apng.Ihdr.Height);

            SKBitmap baseFrame = null;

            for (var index = 0; index < apng.Actl.NumFrames; index++)
            {
                var frame = apng.GetFrame(index);

                SKBitmap rawFrame = SKBitmap.Decode(@"C:\Users\jxw\Desktop\400X200.png");
                baseFrame = SKBitmap.Decode(@"C:\Users\jxw\Desktop\firefox-512.png");

                SKBitmap bitmap = new SKBitmap(new SKImageInfo((int)baseFrame.Width, (int)baseFrame.Height));
                using SKCanvas canvas = new SKCanvas(bitmap);

                if (baseFrame != null)
                {
                    var fullRect = new SKRect(0, 0, fullSize.Width, fullSize.Height);
                    //if (frame.BlendOp == Apng.BlendOps.Source)
                    //    canvas.DrawBitmap(Apng.ClearArea(baseFrame, frame), fullRect);
                    //else
                        canvas.DrawBitmap(baseFrame, fullRect);
                }

                canvas.DrawBitmap(rawFrame, new SKRect((int)frame.Left, (int)frame.Top, (int)frame.Left + (int)frame.Width, (int)frame.Top + (int)frame.Height));


                #region Disposal Method
                switch (frame.DisposeOp)
                {
                    case Apng.DisposeOps.None: //No disposal is done on this frame before rendering the next; the contents of the output buffer are left as is.
                        baseFrame = bitmap;
                        break;
                    case Apng.DisposeOps.Background: //The frame's region of the output buffer is to be cleared to fully transparent black before rendering the next frame.
                        baseFrame = baseFrame == null || Apng.IsFullFrame(frame, fullSize) ? null : Apng.ClearArea(baseFrame, frame);
                        break;
                    case Apng.DisposeOps.Previous: //The frame's region of the output buffer is to be reverted to the previous contents before rendering the next frame.
                                                   //Reuse same base frame.
                        break;
                }
                #endregion

                var fileName = Path.Combine(pathTemp, $"{index}.png");
                using var output = new FileStream(fileName, FileMode.Create);
                var encode = bitmap.Encode(SKEncodedImageFormat.Png, 100);
                encode.SaveTo(output);
            }
        }


    }



}
