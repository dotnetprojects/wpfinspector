using System.Windows;
using System;
using System.Windows.Threading;
using System.Collections.ObjectModel;

namespace ChristianMoser.WpfInspector.Services.ElementTree
{
    
    /// <summary>
    /// Gets the elements of a WPF tree
    /// </summary>
    public abstract class TreeElementService 
    {
        #region Private Members

        private readonly DispatcherTimer _refreshTreeTimer;
        private readonly TreeModel _treeModel = new TreeModel();
        private readonly TreeType _treeType;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeElementService"/> class.
        /// </summary>
        public TreeElementService(TreeType treeType)
        {
            _treeType = treeType;
           
            FindRootElements();

            _refreshTreeTimer = new DispatcherTimer(DispatcherPriority.Background) {Interval = TimeSpan.FromMilliseconds(500)};
            _refreshTreeTimer.Tick += OnRefreshTree;
            _refreshTreeTimer.Start();
        }

        #endregion

        /// <summary>
        /// Occurs when the elements changed.
        /// </summary>
        public event EventHandler ElementsChanged;

        /// <summary>
        /// Refreshes the list
        /// </summary>
        public void Refresh()
        {
            FindRootElements();
        }

        /// <summary>
        /// Gets the elements.
        /// </summary>
        public ObservableCollection<TreeItem> Elements
        {
            get { return _treeModel.TreeItemsView; }
        }

        #region Private Helpers

        private void FindRootElements()
        {
            if (Application.Current != null)
            {
                _treeModel.RootItems.Add(new ApplicationTreeItem(Application.Current, _treeModel, _treeType));
            }
            else
            {
                foreach (PresentationSource presentationSource in PresentationSource.CurrentSources)
                {
                    _treeModel.RootItems.Add(new PresentationSourceTreeItem(presentationSource, _treeModel, _treeType));
                }
            }

            NotifyElementsChangedChanged();
        }

        private void NotifyElementsChangedChanged()
        {
            if (ElementsChanged != null)
            {
                ElementsChanged(this, EventArgs.Empty);
            }
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
