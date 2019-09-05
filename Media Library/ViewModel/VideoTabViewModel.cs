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
using Media_Library.Windows;

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
}
