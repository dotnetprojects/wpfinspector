using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Input;
using ChristianMoser.WpfInspector.Utilities;
using System.Reflection;
using System.Windows;

namespace ChristianMoser.WpfInspector.Services.Theming
{
    public class ApplicationThemeItem : INotifyPropertyChanged
    {
        #region Private Members

        private bool _isActive;
        private readonly List<ResourceDictionary> _resources;

        #endregion

        public ApplicationThemeItem(List<ResourceDictionary> resources, string name, Action<ApplicationThemeItem> activateFunc)
        {
            _resources = resources;
            Name = name;
            ActivateCommand = new Command<object>(o => activateFunc(this));
        }

        /// <summary>
        /// Gets the name of the theme
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the resources.
        /// </summary>
        internal List<ResourceDictionary> Resources
        {
            get { return _resources; }
        }

        /// <summary>
        /// Gets or sets if the theme is active
        /// </summary>
        public bool IsActive
        {
            get { return _isActive; }
            set { PropertyChanged.ChangeAndNotify(ref _isActive, value, this, "IsActive"); }
        }
        
        /// <summary>
        /// Gets or sets the activate command.
        /// </summary>
        public ICommand ActivateCommand { get; private set; }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

    }
}
