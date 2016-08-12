using System;
using ChristianMoser.WpfInspector.Utilities;
using System.Windows.Threading;

namespace ChristianMoser.WpfInspector.Services.ElementTree
{
    /// <summary>
    /// Tracks the selected tree item for the logical and the visual tree
    /// </summary>
    public class SelectedTreeItemService
    {
        #region Private Members

        private TreeItem _selectedVisualTreeItem;
        private TreeItem _selectedLogicalTreeItem;
        private SearchStrategy _searchStrategy;

        #endregion

        public event EventHandler<EventArgs<TreeItem>> SelectedTreeItemChanged;
        public event EventHandler<EventArgs<SearchStrategy>> SearchStrategyChanged;

        public SearchStrategy SearchStrategy
        {
            get { return _searchStrategy; }
            set
            {
                _searchStrategy = value;
                OnSelectedObjectChanged();
            }
        }

        public void SetSearchStrategy(SearchStrategy searchStrategy)
        {
            _searchStrategy = searchStrategy;
            if (SearchStrategyChanged != null)
            {
                SearchStrategyChanged(this, new EventArgs<SearchStrategy> { Data = searchStrategy });
            }
        }

        public TreeItem FindStrategyNearItem(TreeItem treeItem)
        {
            if (treeItem == null)
            {
                return null;
            }

            if (treeItem is LogicalTreeItem && SearchStrategy == SearchStrategy.VisualTree)
            {
                return VisualTreeItem.FindVisualTreeItem(treeItem.Instance);
            }

            if (treeItem is VisualTreeItem && SearchStrategy == SearchStrategy.LogicalTree)
            {
                var logicalItem = LogicalTreeItem.FindLogicalTreeItem(treeItem.Instance);
                while (logicalItem == null)
                {
                    treeItem = treeItem.Parent;
                    logicalItem = LogicalTreeItem.FindLogicalTreeItem(treeItem.Instance);
                }
                treeItem = logicalItem;
            }

            return treeItem;
        }

        public TreeItem SelectedVisualTreeItem
        {
            get { return _selectedVisualTreeItem; }
            set
            {
                _selectedVisualTreeItem = value;
                OnSelectedObjectChanged();
            }
        }

        public TreeItem SelectedLogicalTreeItem
        {
            get { return _selectedLogicalTreeItem; }
            set
            {
                _selectedLogicalTreeItem = value;
                OnSelectedObjectChanged();
            }
        }

        public TreeItem SelectedTreeItem
        {
            get
            {
                if (SearchStrategy == SearchStrategy.LogicalTree)
                {
                    return _selectedLogicalTreeItem;
                }
                return _selectedVisualTreeItem;
            }
        }

        protected virtual void OnSelectedObjectChanged()
        {
            if (SelectedTreeItemChanged != null)
            {
                SelectedTreeItemChanged(this, new EventArgs<TreeItem> { Data = SelectedTreeItem });
            }
        }

    }
}
