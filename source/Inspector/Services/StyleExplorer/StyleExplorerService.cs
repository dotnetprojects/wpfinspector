using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChristianMoser.WpfInspector.Services.ElementTree;
using System.Windows;
using System.Reflection;
using ChristianMoser.WpfInspector.Services.Triggers;
using ChristianMoser.WpfInspector.Services.Xaml.BamlReader;
using ChristianMoser.WpfInspector.Utilities;
using ChristianMoser.WpfInspector.Win32;
using System.IO;
using ChristianMoser.WpfInspector.Baml;

namespace ChristianMoser.WpfInspector.Services.StyleExplorer
{
    public class StyleExplorerService
    {
        #region Private Members

        private readonly List<StyleItem> _styleItems = new List<StyleItem>();
        private readonly List<SetterItem> _setterItems = new List<SetterItem>();

        #endregion

        public event EventHandler StyleChanged;

        /// <summary>
        /// Gets the style items.
        /// </summary>
        public List<StyleItem> StyleItems
        {
            get { return _styleItems; }
        }

        public List<SetterItem> ActiveSetterItems
        {
            get { return _setterItems; }
        }

        public void UpdateStyle(TreeItem treeItem)
        {
            ClearStyleItems();

            if (treeItem == null)
            {
                return;
            }

            var frameworkElement = treeItem.Instance as FrameworkElement;
            if (frameworkElement == null)
            {
                return;
            }

            // Resolve theme style
            var themeStyle = GetNamedThemeStyle(frameworkElement) ??
                             GetClassicThemeStyle(frameworkElement) ??
                             GetGenericThemeStyle(frameworkElement);
            if (themeStyle != null)
            {
                _styleItems.Add(themeStyle);
            }

            // Resolve default styles

            // Resolve local, inherited and app style
            var style = frameworkElement.GetValue(FrameworkElement.StyleProperty) as Style;
            if (style != null)
            {
                AddStyleRecursive(frameworkElement, style);
            }

            ApplyOverrides();

            OnStyleChanged();
        }

        private void ClearStyleItems()
        {
            foreach (var styleItem in _styleItems)
            {
                styleItem.Dispose();
            }
            _styleItems.Clear();
        }

        private void ApplyOverrides()
        {
            var activeSetters = new Dictionary<string, SetterItem>();
            foreach (var styleItem in _styleItems)
            {
                if (styleItem.OverridesDefaultStyle)
                {
                    foreach (var styleSetterItem in activeSetters.Values)
                    {
                        styleSetterItem.IsOverridden = true;
                    }
                }

                foreach (var styleSetterItem in styleItem.SetterItems)
                {
                    if (activeSetters.ContainsKey(styleSetterItem.Property))
                    {
                        activeSetters[styleSetterItem.Property].IsOverridden = true;
                        activeSetters[styleSetterItem.Property] = styleSetterItem;
                    }
                    else
                    {
                        activeSetters.Add(styleSetterItem.Property, styleSetterItem);
                    }
                }
            }

            _setterItems.Clear();
            _setterItems.AddRange(activeSetters.Values);
        }


        private void AddStyleRecursive(FrameworkElement owner, Style style)
        {
            StyleItem styleItem;
            if (style.BasedOn != null)
            {
                AddStyleRecursive(owner, style.BasedOn);
            }
            else
            {
                var localBaseStyles = ResourceHelper.GetResourcesRecursively<Style>(owner).Where(s => s.Key == owner.GetType());
                foreach (var localBaseStyle in localBaseStyles)
                {
                    if (StyleHelper.TryGetStyleItem(owner, localBaseStyle.Value, out styleItem))
                    {
                        _styleItems.Add(styleItem);
                    }
                }
            }

            if (StyleHelper.TryGetStyleItem(owner, style, out styleItem))
            {
                _styleItems.Add(styleItem);
            }
        }

        private static StyleItem GetNamedThemeStyle(FrameworkElement frameworkElement)
        {
            var type = frameworkElement.GetType();
            var themeAssembly = GetThemeAssembly(type, GetThemeName(), false);

            if (themeAssembly != null)
            {
                var bamlResourceName = string.Format("themes/{0}.baml", GetThemeAndColorName());
                return GetStyleFromAssembly(frameworkElement, themeAssembly, bamlResourceName, themeAssembly.GetName().Name);
            }
            return null;
        }

