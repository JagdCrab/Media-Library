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

using Media_Library.ViewModel;
using Media_Library.Data;

namespace Media_Library.Windows
{
    /// <summary>
    /// Interaction logic for SeriesDetailsWindow.xaml
    /// </summary>
    public partial class SeriesDetailsWindow : Window
    {
        public SeriesDetailsWindow(string _path)
        {
            InitializeComponent();
            this.DataContext = new SeriesDetailsWindowViewModel(_path);
        }

        public SeriesDetailsWindow(VideoSeries _series)
        {
            InitializeComponent();
            this.DataContext = new SeriesDetailsWindowViewModel(_series);
        }
    }
}
