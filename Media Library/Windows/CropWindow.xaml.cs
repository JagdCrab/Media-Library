using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

using Media_Library.Components;
using Media_Library.ViewModel;

namespace Media_Library.Windows
{
    /// <summary>
    /// Interaction logic for CropWindow.xaml
    /// </summary>
    public partial class CropWindow : Window
    {
        CropWindowViewModel ViewModel;

        public CropWindow(Observable<BitmapSource> _image)
        {
            InitializeComponent();

            this.Width = _image.Value.PixelWidth;
            this.Height = _image.Value.PixelHeight;

            ViewModel = new CropWindowViewModel(_image) { CloseAction = this.Close };
            this.DataContext = ViewModel;
        }

        private void Overlay_MouseMove(object sender, MouseEventArgs e)
        {
            Point location = e.MouseDevice.GetPosition(Image);
            ViewModel.Anchor.Value = new Point(location.X - ViewModel.CropSize.Value.Width / 2, location.Y - ViewModel.CropSize.Value.Height / 2);
        }

        private void Overlay_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel.BorderVisibility.Value == Visibility.Visible)
            {
                Point location = e.MouseDevice.GetPosition(Image);
                var anchor = new Point(location.X - ViewModel.CropSize.Value.Width / 2, location.Y - ViewModel.CropSize.Value.Height / 2);

                ViewModel.Hole.Value = new Rect(anchor, ViewModel.CropSize.Value);
                ViewModel.BorderVisibility.Value = Visibility.Hidden;
            }
            else
            {
                ViewModel.Hole.Value = new Rect(new Size(ViewModel.Width, ViewModel.Height));
                ViewModel.BorderVisibility.Value = Visibility.Visible;
            }
        }
    }
}
