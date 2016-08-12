using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using ChristianMoser.WpfInspector.Utilities;

namespace ChristianMoser.WpfInspector.Services.StyleExplorer
{
    public static class StyleHelper
    {
        public static bool TryGetStyleItem(FrameworkElement owner, Style style, out StyleItem styleItem)
        {
            styleItem = null;
            int globalIndex = GetGlobalIndex(style);

            foreach (var resource in ResourceHelper.GetResourcesRecursively<Style>(owner))
            {
                var resourceStyle = resource.Value as Style;
                if (resourceStyle != null && globalIndex == GetGlobalIndex(resourceStyle))
                {
                    var fullLocation = GetSource(resource.Owner);
                    string location = fullLocation.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
                    styleItem = new StyleItem(style, owner, GetKeyString(resource.Key), location, StyleScope.Local) { FullLocation = fullLocation };
                    return true;
                }
            }

            foreach (var resource in ResourceHelper.GetResourcesRecursively<Style>(Application.Current.Resources))
            {
                var resourceStyle = resource.Value as Style;
                if (resourceStyle != null && globalIndex == GetGlobalIndex(resourceStyle))
                {
                    // explicit resource key
                    string fullLocation = GetSource(resource.Owner) ?? "App.xml";
                    string location = fullLocation.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();

                    styleItem = new StyleItem(style, owner, GetKeyString(resource.Key), location, StyleScope.Application) { FullLocation = fullLocation };
                    return true;
                }
            }

            return false;
        }

        public static string GetSource(ResourceDictionary resourceDictionary)
        {
            if (resourceDictionary.Source != null)
            {
                return resourceDictionary.Source.ToString();
            }

            var baseUriInfo = typeof(ResourceDictionary).GetField("_baseUri",
                                                                 BindingFlags.Instance | BindingFlags.NonPublic);
            if (baseUriInfo != null)
            {
                var baseUri = baseUriInfo.GetValue(resourceDictionary);
                if (baseUri != null)
                {
                    return baseUri.ToString();
                }
            }

            var contextInfo = typeof(ResourceDictionary).GetField("_inheritanceContext",
                                                                 BindingFlags.Instance | BindingFlags.NonPublic);
            if (contextInfo != null)
            {
                var context = contextInfo.GetValue(resourceDictionary);
                if (context != null)
                {
                    return context.ToString();
                }
            }

            return null;
        }

        public static int GetGlobalIndex(Style style)
        {
            var globalIndexField = typeof(Style).GetField("GlobalIndex", BindingFlags.Instance | BindingFlags.NonPublic);
            if (globalIndexField != null)
            {
                return (int)globalIndexField.GetValue(style);
            }
            return -1;
        }

        public static string GetKeyString(object key)
        {
            var typeKey = key as Type;
            if (typeKey != null)
            {
                // implizit (typed) resource key
                return string.Concat("Default Style (" + typeKey.Name + ")");
            }
            return key.ToString();
        }
    }
}
