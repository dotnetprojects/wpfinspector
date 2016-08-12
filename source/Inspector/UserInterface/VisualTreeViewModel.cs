using System.ComponentModel;
using System.Windows.Input;
using ChristianMoser.WpfInspector.Services;
using ChristianMoser.WpfInspector.Services.ElementTree;
using System.Windows.Data;
using System;
using ChristianMoser.WpfInspector.Utilities;

namespace ChristianMoser.WpfInspector.UserInterface
{
    public class VisualTreeViewModel : TreeViewModelBase, INotifyPropertyChanged
    {
        private readonly SelectedTreeItemService _selectedTreeItemService;
        private bool _isHighlightSelectedItem;
        private bool _disableNotification;

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualTreeViewModel"/> class.
        /// </summary>
        public VisualTreeViewModel()
        {
            _selectedTreeItemService = ServiceLocator.Resolve<SelectedTreeItemService>();
            _selectedTreeItemService.SelectedTreeItemChanged += OnSelectedTreeItemChanged;

            MouseElementServiceSettings = ServiceLocator.Resolve<MouseElementService>().Settings;

            var visualTreeElementService = ServiceLocator.Resolve<VisualTreeService>();
            visualTreeElementService.ElementsChanged += (s, e) => Elements.Refresh();

            //FocusInfo = ServiceLocator.Resolve<FocusedElementService>().Info;

            Elements = new ListCollectionView(visualTreeElementService.Elements);
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

        /// <summary>
        /// Gets or sets the mouse element service settings.
        /// </summary>
        public MouseElementServiceSettings MouseElementServiceSettings { get; private set; }

        public bool IsHighlightSelectedItem
        {
            get { return _isHighlightSelectedItem; }
            set { PropertyChanged.ChangeAndNotify(ref _isHighlightSelectedItem, value, this, "IsHighlightSelectedItem"); }
        }

        #endregion

        #region Private Methods

        private void CurrentElementChanged(object sender, EventArgs e)
        {
            _disableNotification = true;
            var treeItem = Elements.CurrentItem as TreeItem;
            SelectedElement = treeItem;
            _selectedTreeItemService.SelectedVisualTreeItem = treeItem;
            _disableNotification = false;
        }

        private void OnSelectedTreeItemChanged(object sender, EventArgs<TreeItem> e)
        {
            if (e.Data != null && !_disableNotification)
            {
                e.Data.BringIntoView();
                Elements.MoveCurrentTo(e.Data);
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
