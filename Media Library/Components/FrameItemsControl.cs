using System;
using System.Windows.Controls;

namespace Media_Library.Components
{
    public class FrameItemsControl : ItemsControl
    {
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return false;
        }
    }
}
