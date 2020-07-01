using An.Editor.Controls;
using Avalonia;
using Avalonia.Markup.Xaml;

namespace An.Editor
{
    public class TestWindow : MetroWindow
    {
        public TestWindow()
        {
            this.InitializeComponent();
 
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
