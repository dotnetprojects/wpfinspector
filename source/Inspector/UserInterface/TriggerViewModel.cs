using System.ComponentModel;
using ChristianMoser.WpfInspector.Services;
using ChristianMoser.WpfInspector.Services.ElementTree;
using ChristianMoser.WpfInspector.Services.Triggers;
using ChristianMoser.WpfInspector.Utilities;
using ChristianMoser.WpfInspector.UserInterface.Controls;

namespace ChristianMoser.WpfInspector.UserInterface
{
    public class TriggerViewModel : INotifyPropertyChanged
    {
        #region Private Members

        private readonly UpdateTrigger _updateTrigger = new UpdateTrigger();
        private TreeItem _selectedTreeItem;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerViewModel"/> class.
        /// </summary>
        public TriggerViewModel()
        {
            var selectedTreeItemService = ServiceLocator.Resolve<SelectedTreeItemService>();
            selectedTreeItemService.SelectedTreeItemChanged += OnSelectedTreeItemChanged;

            var triggerService = ServiceLocator.Resolve<TriggerService>();
            Triggers = triggerService.Triggers;
            Triggers.CollectionChanged += (s, e) => PropertyChanged.Notify(this,"IsEmpty");

            _updateTrigger.UpdateAction = () => triggerService.UpdateTriggerList(_selectedTreeItem);
        }

        #endregion

        private void OnSelectedTreeItemChanged(object sender, EventArgs<TreeItem> e)
        {
            _selectedTreeItem = e.Data;
            _updateTrigger.IsUpdateRequired = true;
        }

        public ICollectionView Triggers { get; private set; }

        public UpdateTrigger UpdateTrigger
        {
            get { return _updateTrigger; }
        }

        public bool IsEmpty
        {
            get { return Triggers.IsEmpty; }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
