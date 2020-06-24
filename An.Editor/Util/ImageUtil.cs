using An.Image.Gif.Decoder;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace An.Editor.Util
{
    public class ImageUtil
    {
        #region Import From Gif

        public static BitmapDecoder GetDecoder(string fileName, out GifFile gifFile)
        {
            gifFile = null;
            BitmapDecoder decoder = null;

            using (var stream = new FileStream(fileName, FileMode.Open))
            {
                stream.Position = 0;
                decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);

                if (decoder is GifBitmapDecoder)// && !CanReadNativeMetadata(decoder))
                {
                    stream.Position = 0;
                    gifFile = GifFile.ReadGifFile(stream, true);
                }

                //if (decoder == null)
                //    throw new InvalidOperationException("Can't get a decoder from the source.");
            }

            return decoder;
        }

        private static bool CanReadNativeMetadata(BitmapDecoder decoder)
        {
            try
            {
                var m = decoder.Metadata;
                return m != null;
            }
            catch
            {
                return false;
            }
        }

        //public static System.Drawing.Size GetFullSize(BitmapDecoder decoder, GifFile gifMetadata)
        //{
        //    if (gifMetadata != null)
        //    {
        //        var lsd = gifMetadata.Header.LogicalScreenDescriptor;
        //        return new System.Drawing.Size(lsd.Width, lsd.Height);
        //    }

        //    var width = decoder.Metadata.GetQueryOrDefault("/logscrdesc/Width", 0);
        //    var height = decoder.Metadata.GetQueryOrDefault("/logscrdesc/Height", 0);
        //    return new System.Drawing.Size(width, height);
        //}

        private static T GetQueryOrDefault<T>(this BitmapMetadata metadata, string query, T defaultValue)
        {
            if (metadata.ContainsQuery(query))
                return (T)Convert.ChangeType(metadata.GetQuery(query), typeof(T));

            return defaultValue;
        }

        public static FrameMetadata GetFrameMetadata(GifFile gifMetadata, int frameIndex)
        {
            return GetFrameMetadata(gifMetadata.Frames[frameIndex]);
        }

        //private static FrameMetadata GetFrameMetadata(BitmapFrame frame)
        //{
        //    var metadata = (BitmapMetadata)frame.Metadata;
        //    var delay = TimeSpan.FromMilliseconds(100);
        //    var metadataDelay = metadata.GetQueryOrDefault("/grctlext/Delay", 10);

        //    if (metadataDelay != 0)
        //        delay = TimeSpan.FromMilliseconds(metadataDelay * 10);

        //    var disposalMethod = (FrameDisposalMethod)metadata.GetQueryOrDefault("/grctlext/Disposal", 0);

        //    var frameMetadata = new FrameMetadata
        //    {
        //        Left = metadata.GetQueryOrDefault("/imgdesc/Left", 0),
        //        Top = metadata.GetQueryOrDefault("/imgdesc/Top", 0),
        //        Width = metadata.GetQueryOrDefault("/imgdesc/Width", frame.PixelWidth),
        //        Height = metadata.GetQueryOrDefault("/imgdesc/Height", frame.PixelHeight),
        //        Delay = delay,
        //        DisposalMethod = disposalMethod
        //    };

        //    return frameMetadata;
        //}

        private static FrameMetadata GetFrameMetadata(GifFrame gifMetadata)
        {
            var d = gifMetadata.Descriptor;

            var frameMetadata = new FrameMetadata
            {
                Left = d.Left,
                Top = d.Top,
                Width = d.Width,
                Height = d.Height,
                Delay = TimeSpan.FromMilliseconds(100),
                DisposalMethod = FrameDisposalMethod.None
            };

            var gce = gifMetadata.Extensions.OfType<GifGraphicControlExtension>().FirstOrDefault();

            if (gce == null)
                return frameMetadata;

            if (gce.Delay != 0)
                frameMetadata.Delay = TimeSpan.FromMilliseconds(gce.Delay);

            frameMetadata.DisposalMethod = (FrameDisposalMethod)gce.DisposalMethod;

            return frameMetadata;
        }

        public static BitmapSource MakeFrame(System.Drawing.Size fullSize, BitmapSource rawFrame, FrameMetadata metadata, BitmapSource baseFrame)
        {
            //I removed this, so I could save the same as 32bpp
            //if (baseFrame == null && IsFullFrame(metadata, fullSize))
            //{
            //    // No previous image to combine with, and same size as the full image
            //    // Just return the frame as is
            //    return rawFrame;
            //}

            var visual = new DrawingVisual();
            using (var context = visual.RenderOpen())
            {
                if (baseFrame != null)
                {
                    var fullRect = new Rect(0, 0, fullSize.Width, fullSize.Height);
                    context.DrawImage(baseFrame, fullRect);
                }

                var rect = new Rect(metadata.Left, metadata.Top, metadata.Width, metadata.Height);
                context.DrawImage(rawFrame, rect);
            }

            //TODO: Test, DPI was hardcoded to 96.
            var bitmap = new RenderTargetBitmap(fullSize.Width, fullSize.Height, rawFrame.DpiX, rawFrame.DpiY, PixelFormats.Pbgra32);
            bitmap.Render(visual);

            if (bitmap.CanFreeze && !bitmap.IsFrozen)
                bitmap.Freeze();

            return bitmap;
        }

        public static bool IsFullFrame(FrameMetadata metadata, int width, int height)
        {
            return metadata.Left == 0 && metadata.Top == 0 && metadata.Width == width && metadata.Height == height;
        }

        public static BitmapSource ClearArea(BitmapSource frame, FrameMetadata metadata)
        {
            var visual = new DrawingVisual();
            using (var context = visual.RenderOpen())
            {
                var fullRect = new Rect(0, 0, frame.PixelWidth, frame.PixelHeight);
                var clearRect = new Rect(metadata.Left, metadata.Top, metadata.Width, metadata.Height);
                var clip = Geometry.Combine(new RectangleGeometry(fullRect), new RectangleGeometry(clearRect), GeometryCombineMode.Exclude, null);

                context.PushClip(clip);
                context.DrawImage(frame, fullRect);
            }

            var bitmap = new RenderTargetBitmap(frame.PixelWidth, frame.PixelHeight, frame.DpiX, frame.DpiY, PixelFormats.Pbgra32);
            bitmap.Render(visual);

            if (bitmap.CanFreeze && !bitmap.IsFrozen)
                bitmap.Freeze();

            return bitmap;
        }

        /// <summary>
        /// Return frame(s) as list of binary from jpeg, png, bmp or gif image file
        /// </summary>
        /// <param name="fileName">image file name</param>
        /// <returns>System.Collections.Generic.List of byte</returns>
        [Obsolete]
        public static List<Bitmap> GetFrames(string fileName)
        {
            var tmpFrames = new List<byte[]>();

            // Check the image format to determine what format
            // the image will be saved to the memory stream in
            var guidToImageFormatMap = new Dictionary<Guid, ImageFormat>()
            {
                {ImageFormat.Bmp.Guid,  ImageFormat.Bmp},
                {ImageFormat.Gif.Guid,  ImageFormat.Png},
                {ImageFormat.Icon.Guid, ImageFormat.Png},
                {ImageFormat.Jpeg.Guid, ImageFormat.Jpeg},
                {ImageFormat.Png.Guid,  ImageFormat.Png}
            };

            using (var gifImg =System.Drawing. Image.FromFile(fileName, true))
            {
                var imageGuid = gifImg.RawFormat.Guid;

                var imageFormat = (from pair in guidToImageFormatMap where imageGuid == pair.Key select pair.Value).FirstOrDefault();

                if (imageFormat == null)
                    throw new NoNullAllowedException("Unable to determine image format");

                //Get the frame count
                var dimension = new FrameDimension(gifImg.FrameDimensionsList[0]);
                var frameCount = gifImg.GetFrameCount(dimension);

                //Step through each frame
                for (var i = 0; i < frameCount; i++)
                {
                    //Set the active frame of the image and then
                    gifImg.SelectActiveFrame(dimension, i);

                    //write the bytes to the tmpFrames array
                    using (var ms = new MemoryStream())
                    {
                        gifImg.Save(ms, imageFormat);
                        tmpFrames.Add(ms.ToArray());
                    }
                }

                //Get list of frame(s) from image file.
                var myBitmaps = new List<Bitmap>();

                foreach (var item in tmpFrames)
                {
                    var tmpBitmap = ConvertBytesToImage(item);

                    if (tmpBitmap != null)
                    {
                        myBitmaps.Add(tmpBitmap);
                    }
                }

                return myBitmaps;
            }
        }

        /// <summary>
        /// Convert bytes to Bitamp
        /// </summary>
        /// <param name="imageBytes">Image in a byte type</param>
        /// <returns>System.Drawing.Bitmap</returns>
        private static Bitmap ConvertBytesToImage(byte[] imageBytes)
        {
            if (imageBytes == null || imageBytes.Length == 0)
                return null;

            //Read bytes into a MemoryStream
            using (var ms = new MemoryStream(imageBytes))
            {
                //Recreate the frame from the MemoryStream
                using (var bmp = new Bitmap(ms))
                    return (Bitmap)bmp.Clone();
            }
        }

        #endregion
    }
}
