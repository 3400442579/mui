using System;
using System.Collections.Generic;

namespace APNGLib
{
    public class Frame
    {
        public enum DisposeOperation
        {
            NONE, BACKGROUND, PREVIOUS
        }

        public enum BlendOperation
        {
            SOURCE, OVER
        }

        public FcTLChunk Fctl { get; private set; }
        private readonly IList<IDATChunk> idats;
        private readonly IList<FdATChunk> fdats;
        public bool IFrame { get; set; }

        public IEnumerable<IDATChunk> IDATs
        {
            get
            {
                return idats;
            }
        }

        public IEnumerable<FdATChunk> FdATs
        {
            get
            {
                return fdats;
            }
        }

        public void AddChunk(IDATChunk i)
        {
            if (IFrame)
            {
                idats.Add(i);
            }
            else
            {
                throw new ApplicationException("Cannot add IDAT chunk to fdAT frame");
            }
        }

        public void AddChunk(FdATChunk f)
        {
            if (IFrame)
            {
                throw new ApplicationException("Cannot add fdAT chunk to IDAT frame");
            }
            else
            {
                fdats.Add(f);
            }
        }

        public uint Width
        {
            get
            {
                return Fctl.Width;
            }

            set
            {
                Fctl.Width = value;
            }
        }

        public uint Height
        {
            get
            {
                return Fctl.Height;
            }
            set
            {
                Fctl.Height = value;
            }
        }

        public uint XOffset
        {
            get
            {
                return Fctl.XOffset;
            }
            set
            {
                Fctl.XOffset = value;
            }
        }

        public uint YOffset
        {
            get
            {
                return Fctl.YOffset;
            }
            set
            {
                Fctl.YOffset = value;
            }
        }

        public ushort DelayNumerator
        {
            get
            {
                return Fctl.DelayNumerator;
            }
            set
            {
                Fctl.DelayNumerator = value;
                milliFlag = false;
                secFlag = false;
            }
        }

        public ushort DelayDenominator
        {
            get
            {
                return Fctl.DelayDenominator;
            }
            set
            {
                Fctl.DelayDenominator = value;
                milliFlag = false;
                secFlag = false;
            }
        }

        private bool milliFlag = false;
        private int milli;
        public int Milliseconds
        {
            get
            {
                if (!milliFlag)
                {
                    const int MillisecondsPerSecond = 1000;
                    milli = (int)(Seconds * MillisecondsPerSecond);
                    milliFlag = true;
                }
                return milli;
            }
        }

        private bool secFlag = false;
        private float sec;
        public float Seconds
        {
            get
            {
                if (!secFlag)
                {
                    sec = (float)DelayNumerator / (float)DelayDenominator;
                    secFlag = true;
                }
                return sec;
            }
        }

        public DisposeOperation DisposeOp
        {
            get
            {
                switch(Fctl.DisposeOperation)
                {
                    case 0:
                        return DisposeOperation.NONE;
                    case 1:
                        return DisposeOperation.BACKGROUND;
                    case 2:
                        return DisposeOperation.PREVIOUS;
                    default:
                        throw new ApplicationException("Invalid Dispose Op");
                }
            }
            set
            {
                Fctl.DisposeOperation = (byte)value;
            }
        }

        public BlendOperation BlendOp
        {
            get
            {
                switch (Fctl.BlendOperation)
                {
                    case 0:
                        return BlendOperation.SOURCE;
                    case 1:
                        return BlendOperation.OVER;
                    default:
                        throw new ApplicationException("Invalid Blend Op");
                }
            }
            set
            {
                Fctl.BlendOperation = (byte)value;
            }
        }

        public Frame(bool first, FcTLChunk fChunk)
        {
            IFrame = first;
            if (IFrame)
            {
                idats = new List<IDATChunk>();
            }
            else
            {
                fdats = new List<FdATChunk>();
            }
            Fctl = fChunk;
        }

        public IList<byte> ImageData
        {
            get
            {
                IList<byte> value = new List<byte>();
                if (IFrame)
                {
                    foreach (IDATChunk id in idats)
                    {
                        foreach (byte b in id.ImageData)
                        {
                            value.Add(b);
                        }
                    }
                }
                else
                {
                    foreach (FdATChunk fd in fdats)
                    {
                        foreach (byte b in fd.FrameData)
                        {
                            value.Add(b);
                        }
                    }
                }
                return value;
            }
        }
    }
}
