using An.Editor.ViewModels;
using An.Image.Gif.Decoding;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using System.Linq;
using System.IO;
using An.Image.Gif.Encoder;
using SkiaSharp;

namespace An.Editor.Views
{
    public class MainWindow : Window
    {

        private MainWindowViewModel vm;
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            AddHandler(DragDrop.DropEvent, Drop);
            AddHandler(DragDrop.DragOverEvent, DragOver);


            //using FileStream stream = new FileStream(@"C:\Users\jxw\Desktop\e.gif", FileMode.Open, FileAccess.Read);
            //GifDecoder decoder = new GifDecoder(stream);
            //for (int i = 0; i < decoder.Frames.Count; i++)
            //    decoder.RenderFrame(i, $@"E:\T\a\{i}.jpg");

            using FileStream stream = new FileStream(@"C:\Users\jxw\Desktop\1.gif", FileMode.Create, FileAccess.Write);
            GifFile gifFile = new GifFile(stream)
            {
                //TransparentColor = SKColors.Black,
                MaximumNumberColor = 256,
                QuantizationType = ColorQuantizationType.NeuQuant
            };
            gifFile.AddFrame(@"C:\Users\jxw\Desktop\gif问题\鱼\002_00000.png", new Image.Rect(0, 0, 1920, 1080),200);
            gifFile.AddFrame(@"C:\Users\jxw\Desktop\gif问题\鱼\002_00001.png", new Image.Rect(0, 0, 1920, 1080),200);
            gifFile.AddFrame(@"C:\Users\jxw\Desktop\gif问题\鱼\002_00002.png", new Image.Rect(0, 0, 1920, 1080),200);
             
        }

        private void DragOver(object sender, DragEventArgs e)
        {
            // Only allow Copy or Link as Drop Operations.
            e.DragEffects &= (DragDropEffects.Copy | DragDropEffects.Link);
            // Only allow if the dragged data contains text or filenames.
            if (!e.Data.Contains(DataFormats.FileNames))
                e.DragEffects = DragDropEffects.None;
        }

        private void Drop(object sender, DragEventArgs e)
        {
            if (e.Data.Contains(DataFormats.FileNames))
            {
                string[] files = e.Data.GetFileNames().ToArray();
                var vm = this.DataContext as MainWindowViewModel;
                var fs = vm.ValidationFile(files);
                (this.DataContext as MainWindowViewModel).ImportImage(fs.ToArray());
            }
        }
    }
}
