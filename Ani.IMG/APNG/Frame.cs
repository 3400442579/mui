using System;
using System.Collections.Generic;

namespace Ani.IMG.APNG
{
    public class Frame
    {
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

            //set
            //{
            //    Fctl.Width = value;
            //}
        }

        public uint Height
        {
            get
            {
                return Fctl.Height;
            }
            //set
            //{
            //    Fctl.Height = value;
            //}
        }

        public uint XOffset
        {
            get
            {
                return Fctl.XOffset;
            }
            //set
            //{
            //    Fctl.XOffset = value;
            //}
        }

        public uint YOffset
        {
            get
            {
                return Fctl.YOffset;
            }
            //set
            //{
            //    Fctl.YOffset = value;
            //}
        }

        //public ushort DelayNumerator
        //{
        //    get
        //    {
        //        return Fctl.DelayNumerator;
        //    }
        //    set
        //    {
        //        Fctl.DelayNumerator = value;
        //        milliFlag = false;
        //    }
        //}

        //public ushort DelayDenominator
        //{
        //    get
        //    {
        //        return Fctl.DelayDenominator;
        //    }
        //    set
        //    {
        //        Fctl.DelayDenominator = value;
        //        milliFlag = false;
        //    }
        //}


        public int Delay
        {
            get => (int)(Fctl.DelayNumerator / (float)Fctl.DelayDenominator * 1000);
        }

        

        public DisposeOperation DisposeOp
        {
            get
            {
                return Fctl.DisposeOperation switch
                {
                    0 => DisposeOperation.NONE,
                    1 => DisposeOperation.BACKGROUND,
                    2 => DisposeOperation.PREVIOUS,
                    _ => throw new ApplicationException("Invalid Dispose Op"),
                };
            }
            //set
            //{
            //    Fctl.DisposeOperation = (byte)value;
            //}
        }

        public BlendOperation BlendOp
        {
            get
            {
                return Fctl.BlendOperation switch
                {
                    0 => BlendOperation.SOURCE,
                    1 => BlendOperation.OVER,
                    _ => throw new ApplicationException("Invalid Blend Op"),
                };
            }
            //set
            //{
            //    Fctl.BlendOperation = (byte)value;
            //}
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
