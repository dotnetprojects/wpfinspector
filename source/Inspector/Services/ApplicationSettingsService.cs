using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System;
using System.Xml.Linq;
using System.Windows.Threading;

namespace ChristianMoser.WpfInspector.Services
{
    public class ApplicationSettingsService
    {
        #region Private Members

        private readonly ApplicationSettings _applicationSettings = new ApplicationSettings();
        private readonly DispatcherTimer _saveTimer;
        private static string _directoryName = "WPFInspector";

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationSettingsService"/> class.
        /// </summary>
        public ApplicationSettingsService()
        {
            _saveTimer = new DispatcherTimer();
            _saveTimer.Interval = TimeSpan.FromSeconds(1);
            _saveTimer.Tick += (s, e) =>
                                   {
                                       Save();
                                       _saveTimer.Stop();
                                   };

            _applicationSettings.PropertyChanged += (s, e) =>
                                                        {
                                                            _saveTimer.Stop();
                                                            _saveTimer.Start();
                                                        };

            Load();
        }

        #endregion

        #region Public Functionality

        /// <summary>
        /// Gets the application settings.
        /// </summary>
        public ApplicationSettings ApplicationSettings
        {
            get { return _applicationSettings; }
        }

        public void Save()
        {
            try
            {
                var doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
                doc.Add(new XElement("settings", ReadValuesFromSetting()));
                doc.Save(ConfigurationFile);
            }
            catch { }
        }

        #endregion

        #region Private Methods

        private void Load()
        {
            try
            {
                if( !Directory.Exists(ConfigurationFolder))
                {
                    Directory.CreateDirectory(ConfigurationFolder);
                }

                if (!File.Exists(ConfigurationFile))
                {
                    return;
                }

                var settingsType = _applicationSettings.GetType();
                var doc = XDocument.Load(ConfigurationFile);
                foreach (XElement element in doc.Element("settings").Elements())
                {
                    var propertyInfo = settingsType.GetProperty(element.Name.LocalName);
                    if (propertyInfo != null)
                    {
                        var converter = TypeDescriptor.GetConverter(propertyInfo.PropertyType);
                        object value = element.Value;
                        if (converter != null)
                        {
                            value = converter.ConvertFromString(element.Value);
                        }
                        propertyInfo.SetValue(_applicationSettings, value, null);
                    }
                }
            }
            catch { }

        }

        private static string ConfigurationFolder
        {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),_directoryName); }
        }

        private static string ConfigurationFile
        {
            get { return Path.Combine(ConfigurationFolder, "WpfInspector.config"); }
        }

        private IEnumerable<XElement> ReadValuesFromSetting()
        {
            foreach (var propertyInfo in _applicationSettings.GetType().GetProperties())
            {
                yield return new XElement(propertyInfo.Name, propertyInfo.GetValue(_applicationSettings, null));
            }
        }

        #endregion
    }
}
