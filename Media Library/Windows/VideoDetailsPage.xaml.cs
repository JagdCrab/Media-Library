using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Media_Library.Data;
using Media_Library.ViewModel;

namespace Media_Library.Windows
{
    /// <summary>
    /// Interaction logic for VideoDetailsPage.xaml
    /// </summary>
    public partial class VideoDetailsPage : Page
    {
        public VideoDetailsPage(string _path)
        {
            InitializeComponent();
            this.DataContext = new VideoDetailsPageViewModel(_path);
            VideoInfoTab.IsSelected = true;
        }

        public VideoDetailsPage(VideoRecord _videoRecord)
        {
            InitializeComponent();
            this.DataContext = new VideoDetailsPageViewModel(_videoRecord);
            ScreenlistTab.IsSelected = true;
        }

        private void Border_DragEnter(object _sender, DragEventArgs e)
        {
            var sender = (Border)_sender;

            e.Effects = DragDropEffects.Link;
            sender.Background = new SolidColorBrush(Colors.LightGray);
        }

        private void Border_DragLeave(object _sender, DragEventArgs e)
        {
            var sender = (Border)_sender;
            e.Effects = DragDropEffects.None;
            sender.Background = new SolidColorBrush(Colors.Transparent);
        }

        private void Border_Drop(object _sender, DragEventArgs e)
        {
            var sender = (Border)_sender;
            Regex regex = new Regex(".*(\\.jpg|\\.jpeg|\\.png|\\.bmp|\\.gif)");

            string[] inputs = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (inputs.Length == 1)
            {
                if (regex.IsMatch(inputs[0]))
                {
                    BitmapImage image = new BitmapImage();
                    using (var stream = new FileStream(inputs[0], FileMode.Open, FileAccess.Read))
                    {
                        image.BeginInit();
                        image.StreamSource = stream;
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.EndInit();
                        image.Freeze();
                    }

                    ((VideoDetailsPageViewModel)this.DataContext).Icon.Value = image;


                    var cropWindow = new CropWindow(((VideoDetailsPageViewModel)this.DataContext).Icon);
                    cropWindow.Show();
                    cropWindow.Activate();
                }
            }
            sender.Background = new SolidColorBrush(Colors.Transparent);
        }
    }
}
