using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Images that will be included in the gif. The duration is in milliseconds
            List<(string path, int duration)> images = new List<(string, int)>()
{
    //100 => 1 second
    ("image1.jpg", 100),
    ("image2.jpg", 150)
};

            // The final dimensions of the gif
            int width = 500, height = 500;

            // Create a blank canvas for the gif
            using (var gif = new Image<Rgba32>(width, height))
            {
                for (int i = 0; i < images.Count; i++)
                {
                    // Load image that will be added
                    using (var image = SixLabors.ImageSharp.Image.Load(images[i].path))
                    {
                        // Resize the image to the output dimensions
                        image.Mutate(ctx => ctx.Resize(width, height));

                        // Set the duration of the image
                        image.Frames.RootFrame.Metadata.FrameDelay = images[i].duration;

                        // Add the image to the gif
                        gif.Frames.InsertFrame(i, image.Frames.RootFrame);
                    }
                }

                // Save an encode the gif
                using (var fileStream = new FileStream("result.gif",FileMode.OpenOrCreate))
                {
                    gif.SaveAsGif(fileStream);
                }
            }
        }
    }
}
