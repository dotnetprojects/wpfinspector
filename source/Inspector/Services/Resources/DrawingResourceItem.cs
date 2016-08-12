using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ChristianMoser.WpfInspector.Services.Resources
{
    public class DrawingResourceItem : ResourceItem
    {
        public DrawingResourceItem(object resourceKey, ResourceDictionary dictionary, FrameworkElement source, ResourceScope scope)
            :base( resourceKey, dictionary, source,scope)
        {
            
        }

        public override string Category
        {
            get
            {
                return "Drawings";
            }
        }
    }
}
