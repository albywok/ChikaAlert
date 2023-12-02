using System;
using System.Windows;
using System.Windows.Media.Imaging;
using WpfAnimatedGif;

namespace ChikaAlert
{
    public partial class OverlayWindow : Window
    {
        public OverlayWindow()
        {
            InitializeComponent();
            EnableGif();

            ShowInTaskbar = true;
            Topmost = true;
            Closing += OverlayWindow_Closing;
        }

        private void EnableGif()
        {
            string relativePath = "Assets/chika-dance.gif";
            Uri gifUri = new Uri(relativePath, UriKind.Relative);
            ImageBehavior.SetAnimatedSource(gifImage, new BitmapImage(gifUri));
        }

        private void OverlayWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

    }
}