using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Ani.IMG.APNG
{
    public static class APNGAssembler
    {
        public static Apng AssembleAPNG(IList<string> files, bool optimize)
        {
            if (files.Count < 1)
                return null;
            
            uint sequenceCount = 0;
            Apng apng = new Apng();
            PNG first = new PNG();
            using (Stream s = File.OpenRead(files.First()))
            {
                first.Load(s);
            }
            SetupAPNGChunks(apng, first);
            Frame firstFrame = CreateFrame(first.Height, first.Width, 0, 0, ref sequenceCount, true, first.IDATList);
            apng.AddFrame(firstFrame);

            foreach (string file in files.Skip(1))
            {
                Point p = new Point(0, 0);
                PNG png = new PNG();
                using (Stream fileStr = File.OpenRead(file))
                {
                    if (optimize)
                    {
                        using Stream optStr = OptimizeBitmapStream(fileStr, out p);
                        png.Load(optStr);
                    }
                    else
                        png.Load(fileStr);
                }
                Frame f = CreateFrame(png.Height, png.Width, (uint)p.X, (uint)p.Y, ref sequenceCount, false, png.IDATList);
                apng.AddFrame(f);
            }
            apng.AcTL.NumFrames = (uint)apng.FrameCount;

            apng.Validate();
            return apng;
        }

        public static Apng AssembleAPNG(IList<FileInfo> files, bool optimize)
        {
            IList<string> filenames = new List<string>();
            foreach (FileInfo fi in files)
            {
                filenames.Add(fi.FullName);
            }
            return AssembleAPNG(filenames, optimize);
        }

        private static Frame CreateFrame(uint h, uint w, uint xoff, uint yoff, ref uint seq, bool first, IList<IDATChunk> idats)
        {
            FcTLChunk fctl = new FcTLChunk()
            {
                DelayNumerator = 1,
                DelayDenominator = 10,
                Height = h,
                Width = w,
                DisposeOperation = 1,
                BlendOperation = 0,
                XOffset = xoff,
                YOffset = yoff,
                SequenceNumber = seq++
            };
            Frame f = new Frame(first, fctl);
            foreach (IDATChunk idat in idats)
            {
                if (first)
                {
                    f.AddChunk(idat);
                }
                else
                {
                    FdATChunk fdat = new FdATChunk()
                    {
                        FrameData = idat.ImageData,
                        SequenceNumber = seq++
                    };
                    f.AddChunk(fdat);
                }
            }
            return f;
        }

        private static void SetupAPNGChunks(Apng apng, PNG png)
        {
            apng.IHDR = png.IHDR;
            apng.AcTL = new AcTLChunk()
            {
                NumPlays = 0
            };
            foreach (IDATChunk chunk in png.IDATList)
            {
                apng.IDATList.Add(chunk);
            }
            apng.IEND = png.IEND;
            apng.PLTE = png.PLTE;
            apng.TRNS = png.TRNS;
            apng.CHRM = png.CHRM;
            apng.GAMA = png.GAMA;
            apng.ICCP = png.ICCP;
            apng.SBIT = png.SBIT;
            apng.SRGB = png.SRGB;
            foreach (TEXtChunk chunk in png.TEXtList)
            {
                apng.TEXtList.Add(chunk);
            }
            foreach (ZTXtChunk chunk in png.ZTXtList)
            {
                apng.ZTXtList.Add(chunk);
            }
            foreach (ITXtChunk chunk in png.ITXtList)
            {
                apng.ITXtList.Add(chunk);
            }
            apng.BKGD = png.BKGD;
            apng.HIST = png.HIST;
            apng.PHYs = png.PHYs;
            apng.SPLT = png.SPLT;

            DateTime dt = DateTime.Now;
            apng.TIME = new TIMEChunk()
            {
                Day = (byte)dt.Day,
                Month = (byte)dt.Month,
                Year = (ushort)dt.Year,
                Hour = (byte)dt.Hour,
                Minute = (byte)dt.Minute,
                Second = (byte)dt.Second
            };
        }

        private static Stream OptimizeBitmapStream(Stream bmStr, out Point p)
        {
            using SKBitmap bm = SKBitmap.Decode(bmStr);
            using SKBitmap opt = TrimBitmap(bm, out p);
            Stream ret = new MemoryStream();
            opt.Encode(SKEncodedImageFormat.Png, 100).SaveTo(ret);
            ret.Position = 0;
            return ret;
        }

        private static SKBitmap TrimBitmap(SKBitmap source, out Point p)
        {
            int xMin = int.MaxValue,
                xMax = int.MinValue,
                yMin = int.MaxValue,
                yMax = int.MinValue;

            bool foundPixel = false;

            // Find xMin
            for (int x = 0; x < source.Width; x++)
            {
                bool stop = false;
                for (int y = 0; y < source.Height; y++)
                {

                    if (source.GetPixel(x, y).Alpha != 0)
                    {
                        xMin = x;
                        stop = true;
                        foundPixel = true;
                        break;
                    }
                }
                if (stop)
                {
                    break;
                }
            }
            if (!foundPixel)
            {
                // Image is empty...
                p = new Point(0, 0);
                return new SKBitmap(1, 1);
            }
            // Find yMin
            for (int y = 0; y < source.Height; y++)
            {
                bool stop = false;
                for (int x = xMin; x < source.Width; x++)
                {
                    //byte alpha = buffer[(y * data.Stride) + (4 * x) + 3];
                    if (source.GetPixel(x, y).Alpha != 0)
                    {
                        yMin = y;
                        stop = true;
                        break;
                    }
                }
                if (stop)
                {
                    break;
                }
            }
            // Find xMax
            for (int x = source.Width - 1; x >= xMin; x--)
            {
                bool stop = false;
                for (int y = yMin; y < source.Height; y++)
                {
                    //byte alpha = buffer[(y * data.Stride) + (4 * x) + 3];
                    if (source.GetPixel(x, y).Alpha != 0)
                    {
                        xMax = x;
                        stop = true;
                        break;
                    }
                }
                if (stop)
                {
                    break;
                }
            }
            // Find yMax
            for (int y = source.Height - 1; y >= yMin; y--)
            {
                bool stop = false;
                for (int x = xMin; x <= xMax; x++)
                {
                    //byte alpha = buffer[(y * data.Stride) + (4 * x) + 3];
                    if (source.GetPixel(x, y).Alpha != 0)
                    {
                        yMax = y;
                        stop = true;
                        break;
                    }
                }
                if (stop)
                {
                    break;
                }
            }

            SKRect srcRect = SKRect.Create(xMin, yMin, xMax - xMin, yMax - yMin);
            p = new Point(xMin, yMin);

            var destination = new SKBitmap(new SKImageInfo((int)srcRect.Width, (int)srcRect.Height));
            source.ExtractSubset(destination, SKRectI.Ceiling(srcRect));
            return destination;
        }
    }
}