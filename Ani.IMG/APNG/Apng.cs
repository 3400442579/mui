using Ani.IMG.APNG.Chunks;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Ani.IMG.APNG
{
    /// <summary>
    /// Apng encoder and decoder.
    /// https://en.wikipedia.org/wiki/APNG
    /// https://wiki.mozilla.org/APNG_Specification
    /// https://www.w3.org/TR/PNG/
    /// </summary>
    public class Apng : IDisposable
    {
        public enum DisposeOps
        {
            None = 0,
            Background = 1,
            Previous = 2
        }

        public enum BlendOps
        {
            Source = 0,
            Over = 1
        }


        #region Properties

        /// <summary>
        /// The stream which the apgn is writen on.
        /// </summary>
        private Stream InternalStream { get; set; }

        /// <summary>
        /// The total number of frames.
        /// </summary>
        public int FrameCount { get; set; } = 0;

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
        private int SequenceNumber { get; set; } = 0;

        #endregion

        #region Internal chunks

        /// <summary>
        /// The image header chunk.
        /// </summary>
        public IhdrChunk Ihdr { get; private set; }

        /// <summary>
        /// The animation control chunk.
        /// </summary>
        public ActlChunk Actl { get; private set; }

        /// <summary>
        /// All the chunks of the Png, except IHDR, acTL and IEND. 
        /// </summary>
        public List<Chunk> Chunks { get; } = new List<Chunk>();

        #endregion


        public Apng(Stream stream, int frameCount, int repeatCount)
        {
            InternalStream = stream;
            FrameCount = frameCount;
            RepeatCount = repeatCount;
        }

        public Apng(Stream stream)
        {
            InternalStream = stream;
        }

        public void AddFrame(string path, SKRect rect, int delay = 66)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                if (IsFirstFrame)
                {
                    //Png Header: 8 bytes.
                    InternalStream.WriteBytes(stream.ReadBytes(8));

                    //IHDR chunk. 13 bytes (Length + Type + CRC, 4 bytes each) = 25 bytes.
                    InternalStream.WriteBytes(stream.ReadBytes(25));

                    //acTL: Animation control chunk. 8 bytes (Length + Type + CRC, 4 bytes each) = 20 bytes.
                    InternalStream.WriteUInt32(BitHelper.ConvertEndian(8u)); //Length, 4 bytes.
                    InternalStream.WriteBytes(Encoding.ASCII.GetBytes("acTL")); //Chunk type, 4 bytes.
                    InternalStream.WriteUInt32(BitHelper.ConvertEndian((uint)FrameCount)); //NumFrames, 4 bytes.
                    InternalStream.WriteUInt32(BitHelper.ConvertEndian((uint)RepeatCount)); //NumPlays, 4 bytes.
                    InternalStream.WriteUInt32(BitHelper.ConvertEndian(CrcHelper.Calculate(InternalStream.PeekBytes(InternalStream.Position - 12, 12)))); //CRC, 4 bytes.
                }

                //fcTL: Frame control chunk. 26 bytes (Length + Type + CRC, 4 bytes each) = 38 bytes.
                InternalStream.WriteUInt32(BitHelper.ConvertEndian(26u)); //Length, 4 bytes.
                InternalStream.WriteBytes(Encoding.ASCII.GetBytes("fcTL")); //Chunk type, 4 bytes.
                InternalStream.WriteUInt32(BitHelper.ConvertEndian((uint)SequenceNumber++)); //SequenceNumber, 4 bytes.
                InternalStream.WriteUInt32(BitHelper.ConvertEndian((uint)rect.Width)); //Width, 4 bytes.
                InternalStream.WriteUInt32(BitHelper.ConvertEndian((uint)rect.Height)); //Height, 4 bytes.
                InternalStream.WriteUInt32(BitHelper.ConvertEndian((uint)rect.Left)); //OffsetX, 4 bytes.
                InternalStream.WriteUInt32(BitHelper.ConvertEndian((uint)rect.Top)); //OffsetY, 4 bytes.
                InternalStream.WriteUInt16(BitHelper.ConvertEndian((ushort)delay)); //Delay numerator, 2 bytes.
                InternalStream.WriteUInt16(BitHelper.ConvertEndian((ushort)1000)); //Delay denominator, 2 bytes.

                if (IsFirstFrame)
                {
                    InternalStream.WriteByte((byte)DisposeOps.None); //DisposeOp, 1 byte.
                    InternalStream.WriteByte((byte)BlendOps.Source); //BlendOp, 1 byte.
                }
                else
                {
                    InternalStream.WriteByte((byte)DisposeOps.None); //DisposeOp, 1 byte.
                    InternalStream.WriteByte((byte)BlendOps.Over); //BlendOp, 1 byte.
                }

                InternalStream.WriteUInt32(BitHelper.ConvertEndian(CrcHelper.Calculate(InternalStream.PeekBytes(InternalStream.Position - 30, 30)))); //CRC, 4 bytes.

                //fdAT: Frame data chunk. 4 + n bytes (Length + Type + CRC, 4 bytes each) = 16 + n bytes, where n is the frame data.
                var dataList = GetData(stream);

                foreach (var data in dataList)
                {
                    if (IsFirstFrame)
                    {
                        InternalStream.WriteUInt32(BitHelper.ConvertEndian((uint)data.Length)); //Length, 4 bytes.
                        InternalStream.WriteBytes(Encoding.ASCII.GetBytes("IDAT")); //Chunk type, 4 bytes.
                        InternalStream.WriteBytes(data); //Frame data, n bytes.
                        InternalStream.WriteUInt32(BitHelper.ConvertEndian(CrcHelper.Calculate(InternalStream.PeekBytes(InternalStream.Position - (data.Length + 4), data.Length + 4)))); //CRC, 4 bytes.
                    }
                    else
                    {
                        InternalStream.WriteUInt32(BitHelper.ConvertEndian(4 + (uint)data.Length)); //Length, 4 bytes.
                        InternalStream.WriteBytes(Encoding.ASCII.GetBytes("fdAT")); //Chunk type, 4 bytes.
                        InternalStream.WriteUInt32(BitHelper.ConvertEndian((uint)SequenceNumber++)); //SequenceNumber, 4 bytes.
                        InternalStream.WriteBytes(data); //Frame data, n bytes.
                        InternalStream.WriteUInt32(BitHelper.ConvertEndian(CrcHelper.Calculate(InternalStream.PeekBytes(InternalStream.Position - (data.Length + 8), data.Length + 8)))); //CRC, 4 bytes.
                    }
                }
                
                IsFirstFrame = false;
            }
        }

        private static IEnumerable<byte[]> GetData(Stream ms)
        {
            ms.Position = 8 + 25;

            var list = new List<byte[]>();

            while (ms.CanRead)
            {
                var length = BitHelper.ConvertEndian(ms.ReadUInt32());
                var chunkType = Encoding.ASCII.GetString(ms.ReadBytes(4));
                var data = ms.ReadBytes(length);

                if (chunkType == "IDAT")
                    list.Add(data);

                if (chunkType == "IEND")
                    break;

                ms.ReadUInt32();
            }

            return list;
        }

        public bool ReadFrames()
        {
            //Png header, 8 bytes.
            if (!InternalStream.ReadBytes(8).SequenceEqual(new byte[] {137, 80, 78, 71, 13, 10, 26, 10}))
                throw new Exception("Invalid file format, expected PNG signature not found.");

            //IHDR chunk, 25 bytes.
            Ihdr = IhdrChunk.Read(InternalStream);

            //aCTl chunk, 16 bytes.
            Actl = ActlChunk.Read(InternalStream);

            //If there's no animation control chunk, it's a normal Png.
            if (Actl == null)
                return false;

            var masterSequence = 0;
            var frameGroupId = -1;

            //Read frames.
            while (InternalStream.CanRead)
            {
                //Tries to read any chunk, except IEND.
                var chunk = Chunk.Read(InternalStream, masterSequence++);

                //End reached, prematurely or not.
                if (chunk == null || chunk.ChunkType == "IEND")
                    break;
                
                //Chunks can be grouped into frames.
                if (new[] {"fcTL", "fdAT", "IDAT"}.Contains(chunk.ChunkType))
                {
                    if (chunk.ChunkType == "fcTL")
                        frameGroupId++;

                    chunk.FrameGroupId = frameGroupId;
                }

                Chunks.Add(chunk);
            }

            return true;
        }

        public Frame GetFrame(int index)
        {
            //Build each frame using:
            //Starting blocks: IHDR, tIME, zTXt, tEXt, iTXt, pHYs, sPLT, (iCCP | sRGB), sBIT, gAMA, cHRM, PLTE, tRNS, hIST, bKGD.
            //Image data: IDAT.
            //End block: IEND.

            var chunks = Chunks.Where(w => w.FrameGroupId == index).ToList();
            var otherChunks = Chunks.Where(w => w.FrameGroupId == -1 && w.ChunkType != "IDAT").ToList();

            if (!chunks.Any())
                return null;

            var frame = new Frame();

            //First frame • Second frame
            //Default image is part of the animation:       fcTL + IDAT • fcTL + fdAT
            //Default image isn't part of the animation:    IDAT • fcTL + fdAT

            if (chunks[0].ChunkType == "fcTL")
            {
                var fctl = FctlChunk.Read(chunks[0].Length, chunks[0].ChunkData);
                frame.Delay = fctl.DelayNum == 0 ? 10 : (int)(fctl.DelayNum / (fctl.DelayDen == 0 ? 100d : fctl.DelayDen) * 1000d);
                frame.Width = fctl.Width;
                frame.Height = fctl.Height;
                frame.Left = fctl.XOffset;
                frame.Top = fctl.YOffset;
                frame.ColorType = Ihdr.ColorType;
                frame.BitDepth = Ihdr.BitDepth;
                frame.DisposeOp = fctl.DisposeOp;
                frame.BlendOp = fctl.BlendOp;

                using var stream = new MemoryStream();
                //Png signature, 8 bytes.
                stream.WriteBytes(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 });

                //Image header chunk. 25 bytes.
                Ihdr.Write(stream, fctl.Width, fctl.Height);

                //Any other auxiliar chunks.
                foreach (var other in otherChunks)
                    other.Write(stream);

                //Frame has multiple chunks.
                if (chunks.Count > 2)
                {
                    var datas = new List<byte[]>();

                    //Data chunks.
                    for (var i = 1; i < chunks.Count; i++)
                    {
                        switch (chunks[i].ChunkType)
                        {
                            case "fdAT":
                                {
                                    var fdat = FdatChunk.Read(chunks[i].Length, chunks[i].ChunkData);
                                    datas.Add(fdat.FrameData);
                                    break;
                                }
                            case "IDAT":
                                {
                                    var idat = IdatChunk.Read(chunks[i].Length, chunks[i].ChunkData);
                                    datas.Add(idat.FrameData);
                                    break;
                                }
                        }
                    }

                    //Write combined frame data.
                    var length = datas.Sum(s => s.Length);

                    stream.WriteUInt32(BitHelper.ConvertEndian((uint)length)); //4 bytes.
                    stream.WriteBytes(Encoding.ASCII.GetBytes("IDAT")); //4 bytes.
                    stream.WriteBytes(datas.SelectMany(s => s).ToArray()); //XX bytes.
                    stream.WriteUInt32(BitHelper.ConvertEndian(CrcHelper.Calculate(stream.PeekBytes(stream.Position - (length + 4), length + 4)))); //CRC, 4 bytes.
                }
                else
                {
                    switch (chunks[1].ChunkType)
                    {
                        case "fdAT":
                            {
                                var fdat = FdatChunk.Read(chunks[1].Length, chunks[1].ChunkData);
                                fdat.Write(stream);
                                break;
                            }
                        case "IDAT":
                            {
                                var idat = IdatChunk.Read(chunks[1].Length, chunks[1].ChunkData);
                                idat.Write(stream);
                                break;
                            }
                    }
                }

                //End chunk.
                stream.WriteUInt32(BitHelper.ConvertEndian(0u)); //Chunk length, 4 bytes.
                stream.WriteBytes(Encoding.ASCII.GetBytes("IEND")); //Chunk type, 4 bytes.
                stream.WriteUInt32(BitHelper.ConvertEndian(CrcHelper.Calculate(stream.PeekBytes(stream.Position - 4, 4)))); //CRC, 4 bytes.

                //Gets the whole Png.
                frame.ImageData = stream.ToArray();
            }
            else
            {
                //This is not supposed to happen.
                //All chunks with an FrameGroupId are grouped with a starting fcTL, ending with a IDAT or fdAT chunk.
                return null;
            }

            return frame;
        }


        public static SKBitmap MakeFrame(SKSize fullSize, SKImage rawFrame, Frame frame, SKBitmap baseFrame)
        {
            SKBitmap bitmap = new SKBitmap(new SKImageInfo((int)fullSize.Width, (int)fullSize.Height));
            using SKCanvas canvas = new SKCanvas(bitmap);
            if (baseFrame != null)
            {
                var fullRect = new SKRect(0, 0, fullSize.Width, fullSize.Height);
                if (frame.BlendOp == BlendOps.Source)
                    canvas.DrawBitmap(ClearArea(baseFrame, frame), 0, 0);
                else
                    canvas.DrawBitmap(baseFrame, 0, 0);
            }
            //canvas.DrawBitmap(SKBitmap.Decode(@"C:\Users\jxw\Desktop\17\1.png"), (int)frame.Left, (int)frame.Top);
            canvas.DrawImage(rawFrame,new SKPoint((int)frame.Left, (int)frame.Top));
            
            return bitmap;
        }

        public static bool IsFullFrame(Frame metadata, SKSize fullSize)
        {
            return metadata.Left == 0 && metadata.Top == 0 && metadata.Width == fullSize.Width && metadata.Height == fullSize.Height;
        }

        public static SKBitmap ClearArea(SKBitmap frame, Frame metadata)
        {
            SKBitmap sKBitmap = new SKBitmap(new SKImageInfo (frame.Width, frame.Height));
            using (SKCanvas context = new SKCanvas(sKBitmap))
            {
                var fullRect = new SKRect(0, 0, frame.Width, frame.Height);
                var clearRect = new SKRect((int)metadata.Left, (int)metadata.Top, (int)metadata.Width, (int)metadata.Height);

                //context.PushClip(clip);
                context.ClipRect(clearRect, SKClipOperation.Difference);
                context.DrawBitmap(frame, fullRect);
            }

            //var bitmap = new RenderTargetBitmap(frame.Width, frame.Height, frame.DpiX, frame.DpiY, PixelFormats.Pbgra32);
            //bitmap.Render(visual);

            //if (bitmap.CanFreeze && !bitmap.IsFrozen)
            //    bitmap.Freeze();

            return sKBitmap;
        }


        //public static void SaveFrames(string png, string pathTemp)
        //{
        //    string fn;

        //    using var stream = new FileStream(png, FileMode.Open);
        //    var apng = new Apng(stream);

        //    if (!apng.ReadFrames())
        //    {
        //        fn = Path.Combine(pathTemp, $"{Path.GetFileName(Path.GetTempFileName())}.png");
        //        using FileStream fileStream = new FileStream(Path.Combine(pathTemp, $"{fn}.png"), FileMode.Create);
        //        SKImage.FromEncodedData(stream).EncodedData.SaveTo(fileStream);
        //        return;
        //    }

        //    var fullSize = new SKSize((int)apng.Ihdr.Width, (int)apng.Ihdr.Height);

        //    SKBitmap baseFrame = null;
        //    for (var index = 0; index < apng.Actl.NumFrames; index++)
        //    {
        //        var metadata = apng.GetFrame(index);

        //        SKImage rawFrame = SKImage.FromEncodedData(metadata.ImageData);

        //        var bitmapSource = MakeFrame(fullSize, rawFrame, metadata, baseFrame);

        //        #region Disposal Method

        //        switch (metadata.DisposeOp)
        //        {
        //            case DisposeOps.None: //No disposal is done on this frame before rendering the next; the contents of the output buffer are left as is.
        //                baseFrame = bitmapSource;
        //                break;
        //            case DisposeOps.Background: //The frame's region of the output buffer is to be cleared to fully transparent black before rendering the next frame.
        //                baseFrame = baseFrame == null || IsFullFrame(metadata, fullSize) ? null : ClearArea(baseFrame, metadata);
        //                break;
        //            case DisposeOps.Previous: //The frame's region of the output buffer is to be reverted to the previous contents before rendering the next frame.
        //                                      //Reuse same base frame.
        //                break;
        //        }

        //        #endregion

        //        fn = Path.Combine(pathTemp, $"{Path.GetFileName(Path.GetTempFileName())}.png");
        //        using var output = new FileStream(fn, FileMode.Create);
        //        bitmapSource.Encode(SKEncodedImageFormat.Png, 100).SaveTo(output);
        //    }
        //}


        public void Dispose()
        {
            //IEND: The end of the Png datastream. 0 bytes (Length + Type + CRC, 4 bytes each) = 12 bytes.
            InternalStream.WriteUInt32(BitHelper.ConvertEndian(0u)); //Length, 4 bytes.
            InternalStream.WriteBytes(Encoding.ASCII.GetBytes("IEND")); //Chunk type, 4 bytes.
            InternalStream.WriteUInt32(BitHelper.ConvertEndian(CrcHelper.Calculate(InternalStream.PeekBytes(InternalStream.Position - 4, 4)))); //CRC, 4 bytes.

            InternalStream.Flush();
            //Resets the stream position to save afterwards.
            InternalStream.Position = 0;
        }
    }
}