using An.Ava.Controls;
using An.Editor.Models;
using An.Image.Gif.Decoding;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using ReactiveUI;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace An.Editor.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public Control View { get; set; }

        public MainWindowViewModel()
        {
            this.WhenAnyValue(o => o.ScaleIndex).Subscribe(o =>
            {
                Scale = Ruler.Scales[o];
            });

            //Project = new Project { Width = 500, Height = 400, Name = "aa"  };
            //Project.Frames.Add(new Frame { Index = 0, Delay = 100, Source = @"C:\Users\jxw\Desktop\17\1.png" });
            //Project.Frames.Add(new Frame { Index = 1, Delay = 110, Source = @"C:\Users\jxw\Desktop\17\2.png" });

            //DragImportImageCmd = ReactiveCommand.Create<DragEventArgs>(s => ImportImage(s.Data.GetFileNames().ToArray()));
            //DragOverCmd = ReactiveCommand.Create<DragEventArgs>(e => e.DragEffects = e.Data.Contains(DataFormats.FileNames) ? DragDropEffects.Copy : DragDropEffects.None);

            ImportImageCmd = ReactiveCommand.CreateFromTask(async () =>
            {
                if (!(View?.GetVisualRoot() is Window window))
                    return;

                var dialog = new OpenFileDialog
                {
                    AllowMultiple = true,
                    Title = "导入",
                    Filters = new List<FileDialogFilter> {
                        new FileDialogFilter { Name="Image",Extensions=new List<string> { ".jpg", ".jpeg", ".gif", ".bmp", ".png", ".avi", ".mp4", ".wmv" } },
                        new FileDialogFilter { Name="Video",Extensions=new List<string> {   ".avi", ".mp4", ".wmv" } },
                        new FileDialogFilter { Name="Project",Extensions=new List<string>{".stg", ".zip" } }
                    }
                };
                var result = await dialog.ShowAsync(window);
                if (result.Length > 0)
                    ImportImage(result);
            });

        }

        private int progress = 0;
        public int Progress
        {
            set
            {
                this.RaiseAndSetIfChanged(ref progress, value, nameof(Progress));
            }
            get => progress;
        }

        private bool isLoading = false;
        public bool IsLoading
        {
            set
            {
                isLoading = value;
                this.RaisePropertyChanged(nameof(IsLoading));
            }
            get => isLoading;
        }



        public List<string> Greetings => new List<string> { "a", "b" };


        private int scale = 100;
        public int Scale
        {
            set
            {
                if (scale != value)
                {
                    scale = value;
                    this.RaisePropertyChanged(nameof(Scale));
                }
            }
            get
            {
                return scale;
            }
        }

        private int scaleIndex = 15;
        public int ScaleIndex
        {
            set
            {
                if (scaleIndex != value)
                {
                    scaleIndex = value;
                    this.RaisePropertyChanged(nameof(ScaleIndex));
                }
            }
            get
            {
                return scaleIndex;
            }
        }

  
        private Project project;
        public Project Project
        {
            set
            {
                project = value;
                this.RaisePropertyChanged(nameof(Project));
            }
            get
            {
                return project;
            }
        }

        private Frame selectedFrame;
        public Frame SelectedFrame
        {
            set
            {
                selectedFrame = value;
                this.RaisePropertyChanged(nameof(SelectedFrame));
            }
            get
            {
                return selectedFrame;
            }
        }


        #region command

        public ReactiveCommand<DragEventArgs, Unit> DragImportImageCmd { get; }

        public ReactiveCommand<DragEventArgs, Unit> DragOverCmd { get; }

        public ReactiveCommand<Unit, Unit> ImportImageCmd { get; }

        #endregion



        /// <summary>
        /// 验证文件格式
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public List<string> ValidationFile(string[] files)
        {
            var media = new[] { ".jpg", ".jpeg", ".gif", ".bmp", ".png", ".avi", ".mp4", ".wmv" };

            List<string> vfiles = files.Where(s => media.Contains(Path.GetExtension(s).ToLowerInvariant())).ToList();

            var extensionList = vfiles.Select(s => Path.GetExtension(s).ToLowerInvariant()).ToList();

            var projectCount = extensionList.Count(x => !string.IsNullOrEmpty(x) && (x.Equals(".stg") || x.Equals(".zip")));
            var mediaCount = extensionList.Count(x => !string.IsNullOrEmpty(x) && media.Contains(Path.GetExtension(x)));

            //if (projectCount != 0 && mediaCount != 0)
            //{
            //    Dialog.Ok(StringResource("Editor.DragDrop.Invalid.Title"),
            //        StringResource("Editor.DragDrop.MultipleFiles.Instruction"),
            //        StringResource("Editor.DragDrop.MultipleFiles.Message"), Icons.Warning);
            //    return;
            //}

            //if (mediaCount == 0 && projectCount == 0)
            //{
            //    Dialog.Ok(StringResource("Editor.DragDrop.Invalid.Title"),
            //        StringResource("Editor.DragDrop.Invalid.Instruction"),
            //        StringResource("Editor.DragDrop.Invalid.Message"), Icons.Warning);
            //    return;
            //}

            return vfiles;
        }


        public void ImportImage(string[] files)
        {
            IsLoading = true;

            var project = new Project().CreateProjectFolder();

            foreach (string f in files)
            {
                var frames = InsertInternal(f, project.FullPath);
                if (frames == null)
                    continue;

                project.Frames.AddRange(frames);
            }

            if (!project.Any)
                project.ReleaseMutex();
            
            Project = project;
        }

        private List<Frame> InsertInternal(string fileName, string pathTemp)
        {
            List<Frame> listFrames;

            try
            {
                switch (System.IO.Path.GetExtension(fileName).ToLowerInvariant())
                {
                    case "stg":
                    case "zip":
                        {
                            listFrames = ImportFromProject(fileName, pathTemp);
                            break;
                        }

                    case "gif":
                        {
                            listFrames = ImportFromGif(fileName, pathTemp);
                            break;
                        }

                    case "avi":
                    case "mkv":
                    case "mp4":
                    case "wmv":
                    case "webm":
                        {
                            listFrames = ImportFromVideo(fileName, pathTemp);
                            break;
                        }

                    case "apng":
                    case "png":
                        {
                            listFrames = ImportFromApng(fileName, pathTemp);
                            break;
                        }

                    case "webp":
                        {
                            listFrames = ImportFromWebp(fileName, pathTemp);
                            break;
                        }

                    default:
                        {
                            listFrames = ImportFromImage(fileName, pathTemp,null,null);
                            break;
                        }
                }
            }
            catch //(Exception ex)
            {
                listFrames = new List<Frame>();
            }

            return listFrames;
        }
        private List<Frame> ImportFromGif(string source, string pathTemp)
        {
            //ShowProgress(DispatcherStringResource("Editor.ImportingFrames"), 50, true);

            var listFrames = new List<Frame>();

            using FileStream stream = new FileStream(source, FileMode.Open, FileAccess.Read);

            GifDecoder decoder = new GifDecoder(stream);
            if (decoder.Frames.Count <= 0)
                return listFrames;


            for (int i = 0; i < decoder.Frames.Count; i++)
            {
                string fileName = System.IO.Path.Combine(pathTemp, $"{i} {DateTime.Now:hh-mm-ss-ffff}.png");
                decoder.RenderFrame(i, fileName);

                //It should not throw a overflow exception because of the maximum value for the milliseconds.
                var frame = new Frame(fileName, (int)decoder.Frames[i].FrameDelay.TotalMilliseconds);
                listFrames.Add(frame);

                //  UpdateProgress(index);
            }



            // var gifFile = GifFile.ReadGifFile(source, true);


            //// ShowProgress(DispatcherStringResource("Editor.ImportingFrames"), gifMetadata.Frames.Count);

            // if (gifFile.Frames.Count <= 0)
            //     return listFrames;

            // var fullSize = gifFile.GetFullSize();
            // var index = 0;


            // var fileName = System.IO.Path.Combine(pathTemp, $"{index} {DateTime.Now:hh-mm-ss-ffff}.png");
            // foreach (var rawFrame in gifFile.Frames)
            // {
            //     var metadata = gifFile.Frames[index];

            //     var bitmapSource = ImageUtil.MakeFrame(fullSize, rawFrame, metadata, baseFrame);

            //     #region Disposal Method

            //     switch (metadata.DisposalMethod)
            //     {
            //         case FrameDisposalMethod.None:
            //         case FrameDisposalMethod.DoNotDispose:
            //             baseFrame = bitmapSource;
            //             break;
            //         case FrameDisposalMethod.RestoreBackground:
            //             baseFrame = ImageUtil.IsFullFrame(metadata, fullSize.width, fullSize.height) ? null : ImageUtil.ClearArea(bitmapSource, metadata);
            //             break;
            //         case FrameDisposalMethod.RestorePrevious:
            //             //Reuse same base frame.
            //             break;
            //     }

            //     #endregion

            //     #region Each Frame





            //     using (var stream = new System.IO.FileStream(fileName, System.IO. FileMode.Create))
            //     {
            //         var encoder = new PngBitmapEncoder();
            //         encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
            //         encoder.Save(stream);
            //         stream.Close();
            //     }

            //     //It should not throw a overflow exception because of the maximum value for the milliseconds.
            //     var frame = new Frame(fileName, (int)metadata.Delay.TotalMilliseconds);
            //     listFrames.Add(frame);

            //     UpdateProgress(index);

            //     GC.Collect(1);

            //     #endregion

            //     index++;
            // }

            return listFrames;
        }
        private List<Frame> ImportFromImage(string source, string pathTemp,int? width,int? heght)
        {
            var fileName =System.IO. Path.Combine(pathTemp, $"{0} {DateTime.Now:hh-mm-ss-ffff}.png");

            #region Save the Image to the Recording Folder

            SKBitmap bitmap= SkiaSharp.SKBitmap.Decode(source);
            if (width.HasValue && heght.HasValue)
            {
                bitmap.Resize(new SKImageInfo(width.Value, heght.Value), SKFilterQuality.High);// SKBitmapResizeMethod.Box);
            }
            using System.IO.FileStream stream = new System.IO.FileStream(source, System.IO.FileMode.Create);
            bitmap.PeekPixels().Encode(SKEncodedImageFormat.Png, 80).SaveTo(stream);
            //SixLabors.ImageSharp.Image image = SixLabors.ImageSharp.Image.Load(source);
            //image.Metadata.VerticalResolution = 96;
            //image.Metadata.HorizontalResolution = 96;
            // image.Save(fileName);

            //BitmapSource bitmap = new BitmapImage(new Uri(source));

            ////Don't let it import multiple images with different DPI's.
            //if (previousDpi > 0 && Math.Abs(previousDpi - bitmap.DpiX) > 0.09)
            //{
            //    warn = true;
            //    return null;
            //}

            //if (Math.Abs(previousDpi) < 0.01)
            //    previousDpi = bitmap.DpiX;

            //if (bitmap.Format != PixelFormats.Bgra32)
            //    bitmap = new FormatConvertedBitmap(bitmap, PixelFormats.Bgra32, null, 0);

            //using (var stream = new FileStream(fileName, FileMode.Create))
            //{
            //    var encoder = new PngBitmapEncoder();
            //    encoder.Frames.Add(BitmapFrame.Create(bitmap));
            //    encoder.Save(stream);
            //    stream.Close();
            //}

            GC.Collect();

            #endregion

            return new List<Frame> { new Frame(fileName, 66) };
        }

        private List<Frame> ImportFromApng(string source, string pathTemp)
        {
            //ShowProgress(DispatcherStringResource("Editor.ImportingFrames"), 50, true);



            //using FileStream stream = new FileStream(source, FileMode.Open, FileAccess.Read);

            //Apng apng = new Apng(stream);
            //if (!apng.ReadFrames())
            //    return ImportFromImage(source, pathTemp, null, null);


            var listFrames = new List<Frame>();

            //var fullSize = new System.Drawing.Size((int)apng.Ihdr.Width, (int)apng.Ihdr.Height);
           

            //BitmapSource baseFrame = null;
            //for (var index = 0; index < apng.Actl.NumFrames; index++)
            //{
            //    var metadata = apng.GetFrame(index);
            //    var rawFrame = SKBitmap.Decode(metadata.ImageData);


            //    var bitmapSource = Apng.MakeFrame(fullSize, rawFrame, metadata, baseFrame);

            //    #region Disposal Method

            //    switch (metadata.DisposeOp)
            //    {
            //        case Apng.DisposeOps.None: //No disposal is done on this frame before rendering the next; the contents of the output buffer are left as is.
            //            baseFrame = bitmapSource;
            //            break;
            //        case Apng.DisposeOps.Background: //The frame's region of the output buffer is to be cleared to fully transparent black before rendering the next frame.
            //            baseFrame = baseFrame == null || Apng.IsFullFrame(metadata, fullSize) ? null : Apng.ClearArea(baseFrame, metadata);
            //            break;
            //        case Apng.DisposeOps.Previous: //The frame's region of the output buffer is to be reverted to the previous contents before rendering the next frame.
            //                                       //Reuse same base frame.
            //            break;
            //    }

            //    #endregion

            //    #region Each Frame

            //    var fileName = Path.Combine(pathTemp, $"{index} {DateTime.Now:hh-mm-ss-ffff}.png");

            //    //TODO: Do I need to verify the DPI of the image?

            //    using (var output = new FileStream(fileName, FileMode.Create))
            //    {
            //        var encoder = new PngBitmapEncoder();
            //        encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
            //        encoder.Save(output);
            //        stream.Close();
            //    }

            //    list.Add(new FrameInfo(fileName, metadata.Delay));

            //    UpdateProgress(index);

            //    GC.Collect(1);

            //    #endregion
            //}


            return listFrames;
        }
        private List<Frame> ImportFromWebp(string source, string pathTemp)
        {
            return null;
        }

        private List<Frame> ImportFromVideo(string source, string pathTemp) {
            return null;
        }

        private List<Frame> ImportFromProject(string source, string pathTemp)
        {
            return null;
        }
    }
}
