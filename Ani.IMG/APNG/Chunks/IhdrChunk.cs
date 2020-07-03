﻿using System;
using System.IO;
using System.Text;

namespace Ani.IMG.APNG.Chunks
{
    /// <summary>
    /// The image header chunk.
    /// </summary>
    public class IhdrChunk : Chunk
    {
        public uint Width { get; private set; }

        public uint Height { get; private set; }

        public byte BitDepth { get; private set; }

        public byte ColorType { get; private set; }

        public byte CompressionMethod { get; private set; }

        public byte FilterMethod { get; private set; }

        public byte InterlaceMethod { get; private set; }

        /// <summary>
        /// Attempts to read 25 bytes of the stream.
        /// </summary>
        public static IhdrChunk Read(Stream stream)
        {
            var chunk = new IhdrChunk
            {
                Length = BitHelper.ConvertEndian(stream.ReadUInt32()), //Chunk length, 4 bytes.
                ChunkType = Encoding.ASCII.GetString(stream.ReadBytes(4)) //Chunk type, 4 bytes.
            };

            if (chunk.ChunkType != "IHDR")
                throw new Exception("Missing IHDR chunk.");

            //var pos = stream.Position;
            //chunk.ChunkData = stream.ReadBytes(chunk.Length);
            //stream.Position = pos;

            //Chunk details + CRC, 13 bytes + 4 bytes.
            chunk.Width = BitHelper.ConvertEndian(stream.ReadUInt32());
            chunk.Height = BitHelper.ConvertEndian(stream.ReadUInt32());
            chunk.BitDepth = (byte) stream.ReadByte();
            chunk.ColorType = (byte) stream.ReadByte();
            chunk.CompressionMethod = (byte) stream.ReadByte();
            chunk.FilterMethod = (byte) stream.ReadByte();
            chunk.InterlaceMethod = (byte) stream.ReadByte();
            chunk.Crc = BitHelper.ConvertEndian(stream.ReadUInt32());

            return chunk;
        }

        /// <summary>
        /// Write the IHDR chunk to the stream.
        /// If a custom size is given, that's what is written.
        /// </summary>
        public void Write(Stream stream, uint? width = null, uint? height = null)
        {
            stream.WriteUInt32(BitHelper.ConvertEndian(Length)); //4 bytes.
            stream.WriteBytes(Encoding.ASCII.GetBytes(ChunkType)); //4 bytes.
            stream.WriteUInt32(BitHelper.ConvertEndian(width ?? Width)); //4 bytes.
            stream.WriteUInt32(BitHelper.ConvertEndian(height ?? Height)); //4 bytes.
            stream.WriteByte(BitDepth); //1 byte.
            stream.WriteByte(ColorType); //1 byte.
            stream.WriteByte(CompressionMethod); //1 byte.
            stream.WriteByte(FilterMethod); //1 byte.
            stream.WriteByte(InterlaceMethod); //1 byte.
            stream.WriteUInt32(BitHelper.ConvertEndian(CrcHelper.Calculate(stream.PeekBytes(stream.Position - (Length + 4), (int)Length + 4)))); //CRC, 4 bytes.
        }
    }
}