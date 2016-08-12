using ChristianMoser.WpfInspector.Services;
using ChristianMoser.WpfInspector.Services.DataObjects;
using ChristianMoser.WpfInspector.Services.ElementTree;
using ChristianMoser.WpfInspector.UserInterface.Controls;

namespace ChristianMoser.WpfInspector.UserInterface
{
    public class DataContextViewModel
    {
        private readonly DataContextService _dataContextService;
        private readonly UpdateTrigger _updateTrigger = new UpdateTrigger();
        
        public DataContextViewModel()
        {
            var selectedTreeItemService = ServiceLocator.Resolve<SelectedTreeItemService>();
            selectedTreeItemService.SelectedTreeItemChanged += (s, e) => _updateTrigger.IsUpdateRequired = true;
            _updateTrigger.UpdateAction =
                () => _dataContextService.UpdateDataContext(selectedTreeItemService.SelectedTreeItem);

            _dataContextService = ServiceLocator.Resolve<DataContextService>();
        }

        public UpdateTrigger UpdateTrigger { get { return _updateTrigger; } }

        public DataContextInfo DataContextInfo
        {
            get { return _dataContextService.DataContextInfo; }
        }

    }
}
