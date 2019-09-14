using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Data.SQLite;

using Media_Library.Data;
using Media_Library.Components;
using Media_Library.Windows;

namespace Media_Library.ViewModel
{
    class VideoDetailsPageViewModel
    {
        private long vid { get; set; }
        private SQLiteTransaction transaction { get; set; }

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

        public Command ShowScreenlist { get; }

        public Command EnableEditMode { get; }
        public Command SaveChanges { get; }
        public Command RevertChanges { get; }
        public Command PlayNow { get; }
        public Command AddToPlaylist { get; }

        #region Open Record Constructor

        public VideoDetailsPageViewModel(VideoRecord _record) : this()
        {
            vid = Convert.ToInt64(_record.Vid);

            Icon.Value = _record.Icon ;

            Playlist.Value = VideoAccesser.CheckIfInPlaylist(_record);
            Favorite.Value = _record.Favorite;

            ScoreUI = new ScoreEntity(_record.Score);
            IntensityUI = new IntensityEntity(_record.Intensity);
            DurationUI = new DurationEntity(_record.Duration);
            
            VideoTags = new TagPresenter();

            foreach (var tag in _record.Tags)
                VideoTags.Entities.Insert(VideoTags.Entities.Count - 1, new TagEntity(tag, VideoTags.Entities));

            Alias.Value = _record.Alias;
            Series.Value = _record.Series;
            Alt_Alias.Value = _record.Alt_Alias;
            Alt_Series.Value = _record.Alt_Series;

            Duration.Value = _record.Duration.ToString(@"mm\:ss");
            Bitrate.Value = Math.Round(_record.File_Size / _record.Duration.TotalSeconds).ToString() + "bps";
            Frame.Value = _record.Resolution;
            Format.Value = _record.Format;

            FilePath.Value = _record.File_Path;
            FileName.Value = _record.File_Name;
            Extention.Value = _record.File_Extention;
            FileSize.Value = Math.Round(_record.File_Size / 1048576d, 1).ToString() + "MB";

            Screenlist.Value = _record.Screenlist.Screenlist;
            
            IntensityUI.PropertyChanged += (object sender, PropertyChangedEventArgs e) => { VideoAccesser.UpdateIntensity(transaction, vid, IntensityUI.Intensity.Value); };
            ScoreUI.PropertyChanged += (object sender, PropertyChangedEventArgs e) => { VideoAccesser.UpdateScore(transaction, vid, ScoreUI.Score.Value); };

            Alias.PropertyChanged += (object sender, PropertyChangedEventArgs e) => { VideoAccesser.UpdateAlias(transaction, vid, Alias.Value); };
            Series.PropertyChanged += (object sender, PropertyChangedEventArgs e) => { VideoAccesser.UpdateSeries(transaction, vid, Series.Value); };
            Alt_Alias.PropertyChanged += (object sender, PropertyChangedEventArgs e) => { VideoAccesser.UpdateAltAlias(transaction, vid, Alt_Alias.Value); };
            Alt_Series.PropertyChanged += (object sender, PropertyChangedEventArgs e) => { VideoAccesser.UpdateAltSeries(transaction, vid, Alt_Series.Value); };
        }
        #endregion

        #region New Record Constructor
        public VideoDetailsPageViewModel(string _path) : this()
        {
            transaction = VideoAccesser.CreateTransaction();
            vid = VideoAccesser.CreateNewRecord(transaction);

            EditMode.Value = true;

            var fileInfo = new FileInfo(_path);
            var mediaInfo = MediaAccessor.GetMetaData(_path);
            
            VideoAccesser.UpdateDuration(transaction, vid, mediaInfo.Metadata.Duration);
            Duration.Value = mediaInfo.Metadata.Duration.ToString(@"mm\:ss");
            Bitrate.Value = Math.Round(fileInfo.Length / mediaInfo.Metadata.Duration.TotalSeconds).ToString() + "bps";

            VideoAccesser.UpdateResolution(transaction, vid, mediaInfo.Metadata.VideoData.FrameSize);
            Frame.Value = mediaInfo.Metadata.VideoData.FrameSize;

            VideoAccesser.UpdateFormat(transaction, vid, mediaInfo.Metadata.VideoData.Format);
            Format.Value = mediaInfo.Metadata.VideoData.Format;

            VideoAccesser.UpdateFilePath(transaction, vid, fileInfo.FullName);
            FilePath.Value = fileInfo.FullName;

            VideoAccesser.UpdateFileName(transaction, vid, fileInfo.Name);
            FileName.Value = fileInfo.Name;

            VideoAccesser.UpdateFileExtention(transaction, vid, fileInfo.Extension);
            Extention.Value = fileInfo.Extension;

            VideoAccesser.UpdateFileSize(transaction, vid, fileInfo.Length);
            FileSize.Value = Math.Round(fileInfo.Length / 1048576d, 1).ToString() + "MB";
            
            ScoreUI = new ScoreEntity();
            IntensityUI = new IntensityEntity();
            DurationUI = new DurationEntity(mediaInfo.Metadata.Duration);
        }
        #endregion

