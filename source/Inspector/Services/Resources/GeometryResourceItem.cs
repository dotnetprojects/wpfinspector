using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ChristianMoser.WpfInspector.Services.Resources
{
    public class GeometryResourceItem : ResourceItem
    {
        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="GeometryResourceItem"/> class.
        /// </summary>
        public GeometryResourceItem(object resourceKey, ResourceDictionary dictionary, FrameworkElement source, ResourceScope scope)
            :base( resourceKey, dictionary,source, scope)
        {

        }

        #endregion

        /// <summary>
        /// Gets the category.
        /// </summary>
        public override string Category
        {
            get
            {
                return "Geometries";
            }
        }
    }
}
