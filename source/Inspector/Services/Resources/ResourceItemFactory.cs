using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace ChristianMoser.WpfInspector.Services.Resources
{
    public static class ResourceItemFactory
    {
        /// <summary>
        /// Creates a best matching resource item for the provided resource
        /// </summary>
        public static ResourceItem CreateResourceItem(object resourceKey, ResourceDictionary dictionary, FrameworkElement source, ResourceScope scope)
        {
            var resource = dictionary[resourceKey] ?? source.TryFindResource(resourceKey);

            if (resource is Style) return new StyleResourceItem(resourceKey, dictionary, source, scope);
            if (resource is Brush) return new BrushResourceItem(resourceKey, dictionary, source, scope);
            if (resource is Drawing) return new DrawingResourceItem(resourceKey, dictionary, source, scope);
            if (resource is Color) return new ColorResourceItem(resourceKey, dictionary, source, scope);
            if (resource is Geometry) return new GeometryResourceItem(resourceKey, dictionary, source, scope);
            
            return new ResourceItem(resourceKey, dictionary, source, scope);
        }

        public static IList<ResourceItem> GetResourcesForElement(FrameworkElement source)
        {
            var resourceItems = new List<ResourceItem>();
            var resourceKeyList = new List<object>();

            var app = Application.Current;
            if (app != null)
            {
                ReadResourcesRecursive(app.Resources, ResourceScope.Application,source, resourceItems, resourceKeyList);
            }

            ReadResourcesRecursive(source.Resources, ResourceScope.Local, source, resourceItems, resourceKeyList);

            return resourceItems;
        }

        private static void ReadResourcesRecursive(ResourceDictionary dictionary, ResourceScope scope, FrameworkElement source, List<ResourceItem> resourceItems, List<object> resourceKeyList)
        {
            foreach (var resourceKey in dictionary.Keys)
            {
                if (!resourceKeyList.Contains(resourceKey))
                {
                    resourceKeyList.Add(resourceKey);
                    resourceItems.Add(CreateResourceItem(resourceKey, dictionary, source,scope));
                }
            }

            foreach (ResourceDictionary resourceDictionary in dictionary.MergedDictionaries)
            {
                ReadResourcesRecursive(resourceDictionary, scope, source, resourceItems, resourceKeyList);
            }
        }
    }
}
