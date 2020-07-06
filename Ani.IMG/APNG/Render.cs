using SkiaSharp;

namespace Ani.IMG.APNG
{
    public static class Render
    {
        public static SKBitmap MakeFrame(SKSize fullSize, SKBitmap rawFrame, Frame frame, SKBitmap baseFrame)
        {
            SKBitmap bitmap = new SKBitmap(new SKImageInfo((int)fullSize.Width, (int)fullSize.Height));
            using SKCanvas canvas = new SKCanvas(bitmap);
            using SKPaint paint = new SKPaint { BlendMode = SKBlendMode.SrcOver };
            canvas.Clear(SKColors.Transparent);
            if (baseFrame != null)
            {
                var fullRect = new SKRect(0, 0, fullSize.Width, fullSize.Height);
                if (frame.BlendOp == BlendOperation.SOURCE)
                    canvas.DrawBitmap(ClearArea(baseFrame, frame), fullRect, paint);
                else
                    canvas.DrawBitmap(baseFrame, fullRect);
            }
            canvas.DrawBitmap(rawFrame, new SKPoint((int)frame.XOffset, (int)frame.YOffset), paint);

            return bitmap;
        }


        public static SKBitmap ClearArea(SKBitmap frame, Frame metadata)
        {
            SKBitmap sKBitmap = new SKBitmap(new SKImageInfo(frame.Width, frame.Height));
            using SKCanvas canvas = new SKCanvas(sKBitmap);
            using SKPaint paint = new SKPaint { BlendMode = SKBlendMode.SrcOver };

            canvas.Clear(SKColors.Transparent);
            var fullRect = new SKRect(0, 0, frame.Width, frame.Height);
            var clearRect = new SKRect((int)metadata.XOffset, (int)metadata.YOffset, (int)metadata.XOffset + (int)metadata.Width, (int)metadata.YOffset + (int)metadata.Height);

            canvas.ClipRect(clearRect, SKClipOperation.Difference);
            canvas.DrawBitmap(frame, fullRect);

            return sKBitmap;
        }

        public static bool IsFullFrame(Frame frame, SKSize fullSize)
        {
            return frame.XOffset == 0 && frame.YOffset == 0 && frame.Width == fullSize.Width && frame.Height == fullSize.Height;
        }


     
         
    }
}
