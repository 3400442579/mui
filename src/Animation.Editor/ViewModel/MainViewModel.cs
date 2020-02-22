using Animation.Editor.Core;
using Animation.Editor.Models;
using Animation.Editor.Utils;
using Loc;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Animation.Editor.ViewModel
{
    public class MainViewModel //: INotifyPropertyChanged
    {
        #region
       
        //预览定时器
        private DispatcherTimer timer;
        #endregion

        //项目
        public Fis Project { get; private set; } = Fis.Empty();
        public ReactivePropertySlim<bool> IsDoing { get; } = new ReactivePropertySlim<bool>(false);
        /// <summary>
        /// 
        /// </summary>
        public ReactiveProperty<string> Image { get; } = new ReactiveProperty<string>();
        /// <summary>
        /// 
        /// </summary>
        public ReactiveCollection<Fi> Frames { get; } = new ReactiveCollection<Fi>();
        /// <summary>
        /// 
        /// </summary>
        public ReactivePropertySlim<bool> IsInit { get; } = new ReactivePropertySlim<bool>(false);
        /// <summary>
        /// 当前帧
        /// </summary>
        public ReactivePropertySlim<int?> SelectIndex { get; set; } = new ReactivePropertySlim<int?>();
       
        /// <summary>
        /// 画布宽度
        /// </summary>
        public ReactivePropertySlim<double> CanvasWidth { get; } = new ReactivePropertySlim<double>(0d);
        /// <summary>
        /// 画布高度
        /// </summary>
        public ReactivePropertySlim<double> CanvasHeight { get; } = new ReactivePropertySlim<double>(0d);
        
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
            SelectionChangedCommand.Subscribe(s => SelectionChanged(s));

            IObservable<bool> observable1 = Observable.Merge(SelectIndex.Select(o => o >= 0),IsDoing.Select(o=>!o));
            RemoveFrameCommand = observable1.ToReactiveCommand()
                .WithSubscribe(() => RemoveFrames());

            ZoomCommand = SelectIndex.Select(o => o >= 0).ToReactiveCommand<string>()
               .WithSubscribe(s => Zooms(s));

            #endregion

            SelectIndex.Subscribe(s => UpdateImage(s));
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

            
            bool proIsEmpty = Project.IsEmpty;
            if (proIsEmpty)
            {
                string d = Path.Combine(Config.Default.TemporaryFolderResolved, Config.AppName);
                Project = Fis.Create(d);
                Project.CreateMutex();
            }
           
            ImportUtil import = new ImportUtil(Dpi.GetDpiBySystemParameters(false).X);

            List<Fi> addFrames = new List<Fi>();
            foreach (string f in fileNames)
            {
                List<Fi> frames = await Task.Factory.StartNew(() => import.From(f, Project.FullPath));
                if (frames.Count > 0)
                    addFrames.AddRange(frames);
            }

            if (proIsEmpty)
            {
                foreach (var i in addFrames)
                {
                    Frames.Add(i);
                    Project.Frames.Add(i);
                }
                var s1 = ImageUtil.SizeOf(addFrames[0].Path);
                Project.Width = s1.Width;
                Project.Height = s1.Height;
                CanvasWidth.Value = Project.Width;
                CanvasHeight.Value = Project.Height;
                SelectIndex.Value = 0;
                IsInit.Value = true;
            }
            else {
                int index;
                if (SelectIndex.Value.HasValue)
                    index = SelectIndex.Value.Value;
                else
                    index = Frames.Count;

                foreach (var i in addFrames) {
                    Frames.Insert(index, i);
                    Project.Frames.Insert(index, i);
                    index++;
                }
            }
            Project.Persist();
        }
        #endregion

        #region 帧列表 相关事件

        private int odelay = 0;
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
                    IsDoing.Value = false;
                }
            }
            else
            {
                IsDoing.Value = true;
                if (SelectIndex.Value == null)
                    SelectIndex.Value = 0;
                
                Fi frame = Frames[SelectIndex.Value.Value];
                odelay = frame.Delay;

                timer = new DispatcherTimer();
                timer.Tick += Timer_Tick;
                timer.Interval = TimeSpan.FromMilliseconds(frame.Delay);
                timer.Start();
            }
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            int n = SelectIndex.Value.Value + 1;
            if (n == Frames.Count)
                n = 0;
            SelectIndex.Value = n;
            Fi frame = Frames[SelectIndex.Value.Value];
            if (odelay != frame.Delay)
            {
                timer.Stop();
                timer = new DispatcherTimer();
                timer.Tick += Timer_Tick;
                timer.Interval = TimeSpan.FromMilliseconds(frame.Delay);
                odelay = frame.Delay;
                timer.Start();
            }

            //Image.Value = frame.Path;
        }

        /// <summary>
        /// 跳到指定帧
        /// </summary>
        public ReactiveCommand<string> GoToFrameCommand { get; } = new ReactiveCommand<string>();
        private void GoToFrame(string type)
        {
            switch (type)
            {
                case "first":
                    SelectIndex.Value = 0;
                    break;
                case "last":
                    SelectIndex.Value = Frames.Count - 1;
                    break;
                case "forward":

                    if (SelectIndex.Value == 0)
                        SelectIndex.Value = Frames.Count - 1;
                    else
                        SelectIndex.Value -= 1;
                    break;
                case "backward":
                    if (SelectIndex.Value == Frames.Count - 1)
                        SelectIndex.Value = 0;
                    else
                        SelectIndex.Value += 1;
                    break;
            }
            //Image.Value = Frames[SelectIndex.Value.Value].Path;
        }

        public ReactiveCommand<SelectionChangedEventArgs> SelectionChangedCommand { get; } = new ReactiveCommand<SelectionChangedEventArgs>();
        private void SelectionChanged(SelectionChangedEventArgs e) {
            if (e.AddedItems.Count > 0)
                Image.Value = ((Fi)e.AddedItems[^1]).Path;
            else
                Image.Value = "";
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


        //画布缩放
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

        //删除帧
        public ReactiveCommand RemoveFrameCommand { get; }
        private void RemoveFrames()
        {
            IsDoing.Value = true;
            var r = SelectFrameAndIndex();
            try
            {
                ActionStack.SaveState(ActionStack.EditAction.Remove, r.Frames, r.Indexs);

                foreach (var f in r.Frames)
                {
                    Frames.Remove(f);
                    Project.Frames.Remove(f);
                    try { File.Delete(f.Path); }
                    catch { }
                }
            }
            catch (Exception e)
            {
            }

            //更新图片
            if (Frames.Count > 0)
            {
                int n = r.Indexs.Min()-1;
                if (n < 0)
                    n = 0;
                SelectIndex.Value = n;
            }
            else
                SelectIndex.Value = null;

            IsDoing.Value = false;
        }

        
        /// <summary>
        /// 获取先中帧
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        private List<Fi> SelectFrame()
        {
            return Frames.Where(o => o.Selected).ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private (List<Fi> Frames, List<int> Indexs) SelectFrameAndIndex()
        {
            return (
                Frames.Where(o => o.Selected).ToList(),
                Frames.Where(o => o.Selected).Select(o => Frames.IndexOf(o)).ToList()
            );
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

        /// <summary>
        /// 更新Image
        /// </summary>
        /// <param name="index"></param>
        private void UpdateImage(int? index)
        {
            if (index == null || index.Value < 0)
                Image.Value = "";
            else
                Image.Value = Frames[index.Value].Path;
        }

        /// <summary>
        /// 
        /// </summary>
        private void ShowLoading()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        private void CloseLoading()
        {

        }
    }
}
