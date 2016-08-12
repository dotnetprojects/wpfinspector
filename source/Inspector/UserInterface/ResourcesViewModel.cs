using ChristianMoser.WpfInspector.Services;
using ChristianMoser.WpfInspector.Services.ElementTree;
using ChristianMoser.WpfInspector.Services.Resources;
using ChristianMoser.WpfInspector.Utilities;
using System.ComponentModel;
using System.Windows.Data;
using ChristianMoser.WpfInspector.UserInterface.Controls;
using System.Collections;

namespace ChristianMoser.WpfInspector.UserInterface
{
    public class ResourcesViewModel : INotifyPropertyChanged
    {
        #region Private Members

        private string _filter;
        private readonly UpdateTrigger _updateTrigger = new UpdateTrigger();
        private TreeItem _selectedTreeItem;

        private bool _isShowLocal = true;
        private bool _isShowApplication = true;
        private bool _isShowTheme = true;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourcesViewModel"/> class.
        /// </summary>
        public ResourcesViewModel()
        {
            var selectedObjectService = ServiceLocator.Resolve<SelectedTreeItemService>();
            selectedObjectService.SelectedTreeItemChanged += OnSelectedObjectChanged;

            var resourcesService = ServiceLocator.Resolve<ResourcesService>();
            var resources = new ListCollectionView(resourcesService.ResourceItems);
            resources.CustomSort = new ResourceComparer();
            resources.Filter = ResourceFilter;
            Resources = resources;
            resourcesService.ResourcesListChanged += (s, e) => resources.Refresh();

            _updateTrigger.UpdateAction = () => resourcesService.UpdateResourceList(_selectedTreeItem);
        }

        #endregion

        #region Public Functionality

        /// <summary>
        /// Gets or sets the filter criteria
        /// </summary>
        /// <value>The filter.</value>
        public string Filter
        {
            get { return _filter; }
            set
            {
                PropertyChanged.ChangeAndNotify(ref _filter, value, this, "Filter");
                Resources.Refresh();
            }
        }

        public bool IsShowLocal
        {
            get { return _isShowLocal; }
            set
            {
                PropertyChanged.ChangeAndNotify(ref _isShowLocal, value, this, "IsShowLocal");
                Resources.Refresh();
            }
        }

        public bool IsShowTheme
        {
            get { return _isShowTheme; }
            set
            {
                PropertyChanged.ChangeAndNotify(ref _isShowTheme, value, this, "IsShowTheme");
                Resources.Refresh();
            }
        }

        public bool IsShowApplication
        {
            get { return _isShowApplication; }
            set
            {
                PropertyChanged.ChangeAndNotify(ref _isShowApplication, value, this, "IsShowApplication");
                Resources.Refresh();
            }
        }


        /// <summary>
        /// Gets the update trigger.
        /// </summary>
        public UpdateTrigger UpdateTrigger
        {
            get { return _updateTrigger; }
        }

        /// <summary>
        /// Gets the resources.
        /// </summary>
        /// <value>The resources.</value>
        public ICollectionView Resources { get; private set; }

        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// See <see cref="INotifyPropertyChanged.PropertyChanged" />
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Private Methods

        private bool ResourceFilter(object item)
        {
            var resourceItem = item as ResourceItem;
            if (resourceItem != null)
            {
                if (IsShowLocal && resourceItem.ResourceScope == ResourceScope.Local ||
                    IsShowTheme && resourceItem.ResourceScope == ResourceScope.Theme ||
                    IsShowApplication && resourceItem.ResourceScope == ResourceScope.Application)
                {
                    if (string.IsNullOrEmpty(_filter))
                    {
                        return true;
                    }
                    return resourceItem.Name.ToLower().Contains(_filter.ToLower());
                }
            }
            return false;
        }

        private void OnSelectedObjectChanged(object sender, EventArgs<TreeItem> e)
        {
            _selectedTreeItem = e.Data;
            _updateTrigger.IsUpdateRequired = true;
        }

        #endregion

        private class ResourceComparer : IComparer
        {
            #region Implementation of IComparer

            public int Compare(object x, object y)
            {
                var left = x as ResourceItem;
                var right = y as ResourceItem;

                if (left == null || right == null)
                {
                    return 0;
                }

                var catResult = left.Category.CompareTo(right.Category);
                if (catResult == 0)
                {
                    var typeResult = left.Type.Name.CompareTo(right.Type.Name);
                    if (typeResult == 0)
                    {
                        return left.Name.CompareTo(right.Name);
                    }
                }
                return catResult;
            }

            #endregion
        }

    }

    
}
