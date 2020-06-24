using System;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using System.Collections.Generic;
using An.Editor.Controls;
using An.Editor.Models;
using Avalonia.Input;
using System.Linq;
using Avalonia.Controls;
using Avalonia.VisualTree;
using Avalonia.Controls.Shapes;
using System.IO;
using An.Image.Gif.Decoder;
using An.Editor.Util;
using An.Image.Gif.Decoding;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace An.Editor.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public Control View { get; set; }

        public MainWindowViewModel()
        {
            this.WhenAnyValue(o => o.ScaleValue).Subscribe(o =>
            {
                Scale = (int)o;
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
                var dialog = new OpenFileDialog();
                var result = await dialog.ShowAsync(window);
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


        private object scaleValue = 100;
        public object ScaleValue
        {
            set
            {
                if (scaleValue != value)
                {
                    scaleValue = value;
                    this.RaisePropertyChanged(nameof(ScaleValue));
                }
            }
            get
            {
                return scaleValue;
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

        public void ImportImage(string[] files)
        {
            IsLoading = true;

            var project = new Project().CreateProjectFolder();

            foreach (string f in files)
                InsertInternal(f, project.FullPath);
        }

        private List<Frame> InsertInternal(string fileName, string pathTemp)
        {
            List<Frame> listFrames;

            try
            {
                switch (System.IO.Path.GetExtension(fileName).ToLowerInvariant())
                {
                    //case "stg":
                    //case "zip":
                    //    {
                    //        listFrames = ImportFromProject(fileName, pathTemp);
                    //        break;
                    //    }

                    case "gif":
                        {
                            listFrames = ImportFromGif(fileName, pathTemp);
                            break;
                        }

                    //case "avi":
                    //case "mkv":
                    //case "mp4":
                    //case "wmv":
                    //case "webm":
                    //    {
                    //        listFrames = ImportFromVideo(fileName, pathTemp);
                    //        break;
                    //    }

                    //case "apng":
                    //case "png":
                    //    {
                    //        listFrames = ImportFromPng(fileName, pathTemp, ref previousDpi, ref warn);
                    //        break;
                    //    }

                    default:
                        {
                            listFrames = ImportFromImage(fileName, pathTemp);
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                listFrames = new List<Frame>();
            }

            return listFrames;
        }
        private List<Frame> ImportFromGif(string source, string pathTemp)
        {
            //ShowProgress(DispatcherStringResource("Editor.ImportingFrames"), 50, true);

            var listFrames = new List<Frame>();

            GifDecoder decoder = new GifDecoder(source);
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

        private List<Frame> ImportFromImage(string source, string pathTemp)
        {
            var fileName =System.IO. Path.Combine(pathTemp, $"{0} {DateTime.Now:hh-mm-ss-ffff}.png");

            #region Save the Image to the Recording Folder

            SixLabors.ImageSharp.Image image = SixLabors.ImageSharp.Image.Load(source);
            image.Metadata.VerticalResolution = 96;
            image.Metadata.HorizontalResolution = 96;
             image.Save(fileName);

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
    }
}
