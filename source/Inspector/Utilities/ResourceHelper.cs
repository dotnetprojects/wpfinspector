using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Collections;

namespace ChristianMoser.WpfInspector.Utilities
{
    public static class ResourceHelper
    {
        public static IEnumerable<ResourceEntry<T>> GetResourcesRecursivelyIncludingApp<T>(FrameworkElement frameworkElement)
        {
            var resources = GetResourcesRecursively<T>(frameworkElement);
            if (Application.Current != null)
            {
                resources = resources.Union(GetResourcesRecursively<T>(Application.Current.Resources));
            }
            return resources.Distinct();
        }

        public static IEnumerable<ResourceEntry<T>> GetResourcesRecursively<T>(FrameworkElement frameworkElement)
        {
            var resourceEntries = GetResourcesRecursively<T>(frameworkElement.Resources);

            var parent = frameworkElement.Parent as FrameworkElement;
            if( parent != null )
            {
                resourceEntries = resourceEntries.Union(GetResourcesRecursivelyIncludingApp<T>(parent));
            }

            return resourceEntries;
        }

        public static IEnumerable<ResourceEntry<T>> GetResourcesRecursively<T>(ResourceDictionary resourceDictionary)
        {
            if( resourceDictionary == null)
            {
                return Enumerable.Empty<ResourceEntry<T>>();
            }
            
            var resourceEntries = resourceDictionary.Cast<DictionaryEntry>().Where( d => d.Value is T)
                .Select( r => new ResourceEntry<T>{ Key = r.Key, Value= (T)r.Value , Owner =  resourceDictionary});

            foreach (var mergedDictionary in resourceDictionary.MergedDictionaries)
            {
                resourceEntries = resourceEntries.Union(GetResourcesRecursively<T>(mergedDictionary));
            }

            return resourceEntries;
        }
    }

    public class ResourceEntry<T>
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public object Key { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// Gets or sets the owner.
        /// </summary>
        public ResourceDictionary Owner { get; set; }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var resourceEntry = obj as ResourceEntry<T>;

            if( ReferenceEquals(this,resourceEntry))
                return true;

            if (Equals(resourceEntry, null))
                return false;

            return resourceEntry.Key == Key;
        }
    }
}
