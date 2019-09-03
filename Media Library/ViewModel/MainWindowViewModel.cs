using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media_Library.ViewModel
{
    class MainWindowViewModel
    {
        public SettingsTabViewModel SettingsViewModel { get; }
        public VideoTabViewModel VideoViewModel { get; }

        public MainWindowViewModel()
        {
            SettingsViewModel = new SettingsTabViewModel();
            VideoViewModel = new VideoTabViewModel();
        }
    }
}
