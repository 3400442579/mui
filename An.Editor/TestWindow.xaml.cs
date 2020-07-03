using An.Ava.Controls;
using An.Image.APNG;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography;
using Avalonia.Media;
using Avalonia;

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
            //using FileStream webpstream = new FileStream(@"C:\Users\jxw\Desktop\world-cup-2014-42.png", FileMode.Open, FileAccess.Read);
            //ApngDecoder webpDecoder = new ApngDecoder(@"E:\T\a\b0.png");
            //for (int i = 0; i < webpDecoder.FrameCount; i++)
            //    webpDecoder.GetFrame(i, $@"E:\T\a\png_{i}.jpg");

            APNG png = new APNG();
            png.Load(@"C:\Users\jxw\Desktop\world-cup-2014-42.png");
            for (int i = 0; i < png.NumEmbeddedPNG; i++)
            {
 
                png[i].Save(@"E:\T\a\frame" + i + ".png", ImageFormat.Png);
            }
            //using FileStream stream = new FileStream(@"C:\Users\jxw\Desktop\world-cup-2014-42.png", FileMode.Open);
            //Apng apng = new Apng(stream);



           
           
            
        }
    }
}
