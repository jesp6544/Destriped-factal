using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Gui_client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Logic _logic;
        public MainWindow()
        {
            Console.WriteLine("test");
            InitializeComponent();
            _logic = new Logic((int)RenderImage.Width,(int)RenderImage.Height); // TODO get this from the actual img
            RenderImageToGui();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _logic.ModifyJob(1, -3, 2, -2);
            RenderImageToGui();
        }

        private void Img(object sender, MouseButtonEventArgs e)
        {
            _logic.Zoom(e.GetPosition(RenderImage).X / RenderImage.Width,e.GetPosition(RenderImage).Y / RenderImage.Height);
            RenderImageToGui();
        }

        private void RenderImageToGui()
        {
            RenderImage.Source = Convert(_logic.GetImage());
        }
        
        /// <summary>
        /// Takes a bitmap and converts it to an image that can be handled by WPF ImageBrush
        /// </summary>
        /// <param name="src">A bitmap image</param>
        /// <returns>The image as a BitmapImage for WPF</returns>
        public BitmapImage Convert(Bitmap src)
        {
            var ms = new MemoryStream();
            src.Save(ms, ImageFormat.Bmp);
            var image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var result = _logic.GetImage(2000, 2000);
            result.Save("Saved_image.png", ImageFormat.Png);
        }
    }
}