        private static StyleItem GetClassicThemeStyle(FrameworkElement frameworkElement)
        {
            var type = frameworkElement.GetType();
            var themeAssembly = GetThemeAssembly(type, "classic", false);
            if (themeAssembly != null)
            {
                return GetStyleFromAssembly(frameworkElement, themeAssembly, "themes/classic.baml", themeAssembly.GetName().Name);
            }
            return null;
        }

        private static StyleItem GetGenericThemeStyle(FrameworkElement frameworkElement)
        {
            var type = frameworkElement.GetType();
            var themeAssembly = GetThemeAssembly(type, GetThemeName(), true);
            if (themeAssembly != null)
            {
                return GetStyleFromAssembly(frameworkElement, themeAssembly, "themes/generic.baml", themeAssembly.GetName().Name);
            }
            return null;
        }

        private static Assembly GetThemeAssembly(Type type, string themeName, bool generic)
        {
            switch (GetThemeInfo(type, generic))
            {
                case ResourceDictionaryLocation.None:
                    return null;
                case ResourceDictionaryLocation.SourceAssembly:
                    return type.Assembly;
                case ResourceDictionaryLocation.ExternalAssembly:
                    return AssemblyHelper.FindAssemblyByPartialName((string.Format("{0}.{1}", type.Assembly.GetName().Name, themeName)));
            }
            return null;
        }

        private static ResourceDictionaryLocation GetThemeInfo(Type type, bool generic)
        {
            var attributes = type.Assembly.GetCustomAttributes(typeof(ThemeInfoAttribute), false);
            if (attributes.Length > 0)
            {
                var themeInfo = (ThemeInfoAttribute)attributes[0];
                return generic ? themeInfo.GenericDictionaryLocation : themeInfo.ThemeDictionaryLocation;
            }
            return ResourceDictionaryLocation.None;
        }

        private static StyleItem GetStyleFromAssembly(FrameworkElement frameworkElement, Assembly themeAssembly, string bamlResourceName, string location)
        {
            var fieldInfo = typeof(FrameworkElement).GetField("DefaultStyleKeyProperty", BindingFlags.Static | BindingFlags.NonPublic);
            if (fieldInfo != null)
            {
                var defaultStyleKeyProperty = (DependencyProperty)fieldInfo.GetValue(null);
                object defaultStyleKey = frameworkElement.GetValue(defaultStyleKeyProperty);

                var names = themeAssembly.GetManifestResourceNames();
                foreach (var name in names)
                {
                    var stream = themeAssembly.GetManifestResourceStream(name);
                    foreach (Resource resource in new ResourceReader(stream))
                    {
                        if (resource.Name.Equals(bamlResourceName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            var resourceDict = (ResourceDictionary)BamlLoader.LoadBaml((Stream)resource.Value);
                            if (resourceDict != null)
                            {
                                foreach (var key in resourceDict.Keys)
                                {
                                    if (key == defaultStyleKey && resourceDict[key] is Style)
                                    {
                                        return new StyleItem((Style)resourceDict[key], frameworkElement, StyleHelper.GetKeyString(key), location, StyleScope.Theme);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        private static string GetThemeAndColorName()
        {
            var themeName = new StringBuilder(0x200);
            var themeColor = new StringBuilder(0x200);
            NativeMethods.GetCurrentThemeName(themeName, themeName.Capacity, themeColor, themeColor.Capacity, null, 0);
            var name = Path.GetFileNameWithoutExtension(themeName.ToString());
            return string.Format("{0}.{1}", name, themeColor);
        }

        private static string GetThemeName()
        {
            var themeName = new StringBuilder(0x200);
            NativeMethods.GetCurrentThemeName(themeName, themeName.Capacity, null, 0, null, 0);
            return Path.GetFileNameWithoutExtension(themeName.ToString());
        }

        private void OnStyleChanged()
        {
            if (StyleChanged != null)
            {
                StyleChanged(this, EventArgs.Empty);
            }
        }


    }
}
