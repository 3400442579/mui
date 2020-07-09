using An.Ava.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System.Drawing.Imaging;

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

        public void button_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
