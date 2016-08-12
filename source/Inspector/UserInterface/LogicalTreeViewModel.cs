using System;
using ChristianMoser.WpfInspector.Services;
using System.ComponentModel;
using System.Windows.Data;
using ChristianMoser.WpfInspector.Services.ElementTree;

namespace ChristianMoser.WpfInspector.UserInterface
{
    public class LogicalTreeViewModel : TreeViewModelBase
    {
        #region Private Members

        private readonly SelectedTreeItemService _selectedTreeItemService;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualTreeViewModel"/> class.
        /// </summary>
        public LogicalTreeViewModel()
        {
            _selectedTreeItemService = ServiceLocator.Resolve<SelectedTreeItemService>();
            var treeElementService = ServiceLocator.Resolve<LogicalTreeService>();
            Elements = new ListCollectionView(treeElementService.Elements);
            Elements.CurrentChanged += CurrentElementChanged;
            treeElementService.ElementsChanged += (s, e) => Elements.Refresh();
        }

        #endregion

        #region Public Functionality

        /// <summary>
        /// Gets the elements.
        /// </summary>
        public ICollectionView Elements { get; set; }

        #endregion

        #region Private Helpers

        private void CurrentElementChanged(object sender, EventArgs e)
        {
            var treeItem = Elements.CurrentItem as TreeItem;            
            SelectedElement = treeItem;
            _selectedTreeItemService.SelectedLogicalTreeItem = treeItem;
        }

        #endregion
    }

}
