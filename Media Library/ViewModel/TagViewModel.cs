using System;
using System.ComponentModel;
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
        public BindingList<object> Entities { get; }
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
            Entities = new BindingList<object>();
            TemplateSelector = new TagTemplateSelector();
            
            Entities.Add(new AddButtonEntity(Entities));
        }
    }

    public class TagEntity : INotifyPropertyChanged
    {
        public long? Id { get; }
        public string Text { get; set; }
        public Intensity Intensity { get; set; }

        public Observable<SolidColorBrush> Background { get; }
        public Observable<bool> MenuOpened { get; }

        public List<IntensityState> PossibleIntensityStates { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public Command RemoveTag { get; }

        public TagEntity(string _text, BindingList<object> _parent)
        {
            Text = _text;
            Intensity = Intensity.Neutral;

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
            NotifyPropertyChanged("Text");
        }

        public TagEntity(VideoTag _tag, BindingList<object> _parent)
        {
            Id = _tag.Id;
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

        public void NotifyPropertyChanged(string _property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(_property));
        }

    }

    public class NewTagEntity
    {
        public Observable<string> Text { get; }
        public List<string> AutoCompleteCollection { get; }
        
        public Observable<bool> IsFocused { get; set; }

        public Command TakeFocusByForce { get; }

        public Command Submit { get; }
        public Command Cancel { get; }

        public NewTagEntity(BindingList<object> _collection)
        {
            Text = new Observable<string>();
            AutoCompleteCollection = VideoAccesser.GetVideoTagsAutoComplete();

            IsFocused = new Observable<bool>();

            TakeFocusByForce = new Command(new Action(() => { IsFocused.Value = true; }));

            Submit = new Command(new Action(() => {
                _collection.Remove(this);
                _collection.Insert(_collection.Count - 1, new TagEntity(Text.Value, _collection));
            }));

            Cancel = new Command(new Action(() => {
                _collection.Remove(this);
            }));
        }
    }

    public class AddButtonEntity
    {
        public BindingList<object> Collection { get; }

        public Command AddNewTag { get; }

        public AddButtonEntity(BindingList<object> _collection)
        {
            Collection = _collection;
            AddNewTag = new Command(new Action(() => {
                Collection.Insert(Collection.Count - 1, new NewTagEntity(_collection));
            }));
        }
    }


    public class IntensityState
    {
        public Intensity Intensity { get; }

        public string Description { get; }
        public SolidColorBrush Background { get; }

        public Command ChangeState { get; }

        public IntensityState(Intensity _intensity, TagEntity _parent)
        {
            Intensity = _intensity;
            Description = _intensity.ToString();

            var bytes = BitConverter.GetBytes((int)_intensity);
            Background = new SolidColorBrush(Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]));

            ChangeState = new Command(new Action(() => {
                _parent.Background.Value = Background;
                _parent.Intensity = Intensity;
                _parent.NotifyPropertyChanged("Intensity");
            }));
        }
    }
}
