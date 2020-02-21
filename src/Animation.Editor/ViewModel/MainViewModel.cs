using Animation.Editor.Core;
using Animation.Editor.Models;
using Animation.Editor.Utils;
using Loc;
using Reactive.Bindings;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Animation.Editor.ViewModel
{
    public class MainViewModel //: INotifyPropertyChanged
    {
        #region
        //项目
        private Project project = new Project();
        //预览定时器
        private DispatcherTimer timer;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public ReactiveProperty<string> Image { get; } = new ReactiveProperty<string>();
        /// <summary>
        /// 
        /// </summary>
        public ReactiveCollection<FrameView> Frames { get; } = new ReactiveCollection<FrameView>();
        /// <summary>
        /// 
        /// </summary>
        public ReactivePropertySlim<bool> IsInit { get; } = new ReactivePropertySlim<bool>(false);
        /// <summary>
        /// 当前帧
        /// </summary>
        public ReactivePropertySlim<FrameView> CurFrameView { get; set; } = new ReactivePropertySlim<FrameView>();

        /// <summary>
        /// 画布宽度
        /// </summary>
        public ReactiveProperty<double> CanvasWidth { get; } = new ReactiveProperty<double>(0d);
        /// <summary>
        /// 画布高度
        /// </summary>
        public ReactiveProperty<double> CanvasHeight { get; } = new ReactiveProperty<double>(0d);
        /// <summary>
        /// 
        /// </summary>
        public ReactiveProperty<double> Zoom { get; set; } = new ReactiveProperty<double>(1d);



        public MainViewModel()
        {
            #region 事件绑定
            OpenSetting = new ReactiveCommand().WithSubscribe(() =>
            {
                new SkinWindow().ShowDialog();
            });

            DropCommand.Subscribe(e => DropAsync(e));
            DragEnterCommand.Subscribe(e => DragEnter(e));

            OpenFolderCommand.Subscribe(s => OpenFolder(s));
            OpenImageCommand.Subscribe(s => OpenImage(s));

            IObservable<bool> observable = Observable.Merge(IsInit.Select(o => o == true));
            PlayStopCommand.Subscribe(b => PlayStop(b));
            GoToFrameCommand.Subscribe(s => GoToFrame(s));

           
            ZoomCommand = IsInit.ToReactiveCommand<string>()
                .WithSubscribe(s => Zooms(s));

            #endregion

            CurFrameView.Subscribe(fv => { if (fv != null) Image.Value = fv.Path; });
        }

        /// <summary>
        /// 打开设置
        /// </summary>
        public ReactiveCommand OpenSetting { get; }

        #region Drag 打开文件
        /// <summary>
        /// DragEnter
        /// </summary>
        public ReactiveCommand<DragEventArgs> DragEnterCommand { get; } = new ReactiveCommand<DragEventArgs>();
        private void DragEnter(DragEventArgs e)
        {
            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop)
                   ? DragDropEffects.Copy
                   : DragDropEffects.None;
        }
        /// <summary>
        /// Drag 打开文件 
        /// </summary>
        public AsyncReactiveCommand<DragEventArgs> DropCommand { get; } = new AsyncReactiveCommand<DragEventArgs>();
        private async Task DropAsync(DragEventArgs e)
        {
            if (!(e.Data.GetData(DataFormats.FileDrop) is string[] fileNames))
                return;

            if (fileNames.Length == 0)
                return;

            var (b, err) = ChekFile(fileNames);
            if (!b)
                return;

            ImportUtil import = new ImportUtil(Dpi.GetDpiBySystemParameters(false).X);

            if (project.IsEmpty())
            {
                string d = Path.Combine(Config.Default.TemporaryFolderResolved, Config.AppName);
                project = Project.Create(d);
            }
            else { 
            
            }


            var frames = await Task.Factory.StartNew(() => import.FromGif(fileNames[0], project.FullPath, 0));
            var s1 = ImageUtil.SizeOf(project.Frames[0].Path);
            project.Width = s1.Width;
            project.Height = s1.Height;


            Image.Value = project.Frames[0].Path;

            CanvasWidth.Value = project.Width;
            CanvasHeight.Value = project.Height;
            foreach (var i in project.Frames)
                Frames.Add(new FrameView { Delay = i.Delay, Path = i.Path, Index = i.Index });

            IsInit.Value = true;
        }
        #endregion

        #region 帧列表 相关事件

        private int delay = 0, frameIndex;
        /// <summary>
        /// 
        /// </summary>
        public ReactiveCommand<bool?> PlayStopCommand { get; } = new ReactiveCommand<bool?>();
        private void PlayStop(bool? strat)
        {
            if (!strat.HasValue || strat == false)
            {
                if (timer != null)
                {
                    timer.Stop();
                    timer = null;
                }
            }
            else
            {
                if (CurFrameView.Value == null)
                {
                    frameIndex = 0;
                    CurFrameView.Value = Frames.ElementAt(frameIndex);
                }
                else
                    frameIndex = CurFrameView.Value.Index;

                timer = new DispatcherTimer();
                timer.Tick += Timer_Tick;
                timer.Interval = TimeSpan.FromMilliseconds(CurFrameView.Value.Delay);
                delay = CurFrameView.Value.Delay;
                timer.Start();
            }
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            ++frameIndex;
            if (frameIndex == Frames.Count)
                frameIndex = 0;
            CurFrameView.Value = Frames.ElementAt(frameIndex);

            if (delay != CurFrameView.Value.Delay)
            {
                timer.Stop();
                timer = new DispatcherTimer();
                timer.Tick += Timer_Tick;
                timer.Interval = TimeSpan.FromMilliseconds(CurFrameView.Value.Delay);
                delay = CurFrameView.Value.Delay;
            }

            timer.Start();
        }

        /// <summary>
        /// 跳到指定帧
        /// </summary>
        public ReactiveCommand<string> GoToFrameCommand { get; } = new ReactiveCommand<string>();
        private void GoToFrame(string type)
        {
            if (CurFrameView.Value == null)
                frameIndex = 0;
            else
                frameIndex = CurFrameView.Value.Index;

            switch (type)
            {
                case "first":
                    frameIndex = 0;
                    break;
                case "last":
                    frameIndex = Frames.Count - 1;
                    break;
                case "forward":

                    if (frameIndex == 0)
                        frameIndex = Frames.Count - 1;
                    else
                        frameIndex -= 1;
                    break;
                case "backward":
                    if (frameIndex == Frames.Count - 1)
                        frameIndex = 0;
                    else
                        frameIndex += 1;
                    break;
            }

            CurFrameView.Value = Frames.ElementAt(frameIndex);
        }

        /// <summary>
        /// 打开目录
        /// </summary>
        public ReactiveCommand<string> OpenFolderCommand { get; } = new ReactiveCommand<string>();
        private void OpenFolder(string s)
        {
            try
            {
                Process.Start("explorer.exe", $"/select,\"{s}\"");
            }
            catch (Exception ex)
            {

            }
        }
        /// <summary>
        /// 打开图片
        /// </summary>
        public ReactiveCommand<string> OpenImageCommand { get; } = new ReactiveCommand<string>();
        private void OpenImage(string s)
        {
            Process.Start(s);
        }
        #endregion

        public ReactiveCommand<string> ZoomCommand { get; }
        private void Zooms(string s) {
            if (s=="+")
            {
                Zoom.Value += 0.1d;
                if (Zoom.Value > 5d)
                    Zoom.Value = 5d;
            }
            else { 
                Zoom.Value -= 0.1d;
                if (Zoom.Value < 0.1d)
                    Zoom.Value = 0.1d;
            }
        }


        /// <summary>
        /// 文件验证
        /// </summary>
        /// <param name="fileNames"></param>
        /// <returns></returns>
        private (bool b, string err) ChekFile(string[] fileNames)
        {
            var extensionList = fileNames.Select(s => Path.GetExtension(s).ToLowerInvariant()).ToList();
            var media = new[] { ".jpg", ".jpeg", ".gif", ".bmp", ".png" };//, ".avi", ".mp4", ".wmv" };

            var projectCount = extensionList.Count(x => !string.IsNullOrEmpty(x) && (x.Equals(".stg") || x.Equals(".zip")));
            var mediaCount = extensionList.Count(x => !string.IsNullOrEmpty(x) && media.Contains(Path.GetExtension(x)));

            if (mediaCount == 0 && projectCount == 0)
                return (false, LangManager.Instance["FileFormatError"]);

            if (projectCount != 0 && mediaCount != 0)
                return (false, LangManager.Instance["ProjectImportError"]);

            return (true, string.Empty);
        }

        private void ShowLoading()
        {

        }
        private void CloseLoading()
        {

        }
    }
}
