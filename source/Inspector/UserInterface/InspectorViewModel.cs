using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChristianMoser.WpfInspector.Services;
using System.ComponentModel;
using ChristianMoser.WpfInspector.Services.ElementTree;
using ChristianMoser.WpfInspector.Utilities;
using Microsoft.Win32;
using ChristianMoser.WpfInspector.Services.Theming;
using System.Windows.Data;
using ChristianMoser.WpfInspector.Services.Resizing;
using System.Windows.Input;

namespace ChristianMoser.WpfInspector.UserInterface
{
    public class InspectorViewModel : INotifyPropertyChanged
    {
        private int _selectedIndex;
        private readonly SelectedTreeItemService _selectedTreeItemService;
        private readonly ApplicationThemeService _applicationThemeService;
        private bool _showUsageHint;

        public InspectorViewModel()
        {
            _selectedTreeItemService = ServiceLocator.Resolve<SelectedTreeItemService>();
            _selectedTreeItemService.SearchStrategyChanged += (s, e) => SelectedTreeIndex = e.Data == SearchStrategy.LogicalTree ? 1 : 0;

            _applicationThemeService = ServiceLocator.Resolve<ApplicationThemeService>();
            Themes = new ListCollectionView(_applicationThemeService.Themes);

            ApplicationSettings = ServiceLocator.Resolve<ApplicationSettingsService>().ApplicationSettings;

            var resizeService = ServiceLocator.Resolve<ApplicationResizeService>();
            Sizes = new ListCollectionView(resizeService.Sizes);

            AboutCommand = new Command<object>(o => new AboutWindow().ShowDialog());
            DetachCommand = new Command<object>(o => ServiceLocator.Resolve<InspectorWindow>().Close());

            UpdateShowUsageHint();
        }

        /// <summary>
        /// Gets or sets the application settings.
        /// </summary>
        public ApplicationSettings ApplicationSettings { get; private set; }

        /// <summary>
        /// Gets the application themes.
        /// </summary>
        public ICollectionView Themes { get; private set; }

        /// <summary>
        /// Gets the sizes.
        /// </summary>
        public ICollectionView Sizes { get; private set; }

        /// <summary>
        /// Gets the about command.
        /// </summary>
        public ICommand AboutCommand { get; private set; }

        /// <summary>
        /// Gets the detach command.
        /// </summary>
        public ICommand DetachCommand { get; private set; }

        /// <summary>
        /// Gets if the usage hint should be shown at startup
        /// </summary>
        public bool ShowUsageHint
        {
            get { return _showUsageHint; }
        }

        public int SelectedTreeIndex
        {
            get { return _selectedIndex; }
            set
            {
                PropertyChanged.ChangeAndNotify(ref _selectedIndex, value, this, "SelectedIndex");
                var searchStrategy = value == 0 ? SearchStrategy.VisualTree : SearchStrategy.LogicalTree;
                _selectedTreeItemService.SearchStrategy = searchStrategy;
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Private Helpers

        private void UpdateShowUsageHint()
        {
            try
            {
                var dontShowUsageHint = Registry.GetValue(@"HKEY_CURRENT_USER\Software\WPFInspector", "DontShowUsageHint", null) as string;
                if (dontShowUsageHint == null)
                {
                    _showUsageHint = true;
                }
            }
            catch (Exception)
            {
            }
        }

        #endregion
    }
}
