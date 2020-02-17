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
using Microsoft.Win32;

using Media_Library.Data;
using Media_Library.Components;
using Media_Library.Windows;

namespace Media_Library.ViewModel
{
    class VideoDetailsPageViewModel
    {
        private string vid { get; set; }

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
            vid = _record.Vid;

            Icon.Value = _record.Icon;

            Playlist.Value = VideoAccesser.CheckIfInPlaylist(_record);
            Favorite.Value = _record.Favorite;

            ScoreUI = new ScoreEntity(_record.Score);
            IntensityUI = new IntensityEntity(_record.Intensity);
            DurationUI = new DurationEntity(_record.Duration);

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

            Icon.PropertyChanged += (object sender, PropertyChangedEventArgs e) => { VideoAccesser.UpdateIcon(vid, Icon.Value); };

            Playlist.PropertyChanged += (object sender, PropertyChangedEventArgs e) => { throw new NotImplementedException(); };
            Favorite.PropertyChanged += (object sender, PropertyChangedEventArgs e) => { VideoAccesser.UpdateFavorite(vid, Favorite.Value); };

            ScoreUI.PropertyChanged += (object sender, PropertyChangedEventArgs e) => { VideoAccesser.UpdateScore(vid, ScoreUI.Score.Value); };
            IntensityUI.PropertyChanged += (object sender, PropertyChangedEventArgs e) => { VideoAccesser.UpdateIntensity(vid, IntensityUI.Intensity.Value); };

            VideoTags.Entities.ListChanged += (object o, ListChangedEventArgs e) => {
                if (e.ListChangedType == ListChangedType.ItemChanged)
                {
                    var collection = (BindingList<TagEntityBase>)o;
                    var tag = (TagEntity)collection[e.NewIndex];

                    VideoAccesser.UpsertTag(vid, tag.Text, tag.Intensity, tag.Deleted);
                }
            };
            
            Alias.PropertyChanged += (object sender, PropertyChangedEventArgs e) => { VideoAccesser.UpdateAlias(vid, Alias.Value); };
            Series.PropertyChanged += (object sender, PropertyChangedEventArgs e) => { VideoAccesser.UpdateSeries(vid, Series.Value); };
            Alt_Alias.PropertyChanged += (object sender, PropertyChangedEventArgs e) => { VideoAccesser.UpdateAltAlias(vid, Alt_Alias.Value); };
            Alt_Series.PropertyChanged += (object sender, PropertyChangedEventArgs e) => { VideoAccesser.UpdateAltSeries(vid, Alt_Series.Value); };

            Screenlist.PropertyChanged += (object sender, PropertyChangedEventArgs e) => { VideoAccesser.UpsertScreenlist(vid, Screenlist.Value); };
        }
        #endregion

        #region New Record Constructor
        public VideoDetailsPageViewModel(string _path) : this()
        {
            vid = Guid.NewGuid().ToString();
            VideoAccesser.CreateNewRecord(vid);

            EditMode.Value = true;

            var fileInfo = new FileInfo(_path);
            var mediaInfo = MediaAccessor.GetMetaData(_path);

            VideoAccesser.UpdateScore(vid, -1);
            VideoAccesser.UpdateFavorite(vid, false);
            VideoAccesser.UpdateIntensity(vid, "N/A");
            
            VideoAccesser.UpdateAlias(vid, fileInfo.Name);
            Alias.Value = fileInfo.Name;

            VideoAccesser.UpdateSeries(vid, fileInfo.DirectoryName);
            Series.Value = fileInfo.DirectoryName;

            VideoAccesser.UpdateDuration(vid, mediaInfo.Metadata.Duration);
            Duration.Value = mediaInfo.Metadata.Duration.ToString(@"mm\:ss");
            Bitrate.Value = Math.Round(fileInfo.Length / mediaInfo.Metadata.Duration.TotalSeconds).ToString() + "bps";

            VideoAccesser.UpdateResolution(vid, mediaInfo.Metadata.VideoData.FrameSize);
            Frame.Value = mediaInfo.Metadata.VideoData.FrameSize;

            VideoAccesser.UpdateFormat(vid, mediaInfo.Metadata.VideoData.Format);
            Format.Value = mediaInfo.Metadata.VideoData.Format;

            VideoAccesser.UpdateFilePath(vid, fileInfo.FullName);
            FilePath.Value = fileInfo.FullName;

            VideoAccesser.UpdateFileName(vid, fileInfo.Name);
            FileName.Value = fileInfo.Name;

            VideoAccesser.UpdateFileExtention(vid, fileInfo.Extension);
            Extention.Value = fileInfo.Extension;

            VideoAccesser.UpdateFileSize(vid, fileInfo.Length);
            FileSize.Value = Math.Round(fileInfo.Length / 1048576d, 1).ToString() + "MB";
            
            ScoreUI = new ScoreEntity();
            IntensityUI = new IntensityEntity();
            DurationUI = new DurationEntity(mediaInfo.Metadata.Duration);
            
            LoadingIndicatorVisibility.Value = Visibility.Visible;

            Task.Factory.StartNew(new Action(() => {
                BitmapSource result = MediaAccessor.CreateGridScreenlist(mediaInfo, LoadingIndicatorCurrent, LoadingIndicatorMax);

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => { Screenlist.Value = result; }));
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => { LoadingIndicatorVisibility.Value = Visibility.Hidden; }));
                
