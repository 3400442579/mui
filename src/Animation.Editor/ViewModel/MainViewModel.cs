using Animation.Editor.Core;
using Animation.Editor.Models;
using Animation.Editor.Utils;
using DH.MUI.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Loc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;

namespace Animation.Editor.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region
        private readonly Notifier _notifier;

        //预览定时器
        private DispatcherTimer timer;
        #endregion

        //项目
        public Fis Project { get; private set; } = Fis.Empty();

        private bool isDoing = false;
        public bool IsDoing
        {
            get => isDoing;
            set => Set(ref isDoing, value);
        }


        private string image;
        public string Image
        {
            get => image;
            set => Set(ref image, value);
        }
        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<Fi> Frames { get; } = new ObservableCollection<Fi>();

        private bool isInit = false;
        public bool IsInit
        {
            get => isInit;
            set {
                if (isInit != value)
                {
                    isInit = value;
                    RaisePropertyChanged("IsInit");
                     
                   // RemoveFrameCommand.RaiseCanExecuteChanged();
                }
            }
        }
        /// <summary>
        /// 当前帧
        /// </summary>
        private int? selectIndex;
        public int? SelectIndex
        {
            get => selectIndex;
            set
            {
                if (selectIndex != value)
                {
                    selectIndex = value;
                    RaisePropertyChanged("SelectIndex");
                    UpdateImage(selectIndex);

                    //RemoveFrameCommand.RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// 画布宽度
        /// </summary>
        private double canvasWidth = 0d;
        public double CanvasWidth
        {
            get => canvasWidth;
            set => Set(ref canvasWidth, value);
        }

        /// <summary>
        /// 画布高度
        /// </summary>
        private double canvasHeight=0d;
        public double CanvasHeight
        {
            get => canvasHeight;
            set => Set(ref canvasHeight, value);
        }
        
        /// <summary>
        /// 
        /// </summary>
        private double zoom = 1d;
        public double Zoom
        {
            get => zoom;
            set => Set(ref zoom, value);
        }


        public MainViewModel()
        {
            _notifier = new Notifier(cfg =>
            {
                cfg.PositionProvider = new WindowPositionProvider(
                    parentWindow: Application.Current.MainWindow,
                    corner: Corner.BottomRight,
                    offsetX: 25,
                    offsetY: 100);

                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: TimeSpan.FromSeconds(6),
                    maximumNotificationCount: MaximumNotificationCount.FromCount(6));

                cfg.Dispatcher = Application.Current.Dispatcher;

                cfg.DisplayOptions.TopMost = false;
                cfg.DisplayOptions.Width = 250;
            });

            #region 事件绑定
            //OpenSetting = new RelayCommand(() => new SkinWindow().ShowDialog());

            DropCommand = new RelayCommand<DragEventArgs>(async e => await DropAsync(e));
            DropCommand = new RelayCommand<DragEventArgs>(async e => await DropAsync(e));

            OpenFolderCommand = new RelayCommand<string>(s => OpenFolder(s));
            OpenImageCommand = new RelayCommand<string>(s => OpenImage(s));

            //IObservable<bool> observable = Observable.Merge(IsInit.Select(o => o == true));
            PlayStopCommand = new RelayCommand<bool?>(b => PlayStop(b), canExecute: (b) => IsInit);
            GoToFrameCommand = new RelayCommand<string>(s => GoToFrame(s), canExecute: (b) => IsInit);
            SelectionChangedCommand = new RelayCommand<SelectionChangedEventArgs>(s => SelectionChanged(s));

            //IObservable<bool> observable1 = Observable.Merge(SelectIndex.Select(o => o >= 0),IsDoing.Select(o=>!o));
            RemoveFrameCommand = new RelayCommand(
                execute: async () => await RemoveFramesAsync());//,
               // canExecute: () => SelectIndex.HasValue && !IsDoing);

            ZoomCommand = new RelayCommand<string>(s => Zooms(s), canExecute: b => SelectIndex.HasValue);

            #endregion
        }

       

        /// <summary>
        /// 打开设置
        /// </summary>
        public RelayCommand OpenSetting { get; private set; }

        #region Drag 打开文件
        /// <summary>
        /// DragEnter
        /// </summary>
        public RelayCommand<DragEventArgs> DragEnterCommand { get; private set; }
        private void DragEnter(DragEventArgs e)
        {
            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop)
                   ? DragDropEffects.Copy
                   : DragDropEffects.None;
        }
        /// <summary>
        /// Drag 打开文件 
        /// </summary>
        public RelayCommand<DragEventArgs> DropCommand { get; private set; } 
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
                CanvasWidth = Project.Width;
                CanvasHeight = Project.Height;
                SelectIndex = 0;
                IsInit = true;
            }
            else {
                int index;
                if (SelectIndex.HasValue)
                    index = SelectIndex.Value;
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
        public RelayCommand<bool?> PlayStopCommand { get; }
        private void PlayStop(bool? strat)
        {
            if (!strat.HasValue || strat == false)
            {
                if (timer != null)
                {
                    timer.Stop();
                    timer = null;
                    IsDoing = false;
                }
            }
            else
            {
                IsDoing = true;
                if (SelectIndex == null)
                    SelectIndex = 0;
                
                Fi frame = Frames[SelectIndex.Value];
                odelay = frame.Delay;

                timer = new DispatcherTimer();
                timer.Tick += Timer_Tick;
                timer.Interval = TimeSpan.FromMilliseconds(frame.Delay);
                timer.Start();
            }
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            int n = SelectIndex.Value + 1;
            if (n == Frames.Count)
                n = 0;
            SelectIndex = n;
            Fi frame = Frames[SelectIndex.Value];
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
        public RelayCommand<string> GoToFrameCommand { get; } 
        private void GoToFrame(string type)
        {
            switch (type)
            {
                case "first":
                    SelectIndex = 0;
                    break;
                case "last":
                    SelectIndex = Frames.Count - 1;
                    break;
                case "forward":

                    if (SelectIndex.Value == 0)
                        SelectIndex = Frames.Count - 1;
                    else
                        SelectIndex -= 1;
                    break;
                case "backward":
                    if (SelectIndex.Value == Frames.Count - 1)
                        SelectIndex = 0;
                    else
                        SelectIndex += 1;
                    break;
            }
            //Image.Value = Frames[SelectIndex.Value.Value].Path;
        }

        public RelayCommand<SelectionChangedEventArgs> SelectionChangedCommand { get; }
        private void SelectionChanged(SelectionChangedEventArgs e) {
            if (e.AddedItems.Count > 0)
                Image = ((Fi)e.AddedItems[^1]).Path;
            else
                Image = "";
        }

        /// <summary>
        /// 打开目录
        /// </summary>
        public RelayCommand<string> OpenFolderCommand { get; } 
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
        public RelayCommand<string> OpenImageCommand { get; }
        private void OpenImage(string s)
        {
            Process.Start(s);
        }
        #endregion


        //画布缩放
        public RelayCommand<string> ZoomCommand { get; }
        private void Zooms(string s) {
            if (s=="+")
            {
                Zoom += 0.1d;
                if (Zoom > 5d)
                    Zoom = 5d;
            }
            else { 
                Zoom -= 0.1d;
                if (Zoom < 0.1d)
                    Zoom = 0.1d;
            }
        }

        /// <summary>
        /// 修改帧的延迟时间
        /// </summary>
        public RelayCommand ChangeDelayCommand { get; }
        private void ChangeDelay()
        {

        }

        //添加帧
        public RelayCommand AddFrameCommand { get; } 
        private void AddFrame() { 
        
        }

        //删除帧
        public RelayCommand RemoveFrameCommand { get; private set; }
        private async Task RemoveFramesAsync()
        {
            //var dlg = new ModernDialog{ Title = Lang.Data.DeleteTitle,   Content = Lang.Data.ConfirmDelete};
            //dlg.Buttons = new Button[] { dlg.CreateButton(MessageBoxResult.OK, Lang.Data.Ok,true) ,dlg.YesButton, dlg.CancelButton };
            //dlg.ShowDialog()

            var messageResult = ModernDialog.ShowMessage(Lang.Data.DeleteTitle,
                 Lang.Data.ConfirmDelete,
                 MessageBoxButton.YesNo,
                 new Dictionary<string, string> { { "Yes", Lang.Data.Yes }, { "No", Lang.Data.No } });
            if (messageResult != MessageBoxResult.Yes)
                return;

            IsDoing = true;

            await Task.Factory.StartNew(async () =>
                  {
                      var r = SelectFrameAndIndex();
                      await Task.Delay(2000);
                      ActionStack.SaveState(ActionStack.EditAction.Remove, r.Frames, r.Indexs);
                      foreach (var f in r.Frames)
                      {
                          Application.Current.Dispatcher.Invoke(() => Frames.Remove(f));
                          //Frames.Remove(f);
                          Project.Frames.Remove(f);
                          try { File.Delete(f.Path); }
                          catch { }
                      }

                      //更新图片
                      if (Frames.Count > 0)
                      {
                          int n = r.Indexs.Min() - 1;
                          if (n < 0)
                              n = 0;
                          SelectIndex = n;
                      }
                      else
                          SelectIndex = null;

                      IsDoing = false;
                  });

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
                return (false, Lang.Data["FileFormatError"]);

            if (projectCount != 0 && mediaCount != 0)
                return (false, Lang.Data["ProjectImportError"]);

            return (true, string.Empty);
        }

        /// <summary>
        /// 更新Image
        /// </summary>
        /// <param name="index"></param>
        private void UpdateImage(int? index)
        {
            if (index == null || index.Value < 0)
                Image = "";
            else
                Image = Frames[index.Value].Path;
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
        ~MainViewModel() {
            _notifier.Dispose();
        }
    }
}
