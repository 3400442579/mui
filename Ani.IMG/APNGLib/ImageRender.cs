using SkiaSharp;

namespace APNGLib
{
    public static class ImageRender
    {
        public static void DisposeBuffer(SKBitmap buffer, SKRect region, Frame.DisposeOperation dispose, SKBitmap prevBuffer)
        {
            using SKCanvas canvas = new SKCanvas(buffer);
            //using (Graphics g = Graphics.FromImage(buffer))
            using SKPaint paint = new SKPaint
            {
                BlendMode = SKBlendMode.Src,
                Color = SKColors.Transparent
            };
            //g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;

            //Brush b = new SolidBrush(Color.Transparent);
            switch (dispose)
            {
                case Frame.DisposeOperation.NONE:
                    break;
                case Frame.DisposeOperation.BACKGROUND:
                    canvas.DrawRect(region, paint);
                    break;
                case Frame.DisposeOperation.PREVIOUS:
                    if (prevBuffer != null)
                    {
                        canvas.DrawRect(region, paint);
                        //g.FillRectangle(b, region);
                        canvas.DrawBitmap(prevBuffer, region, region, paint);
                    }
                    break;
                default:
                    break;
            }

        }

        public static void RenderNextFrame(SKBitmap buffer, SKPoint point, SKBitmap nextFrame, Frame.BlendOperation blend)
        {
            
            using SKCanvas canvas = new SKCanvas(buffer);
            //using (Graphics g = Graphics.FromImage(buffer))
            using SKPaint paint = new SKPaint();
            switch (blend)
            {
                case Frame.BlendOperation.OVER:
                    paint.BlendMode = SKBlendMode.SrcOver;
                    //g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                    break;
                case Frame.BlendOperation.SOURCE:
                    paint.BlendMode = SKBlendMode.Src;
                    //g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                    break;
                default:
                    break;
            }
            canvas.DrawBitmap(nextFrame, point, paint);

        }

        public static void ClearFrame(SKBitmap buffer)
        {
            using SKCanvas canvas = new SKCanvas(buffer);
            canvas.Clear(SKColors.Transparent);
            //using (Graphics g = Graphics.FromImage(buffer))
            //{
            //    g.Clear(Color.Transparent);
            //}
        }
    }
}
