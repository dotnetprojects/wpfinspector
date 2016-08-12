using System.ComponentModel;
using System.Windows.Data;
using ChristianMoser.WpfInspector.Services;
using ChristianMoser.WpfInspector.Services.ElementTree;
using ChristianMoser.WpfInspector.Utilities;
using System;

namespace ChristianMoser.WpfInspector.UserInterface
{
    public class AutomationViewModel
    {
        #region Private Members

        private readonly SelectedTreeItemService _selectedTreeItemService;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="AutomationViewModel"/> class.
        /// </summary>
        public AutomationViewModel()
        {
            var automationTreeService = ServiceLocator.Resolve<AutomationTreeService>();
            _selectedTreeItemService = ServiceLocator.Resolve<SelectedTreeItemService>();

            Elements = new ListCollectionView(automationTreeService.Elements);
            Elements.CurrentChanged += CurrentElementChanged;
            Elements.MoveCurrentToFirst();
        }

        #endregion

        #region Public Functionality

        /// <summary>
        /// Gets the elements.
        /// </summary>
        /// <value>The elements.</value>
        public ICollectionView Elements { get; set; }

        #endregion

        #region Private Methods

        private void CurrentElementChanged(object sender, EventArgs e)
        {
            var treeItem = Elements.CurrentItem as TreeItem;
            _selectedTreeItemService.SelectedVisualTreeItem = treeItem;
        }

        #endregion



    }
}
