using System;
using ChristianMoser.WpfInspector.Services;
using ChristianMoser.WpfInspector.Services.ElementTree;
using ChristianMoser.WpfInspector.UserInterface.Controls;
using ChristianMoser.WpfInspector.Utilities;
using ChristianMoser.WpfInspector.Services.Events;
using System.ComponentModel;
using System.Windows.Data;

namespace ChristianMoser.WpfInspector.UserInterface
{
    public class EventsViewModel
    {
        #region Private Members

        private readonly RoutedEventTrackingService _eventTrackingService;
        private readonly UpdateTrigger _updateTrigger = new UpdateTrigger();
        private TreeItem _selectedTreeItem;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="EventsViewModel"/> class.
        /// </summary>
        public EventsViewModel()
        {
            var selectedTreeItemService = ServiceLocator.Resolve<SelectedTreeItemService>();
            selectedTreeItemService.SelectedTreeItemChanged += OnSelectedTreeItemChanged;

            _eventTrackingService = ServiceLocator.Resolve<RoutedEventTrackingService>();

            // Event Handlers
            EventHandlers = new ListCollectionView(_eventTrackingService.RoutedEvents);
            EventHandlers.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            EventHandlers.GroupDescriptions.Add(new PropertyGroupDescription("Type"));
            _eventTrackingService.EventHandlersChanged += (s, e) => EventHandlers.Refresh();

            // Events
            Events = new ListCollectionView(_eventTrackingService.EventItems);
            _eventTrackingService.EventsChanged += (s, e) => Events.Refresh();

            _updateTrigger.UpdateAction = () => _eventTrackingService.UpdateRoutedEvents(_selectedTreeItem);
        }

        #endregion

        /// <summary>
        /// Gets the routed events.
        /// </summary>
        public ICollectionView EventHandlers { get; private set; }

        /// <summary>
        /// Gets the routed events.
        /// </summary>
        public ICollectionView Events { get; private set; }

        /// <summary>
        /// Gets the update trigger.
        /// </summary>
        public UpdateTrigger UpdateTrigger
        {
            get { return _updateTrigger; }
        }

        #region Private Members

        private void OnSelectedTreeItemChanged(object sender, EventArgs<TreeItem> e)
        {
            _selectedTreeItem = e.Data;
            _updateTrigger.IsUpdateRequired = true;
        }

        #endregion
    }
}
