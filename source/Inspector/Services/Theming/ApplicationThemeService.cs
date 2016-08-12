using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Reflection;
using ChristianMoser.WpfInspector.Baml;
using ChristianMoser.WpfInspector.Services.Xaml.BamlReader;
using ChristianMoser.WpfInspector.Utilities;

namespace ChristianMoser.WpfInspector.Services.Theming
{
    /// <summary>
    /// Service to switch the themes of an application at runtime
    /// </summary>
    public class ApplicationThemeService
    {

        #region Construction

        public ApplicationThemeService()
        {
            Themes = LoadThemes();
        }

        #endregion

        /// <summary>
        /// Gets or sets the themes.
        /// </summary>
        public List<ApplicationThemeItem> Themes { get; private set; }

        #region Private Helpers

        private List<ApplicationThemeItem> LoadThemes()
        {
            var themes = new List<ApplicationThemeItem>();

            var aeroAssembly = Assembly.Load(KnownAssemblyNames.PresentationFrameworkAero);
            if( aeroAssembly != null)
            {
                themes.Add(new ApplicationThemeItem(GetResourcesFromThemeAssembly(aeroAssembly, "Themes/Aero.normalcolor.baml"), "Aero", ActivateTheme));    
            }
            var lunaAssembly = Assembly.Load(KnownAssemblyNames.PresentationFrameworkLuna);
            if (lunaAssembly != null)
            {
                themes.Add(new ApplicationThemeItem(GetResourcesFromThemeAssembly(lunaAssembly, "Themes/Luna.normalcolor.baml"), "Luna (blue)", ActivateTheme));
                themes.Add(new ApplicationThemeItem(GetResourcesFromThemeAssembly(lunaAssembly, "Themes/Luna.homestead.baml"), "Luna (homestead)", ActivateTheme));
                themes.Add(new ApplicationThemeItem(GetResourcesFromThemeAssembly(lunaAssembly, "Themes/Luna.metallic.baml"), "Luna (metallic)", ActivateTheme));    
            }
            var classicAssembly = Assembly.Load(KnownAssemblyNames.PresentationFrameworkClassic);
            if (classicAssembly != null)
            {
                themes.Add(new ApplicationThemeItem(GetResourcesFromThemeAssembly(classicAssembly, "Themes/classic.baml"), "Classic", ActivateTheme));
            }
            var royaleAssembly = Assembly.Load(KnownAssemblyNames.PresentationFrameworkRoyale);
            if (royaleAssembly != null)
            {
                themes.Add(new ApplicationThemeItem(GetResourcesFromThemeAssembly(royaleAssembly, "Themes/royale.normalcolor.baml"), "Royale", ActivateTheme));
            }
            return themes;
        }

        public void ActivateTheme(ApplicationThemeItem applicationThemeItem)
        {
            foreach (var themeItem in Themes)
            {
                if (themeItem.IsActive)
                {
                    UnLoadThemeResources(themeItem.Resources);
                    themeItem.IsActive = false;
                }
            }

            LoadThemeResources(applicationThemeItem.Resources);
            applicationThemeItem.IsActive = true;
        }

        private static void LoadThemeResources(IEnumerable<ResourceDictionary> resources)
        {
            var app = Application.Current;
            if (app != null)
            {
                foreach (ResourceDictionary skinResource in resources)
                {
                    app.Resources.MergedDictionaries.Insert(0,skinResource);
                }
            }
        }

        private static void UnLoadThemeResources(IEnumerable<ResourceDictionary> resources)
        {
            var app = Application.Current;
            if (app != null)
            {
                foreach (ResourceDictionary skinResource in resources)
                {
                    app.Resources.MergedDictionaries.Remove(skinResource);
                }
            }
        }

        private static List<ResourceDictionary> GetResourcesFromThemeAssembly(Assembly assembly, string bamlResourceName)
        {
            var resourceDictionaries = new List<ResourceDictionary>();

            var names = assembly.GetManifestResourceNames();
            foreach (var name in names)
            {
                var stream = assembly.GetManifestResourceStream(name);
                foreach (Resource resource in new ResourceReader(stream))
                {
                    if (resource.Name.Equals(bamlResourceName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        var resourceDictionary = BamlLoader.LoadBaml((Stream) resource.Value) as ResourceDictionary;
                        if (resourceDictionary != null)
                        {
                            resourceDictionaries.Add(resourceDictionary);
                        }
                    }
                }
            }
            return resourceDictionaries;
        }

        #endregion
    }
}
