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
    class IwaraViewModel
    {
        public ListCollectionView IwaraEntitiesView { get; }

        public Observable<string> AutoCompleteText { get; }
        public Observable<VideoSearchEntity> SelectedSearchEntity { get; }
        public Collection<VideoSearchEntity> VideoSearchEntities { get; }


    }
}
