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
    class ClipTabViewModel
    {
        public ListCollectionView ClipRecordEntitiesView { get; }

        public Observable<string> AutoCompleteText { get; }
        public Observable<ClipSearchEntity> SelectedSearchEntity { get; }
        public Collection<ClipSearchEntity> ClipSearchEntities { get; }

        public Observable<Visibility> ClearButtonVisibility { get; }
        public ObservableCollection<ClipSearchEntity> FilterEntities { get; }

        public Observable<string> SelectedOrdering { get; }
        public List<string> OrderingOptions { get { return new List<string>() { "Alphabetical", "Source" }; } }

        public Command OrderingChanged
        {
            get
            {
                return new Command(new Action(() => {
                    string selection = SelectedOrdering.Value;

                    if (SelectedOrdering.Value == "Source")
                    {
                        ClipRecordEntitiesView.SortDescriptions.Clear();
                        ClipRecordEntitiesView.SortDescriptions.Add(new SortDescription("Inserted", ListSortDirection.Ascending));
                    }
                    else
                    {
                        ClipRecordEntitiesView.SortDescriptions.Clear();
                        ClipRecordEntitiesView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
                    }
                }));
            }
        }

        public Command AddFilter
        {
            get
            {
                return new Command(new Action(() => {
                    if (SelectedSearchEntity.Value != null)
                        FilterEntities.Add(SelectedSearchEntity.Value);
                    else
                        FilterEntities.Add(new ClipSearchEntity(AutoCompleteText.Value));

                    SelectedSearchEntity.Value = null;
                    AutoCompleteText.Value = string.Empty;

                    if (ClearButtonVisibility.Value != Visibility.Visible)
                        ClearButtonVisibility.Value = Visibility.Visible;

                    ClipRecordEntitiesView.Filter = vse => {
                        if (FilterEntities.All(x => {
                            if (x.Type != "Other")
                                return ((ClipRecordEntity)vse).SearchEntities.Any(y => y.Text == x.Text);
                            else
                                return ((ClipRecordEntity)vse).SearchEntities.Any(y => y.Text.Contains(x.Text));
                        }))
                            return true;
                        else
                            return false;
                    };
                }));
            }
        }

        public Command ClearFilters
        {
            get
            {
                return new Command(new Action(() => {
                    FilterEntities.Clear();
                    ClearButtonVisibility.Value = Visibility.Collapsed;
                    ClipRecordEntitiesView.Filter = vse => { return true; };
                }));
            }
        }

        public ClipTabViewModel()
        {
            var collection = new List<ClipRecordEntity>();

            AutoCompleteText = new Observable<string>();
            SelectedSearchEntity = new Observable<ClipSearchEntity>();
            ClipSearchEntities = new Collection<ClipSearchEntity>();

            ClearButtonVisibility = new Observable<Visibility>() { Value = Visibility.Collapsed };
            FilterEntities = new ObservableCollection<ClipSearchEntity>();

            foreach (var searchEntity in ClipAccesser.GetSearchEntities())
                ClipSearchEntities.Add(new ClipSearchEntity(searchEntity));

            foreach (var clip in ClipAccesser.GetClipCollection())
                collection.Add(new ClipRecordEntity(clip));

            ClipRecordEntitiesView = new ListCollectionView(collection);

            SelectedOrdering = new Observable<string>() { Value = "Alphabetical" };
        }
    }
}
