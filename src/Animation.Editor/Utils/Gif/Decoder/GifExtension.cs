using System.Collections.Generic;
using System.IO;

namespace Animation.Editor.Utils.Gif.Decoder
{
    public abstract class GifExtension : GifBlock
    {
        public const int ExtensionIntroducer = 0x21;

        public static GifExtension ReadExtension(Stream stream, IEnumerable<GifExtension> controlExtensions, bool metadataOnly)
        {
            //Note: at this point, the Extension Introducer (0x21) has already been read
            var label = stream.ReadByte();

            if (label < 0)
                throw GifHelpers.UnexpectedEndOfStreamException();

            return label switch
            {
                GifGraphicControlExtension.ExtensionLabel => GifGraphicControlExtension.ReadGraphicsControl(stream),
                GifCommentExtension.ExtensionLabel => GifCommentExtension.ReadComment(stream),
                GifPlainTextExtension.ExtensionLabel => GifPlainTextExtension.ReadPlainText(stream, controlExtensions, metadataOnly),
                GifApplicationExtension.ExtensionLabel => GifApplicationExtension.ReadApplication(stream),
                _ => throw GifHelpers.UnknownExtensionTypeException(label),
            };
        }
    }
}