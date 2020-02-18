using System.Collections.Generic;
using System.IO;

namespace Animation.Editor.Utils.Gif.Decoder
{
    public abstract class GifBlock
    {
        public static GifBlock ReadBlock(Stream stream, IEnumerable<GifExtension> controlExtensions, bool metadataOnly)
        {
            var blockId = stream.ReadByte();

            if (blockId < 0)
                throw GifHelpers.UnexpectedEndOfStreamException();

            return blockId switch
            {
                GifExtension.ExtensionIntroducer => GifExtension.ReadExtension(stream, controlExtensions, metadataOnly),
                GifFrame.ImageSeparator => GifFrame.ReadFrame(stream, controlExtensions, metadataOnly),
                GifTrailer.TrailerByte => GifTrailer.ReadTrailer(),
                _ => throw GifHelpers.UnknownBlockTypeException(blockId),
            };
        }

        public abstract GifBlockKind Kind { get; }
    }
}