                var crc64 = new Crc64Iso();
                string hash = string.Empty;

                using (var fs = File.Open(fileInfo.FullName, FileMode.Open))
                    foreach (var b in crc64.ComputeHash(fs))
                        hash += b.ToString("x2").ToLower();

                VideoAccesser.UpdateChecksum(vid, hash);
            }));

            Icon.PropertyChanged += (object sender, PropertyChangedEventArgs e) => { VideoAccesser.UpdateIcon(vid, Icon.Value); };

            Playlist.PropertyChanged += (object sender, PropertyChangedEventArgs e) => { throw new NotImplementedException(); };
            Favorite.PropertyChanged += (object sender, PropertyChangedEventArgs e) => { VideoAccesser.UpdateFavorite(vid, Favorite.Value); };

            ScoreUI.PropertyChanged += (object sender, PropertyChangedEventArgs e) => { VideoAccesser.UpdateScore(vid, ScoreUI.Score.Value); };
            IntensityUI.PropertyChanged += (object sender, PropertyChangedEventArgs e) => { VideoAccesser.UpdateIntensity(vid, IntensityUI.Intensity.Value); };

            VideoTags.Entities.ListChanged += (object o, ListChangedEventArgs e) => {
                if (e.ListChangedType == ListChangedType.ItemChanged)
                {
                    var collection = (BindingList<TagEntityBase>)o;
                    var tag = (TagEntity)collection[e.NewIndex];

                    VideoAccesser.UpsertTag(vid, tag.Text, tag.Intensity, tag.Deleted);
                }
            };

            Alias.PropertyChanged += (object sender, PropertyChangedEventArgs e) => { VideoAccesser.UpdateAlias(vid, Alias.Value); };
            Series.PropertyChanged += (object sender, PropertyChangedEventArgs e) => { VideoAccesser.UpdateSeries(vid, Series.Value); };
            Alt_Alias.PropertyChanged += (object sender, PropertyChangedEventArgs e) => { VideoAccesser.UpdateAltAlias(vid, Alt_Alias.Value); };
            Alt_Series.PropertyChanged += (object sender, PropertyChangedEventArgs e) => { VideoAccesser.UpdateAltSeries(vid, Alt_Series.Value); };

            Screenlist.PropertyChanged += (object sender, PropertyChangedEventArgs e) => { VideoAccesser.UpsertScreenlist(vid, Screenlist.Value); };
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
                    VideoAccesser.UpdateFavorite(vid, Favorite.Value);
                }
            }));


            RefreshMetadata = new Command(new Action(() => {
                var fileInfo = new FileInfo(FilePath.Value);
                var mediaInfo = MediaAccessor.GetMetaData(FilePath.Value);

                VideoAccesser.UpdateDuration(vid, mediaInfo.Metadata.Duration);
                Duration.Value = mediaInfo.Metadata.Duration.ToString(@"mm\:ss");
                Bitrate.Value = Math.Round(fileInfo.Length / mediaInfo.Metadata.Duration.TotalSeconds).ToString() + "bps";

                VideoAccesser.UpdateResolution(vid, mediaInfo.Metadata.VideoData.FrameSize);
                Frame.Value = mediaInfo.Metadata.VideoData.FrameSize;

                VideoAccesser.UpdateFormat(vid, mediaInfo.Metadata.VideoData.Format);
                Format.Value = mediaInfo.Metadata.VideoData.Format;
            }));

            RefreshFileInfo = new Command(new Action(() => {
                OpenFileDialog dialog = new OpenFileDialog();

                if(dialog.ShowDialog() == true)
                {
                    var fileInfo = new FileInfo(dialog.FileName);

                    VideoAccesser.UpdateFilePath(vid, fileInfo.FullName);
                    FilePath.Value = fileInfo.FullName;

                    VideoAccesser.UpdateFileName(vid, fileInfo.Name);
                    FileName.Value = fileInfo.Name;

                    VideoAccesser.UpdateFileExtention(vid, fileInfo.Extension);
                    Extention.Value = fileInfo.Extension;

                    VideoAccesser.UpdateFileSize(vid, fileInfo.Length);
                    FileSize.Value = Math.Round(fileInfo.Length / 1048576d, 1).ToString() + "MB";

                }
            }));

            RefreshScreenlist = new Command(new Action(() => {
                LoadingIndicatorVisibility.Value = Visibility.Visible;

                Task.Factory.StartNew(new Action(() => {
                    var mediaInfo = MediaAccessor.GetMetaData(FilePath.Value);
                    BitmapSource result = MediaAccessor.CreateGridScreenlist(mediaInfo, LoadingIndicatorCurrent, LoadingIndicatorMax);

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
                EditMode.Value = true;
            }));

            SaveChanges = new Command(new Action(() => {
                EditMode.Value = false;
                Accesser.Instance.Commit();
            }));

            RevertChanges = new Command(new Action(() => {
                EditMode.Value = false;
                Accesser.Instance.Rollback();
            }));
            #endregion

        }
        #endregion
    }
}
