using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using System.Windows;

using Media_Library.Data;
using Media_Library.Components;

namespace Media_Library.ViewModel
{
    class VideoDetailsPageViewModel
    {
        public Observable<bool> EditMode { get; }
        public Observable<Visibility> AliasError { get; }
        public Observable<Visibility> SeriesError { get; }

        public List<string> SeriesAutoCompleteEntities { get; }
        public List<string> AltSeriesAutoCompleteEntities { get; }

        public Observable<BitmapSource> Icon { get; }

        public Observable<bool> Playlist { get; }
        public Observable<bool> Favorite { get; }

        public ScoreEntity ScoreUI { get; }
        public IntensityEntity IntensityUI { get; }
        public DurationEntity DurationUI { get; }

        public TagPresenter VideoTags { get; }

        public Observable<string> Alias { get; }
        public Observable<string> Series { get; }
        public Observable<string> Alt_Alias { get; }
        public Observable<string> Alt_Series { get; }

        public Observable<string> Duration { get; }
        public Observable<string> Bitrate { get; }
        public Observable<string> Frame { get; }
        public Observable<string> Format { get; }

        public Observable<string> FilePath { get; }
        public Observable<string> FileName { get; }
        public Observable<string> Extention { get; }
        public Observable<string> FileSize { get; }

        public Observable<BitmapSource> Screenlist { get; }

        public Observable<Visibility> LoadingIndicatorVisibility { get; }
        public Observable<int> LoadingIndicatorMax { get; }
        public Observable<int> LoadingIndicatorCurrent { get; }

        public Command SwitchPlaylist { get; }
        public Command SwitchFavorite { get; }

        public Command RefreshMetadata { get; }
        public Command RefreshFileInfo { get; }
        public Command RefreshScreenlist { get; }

        public Command EnableEditMode { get; }
        public Command SaveChanges { get; }
        public Command RevertChanges { get; }
        public Command PlayNow { get; }
        public Command AddToPlaylist { get; }

        public VideoDetailsPageViewModel(VideoRecord _record)
        {
            #region DataAttributes

            SeriesAutoCompleteEntities = VideoAccesser.GetVideoSeriesAutoComplete();
            AltSeriesAutoCompleteEntities = VideoAccesser.GetVideoAltSeriesAutoComplete();

            Icon = new Observable<BitmapSource>() { Value = _record.Icon };

            Playlist = new Observable<bool>() { Value = VideoAccesser.CheckIfInPlaylist(_record) };
            Favorite = new Observable<bool>() { Value = _record.Favorite };

            ScoreUI = new ScoreEntity(_record.Score);
            IntensityUI = new IntensityEntity(_record.Intensity);
            DurationUI = new DurationEntity(_record.Duration);

            VideoTags = new TagPresenter();

            foreach (var tag in _record.Tags)
                VideoTags.Entities.Insert(VideoTags.Entities.Count - 1, new TagEntity(tag, VideoTags.Entities));

            Alias = new Observable<string>() { Value = _record.Alias };
            Series = new Observable<string>() { Value = _record.Series };
            Alt_Alias = new Observable<string>() { Value = _record.Alt_Alias };
            Alt_Series = new Observable<string>() { Value = _record.Alt_Series };

            Duration = new Observable<string>() { Value = _record.Duration.ToString(@"mm\:ss") };
            Bitrate = new Observable<string>() { Value = Math.Round(_record.File_Size / (double)_record.Duration.TotalSeconds).ToString() + "bps" };
            Frame = new Observable<string>() { Value = _record.Resolution };
            Format = new Observable<string>() { Value = _record.Format };

            FilePath = new Observable<string>() { Value = _record.File_Path };
            FileName = new Observable<string>() { Value = _record.File_Name };
            Extention = new Observable<string>() { Value = _record.File_Extention };
            FileSize = new Observable<string>() { Value = Math.Round(_record.File_Size / 1048576d, 1).ToString() + "MB" };

            Screenlist = new Observable<BitmapSource>() { Value = _record.Screenlist.Screenlist };

            #endregion

            #region OperationalAttributes

            EditMode = new Observable<bool>() { Value = false };
            LoadingIndicatorCurrent = new Observable<int>();
            LoadingIndicatorMax = new Observable<int>();
            LoadingIndicatorVisibility = new Observable<Visibility>() { Value = Visibility.Hidden };

            #endregion

            #region Commands

            SwitchPlaylist = new Command(new Action(() => {
                Playlist.Value = !Playlist.Value;
            }));

            SwitchFavorite = new Command(new Action(() => {
                if (EditMode.Value)
                    Favorite.Value = !Favorite.Value;
            }));

            RefreshScreenlist = new Command(new Action(() => {
                LoadingIndicatorVisibility.Value = Visibility.Visible;

                Task.Factory.StartNew(new Action(() => {
                    BitmapSource result = MediaAccessor.CreateGridScreenlist(FilePath.Value, LoadingIndicatorCurrent, LoadingIndicatorMax);

                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => { Screenlist.Value = result; }));
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => { LoadingIndicatorVisibility.Value = Visibility.Hidden; }));
                }));
            }));

            EnableEditMode = new Command(new Action(() => {
                EditMode.Value = true;
            }));

            SaveChanges = new Command(new Action(() => {
                EditMode.Value = false;
            }));

            RevertChanges = new Command(new Action(() => {
                EditMode.Value = false;
            }));
            #endregion
        }
    }
}
