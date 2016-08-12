using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace ChristianMoser.WpfInspector.Services.ElementTree
{
    /// <summary>
    /// 
    /// </summary>
    public class TreeModel
    {
        #region Private Members

        private readonly ObservableCollection<TreeItem> _allItems = new ObservableCollection<TreeItem>();
        private readonly ObservableCollection<TreeItem> _rootItems = new ObservableCollection<TreeItem>();

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeModel"/> class.
        /// </summary>
        public TreeModel()
        {
            _rootItems.CollectionChanged += OnRootItemsChanged;
        }

        #endregion

        /// <summary>
        /// Gets the tree items view.
        /// </summary>
        public ObservableCollection<TreeItem> TreeItemsView
        {
            get { return _allItems; }
        }

        /// <summary>
        /// Gets the root items.
        /// </summary>
        public ObservableCollection<TreeItem> RootItems
        {
            get { return _rootItems; }
        }

        /// <summary>
        /// Expands the item.
        /// </summary>
        internal int ExpandItem(TreeItem item)
        {
            int childAdded = 0;
            int localAdded = 0;
            if (IsWholePathExpanded(item))
            {
                for (int i = 0; i < item.Children.Count; i++)
                {
                    int parentIndex = _allItems.IndexOf(item);
                    var child = item.Children[i];
                    if (!_allItems.Contains(child))
                    {
                        _allItems.Insert(parentIndex + childAdded + i + 1, child);
                        localAdded++;
                    }

                    if (child.IsExpanded)
                    {
                        childAdded += ExpandItem(child);
                    }
                }
            }
            return childAdded + localAdded;
        }

        /// <summary>
        /// Collapses the item.
        /// </summary>
        internal void CollapseItem(TreeItem item)
        {           
            int removeIndex = _allItems.IndexOf(item) + 1;
            while ((removeIndex < _allItems.Count) && (_allItems[removeIndex].Level > item.Level))
            {
                _allItems.RemoveAt(removeIndex);
            }
        }

        /// <summary>
        /// Updates the item.
        /// </summary>
        internal void UpdateItem(TreeItem item, IEnumerable<TreeItem> addedItems, IEnumerable<TreeItem> removedItems, int childenCountBefore)
        {
            if (IsWholePathExpanded(item))
            {
                int insertIndex = _allItems.IndexOf(item) + 1 + childenCountBefore;
                foreach (var removedItem in removedItems)
                {
                    _allItems.Remove(removedItem);
                }
                foreach (var addedItem in addedItems)
                {
                    _allItems.Insert(insertIndex++, addedItem);
                }
            }
        }

        /// <summary>
        /// Removes the item.
        /// </summary>
        internal void RemoveItem(TreeItem item)
        {
            _allItems.Remove(item);
        }

        #region Private Helpers

        private static bool IsWholePathExpanded(TreeItem item)
        {
            while (item != null)
            {
                if (!item.IsExpanded)
                {
                    return false;
                }
                item = item.Parent;
            }
            return true;
        }

        private void OnRootItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
            {
                foreach (TreeItem newItem in e.NewItems)
                {
                    _allItems.Add(newItem);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
            {
                foreach (TreeItem oldItem in e.OldItems)
                {
                    _allItems.Remove(oldItem);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                _allItems.Clear();
                foreach (var rootItem in _rootItems)
                {
                    _allItems.Add(rootItem);
                }
            }
        }

        #endregion
    }
}
