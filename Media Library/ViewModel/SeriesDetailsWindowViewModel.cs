using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Media_Library.Windows;
using Media_Library.Data;

namespace Media_Library.ViewModel
{
    class SeriesDetailsWindowViewModel
    {
        public string Series { get; }
        public ObservableCollection<VideoDetailsPage> VideoDetailsPages { get; }

        public SeriesDetailsWindowViewModel(VideoSeries _series)
        {
            Series = _series.Series;
            VideoDetailsPages = new ObservableCollection<VideoDetailsPage>();

            foreach (var video in _series.VideoRecords)
                VideoDetailsPages.Add(new VideoDetailsPage(video));
        }
    }
}
