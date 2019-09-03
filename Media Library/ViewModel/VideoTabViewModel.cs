using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Media_Library.Components;
using Media_Library.Data;

namespace Media_Library.ViewModel
{
    class VideoTabViewModel
    {
        public ObservableCollection<VideoSeriesEntity> VideoSeriesEntities { get; }
        public ICollectionView VideoSeriesEntitiesView { get; }
        
        public Observable<string> AutoCompleteText { get; }
        public Observable<VideoSearchEntity> SelectedSearchEntity { get; }
        public Collection<VideoSearchEntity> VideoSearchEntities { get; }

        public Observable<Visibility> ClearButtonVisibility { get; }
        public ObservableCollection<VideoSearchEntity> FilterEntities { get; }

        public Observable<string> SelectedOrdering { get; }
        public List<string> OrderingOptions { get { return new List<string>() { "Alphabetical","Source"}; } }

        public Command OrderingChanged {
            get {
                return new Command(new Action(()=> {
                    string selection = SelectedOrdering.Value;

                    if (SelectedOrdering.Value == "Source")
                    {
                        VideoSeriesEntitiesView.SortDescriptions.Clear();
                        VideoSeriesEntitiesView.SortDescriptions.Add(new SortDescription("Inserted", ListSortDirection.Ascending));
                    }
                    else
                    {
                        VideoSeriesEntitiesView.SortDescriptions.Clear();
                        VideoSeriesEntitiesView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
                    }
                }));
            }
        }

        public Command AddFilter {
            get {
                return new Command(new Action(() => {
                    if (SelectedSearchEntity.Value != null)
                        FilterEntities.Add(SelectedSearchEntity.Value);
                    else
                        FilterEntities.Add(new VideoSearchEntity(AutoCompleteText.Value));

                    SelectedSearchEntity.Value = null;
                    AutoCompleteText.Value = string.Empty;

                    if (ClearButtonVisibility.Value != Visibility.Visible)
                        ClearButtonVisibility.Value = Visibility.Visible;

                    VideoSeriesEntitiesView.Filter = vse => {
                        if (FilterEntities.All(x => {
                            if (x.Type != "Other")
                                return ((VideoSeriesEntity)vse).SearchEntities.Any(y => y.Text == x.Text);
                            else
                                return ((VideoSeriesEntity)vse).SearchEntities.Any(y => y.Text.Contains(x.Text));
                        }))
                            return true;
                        else
                            return false;
                    };
                }));
            }
        }

        public Command ClearFilters {
            get {
                return new Command(new Action(() => {
                    FilterEntities.Clear();
                    ClearButtonVisibility.Value = Visibility.Collapsed;
                    VideoSeriesEntitiesView.Filter = vse => { return true; };
                }));
            }
        }

        public VideoTabViewModel()
        {
            VideoSeriesEntities = new ObservableCollection<VideoSeriesEntity>();
            VideoSeriesEntitiesView = CollectionViewSource.GetDefaultView(VideoSeriesEntities);

            AutoCompleteText = new Observable<string>();
            SelectedSearchEntity = new Observable<VideoSearchEntity>();
            VideoSearchEntities = new Collection<VideoSearchEntity>();

            ClearButtonVisibility = new Observable<Visibility>() { Value = Visibility.Collapsed };
            FilterEntities = new ObservableCollection<VideoSearchEntity>();
            
            foreach (var searchEntity in VideoAccesser.GetSearchEntities())
                VideoSearchEntities.Add(new VideoSearchEntity(searchEntity));

            foreach (var series in VideoAccesser.GetVideoSeries())
                VideoSeriesEntities.Add(new VideoSeriesEntity(series));
            
            SelectedOrdering = new Observable<string>() { Value = "Alphabetical" };
        }
    }

    class VideoSeriesEntity
    {
        public VideoSeries VideoSeries;

        public int? Sid { get { return VideoSeries.Sid; } }
        public string Name { get { return VideoSeries.Series; } }
        public BitmapSource Icon { get { return VideoSeries.Icon; } }

        public SearchEntityCollection SearchEntities;

        public Observable<Visibility> Visible;
        public Command GetSeriesDetails { get {
                return new Command(new Action(() => {

                }));
            }
        }

        public VideoSeriesEntity(VideoSeries _series)
        {
            VideoSeries = _series;
            Visible = new Observable<Visibility>() { Value = Visibility.Visible };
            SearchEntities = _series.SearchEntities;
        }
    }

    class VideoSearchEntity : SearchEntity
    {
        public Brush FontColor { get; }
        public Brush Background { get; }

        public VideoSearchEntity(string _text) : base("Other", _text)
        {
            switch (Type)
            {
                case "Series":
                    FontColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#db3a34"));
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e15d58"));
                    break;
                case "Alt Series":
                    FontColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fca311"));
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fdc467"));
                    break;
                case "Tag":
                    FontColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#084c61"));
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#618d9a"));
                    break;
                default:
                    FontColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#323031"));
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7c7b7b"));
                    break;
            }
        }

        public VideoSearchEntity(SearchEntity _searchEntity) : base(_searchEntity.Type, _searchEntity.Text)
        {
            switch (Type)
            {
                case "Series":
                    FontColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#db3a34"));
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e15d58"));
                    break;
                case "Alt Series":
                    FontColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fca311"));
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fdc467"));
                    break;
                case "Tag":
                    FontColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#084c61"));
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#618d9a"));
                    break;
                default:
                    FontColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#323031"));
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7c7b7b"));
                    break;
            }
        }
    }
}
