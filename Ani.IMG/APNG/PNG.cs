using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;

namespace Ani.IMG.APNG
{
    internal static class PNGSignature
    {
        public static byte[] Signature = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };

        public static void Compare(byte[] sig)
        {
            if (sig.Length == Signature.Length)
            {
                for (int i = 0; i < Signature.Length; i++)
                {
                    // Invalid signature
                    if (Signature[i] != sig[i])
                    {
                        throw new ApplicationException("APNG signature not found.");
                    }
                }
            }
            else
            {
                throw new ApplicationException("APNG signature not found.");
            }
        }
    }

    public class PNG
    {
        public IHDRChunk IHDR { get; set; }
        public IList<IDATChunk> IDATList { get; private set; }
        public IENDChunk IEND { get; set; }

        public PLTEChunk PLTE { get; set; }
        public TRNSChunk TRNS { get; set; }
        public CHRMChunk CHRM { get; set; }
        public GAMAChunk GAMA { get; set; }
        public ICCPChunk ICCP { get; set; }
        public SBITChunk SBIT { get; set; }
        public SRGBChunk SRGB { get; set; }
        public ICollection<TEXtChunk> TEXtList { get; private set; }
        public ICollection<ZTXtChunk> ZTXtList { get; private set; }
        public ICollection<ITXtChunk> ITXtList { get; private set; }
        public BKGDChunk BKGD { get; set; }
        public HISTChunk HIST { get; set; }
        public PHYsChunk PHYs { get; set; }
        public SPLTChunk SPLT { get; set; }
        public TIMEChunk TIME { get; set; }

        protected ICollection<PNGChunk> chunks;

        public PNG()
        {
            IDATList = new List<IDATChunk>();
            TEXtList = new HashSet<TEXtChunk>();
            ZTXtList = new HashSet<ZTXtChunk>();
            ITXtList = new HashSet<ITXtChunk>();

            chunks = new List<PNGChunk>();
        }

        public uint Width
        {
            get
            {
                return IHDR.Width;
            }
        }

        public uint Height
        {
            get
            {
                return IHDR.Height;
            }
        }

        public virtual SKBitmap ToBitmap()
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

        

        public void WriteImageData(Stream s, IList<byte> imageData, uint width, uint height)
        {
            WriteSignature(s);
            
            WriteChunk(s, new IHDRChunk
            {
                ChunkData = IHDR.ChunkData,
                Width = width,
                Height = height
            });

            WriteAncillaryChunks(s);
            
            WriteChunk(s, new IDATChunk
            {
                ChunkData = imageData.ToArray()
            });
            WriteChunk(s, IEND);
        }

    
        protected void WriteImageData(Stream s, IList<byte> imageData)
        {
            WriteImageData(s, imageData, IHDR.Width, IHDR.Height);
        }

        protected void WriteSignature(Stream s)
        {
            s.Write(PNGSignature.Signature, 0, PNGSignature.Signature.Length);
        }

        protected void WriteAncillaryChunks(Stream s)
        {
            WriteChunk(s, PLTE);
            WriteChunk(s, TRNS);
            WriteChunk(s, CHRM);
            WriteChunk(s, GAMA);
            WriteChunk(s, ICCP);
            WriteChunk(s, SBIT);
            WriteChunk(s, SRGB);
            WriteChunk(s, BKGD);
            WriteChunk(s, HIST);
            WriteChunk(s, PHYs);
            WriteChunk(s, SPLT);
            WriteChunk(s, TIME);

            foreach (TEXtChunk text in TEXtList)
                WriteChunk(s, text);

            foreach (ZTXtChunk ztxt in ZTXtList)
                WriteChunk(s, ztxt);

            foreach (ITXtChunk itxt in ITXtList)
                WriteChunk(s, itxt);

            foreach (PNGChunk chunk in chunks)
                WriteChunk(s, chunk);
        }

        protected static void WriteChunk(Stream s, PNGChunk chunk)
        {
            if (chunk != null)
            {
                byte[] chArray = chunk.Chunk;
                s.Write(chArray, 0, chArray.Length);
            }
        }

        protected PNGChunk GetNextChunk(Stream stream)
        {
            PNGChunk value = new PNGChunk();

            byte[] size = new byte[sizeof(uint)];
            stream.Read(size, 0, sizeof(uint));
            uint readLength = PNGUtils.ParseUint(size);

            byte[] type = new byte[4];
            stream.Read(type, 0, 4);
            value.ChunkType = PNGUtils.ParseString(type, 4);

            byte[] data = new byte[readLength];
            stream.Read(data, 0, (int)readLength);
            value.ChunkData = data;

            byte[] crc = new byte[sizeof(uint)];
            stream.Read(crc, 0, sizeof(uint));
            uint readCRC = PNGUtils.ParseUint(crc);

            uint calcCRC = value.CalculateCRC();
            if (readCRC != calcCRC)
            {
                throw new ApplicationException(string.Format("APNG Chunk CRC Mismatch.  Chunk CRC = {0}, Calculated CRC = {1}.", readCRC, calcCRC));
            }
            return value;
        }

        public void Load(Stream stream)
        {
            byte[] sig = new byte[PNGSignature.Signature.Length];
            stream.Read(sig, 0, PNGSignature.Signature.Length);
            PNGSignature.Compare(sig);

            PNGChunk chunk = GetNextChunk(stream);

            if (chunk.ChunkType != IHDRChunk.NAME)
            {
                throw new ApplicationException("First chunk is not IHDR chunk");
            }

            Handle_IHDR(chunk);

            do
            {
                chunk = GetNextChunk(stream);
                if (!HandleChunk(chunk))
                {
                    HandleDefaultChunk(chunk);
                }
            } while (chunk.ChunkType != IENDChunk.NAME);

            Validate();
        }

        protected virtual bool HandleChunk(PNGChunk chunk)
        {
            switch (chunk.ChunkType)
            {
                case IHDRChunk.NAME:
                    Handle_IHDR(chunk);
                    break;
                case PLTEChunk.NAME:
                    Handle_PLTE(chunk);
                    break;
                case IDATChunk.NAME:
                    Handle_IDAT(chunk);
                    break;
                case IENDChunk.NAME:
                    Handle_IEND(chunk);
                    break;
                case TRNSChunk.NAME:
                    Handle_tRNS(chunk);
                    break;
                case CHRMChunk.NAME:
                    Handle_cHRM(chunk);
                    break;
                case GAMAChunk.NAME:
                    Handle_gAMA(chunk);
                    break;
                case ICCPChunk.NAME:
                    Handle_iCCP(chunk);
                    break;
                case SBITChunk.NAME:
                    Handle_sBIT(chunk);
                    break;
                case SRGBChunk.NAME:
                    Handle_sRGB(chunk);
                    break;
                case TEXtChunk.NAME:
                    Handle_tEXt(chunk);
                    break;
                case ZTXtChunk.NAME:
                    Handle_zTXt(chunk);
                    break;
                case ITXtChunk.NAME:
                    Handle_iTXt(chunk);
                    break;
                case BKGDChunk.NAME:
                    Handle_bKGD(chunk);
                    break;
                case HISTChunk.NAME:
                    Handle_hIST(chunk);
                    break;
                case PHYsChunk.NAME:
                    Handle_pHYs(chunk);
                    break;
                case SPLTChunk.NAME:
                    Handle_sPLT(chunk);
                    break;
                case TIMEChunk.NAME:
                    Handle_tIME(chunk);
                    break;
                default:
                    return false;
            }
            return true;
        }

        private void Handle_tIME(PNGChunk chunk)
        {
            if (TIME != null)
            {
                throw new ApplicationException("tIME chunk encountered more than once");
            }
            TIME = new TIMEChunk();
            TIME.ChunkData = chunk.ChunkData;
        }

        private void Handle_sPLT(PNGChunk chunk)
        {
            if (SPLT != null)
            {
                throw new ApplicationException("sPLT chunk encountered more than once");
            }
            SPLT = new SPLTChunk();
            SPLT.ChunkData = chunk.ChunkData;
        }

        private void Handle_pHYs(PNGChunk chunk)
        {
            if (PHYs != null)
            {
                throw new ApplicationException("pHYs chunk encountered more than once");
            }
            PHYs = new PHYsChunk();
            PHYs.ChunkData = chunk.ChunkData;
        }

        private void Handle_hIST(PNGChunk chunk)
        {
            if (HIST != null)
            {
                throw new ApplicationException("hIST chunk encountered more than once");
            }
            HIST = new HISTChunk();
            HIST.ChunkData = chunk.ChunkData;
        }

        private void Handle_bKGD(PNGChunk chunk)
        {
            if (BKGD != null)
            {
                throw new ApplicationException("bKGD chunk encountered more than once");
            }
            switch (IHDR.ColorType)
            {
                case 0:
                    BKGD = new BKGDChunkType0();
                    break;
                case 2:
                    BKGD = new BKGDChunkType2();
                    break;
                case 3:
                    BKGD = new BKGDChunkType3();
                    break;
                case 4:
                    BKGD = new BKGDChunkType4();
                    break;
                case 6:
                    BKGD = new bKGDChunkType6();
                    break;
                default:
                    throw new ApplicationException("Colour type is not supported");
            }
            BKGD.ChunkData = chunk.ChunkData;
        }

        private void Handle_iTXt(PNGChunk chunk)
        {
            ITXtChunk it = new ITXtChunk();
            it.ChunkData = chunk.ChunkData;
            ITXtList.Add(it);
        }

        private void Handle_zTXt(PNGChunk chunk)
        {
            ZTXtChunk zt = new ZTXtChunk();
            zt.ChunkData = chunk.ChunkData;
            ZTXtList.Add(zt);
        }

        private void Handle_tEXt(PNGChunk chunk)
        {
            TEXtChunk txt = new TEXtChunk();
            txt.ChunkData = chunk.ChunkData;
            TEXtList.Add(txt);
        }

        private void Handle_sRGB(PNGChunk chunk)
        {
            if (SRGB != null)
            {
                throw new ApplicationException("sRGB chunk encountered more than once");
            }
            SRGB = new SRGBChunk();
            SRGB.ChunkData = chunk.ChunkData;
        }

        private void Handle_sBIT(PNGChunk chunk)
        {
            if (SBIT != null)
            {
                throw new ApplicationException("sBIT chunk encountered more than once");
            }
            switch (IHDR.ColorType)
            {
                case 0:
                    SBIT = new SBITChunkType0();
                    break;
                case 2:
                    SBIT = new SBITChunkType2();
                    break;
                case 3:
                    SBIT = new SBITChunkType3();
                    break;
                case 4:
                    SBIT = new SBITChunkType4();
                    break;
                case 6:
                    SBIT = new SBITChunkType6();
                    break;
                default:
                    throw new ApplicationException("Colour type is not supported");
            }
            SBIT.ChunkData = chunk.ChunkData;
        }

        private void Handle_iCCP(PNGChunk chunk)
        {
            if (ICCP != null)
            {
                throw new ApplicationException("iCCP chunk encountered more than once");
            }
            ICCP = new ICCPChunk();
            ICCP.ChunkData = chunk.ChunkData;
        }

        private void Handle_gAMA(PNGChunk chunk)
        {
            if (GAMA != null)
            {
                throw new ApplicationException("gAMA chunk encountered more than once");
            }
            GAMA = new GAMAChunk();
            GAMA.ChunkData = chunk.ChunkData;
        }

        private void Handle_cHRM(PNGChunk chunk)
        {
            if (CHRM != null)
            {
                throw new ApplicationException("cHRM chunk encountered more than once");
            }
            CHRM = new CHRMChunk();
            CHRM.ChunkData = chunk.ChunkData;
        }

        private void Handle_tRNS(PNGChunk chunk)
        {
            if (TRNS != null)
            {
                throw new ApplicationException("tRNS chunk encountered more than once");
            }
            switch (IHDR.ColorType)
            {
                case 0:
                    TRNS = new TRNSChunkType0();
                    break;
                case 2:
                    TRNS = new TRNSChunkType2();
                    break;
                case 3:
                    TRNS = new TRNSChunkType3();
                    break;
                case 4:
                case 6:
                    throw new ApplicationException("tRNS chunk encountered, Colour type does not support");
                default:
                    throw new ApplicationException("Colour type is not supported");
            }
            TRNS.ChunkData = chunk.ChunkData;
        }

        private void Handle_PLTE(PNGChunk chunk)
        {
            if (PLTE != null)
            {
                throw new ApplicationException("PLTE chunk encountered more than once");
            }
            PLTE = new PLTEChunk();
            PLTE.ChunkData = chunk.ChunkData;
        }

        private void Handle_IHDR(PNGChunk chunk)
        {
            if (IHDR != null)
            {
                throw new ApplicationException("IHDR defined more than once");
            }
            IHDR = new IHDRChunk
            {
                ChunkData = chunk.ChunkData
            };
        }

        private void Handle_IDAT(PNGChunk chunk)
        {
            IDATChunk idatC = new IDATChunk
            {
                ChunkData = chunk.ChunkData
            };
            IDATList.Add(idatC);
        }

        private void Handle_IEND(PNGChunk chunk)
        {
            if (IEND != null)
            {
                throw new ApplicationException("IEND defined more than once");
            }
            IEND = new IENDChunk
            {
                ChunkData = chunk.ChunkData
            };
        }

        private void HandleDefaultChunk(PNGChunk chunk)
        {
            chunks.Add(chunk);
        }

        public virtual void Validate()
        {
            if (IHDR == null || IDATList.Count < 1 || IEND == null)
            {
                throw new ApplicationException("Required chunk(s) missing");
            }
            if (HIST != null && PLTE == null)
            {
                throw new ApplicationException("Cannot have a hIST chunk without a PLTE chunk");
            }
            if (HIST != null && HIST.Frequency.Length != PLTE.PaletteEntries.Count)
            {
                throw new ApplicationException("Number of hIST chunk entries different from number of PLTE chunk entries");
            }
        }
    }
}
