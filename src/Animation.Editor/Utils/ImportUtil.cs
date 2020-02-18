using Animation.Editor.Models;
using Animation.Editor.Utils.Gif.Decoder;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Animation.Editor.Utils
{
    public class ImportUtil
    {
        private readonly double Dpi;
       
        public ImportUtil(double dpi) {
            Dpi = dpi;
        }

        //public List<Frame> From(string source, string tempPath)
        //{
        //    List<Frame> frames;

        //    try
        //    {
        //        switch (Path.GetExtension(source).ToLowerInvariant())
        //        {
        //            case ".stg":
        //            case ".zip":
        //                frames = FromProject(source, tempPath);
        //                break;
        //            case ".gif":
        //                frames = FromGif(source, tempPath);
        //                break;
        //            case ".mp4":
        //            case ".wmv":
        //            case ".avi":
        //                frames = FromVideo(source, tempPath);
        //                break;
        //            default:
        //                frames = FromImage(source, tempPath);
        //                break;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        frames = new List<Frame>();
        //    }
        //    return frames;
        //}

        #region From Gif Import
        public List<Frame> FromGif(string source, string tempPath, int sIndex)
        {
            List<Frame> frames = new List<Frame>();

            var decoder = GetDecoder(source, out var gifMetadata) as GifBitmapDecoder;

            //ShowProgress(DispatcherStringResource("导入帧"), decoder.Frames.Count);

            if (decoder.Frames.Count <= 0)
                return frames;

            var fullSize = GetFullSize(decoder, gifMetadata);
            int index = 0;

            int stratIndex = sIndex;
            BitmapSource baseFrame = null;
            foreach (var rawFrame in decoder.Frames)
            {
                var metadata = GetFrameMetadata(decoder, gifMetadata, index);

                var bitmapSource = MakeFrame(fullSize, rawFrame, metadata, baseFrame);

                #region Disposal Method

                switch (metadata.DisposalMethod)
                {
                    case FrameDisposalMethod.None:
                    case FrameDisposalMethod.DoNotDispose:
                        baseFrame = bitmapSource;
                        break;
                    case FrameDisposalMethod.RestoreBackground:
                        baseFrame = IsFullFrame(metadata, fullSize) ? null : ClearArea(bitmapSource, metadata);
                        break;
                    case FrameDisposalMethod.RestorePrevious:
                        //Reuse same base frame.
                        break;
                }

                #endregion

                #region Each Frame

                var fileName = Path.Combine(tempPath, $"{stratIndex+index}_{DateTime.Now:hh-mm-ss-ffff}.png");

                using (var stream = new FileStream(fileName, FileMode.Create))
                {
                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                    encoder.Save(stream);
                    stream.Close();
                }

                if (rawFrame.DpiX != Dpi)
                {
                    var sf = new System.Drawing.Bitmap(fileName);
                    using var bm = new System.Drawing.Bitmap(sf.Width, sf.Height);
                    using (var gr = System.Drawing.Graphics.FromImage(bm))
                    {
                        gr.DrawImage(sf, new System.Drawing.RectangleF(0, 0, sf.Width, sf.Height));
                        sf.Dispose();
                    }

                    bm.SetResolution((float)Dpi, (float)Dpi);
                    bm.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
                }

                
                //It should not throw a overflow exception because of the maximum value for the milliseconds.

                frames.Add(new Frame(fileName, (int)metadata.Delay.TotalMilliseconds, stratIndex + index));

                // UpdateProgress(index);

                GC.Collect(1);

                #endregion

                index++;
            }

            return frames;
        }
        private BitmapDecoder GetDecoder(string fileName, out GifFile gifFile)
        {
            gifFile = null;
            BitmapDecoder decoder = null;

            using (var stream = new FileStream(fileName, FileMode.Open))
            {
                stream.Position = 0;
                decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);

                if (decoder is GifBitmapDecoder)// && !CanReadNativeMetadata(decoder))
                {
                    stream.Position = 0;
                    gifFile = GifFile.ReadGifFile(stream, true);
                }

                //if (decoder == null)
                //    throw new InvalidOperationException("Can't get a decoder from the source.");
            }

            return decoder;
        }
        private FrameMetadata GetFrameMetadata(BitmapDecoder decoder, GifFile gifMetadata, int frameIndex)
        {
            if (gifMetadata != null && gifMetadata.Frames.Count > frameIndex)
            {
                return GetFrameMetadata(gifMetadata.Frames[frameIndex]);
            }

            return GetFrameMetadata(decoder.Frames[frameIndex]);
        }
        private  FrameMetadata GetFrameMetadata(BitmapFrame frame)
        {
            var metadata = (BitmapMetadata)frame.Metadata;
            var delay = TimeSpan.FromMilliseconds(100);
            var metadataDelay = GetQueryOrDefault(metadata,"/grctlext/Delay", 10);

            if (metadataDelay != 0)
                delay = TimeSpan.FromMilliseconds(metadataDelay * 10);

            var disposalMethod = (FrameDisposalMethod)GetQueryOrDefault(metadata,"/grctlext/Disposal", 0);

            var frameMetadata = new FrameMetadata
            {
                Left = GetQueryOrDefault(metadata,"/imgdesc/Left", 0),
                Top = GetQueryOrDefault(metadata,"/imgdesc/Top", 0),
                Width = GetQueryOrDefault(metadata,"/imgdesc/Width", frame.PixelWidth),
                Height = GetQueryOrDefault(metadata,"/imgdesc/Height", frame.PixelHeight),
                Delay = delay,
                DisposalMethod = disposalMethod
            };

            return frameMetadata;
        }
        private  FrameMetadata GetFrameMetadata(GifFrame gifMetadata)
        {
            var d = gifMetadata.Descriptor;

            var frameMetadata = new FrameMetadata
            {
                Left = d.Left,
                Top = d.Top,
                Width = d.Width,
                Height = d.Height,
                Delay = TimeSpan.FromMilliseconds(100),
                DisposalMethod = FrameDisposalMethod.None
            };

            var gce = gifMetadata.Extensions.OfType<GifGraphicControlExtension>().FirstOrDefault();

            if (gce != null)
            {
                if (gce.Delay != 0)
                    frameMetadata.Delay = TimeSpan.FromMilliseconds(gce.Delay);

                frameMetadata.DisposalMethod = (FrameDisposalMethod)gce.DisposalMethod;
            }

            return frameMetadata;
        }
        private BitmapSource MakeFrame(System.Drawing.Size fullSize, BitmapSource rawFrame, FrameMetadata metadata, BitmapSource baseFrame)
        {
             
            var visual = new DrawingVisual();
            using (var context = visual.RenderOpen())
            {
                if (baseFrame != null)
                {
                    var fullRect = new Rect(0, 0, fullSize.Width, fullSize.Height);
                    context.DrawImage(baseFrame, fullRect);
                }

                var rect = new Rect(metadata.Left, metadata.Top, metadata.Width, metadata.Height);
                context.DrawImage(rawFrame, rect);
            }

            //TODO: Test, DPI was hardcoded to 96.
            var bitmap = new RenderTargetBitmap(fullSize.Width, fullSize.Height, rawFrame.DpiX, rawFrame.DpiY, PixelFormats.Pbgra32);
            bitmap.Render(visual);

            if (bitmap.CanFreeze && !bitmap.IsFrozen)
                bitmap.Freeze();

            return bitmap;
        }
        private bool IsFullFrame(FrameMetadata metadata, System.Drawing.Size fullSize)
        {
            return metadata.Left == 0
                   && metadata.Top == 0
                   && metadata.Width == fullSize.Width
                   && metadata.Height == fullSize.Height;
        }
        private BitmapSource ClearArea(BitmapSource frame, FrameMetadata metadata)
        {
            var visual = new DrawingVisual();
            using (var context = visual.RenderOpen())
            {
                var fullRect = new Rect(0, 0, frame.PixelWidth, frame.PixelHeight);
                var clearRect = new Rect(metadata.Left, metadata.Top, metadata.Width, metadata.Height);
                var clip = Geometry.Combine(new RectangleGeometry(fullRect), new RectangleGeometry(clearRect), GeometryCombineMode.Exclude, null);

                context.PushClip(clip);
                context.DrawImage(frame, fullRect);
            }

            var bitmap = new RenderTargetBitmap(frame.PixelWidth, frame.PixelHeight, frame.DpiX, frame.DpiY, PixelFormats.Pbgra32);
            bitmap.Render(visual);

            if (bitmap.CanFreeze && !bitmap.IsFrozen)
                bitmap.Freeze();

            return bitmap;
        }
        private System.Drawing.Size GetFullSize(BitmapDecoder decoder, GifFile gifMetadata)
        {
            if (gifMetadata != null)
            {
                var lsd = gifMetadata.Header.LogicalScreenDescriptor;
                return new System.Drawing.Size(lsd.Width, lsd.Height);
            }

            var width = GetQueryOrDefault(decoder.Metadata,"/logscrdesc/Width", 0);
            var height = GetQueryOrDefault(decoder.Metadata,"/logscrdesc/Height", 0);
            return new System.Drawing.Size(width, height);
        }
        private T GetQueryOrDefault<T>( BitmapMetadata metadata, string query, T defaultValue)
        {
            if (metadata.ContainsQuery(query))
                return (T)Convert.ChangeType(metadata.GetQuery(query), typeof(T));

            return defaultValue;
        }
        #endregion


        public List<Frame> FromImage(string source, string tempPath,int sindex)
        {
            var fileName = Path.Combine(tempPath, $"{sindex}_{DateTime.Now:hh-mm-ss-ffff}.png");

            
            //BitmapSource bitmap = new BitmapImage(new Uri(sourceFileName));

            //if (bitmap.Format != PixelFormats.Bgra32)
            //    bitmap = new FormatConvertedBitmap(bitmap, PixelFormats.Bgra32, null, 0);

            //using (var stream = new FileStream(fileName, FileMode.Create))
            //{
            //    var encoder = new PngBitmapEncoder();
            //    encoder.Frames.Add(BitmapFrame.Create(bitmap));
            //    encoder.Save(stream);
            //    stream.Close();
            //}

            using (var sf = new System.Drawing.Bitmap(source))
            {
                using var bm = new System.Drawing.Bitmap(sf.Width, sf.Height);
                using (var gr = System.Drawing.Graphics.FromImage(bm))
                {
                    //gr.Clear(System.Drawing.Color.Red);
                    gr.DrawImage(sf, new System.Drawing.RectangleF(0, 0, sf.Width, sf.Height));
                }
                bm.SetResolution((float)Dpi, (float)Dpi);
                bm.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
            }

            GC.Collect();

            

            return new List<Frame> { new Frame(fileName, 66, sindex) };
        }
        


        //public List<Frame> FromVideo(string source, string tempPath)
        //{
        //    //var delay = 66;
        //    //var frameList = Dispatcher.Invoke(() =>
        //    //{
        //    //    var videoSource = new VideoSource(fileName) { Owner = this };
        //    //    var result = videoSource.ShowDialog();

        //    //    delay = videoSource.Delay;

        //    //    if (result.HasValue && result.Value)
        //    //        return videoSource.FrameList;

        //    //    return null;
        //    //});

        //    //return frameList ?? new List<Frame>();

           

        //    if (frameList == null)
        //        return new List<Frame>();

        //   // ShowProgress(DispatcherStringResource("Editor.ImportingFrames"), frameList.Count);

        //    #region Saves the Frames to the Disk

        //    var frames = new List<Frame>();
        //    var count = 0;
            
        //    foreach (var frame in frameList)
        //    {
        //        var frameName = Path.Combine(tempPath, $"{count} {DateTime.Now:hh-mm-ss-ffff}.png");

        //        using (var stream = new FileStream(frameName, FileMode.Create))
        //        {
        //            var encoder = new PngBitmapEncoder();
        //            encoder.Frames.Add(frame);
        //            encoder.Save(stream);
        //            stream.Close();
        //        }

        //        var Frame = new Frame(frameName, delay);
        //        frames.Add(Frame);

        //        GC.Collect(1, GCCollectionMode.Forced);
        //        count++;

        //       // UpdateProgress(count);
        //    }

        //    frameList.Clear();
        //    GC.Collect();

        //    #endregion

        //    return frames;
        //}



        public List<Frame> FromProject(string source, string tempPath)
        {
            try
            {
                ZipFile.ExtractToDirectory(source, tempPath);

                if (File.Exists(Path.Combine(tempPath, "Project.json")))
                {
                    Project project = Json.DeserializeByFile<Project>(Path.Combine(tempPath, "info.json"));
                    
                    //ShowProgress("Importing Frames", list.Count);

                    var count = 0;
                    foreach (var frame in project.Frames)
                    {
                        //Change the file path to the current one.
                        frame.Path = Path.Combine(tempPath, Path.GetFileName(frame.Path));

                        count++;
                        //UpdateProgress(count);
                    }

                    return project.Frames;
                }
                else
                {
                    if (File.Exists(Path.Combine(tempPath, "List.sb")))
                        throw new Exception("Project not compatible with this version");

                    throw new FileNotFoundException("Impossible to open project.", "List.sb");
                }

                
            }
            catch (Exception ex)
            {
                //LogWriter.Log(ex, "Importing project");
                // Dispatcher.Invoke(() => Dialog.Ok("GifMake", "Impossible to load project", ex.Message));
                return new List<Frame>();
            }
        }
    }
}
