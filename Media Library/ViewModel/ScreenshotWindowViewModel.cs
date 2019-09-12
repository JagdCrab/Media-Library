using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

using Media_Library.Components;

namespace Media_Library.ViewModel
{
    class ScreenshotWindowViewModel
    {
        public BitmapSource Image { get; }
        public Command CloseWindow { get; set; }

        public Observable<int> Width { get; }
        public Observable<int> Height { get; }

        public ScreenshotWindowViewModel(BitmapSource _image)
        {
            Image = _image;
            Width = new Observable<int>() { Value = _image.PixelWidth };
            Height = new Observable<int>() { Value = _image.PixelHeight };
        }
    }
}