        #region Common Constructor
        private VideoDetailsPageViewModel()
        {
            #region DataAttributes

            EditMode = new Observable<bool>() { Value = false };
            AliasError = new Observable<Visibility>() { Value = Visibility.Hidden };
            SeriesError = new Observable<Visibility>() { Value = Visibility.Hidden };

            SeriesAutoCompleteEntities = VideoAccesser.GetVideoSeriesAutoComplete();
            AltSeriesAutoCompleteEntities = VideoAccesser.GetVideoAltSeriesAutoComplete();

            Icon = new Observable<BitmapSource>();

            Playlist = new Observable<bool>();
            Favorite = new Observable<bool>();
            
            VideoTags = new TagPresenter();
            
            Alias = new Observable<string>();
            Series = new Observable<string>();
            Alt_Alias = new Observable<string>();
            Alt_Series = new Observable<string>();

            Duration = new Observable<string>();
            Bitrate = new Observable<string>();
            Frame = new Observable<string>();
            Format = new Observable<string>();

            FilePath = new Observable<string>();
            FileName = new Observable<string>();
            Extention = new Observable<string>();
            FileSize = new Observable<string>();

            Screenlist = new Observable<BitmapSource>();
            
            LoadingIndicatorVisibility = new Observable<Visibility>() { Value = Visibility.Hidden };
            LoadingIndicatorMax = new Observable<int>();
            LoadingIndicatorCurrent = new Observable<int>();
            #endregion

            #region Commands

            SwitchPlaylist = new Command(new Action(() => {
                Playlist.Value = !Playlist.Value;
            }));

            SwitchFavorite = new Command(new Action(() => {
                if (EditMode.Value)
                {
                    Favorite.Value = !Favorite.Value;
                    VideoAccesser.UpdateFavorite(transaction, vid, Favorite.Value);
                }
            }));


            RefreshMetadata = new Command(new Action(() => {
                throw new NotImplementedException();
            }));

            RefreshFileInfo = new Command(new Action(() => {
                throw new NotImplementedException();
            }));

            RefreshScreenlist = new Command(new Action(() => {
                LoadingIndicatorVisibility.Value = Visibility.Visible;

                Task.Factory.StartNew(new Action(() => {
                    BitmapSource result = MediaAccessor.CreateGridScreenlist(FilePath.Value, LoadingIndicatorCurrent, LoadingIndicatorMax);

                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => { Screenlist.Value = result; }));
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => { LoadingIndicatorVisibility.Value = Visibility.Hidden; }));
                }));
            }));

            ShowScreenlist = new Command(new Action(() => {
                var viewer = new ScreenshotWindow(Screenlist.Value);
                viewer.Show();
                viewer.Activate();
            }));

            EnableEditMode = new Command(new Action(() => {
                transaction = VideoAccesser.CreateTransaction();
                EditMode.Value = true;
            }));

            SaveChanges = new Command(new Action(() => {
                EditMode.Value = false;
                transaction.Rollback();
                transaction.Dispose();
            }));

            RevertChanges = new Command(new Action(() => {
                EditMode.Value = false;
                transaction.Rollback();
                transaction.Dispose();
            }));
            #endregion

        }
        #endregion
    }
}
