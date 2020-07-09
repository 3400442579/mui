using An.Ava.Controls;
using An.Editor.Models;
using Ani.IMG.APNG;
using Ani.IMG.GIF;
using Ani.IMG.WEBP;
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

            using GifDecoder decoder = new GifDecoder(source);

            for (int i = 0; i < decoder.FrameCount; i++)
            {
                string fileName = System.IO.Path.Combine(pathTemp, $"{i} {DateTime.Now:hh-mm-ss-ffff}.png");
                int duration = decoder.GetFrame(i, fileName);

                var frame = new Frame(fileName, duration);
                listFrames.Add(frame);
                //  UpdateProgress(index);
            }

            GC.Collect(1);

            return listFrames;
        }
        private List<Frame> ImportFromImage(string source, string pathTemp,int? width,int? heght)
        {
            var fileName =System.IO. Path.Combine(pathTemp, $"{0} {DateTime.Now:hh-mm-ss-ffff}.png");

            SKBitmap bitmap= SkiaSharp.SKBitmap.Decode(source);
            if (width.HasValue && heght.HasValue)
            {
                bitmap.Resize(new SKImageInfo(width.Value, heght.Value), SKFilterQuality.High);// SKBitmapResizeMethod.Box);
            }

            //using System.IO.FileStream stream = new System.IO.FileStream(source, System.IO.FileMode.Create);
            //bitmap.PeekPixels().Encode(SKEncodedImageFormat.Png, 80).SaveTo(stream);
           

            GC.Collect();

            return new List<Frame> { new Frame(fileName, 66) };
        }

        private List<Frame> ImportFromApng(string source, string pathTemp)
        {
            //ShowProgress(DispatcherStringResource("Editor.ImportingFrames"), 50, true);

            var listFrames = new List<Frame>();

            using var stream = new FileStream(source, FileMode.Open);
            Apng apng = new Apng();
            apng.Load(stream);

            if (!apng.IsAnimated)
                return ImportFromImage(source, pathTemp, null, null);

            SKBitmap baseFrame = null;

            var fullSize = new SKSize((int)apng.Width, (int)apng.Height);
            for (int index = 0; index < apng.FrameCount; index++)
            {
                using SKBitmap rawFrame = apng.ToBitmap(index, out Ani.IMG.APNG.Frame frame);

                using SKBitmap bitmap = Render.MakeFrame(fullSize, rawFrame, frame, baseFrame);
                switch (frame.DisposeOp)
                {
                    case DisposeOperation.NONE:
                        baseFrame = bitmap.Copy();
                        break;
                    case DisposeOperation.BACKGROUND:
                        baseFrame = Render.IsFullFrame(frame, fullSize) ? null : Render.ClearArea(bitmap, frame);
                        break;
                    case DisposeOperation.PREVIOUS:
                        //Reuse same base frame.
                        break;
                }

                string fn = Path.Combine(pathTemp, $"{index}.png");
                using var output = new FileStream(fn, FileMode.Create);
                bitmap.Encode(SKEncodedImageFormat.Png, 100).SaveTo(output);
            }
            baseFrame?.Dispose();

            return listFrames;
        }

        private List<Frame> ImportFromWebp(string source, string pathTemp)
        {
            //ShowProgress(DispatcherStringResource("Editor.ImportingFrames"), 50, true);

            var listFrames = new List<Frame>();

            using WebpDecoder decoder = new WebpDecoder(source);

            for (int i = 0; i < decoder.FrameCount; i++)
            {
                string fileName = System.IO.Path.Combine(pathTemp, $"{i} {DateTime.Now:hh-mm-ss-ffff}.png");
                int duration = decoder.GetFrame(i, fileName);

                var frame = new Frame(fileName, duration);
                listFrames.Add(frame);
                //  UpdateProgress(index);
            }

            GC.Collect(1);

            return listFrames;
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
