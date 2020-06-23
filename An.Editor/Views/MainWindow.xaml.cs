using An.Editor.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using System.Linq;

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
                (this.DataContext as MainWindowViewModel).ImportImage(e.Data.GetFileNames());
            }
        }
    }
}
