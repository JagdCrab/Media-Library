using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

using Media_Library.Components;

namespace Media_Library.ViewModel
{
    class CropWindowViewModel
    {
        public double Width { get; }
        public double Height { get; }
        public Action<BitmapSource> Callback { get; }

        public Observable<BitmapSource> Image { get; }

        public Observable<Point> Anchor { get; }
        public Observable<Size> CropSize { get; }

        public Rect Blackout { get; }
        public Observable<Rect> Hole { get; }

        public Observable<Visibility> BorderVisibility { get; }

        public Command IncreaseCropSize { get; }
        public Command DecreaseCropSize { get; }
        public Command Submit { get; }

        public Action CloseAction { get; set; }

        public CropWindowViewModel(Observable<BitmapSource> _image)
        {
            Image = _image;
            Width = _image.Value.PixelWidth;
            Height = _image.Value.PixelHeight;

            if (Width > 1600 || Height > 900)
            {
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
                var Result = new CroppedBitmap(Image.Value, Rect);

                if (Callback == null)
                    Image.Value = Result;
                else
                    Callback(Result);

                CloseAction();
            }));
        }
    }
}
