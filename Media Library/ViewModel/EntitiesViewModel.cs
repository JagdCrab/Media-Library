using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Media_Library.Components;
using Media_Library.Data;
using Media_Library.Windows;

namespace Media_Library.ViewModel
{
    #region VideoTabEntities

    class VideoSeriesEntity
    {
        public VideoSeries VideoSeries;
        
        public string Name { get { return VideoSeries.Series; } }
        public BitmapSource Icon { get { return VideoSeries.Icon; } }

        public SearchEntityCollection SearchEntities;

        public Observable<Visibility> Visible;
        public Command GetSeriesDetails
        {
            get
            {
                return new Command(new Action(() => {
                    var seriesDetailsWindow = new SeriesDetailsWindow(VideoSeries);
                    seriesDetailsWindow.Show();
                    seriesDetailsWindow.Activate();
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
    #endregion

    #region Details

    class ScoreEntity : INotifyPropertyChanged
    {
        public Observable<int> Score { get; }
        public Observable<SolidColorBrush> Background { get; }

        public Observable<bool> MenuOpened { get; }

        public List<ScoreEntity> PossibleStates
        {
            get
            {
                return new List<ScoreEntity>() {
                    new ScoreEntity(1, this),
                    new ScoreEntity(2, this),
                    new ScoreEntity(3, this),
                    new ScoreEntity(4, this),
                    new ScoreEntity(5, this),
                    new ScoreEntity(6, this),
                    new ScoreEntity(7, this),
                    new ScoreEntity(8, this),
                    new ScoreEntity(9, this),
                    new ScoreEntity(10, this)
                };
            }
        }

        public Command ChangeState { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public ScoreEntity()
        {
            Score = new Observable<int>() { Value = -1 };
            Background = new Observable<SolidColorBrush>() { Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C0C0C0")) };

            MenuOpened = new Observable<bool>() { Value = false };
        }

        public ScoreEntity(int _score)
        {
            Score = new Observable<int>() { Value = _score };
            Background = new Observable<SolidColorBrush>();

            MenuOpened = new Observable<bool>() { Value = false };

            switch (_score)
            {
                case -1:
                    Background.Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C0C0C0"));
                    break;
                case 1:
                case 2:
                    Background.Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F25F5C"));
                    break;
                case 3:
                case 4:
                    Background.Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FCBF49"));
                    break;
                case 5:
                case 6:
                    Background.Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#83B692"));
                    break;
                case 7:
                case 8:
                    Background.Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#247BA0"));
                    break;
                case 9:
                case 10:
                    Background.Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#582C83"));
                    break;
            }
        }

        public ScoreEntity(int _score, ScoreEntity _parent)
        {
            Score = new Observable<int>() { Value = _score };
            Background = new Observable<SolidColorBrush>();

            switch (_score)
            {
                case -1:
                    Background.Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C0C0C0"));
                    break;
                case 1:
                case 2:
                    Background.Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F25F5C"));
                    break;
                case 3:
                case 4:
                    Background.Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FCBF49"));
                    break;
                case 5:
                case 6:
                    Background.Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#83B692"));
                    break;
                case 7:
                case 8:
                    Background.Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#247BA0"));
                    break;
                case 9:
                case 10:
                    Background.Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#582C83"));
                    break;
            }

            ChangeState = new Command(new Action(() => {
                _parent.Score.Value = Score.Value;
                _parent.Background.Value = Background.Value;

                _parent.MenuOpened.Value = false;
                _parent.NotifyPropertyChanged("Intensity");
            }));
        }

        private void NotifyPropertyChanged(string _property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(_property));
        }
    }
    
    class DurationEntity
    {
        public Observable<string> Duration { get; }
        public Observable<SolidColorBrush> Background { get; }

        public List<DurationEntity> PossibleStates
        {
            get
            {
                return new List<DurationEntity>() {
                    new DurationEntity( new TimeSpan(0,10,0), this),
                    new DurationEntity( new TimeSpan(0,25,0), this),
                    new DurationEntity( new TimeSpan(1,0,0), this)
                };
            }
        }

        public Command ChangeState { get; }
        
        public DurationEntity(TimeSpan _duration)
        {
            if (_duration.TotalMinutes < 20)
            {
                Duration = new Observable<string>() { Value = "Short" };
                Background = new Observable<SolidColorBrush>() { Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FCBF49")) };
            }
            else if (_duration.TotalMinutes < 30)
            {
                Duration = new Observable<string>() { Value = "Medium" };
                Background = new Observable<SolidColorBrush>() { Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#83B692")) };
            }
            else
            {
                Duration = new Observable<string>() { Value = "Long" };
                Background = new Observable<SolidColorBrush>() { Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#247BA0")) };
            }
        }

        public DurationEntity(TimeSpan _duration, DurationEntity _parent)
        {
            if (_duration.TotalMinutes < 20)
            {
                Duration = new Observable<string>() { Value = "Short" };
                Background = new Observable<SolidColorBrush>() { Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FCBF49")) };
            }
            else if (_duration.TotalMinutes < 30)
            {
                Duration = new Observable<string>() { Value = "Medium" };
                Background = new Observable<SolidColorBrush>() { Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#83B692")) };
            }
            else
            {
                Duration = new Observable<string>() { Value = "Long" };
                Background = new Observable<SolidColorBrush>() { Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#247BA0")) };
            }

            ChangeState = new Command(new Action(() => {
                _parent.Duration.Value = Duration.Value;
                _parent.Background.Value = Background.Value;
            }));
        }
    }

    public class IntensityEntity : INotifyPropertyChanged
    {
        public Observable<string> Intensity { get; }
        public Observable<SolidColorBrush> Background { get; }

        public Observable<bool> MenuOpened { get; }
        
        public List<IntensityEntity> PossibleStates {
            get {
                return new List<IntensityEntity>() {
                    new IntensityEntity("Action", this),
                    new IntensityEntity("Mix", this),
                    new IntensityEntity("Story", this)
                };
            }
        }

        public Command ChangeState { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public IntensityEntity()
        {
            Intensity = new Observable<string>() { Value = "N/A" };
            Background = new Observable<SolidColorBrush>() { Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C0C0C0")) };

            MenuOpened = new Observable<bool>() { Value = false };
        }

        public IntensityEntity(string _intensity)
        {
            Intensity = new Observable<string>() { Value = _intensity };
            Background = new Observable<SolidColorBrush>();

            MenuOpened = new Observable<bool>() { Value = false };

            switch (_intensity)
            {
                case "Story":
                    Background.Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F25F5C"));
                    break;
                case "Mix":
                    Background.Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#83B692"));
                    break;
                case "Action":
                    Background.Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#582c83"));
                    break;
                default:
                    Background.Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C0C0C0"));
                    break;
            }
        }

        public IntensityEntity(string _intensity, IntensityEntity _parent)
        {
            Intensity = new Observable<string>() { Value = _intensity };
            Background = new Observable<SolidColorBrush>();

            switch (_intensity)
            {
                case "Story":
                    Background.Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F25F5C"));
                    break;
                case "Mix":
                    Background.Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#83B692"));
                    break;
                case "Action":
                    Background.Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#582c83"));
                    break;
                default:
                    Background.Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C0C0C0"));
                    break;
            }

            ChangeState = new Command(new Action(() => {
                _parent.Intensity.Value = Intensity.Value;
                _parent.Background.Value = Background.Value;
                _parent.MenuOpened.Value = false;

                _parent.NotifyPropertyChanged("Intensity");
            }));
        }

        private void NotifyPropertyChanged(string _property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(_property));
        }
    }
    #endregion
}
