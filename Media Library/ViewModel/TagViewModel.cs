using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Media_Library.Components;
using Media_Library.Data;

namespace Media_Library.ViewModel
{
    class TagPresenter
    {
        public ObservableCollection<object> Entities { get; }
        public TagTemplateSelector TemplateSelector { get; }

        public class TagTemplateSelector : DataTemplateSelector
        {
            public override DataTemplate SelectTemplate(object item, DependencyObject container)
            {
                var element = (FrameworkElement)container;

                if (element != null && item != null)
                {
                    if (item is AddButtonEntity)
                        return (DataTemplate)element.FindResource("AddButtonTemplate");
                    else if (item is NewTagEntity)
                        return (DataTemplate)element.FindResource("NewTagTemplate");
                    else
                        return (DataTemplate)element.FindResource("TagTemplate");
                }
                else
                    throw new Exception();
            }
        }

        public TagPresenter()
        {
            Entities = new ObservableCollection<object>();
            TemplateSelector = new TagTemplateSelector();

            Entities.Add(new AddButtonEntity(Entities));
        }
    }

    public class TagEntity
    {
        public string Text { get; }
        public Intensity Intensity { get; set; }

        public Observable<SolidColorBrush> Background { get; }

        public List<IntensityState> PossibleIntensityStates { get; }

        public Command RemoveTag { get; }

        public TagEntity(string _text, ObservableCollection<object> _parent)
        {
            Text = _text;
            Intensity = Intensity.Neutral;

            var bytes = BitConverter.GetBytes((int)Intensity);
            Background.Value = new SolidColorBrush(Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]));

            RemoveTag = new Command(new Action(() => {
                _parent.Remove(this);
            }));

            PossibleIntensityStates = new List<IntensityState>() {
                new IntensityState(Intensity.Lowest, this),
                new IntensityState(Intensity.Low, this),
                new IntensityState(Intensity.Neutral, this),
                new IntensityState(Intensity.High, this),
                new IntensityState(Intensity.Highest, this)
            };
        }

        public TagEntity(VideoTag _tag, ObservableCollection<object> _parent)
        {
            Text = _tag.Text;
            Intensity = _tag.Intensity;

            var bytes = BitConverter.GetBytes((int)Intensity);
            Background = new Observable<SolidColorBrush>() { Value = new SolidColorBrush(Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0])) };

            RemoveTag = new Command(new Action(() => {
                _parent.Remove(this);
            }));

            PossibleIntensityStates = new List<IntensityState>() {
                new IntensityState(Intensity.Lowest, this),
                new IntensityState(Intensity.Low, this),
                new IntensityState(Intensity.Neutral, this),
                new IntensityState(Intensity.High, this),
                new IntensityState(Intensity.Highest, this)
            };

        }
    }

    public class NewTagEntity
    {
        public Observable<string> Text { get; }
        public List<string> AutoCompleteCollection { get; }

        public Command Submit { get; }
        public Command Cancel { get; }

        public NewTagEntity(ObservableCollection<object> _collection)
        {
            Text = new Observable<string>();
            AutoCompleteCollection = VideoAccesser.GetVideoTagsAutoComplete();

            Submit = new Command(new Action(() => {
                _collection.Remove(this);
                _collection.Add(new TagEntity(Text.Value, _collection));
            }));

            Cancel = new Command(new Action(() => {
                _collection.Remove(this);
            }));
        }
    }

    public class AddButtonEntity
    {
        public ObservableCollection<object> Collection { get; }

        public Command AddNewTag { get; }

        public AddButtonEntity(ObservableCollection<object> _collection)
        {
            Collection = _collection;
            AddNewTag = new Command(new Action(() => {
                Collection.Insert(Collection.Count - 2, new NewTagEntity(_collection));
            }));
        }
    }


    public class IntensityState
    {
        public Intensity Intensity { get; }

        public string Description { get; }
        public SolidColorBrush Background { get; }

        public Command ChangeState;

        public IntensityState(Intensity _intensity, TagEntity _parent)
        {
            Intensity = _intensity;
            Description = _intensity.ToString();

            var bytes = BitConverter.GetBytes((int)_intensity);
            Background = new SolidColorBrush(Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]));

            ChangeState = new Command(new Action(() => {
                _parent.Background.Value = Background;
                _parent.Intensity = Intensity;
            }));
        }
    }
}
