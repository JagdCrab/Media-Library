using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

using Media_Library.Data;
using Media_Library.Components;

namespace Media_Library.ViewModel
{
    class CropWindowViewModel
    {
        public double Width { get; }
        public double Height { get; }
        public Action<BitmapSource> Callback { get; }

        public BitmapSource Image { get; }
        public Observable<BitmapSource> Icon { get; }

        public Observable<Point> Anchor { get; }
        public Observable<Size> CropSize { get; }

        public Rect Blackout { get; }
        public Observable<Rect> Hole { get; }

        public Observable<Visibility> BorderVisibility { get; }

        public Command IncreaseCropSize { get; }
        public Command DecreaseCropSize { get; }
        public Command Submit { get; }

        public Action CloseAction { get; set; }

        public CropWindowViewModel(BitmapSource _image, Observable<BitmapSource> _icon)
        {
            Icon = _icon;
            
            if (_image.PixelWidth > 1600 || _image.PixelHeight > 900)
            {
                Width = _image.PixelWidth;
                Height = _image.PixelHeight;

                double widthScaleFactor = 1600 / Width;
                double heightScaleFactor = 900 / Height;

                if (widthScaleFactor <= heightScaleFactor)
                {
                    Width = Width * widthScaleFactor;
                    Height = Height * widthScaleFactor;
                }
                else
                {
                    Width = Width * heightScaleFactor;
                    Height = Height * heightScaleFactor;
                }
                
                Image = MediaAccessor.ResizeImage(_image, Convert.ToInt32(Width), Convert.ToInt32(Height), false);
            }
            else
            {
                Image = _image;
                Width = _image.PixelWidth;
                Height = _image.PixelHeight;
            }

            Anchor = new Observable<Point>();
            CropSize = new Observable<Size>();
            Hole = new Observable<Rect>();
            BorderVisibility = new Observable<Visibility>();

            CropSize.Value = new Size(250, 250);

            Blackout = new Rect(new Size(Width, Height));
            Hole.Value = new Rect(new Size(Width, Height));
            BorderVisibility.Value = Visibility.Visible;

            IncreaseCropSize = new Command(new Action(() => {
                if (CropSize.Value.Width + 25 < Width && CropSize.Value.Height + 25 < Height)
                    CropSize.Value = new Size(CropSize.Value.Width + 25, CropSize.Value.Height + 25);
            }));

            DecreaseCropSize = new Command(new Action(() => {
                if (CropSize.Value.Width - 25 > 125 && CropSize.Value.Height - 25 > 125)
                    CropSize.Value = new Size(CropSize.Value.Width - 25, CropSize.Value.Height - 25);
            }));

            Submit = new Command(new Action(() => {
                int X = Convert.ToInt32(Hole.Value.X);
                int Y = Convert.ToInt32(Hole.Value.Y);
                int W = Convert.ToInt32(Hole.Value.Width);
                int H = Convert.ToInt32(Hole.Value.Height);

                var Rect = new Int32Rect(X, Y, W, H);
                var Result = new CroppedBitmap(Image, Rect);

                if (Callback == null)
                    Icon.Value = Result;
                else
                    Callback(Result);

                CloseAction();
            }));
        }
    }
}
