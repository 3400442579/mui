using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace APNGLib
{
    public class APNG : PNG
    {
        public AcTLChunk AcTL { get; set; }
        protected IList<Frame> frames;

        public APNG()
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
            Stream s = DefaultImageToStream();
            SKBitmap b = SKBitmap.Decode(s);
            return b;
        }

        public SKBitmap ToBitmap(int index)
        {
            Stream s = ToStream(index);
            SKBitmap b = SKBitmap.Decode(s) ;

           // b.Save($"{index}.png");
            return b;
        }

        public override Stream ToStream()
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
                {
                    WriteChunk(s, idat);
                }
            }
            else
            {
                foreach (IDATChunk idat in IDATList)
                {
                    WriteChunk(s, idat);
                }
                WriteChunk(s, first.Fctl);
                foreach (FdATChunk fdat in first.FdATs)
                {
                    WriteChunk(s, fdat);
                }
            }
            foreach (Frame f in frames.Skip(1))
            {
                WriteChunk(s, f.Fctl);
                foreach (FdATChunk fdat in f.FdATs)
                {
                    WriteChunk(s, fdat);
                }
            }
            WriteChunk(s, IEND);
            return s;
        }

        public Stream ToStream(int index)
        {
            Validate();

            Stream s = new MemoryStream();
            Frame f = GetFrame(index);
            WriteImageData(s, f.ImageData, f.Width, f.Height);
            return s;
        }

        public Stream DefaultImageToStream()
        {
            Validate();

            IList<byte> imageData = new List<byte>();
            foreach (IDATChunk idat in IDATList)
            {
                foreach (byte b in idat.ImageData)
                {
                    imageData.Add(b);
                }
            }
            Stream s = new MemoryStream();
            WriteImageData(s, imageData);
            return s;
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
    }
}
