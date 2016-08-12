using System;
using System.Collections.ObjectModel;
using System.Windows.Automation;
using System.Windows.Threading;

namespace ChristianMoser.WpfInspector.Services.ElementTree
{
    /// <summary>
    /// Service to access the UI automation tree
    /// </summary>
    public class AutomationTreeService
    {
        #region Private Members

        private readonly DispatcherTimer _refreshTreeTimer;
        private readonly TreeModel _treeModel = new TreeModel();
        
        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="AutomationTreeService"/> class.
        /// </summary>
        public AutomationTreeService()
        {
            FindRootElements();

            _refreshTreeTimer = new DispatcherTimer(DispatcherPriority.Background) { Interval = TimeSpan.FromMilliseconds(3000) };
            _refreshTreeTimer.Tick += OnRefreshTree;
            _refreshTreeTimer.Start();
        }

        #endregion

        #region Public Functionality

        /// <summary>
        /// Gets the elements.
        /// </summary>
        public ObservableCollection<TreeItem> Elements
        {
            get { return _treeModel.TreeItemsView; }
        }

        #endregion

        #region Private Helpers

        private void FindRootElements()
        {
            var rootItem = new AutomationItem(AutomationElement.RootElement, null, _treeModel, 0);
            _treeModel.RootItems.Add(rootItem);
            rootItem.Refresh();
        }

        private void OnRefreshTree(object sender, EventArgs e)
        {
            foreach (var rootItem in _treeModel.RootItems)
            {
                rootItem.Refresh();
            }
        }

        #endregion
    }
}
