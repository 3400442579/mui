using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ani.IMG.APNG
{
    public enum DisposeOperation
    {
        NONE, BACKGROUND, PREVIOUS
    }

    public enum BlendOperation
    {
        SOURCE, OVER
    }

    public class Apng : PNG
    {
        public AcTLChunk AcTL { get; set; }
        protected IList<Frame> frames;

        public Apng()
        {
            frames = new List<Frame>();
            chunks = new HashSet<PNGChunk>();
        }

        public int FrameCount
        {
            get
            {
                return frames.Count;
            }
        }

        public bool IsAnimated
        {
            get
            {
                return frames.Count > 0;
            }
        }

        public uint MaxPlays
        {
            get
            {
                return AcTL.NumPlays;
            }
        }

        public void AddFrame(Frame f)
        {
            frames.Add(f);
        }

        public void RemoveFrame(Frame f)
        {
            frames.Remove(f);
        }

        public override SKBitmap ToBitmap()
        {
            return DefaultImageToStream();
        }



        public Stream ToStream()
        {
            Validate();

            Stream s = new MemoryStream();
            WriteSignature(s);
            WriteChunk(s, IHDR);
            WriteChunk(s, AcTL);
            WriteAncillaryChunks(s);
            Frame first = frames.First();
            if (first.IFrame)
            {
                WriteChunk(s, first.Fctl);
                foreach (IDATChunk idat in first.IDATs)
                    WriteChunk(s, idat);

            }
            else
            {
                foreach (IDATChunk idat in IDATList)
                    WriteChunk(s, idat);

                WriteChunk(s, first.Fctl);
                foreach (FdATChunk fdat in first.FdATs)
                    WriteChunk(s, fdat);

            }
            foreach (Frame f in frames.Skip(1))
            {
                WriteChunk(s, f.Fctl);
                foreach (FdATChunk fdat in f.FdATs)
                    WriteChunk(s, fdat);

            }
            WriteChunk(s, IEND);
            return s;
        }

        public SKBitmap ToBitmap(int index, out Frame frame)
        {
            Validate();

            using MemoryStream s = new MemoryStream();
            frame = GetFrame(index);
            WriteImageData(s, frame.ImageData, frame.Width, frame.Height);
            s.Position = 0;
            return SKBitmap.Decode(s);
        }

        public SKBitmap ToBitmap(int index)
        {
            Validate();

            using Stream s = new MemoryStream();
            Frame frame = GetFrame(index);
            WriteImageData(s, frame.ImageData, frame.Width, frame.Height);
            s.Position = 0;
            return SKBitmap.Decode(s);
        }

        public SKBitmap DefaultImageToStream()
        {
            Validate();

            IList<byte> imageData = new List<byte>();
            foreach (IDATChunk idat in IDATList)
            {
                foreach (byte b in idat.ImageData)
                    imageData.Add(b);

            }
            using Stream s = new MemoryStream();
            WriteImageData(s, imageData);
            s.Position = 0;
            return SKBitmap.Decode(s);

        }

        public Frame GetFrame(int index)
        {
            return frames[index];
        }

        public override void Validate()
        {
            base.Validate();
            if (AcTL != null && AcTL.NumFrames != frames.Count)
            {
                throw new ApplicationException("Number of frames not specified correctly in acTL chunk");
            }
        }

        protected override bool HandleChunk(PNGChunk chunk)
        {
            switch (chunk.ChunkType)
            {
                case IDATChunk.NAME:
                    Handle_IDAT(chunk);
                    break;
                case FcTLChunk.NAME:
                    Handle_fcTL(chunk);
                    break;
                case FdATChunk.NAME:
                    Handle_fdAT(chunk);
                    break;
                case AcTLChunk.NAME:
                    Handle_acTL(chunk);
                    break;
                default:
                    return base.HandleChunk(chunk);
            }
            return true;
        }

        private void Handle_acTL(PNGChunk chunk)
        {
            if (AcTL != null)
            {
                throw new ApplicationException("acTL defined more than once");
            }
            AcTL = new AcTLChunk
            {
                ChunkData = chunk.ChunkData
            };
        }

        private void Handle_fcTL(PNGChunk chunk)
        {
            bool IFrame = IDATList.Count < 1;
            FcTLChunk fctlC = new FcTLChunk
            {
                ChunkData = chunk.ChunkData
            };
            if ((fctlC.XOffset + fctlC.Width) > IHDR.Width || (fctlC.YOffset + fctlC.Height) > IHDR.Height)
            {
                throw new ApplicationException("Frame is outside of image space");
            }
            Frame f = new Frame(IFrame, fctlC);
            frames.Add(f);
        }

        private void Handle_fdAT(PNGChunk chunk)
        {
            FdATChunk fdatC = new FdATChunk
            {
                ChunkData = chunk.ChunkData
            };
            Frame f = frames.LastOrDefault();
            if (f == null)
            {
                throw new ApplicationException("No fctl chunk defined, fdat chunk received out of order");
            }
            else
            {
                f.AddChunk(fdatC);
            }
        }

        private void Handle_IDAT(PNGChunk chunk)
        {
            IDATChunk idatC = new IDATChunk
            {
                ChunkData = chunk.ChunkData
            };
            IDATList.Add(idatC);

            if (frames.Count > 1)
            {
                throw new ApplicationException("IDAT chunk encountered out of order");
            }
            else if (frames.Count == 1)
            {
                Frame f = frames.First();
                f.AddChunk(idatC);
            }
        }





        //public void sil()
        //{
        //    string pathTemp = @"test\png";

        //    using var stream = new FileStream(@"test\world-cup-2014-42.png", FileMode.Open);
        //    var apng = new Apng();
        //    apng.Load(stream);



        //    if (!apng.IsAnimated)
        //        return;

        //    SKBitmap baseFrame = null;

        //    var fullSize = new SKSize((int)apng.Width, (int)apng.Height);
        //    for (int index = 0; index < apng.FrameCount; index++)
        //    {
        //        SKBitmap rawFrame = apng.ToBitmap(index, out Frame frame);


        //        var bitmapSource = Render.MakeFrame(fullSize, rawFrame, frame, baseFrame);
        //        switch (frame.DisposeOp)
        //        {
        //            case DisposeOperation.NONE:
        //                baseFrame = bitmapSource.Copy();
        //                break;
        //            case DisposeOperation.BACKGROUND:
        //                baseFrame = Render.IsFullFrame(frame, fullSize) ? null : Render.ClearArea(bitmapSource, frame);
        //                break;
        //            case DisposeOperation.PREVIOUS:
        //                //Reuse same base frame.
        //                break;
        //        }


        //        string fn = Path.Combine(pathTemp, $"{index}.png");
        //        using var output = new FileStream(fn, FileMode.Create);
        //        bitmapSource.Encode(SKEncodedImageFormat.Png, 100).SaveTo(output);
        //    }

        //}

    }
}
