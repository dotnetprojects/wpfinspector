using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using ChristianMoser.WpfInspector.Baml;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using ChristianMoser.WpfInspector.Services.ElementTree;
using System.Xml;
using System.Text.RegularExpressions;
using ChristianMoser.WpfInspector.Services.Xaml.BamlReader;

namespace ChristianMoser.WpfInspector.Services.Xaml
{
    /// <summary>
    /// Service to read xaml sources out of assembly resources
    /// </summary>
    public class XamlSourceService
    {
        private readonly Dictionary<string, string> _xamlSources = new Dictionary<string, string>();

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="XamlSourceService"/> class.
        /// </summary>
        public XamlSourceService()
        {
            //foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            //{
            //    LoadXamlSourcesForAssembly(assembly);
            //}
            //AppDomain.CurrentDomain.AssemblyLoad += (s, e) => LoadXamlSourcesForAssembly(e.LoadedAssembly);
        }

        #endregion

        /// <summary>
        /// Gets the xaml source for the specified tree item
        /// </summary>
        public string GetXamlSource(TreeItem treeItem)
        {
            while (!IsValidSource(treeItem) && treeItem != null)
            {
                treeItem = treeItem.Parent;
            }

            if (treeItem == null)
            {
                return string.Empty;
            }

            var name = treeItem.Instance.GetType().Name;

            foreach (var xamlSource in _xamlSources)
            {
                if (xamlSource.Key.Contains(name))
                {
                    return xamlSource.Value;
                }
            }
            return string.Empty;
        }

        #region Private Methods

        private void LoadXamlSourcesForAssembly(Assembly assembly)
        {
            var names = assembly.GetManifestResourceNames();
            foreach (var name in names)
            {
                if (name.Contains("PresentationFramework"))
                {
                    continue;
                }

                var stream = assembly.GetManifestResourceStream(name);
                foreach (Resource resource in new ResourceReader(stream))
                {
                    if (resource.Name.EndsWith("baml"))
                    {
                        string xaml = new BamlTranslator((Stream)resource.Value).ToString();
                        string sourceName = resource.Name.Replace(".baml", "");
                        ParseXamlSource(sourceName, xaml);
                    }
                }
            }
        }

        private void ParseXamlSource(string sourceName, string xaml)
        {
            var xamlDom = new XmlDocument();
            xamlDom.LoadXml(xaml);
            var rootElement = (XmlElement)xamlDom.FirstChild;
            
            if( rootElement.Name == "ResourceDictionary")
            {
                foreach (var xmlNode in rootElement.ChildNodes)
                {
                    var xmlElement = xmlNode as XmlElement;
                    if( xmlElement != null)
                    {
                        switch( xmlElement.Name)
                        {
                            case "Style":
                                ReadStyleElement(xmlElement);
                                break;
                        }
                    }

                }
            }
            else
            {
                _xamlSources.Add(rootElement.Name, rootElement.InnerXml);
            }
        }

        private void ReadStyleElement(XmlElement styleElement)
        {
            var key = styleElement.GetAttribute("x:Key");
            if( key != null)
            {
                key = Regex.Replace(key, @"\{x:Type (.*)\}", "[$1]");
            }
            var targetType = Regex.Replace(styleElement.GetAttribute("TargetType"),@"\{x:Type (.*)\}","[$1]");

            if( key == null)
            {
                key = targetType;
            }

            string sourceKey = "Style:" + key;

            if (!_xamlSources.ContainsKey(sourceKey))
            {
                _xamlSources.Add(sourceKey, styleElement.InnerXml);
            }
        }

        private static bool IsValidSource(TreeItem item)
        {
            if (item == null)
                return false;
            return item.Instance is Window ||
                   item.Instance is UserControl;
        }

        #endregion
    }
}
