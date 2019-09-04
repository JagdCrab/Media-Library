using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public List<string> Alt_SeriesAutoCompleteEntities { get; }

        public Observable<BitmapSource> Icon { get; }

        public Observable<bool> Favorite { get; }

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

        public Command SwitchFavorite { get; }

        public Command RefreshMetadata { get; }
        public Command RefreshFileInfo { get; }

        public Command EnableEditMode { get; }
        public Command SaveChanges { get; }
        public Command PlayNow { get; }
        public Command AddToPlaylist { get; }

        public VideoDetailsPageViewModel(VideoRecord _record)
        {
            #region DataAttributes
            Icon = new Observable<BitmapSource>() { Value = _record.Icon };
            Favorite = new Observable<bool>() { Value = _record.Favorite };

            #endregion

            #region OperationalAttributes

            EditMode = new Observable<bool> { Value = false };

            #endregion

            #region Commands

            SwitchFavorite = new Command(new Action(() => {
                if (EditMode.Value)
                    Favorite.Value = !Favorite.Value;
            }));


            EnableEditMode = new Command(new Action(() => {
                EditMode.Value = true;
            }));

            SaveChanges = new Command(new Action(() => {
                EditMode.Value = false;
            }));
            #endregion
        }
    }
}
