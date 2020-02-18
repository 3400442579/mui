using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Animation.Editor.Core;
using Animation.Editor.Models;
using Animation.Editor.Utils;
using Loc;
using System.Reactive.Concurrency; // using Namespace
using System.Reactive.Linq;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Windows.Input;
using DH.MUI.Core;
using System.Windows.Threading;

namespace Animation.Editor.ViewModel
{
    public class MainViewModel //: INotifyPropertyChanged
    {

        private Project project;

        //预览
        private DispatcherTimer timer;

        public ReactiveProperty<string> Image { get; } = new ReactiveProperty<string>();
        public ReactiveCollection<FrameView> Frames { get; set; } = new ReactiveCollection<FrameView>();
        public ReactiveProperty<bool> IsInit { get;  } = new ReactiveProperty<bool>(false);

        public ReactiveProperty<FrameView> CurFrameView { get; set; } = new ReactiveProperty<FrameView>();

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
        public ReactiveCommand<DragEventArgs> DropCommand { get; }= new ReactiveCommand<DragEventArgs>();
        private void Drop(DragEventArgs e) {
            if (!(e.Data.GetData(DataFormats.FileDrop) is string[] fileNames))
                return;

            if (fileNames.Length == 0)
                return;

            var r = ChekFile(fileNames);
            if (!r.b)
                return;

            ImportUtil import = new ImportUtil(Dpi.GetDpiBySystemParameters(false).X);

            string d = Path.Combine(Config.Exa.TemporaryFolderResolved, Config.AppName);
            project = Project.Create(d);
            project.Frames = import.FromGif(fileNames[0], project.FullPath, 0);

            Image.Value = project.Frames[0].Path;

            foreach (var i in project.Frames)
                Frames.Add(new FrameView { Delay = i.Delay, Path = i.Path, Index = i.Index });

            IsInit.Value = true;
        }
        #endregion

        #region frame list view event

        private int delay = 0, frameIndex;
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

            if (delay != CurFrameView.Value.Delay) {
                timer.Stop();
                timer = new DispatcherTimer();
                timer.Tick += Timer_Tick;
                timer.Interval = TimeSpan.FromMilliseconds(CurFrameView.Value.Delay);
                delay = CurFrameView.Value.Delay;
            }

            timer.Start();
        }

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
        public ReactiveCommand<string> OpenFolderCommand { get; }=new ReactiveCommand<string>();
        private void OpenFolder(string s) {
            try
            {
                Process.Start("explorer.exe", $"/select,\"{s}\"");
            }
            catch (Exception ex)
            {
                
            }
        }
        public ReactiveCommand<string> OpenImageCommand { get; } = new ReactiveCommand<string>();
        private void OpenImage(string s) {
            Process.Start(s);
        }
        #endregion

        public MainViewModel()
        {
            OpenSetting = new ReactiveCommand().WithSubscribe(() =>
            {
                new SkinWindow().ShowDialog();
            });

            DropCommand.Subscribe<DragEventArgs>(e => Drop(e));
            DragEnterCommand.Subscribe(e => DragEnter(e));

            OpenFolderCommand.Subscribe(s => OpenFolder(s));
            OpenImageCommand.Subscribe(s => OpenImage(s));

            IObservable<bool> observable = Observable.Merge(IsInit.Select(o=>o==true));
            PlayStopCommand.Subscribe(b => PlayStop(b));
            GoToFrameCommand.Subscribe(s => GoToFrame(s));

            CurFrameView.Subscribe(fv => { if (fv != null) Image.Value = fv.Path; });
        }



        /// <summary>
        /// 文件验证
        /// </summary>
        /// <param name="fileNames"></param>
        /// <returns></returns>
        private (bool b, string err) ChekFile(string[] fileNames) {

            var extensionList = fileNames.Select(s => Path.GetExtension(s).ToLowerInvariant()).ToList();
            var media = new[] { ".jpg", ".jpeg", ".gif", ".bmp", ".png" };//, ".avi", ".mp4", ".wmv" };

            var projectCount = extensionList.Count(x => !string.IsNullOrEmpty(x) && (x.Equals(".stg") || x.Equals(".zip")));
            var mediaCount = extensionList.Count(x => !string.IsNullOrEmpty(x) && media.Contains(Path.GetExtension(x)));

            if (mediaCount == 0 && projectCount == 0)
                return (false, LangManager.Instance["FileFormatError"]);

            if (projectCount != 0 && mediaCount != 0)
                return (false, LangManager.Instance["ProjectImportError"] );

            return (true, string.Empty);
        }

        private void ShowLoading() { 
        
        }
        private void CloseLoading()
        {

        }
    }
}
