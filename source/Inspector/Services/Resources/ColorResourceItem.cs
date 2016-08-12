using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace ChristianMoser.WpfInspector.Services.Resources
{
    public class ColorResourceItem : ResourceItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColorResourceItem"/> class.
        /// </summary>
        public ColorResourceItem(object resourceKey, ResourceDictionary dictionary, FrameworkElement source, ResourceScope scope)
            :base( resourceKey, dictionary,source, scope)
        {
            
        }

        public override object Value
        {
            get
            {
                return new SolidColorBrush((Color) base.Value);
            }
        }

        /// <summary>
        /// Gets the category.
        /// </summary>
        /// <value>The category.</value>
        public override string Category
        {
            get
            {
                return "Colors";
            }
        }
    }
}
