using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ChristianMoser.WpfInspector.Services.Resources
{
    public sealed class StyleResourceItem : ResourceItem
    {
        private readonly object _resourceKey;
        private readonly Style _style;

        public StyleResourceItem(object resourceKey, ResourceDictionary dictionary, FrameworkElement source, ResourceScope scope)
            : base(resourceKey, dictionary, source, scope)
        {
            _resourceKey = resourceKey;
            _style = Value as Style;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                var type = _resourceKey as Type;
                if (type != null)
                {
                    return string.Format("{0} (Default)", type.Name);
                }
                return _resourceKey.ToString();
            }
        }

        /// <summary>
        /// Gets the category.
        /// </summary>
        public override string Category
        {
            get
            {
                return "Styles";
            }
        }
    }
}
