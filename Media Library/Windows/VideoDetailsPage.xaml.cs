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
        public VideoDetailsPage(VideoRecord _videoRecord)
        {
            InitializeComponent();
            this.DataContext = new VideoDetailsPageViewModel(_videoRecord);
        }
    }
}
