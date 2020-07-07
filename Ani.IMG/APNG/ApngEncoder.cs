using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Ani.IMG.APNG
{
    public class ApngEncoder
    {
        #region Properties

        /// <summary>
        /// The stream which the apgn is writen on.
        /// </summary>
        private Stream InternalStream { get; set; }



        /// <summary>
        /// Repeat Count for the apng.
        /// </summary>
        public int RepeatCount { get; set; } = 0;

        /// <summary>
        /// True if it's the first frame of the apgn.
        /// </summary>
        private bool IsFirstFrame { get; set; } = true;

        /// <summary>
        /// The sequence number of frame.
        /// </summary>
        private uint SequenceNumber { get; set; } = 0;

        private Apng apng;

        #endregion
        public ApngEncoder(Stream stream, uint frameCount, int repeatCount)
        {
            InternalStream = stream;

            RepeatCount = repeatCount;

            apng = new Apng();

            apng.AcTL.NumFrames = frameCount;
        }


        public void AddFrame(string path, SKRectI rect, ushort delay = 66)
        {
            PNG first = new PNG();
            using Stream s = File.OpenRead(path);
            first.Load(s);

            if (IsFirstFrame)
            {
                SetupAPNGChunks(first);

                Frame firstFrame = CreateFrame((uint)rect.Height, (uint)rect.Width, 0, 0, delay, IsFirstFrame, first.IDATList);
                apng.AddFrame(firstFrame);

                IsFirstFrame = false;
            }
            else
            {
                PNG png = new PNG();
                using Stream fileStr = File.OpenRead(path);

                //if (optimize)
                //{
                //    using Stream optStr = OptimizeBitmapStream(fileStr, out p);
                //    png.Load(optStr);
                //}
                //else
                png.Load(fileStr);

                Frame f = CreateFrame(png.Height, png.Width, (uint)rect.Left, (uint)rect.Top, delay, false, png.IDATList);
                apng.AddFrame(f);
            }
            // apng.AcTL.NumFrames = (uint)apng.FrameCount;

            //apng.Validate();
        }

        private void SetupAPNGChunks(PNG png)
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

        private Frame CreateFrame(uint h, uint w, uint xoff, uint yoff, ushort delay, bool first, IList<IDATChunk> idats)
        {
            FcTLChunk fctl = new FcTLChunk()
            {
                DelayNumerator = delay,
                DelayDenominator = 1000,
                Height = h,
                Width = w,
                DisposeOperation = 1,
                BlendOperation = 0,
                XOffset = xoff,
                YOffset = yoff,
                SequenceNumber = SequenceNumber++
            };
            Frame f = new Frame(first, fctl);
            foreach (IDATChunk idat in idats)
            {
                if (first)
                    f.AddChunk(idat);
                else
                    f.AddChunk(new FdATChunk()
                    {
                        FrameData = idat.ImageData,
                        SequenceNumber = SequenceNumber++
                    });
            }
            return f;
        }

    }
}
