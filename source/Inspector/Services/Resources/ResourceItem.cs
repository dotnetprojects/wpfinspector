using System.Windows;
using ChristianMoser.WpfInspector.Utilities;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System;

namespace ChristianMoser.WpfInspector.Services.Resources
{
    public enum ResourceScope
    {
        Local,
        Application,
        Theme,
        Style,
        Template
    }

    public class ResourceItem 
    {
        private readonly object _resourceKey;
        private readonly object _value;
        private readonly ResourceScope _resourceScope;
        private readonly ResourceDictionary _dictionary;
        
        public virtual string Name
        {
            get
            {
                if (_resourceKey is ComponentResourceKey)
                {
                    return ((ComponentResourceKey) _resourceKey).ResourceId.ToString();
                }
                return _resourceKey.ToString();
            }
        }

        public ResourceScope ResourceScope
        {
            get { return _resourceScope; }
        }

        public string Source
        {
            get
            {
                string source = string.Empty;
                if( _dictionary.Source != null)
                {
                    source = _dictionary.Source.ToString();
                }
                return source;
            }
        }

        public Type Type
        {
            get { return _value == null ? typeof(object) : _value.GetType(); }
        }

        /// <summary>
        /// Gets the category.
        /// </summary>
        public virtual string Category
        {
            get { return "Misc"; }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public virtual object Value
        {
            get
            {
                if (_value is ContextMenu || _value is Popup)
                {
                    return "ContextMenu";
                }
                return _value;
            }
        }

        public ResourceItem(object resourceKey, ResourceDictionary dictionary, FrameworkElement source, ResourceScope resourceScope)
        {
            _resourceKey = resourceKey;
            _resourceScope = resourceScope;
            _dictionary = dictionary;
            _value = dictionary[resourceKey];
            if( _value == null)
            {
                _value = source.TryFindResource(resourceKey);
            }
        }
    }
}
