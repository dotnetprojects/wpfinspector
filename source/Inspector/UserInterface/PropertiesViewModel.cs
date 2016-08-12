using System;
using System.ComponentModel;
using System.Windows.Threading;
using ChristianMoser.WpfInspector.Services;
using ChristianMoser.WpfInspector.Services.Analyzers;
using ChristianMoser.WpfInspector.Services.ElementTree;
using ChristianMoser.WpfInspector.Utilities;
using ChristianMoser.WpfInspector.UserInterface.Controls;

namespace ChristianMoser.WpfInspector.UserInterface
{
    public class PropertiesViewModel : INotifyPropertyChanged
    {
        #region Private Members

        private readonly SelectedTreeItemService _selectedTreeItemService;
        private readonly UpdateTrigger _updateTrigger = new UpdateTrigger();

        private string _selectedProperty;
        private TreeItem _selectedTreeItem;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertiesViewModel"/> class.
        /// </summary>
        public PropertiesViewModel()
        {
            // Property List
            _selectedTreeItemService = ServiceLocator.Resolve<SelectedTreeItemService>();
            _selectedTreeItemService.SelectedTreeItemChanged += OnSelectedTreeItemChanged;

            var issuesAnalyzerService = ServiceLocator.Resolve<IssuesAnalyzerService>();
            issuesAnalyzerService.IssueSelected += OnIssueSelected;

            _updateTrigger.UpdateAction = () => SelectedTreeItem = _selectedTreeItemService.SelectedTreeItem;
        }

        #endregion

        private void OnSelectedTreeItemChanged(object sender, EventArgs<TreeItem> e)
        {
            _updateTrigger.IsUpdateRequired = true;
        }

        public string SelectedProperty
        {
            get { return _selectedProperty; }
            private set { PropertyChanged.ChangeAndNotify(ref _selectedProperty, value, this, "SelectedProperty"); }
        }

        private void OnIssueSelected(object sender, EventArgs<Issue> e)
        {
            if (e.Data.Property != null )
            {
                SelectedProperty = e.Data.Property.Name;
            }
        }

        public UpdateTrigger UpdateTrigger { get { return _updateTrigger; } }

        public TreeItem SelectedTreeItem
        {
            get { return _selectedTreeItem; }
            set { PropertyChanged.ChangeAndNotify(ref _selectedTreeItem, value, this, "SelectedTreeItem"); }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
