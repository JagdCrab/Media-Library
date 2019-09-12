using System;
using System.Collections.ObjectModel;
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
    /// Interaction logic for ScreenshotWindow.xaml
    /// </summary>
    public partial class ScreenshotWindow : Window
    {
        public ScreenshotWindow(BitmapSource _source)
        {
            InitializeComponent();
            this.DataContext = new ScreenshotWindowViewModel(_source) { CloseWindow = new Command(new Action(() => { this.Close(); })) };
        }

        public ScreenshotWindow(ref ObservableCollection<BitmapSource> _sourceCollection)
        {
            InitializeComponent();
        }
    }
}
