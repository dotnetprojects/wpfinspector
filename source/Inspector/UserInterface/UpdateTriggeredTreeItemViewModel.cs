using ChristianMoser.WpfInspector.Services;
using ChristianMoser.WpfInspector.Services.ElementTree;
using ChristianMoser.WpfInspector.UserInterface.Controls;
using ChristianMoser.WpfInspector.Utilities;

namespace ChristianMoser.WpfInspector.UserInterface
{
    /// <summary>
    /// Base class for all view models
    /// </summary>
    public abstract class UpdateTriggeredTreeItemViewModel
    {
        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourcesViewModel"/> class.
        /// </summary>
        protected UpdateTriggeredTreeItemViewModel()
        {
            var selectedObjectService = ServiceLocator.Resolve<SelectedTreeItemService>();
            selectedObjectService.SelectedTreeItemChanged += OnSelectedObjectChanged;

            UpdateTrigger = new UpdateTrigger { UpdateAction = OnTriggerUpdate };
        }

        #endregion

        /// <summary>
        /// Gets the update trigger.
        /// </summary>
        public UpdateTrigger UpdateTrigger { get; private set; }

        #region Protected Functionality

        /// <summary>
        /// Gets the selected tree item.
        /// </summary>
        protected TreeItem SelectedTreeItem { get; private set; }

        /// <summary>
        /// Called when the selected tree item changed an UI update is required
        /// </summary>
        protected abstract void OnTriggerUpdate();

        #endregion

        #region Private Methods

        private void OnSelectedObjectChanged(object sender, EventArgs<TreeItem> e)
        {
            SelectedTreeItem = e.Data;
            UpdateTrigger.IsUpdateRequired = true;
        }

        #endregion

    }
}
