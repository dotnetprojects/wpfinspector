using System.Collections.Generic;
using System.Windows;
using System;
using ChristianMoser.WpfInspector.Services.ElementTree;

namespace ChristianMoser.WpfInspector.Services.Events
{
    public class RoutedEventTrackingService
    {
        #region Private Members

        private readonly List<RoutedEventHandler> _eventHandlers = new List<RoutedEventHandler>();
        private readonly List<RoutedEventItem> _eventItems = new List<RoutedEventItem>();

        #endregion

        /// <summary>
        /// Occurs when the routed events changed.
        /// </summary>
        public event EventHandler EventHandlersChanged;

        /// <summary>
        /// Occurs when the routed events changed.
        /// </summary>
        public event EventHandler EventsChanged;

        /// <summary>
        /// Gets the routed events.
        /// </summary>
        public List<RoutedEventHandler> RoutedEvents
        {
            get { return _eventHandlers; }
        }

        /// <summary>
        /// Gets the event items.
        /// </summary>
        public List<RoutedEventItem> EventItems
        {
            get { return _eventItems; }
        }

        #region Private Methods

        public void UpdateRoutedEvents(TreeItem treeItem)
        {
            _eventItems.Clear();
            OnEventsChanged();

            foreach (var routedEvent in RoutedEvents)
            {
                routedEvent.Dispose();
            }
            
            RoutedEvents.Clear();

            if (treeItem != null )
            {
                var fe = treeItem.Instance as FrameworkElement;
                if (fe != null)
                {
                    var routedEvents = EventManager.GetRoutedEvents();
                    if (routedEvents != null)
                    {
                        foreach (RoutedEvent routedEvent in routedEvents)
                        {
                            RoutedEvents.Add(new RoutedEventHandler(routedEvent, fe, AddEvent));
                        }
                    }
                }
            }

            OnRoutedEventsChanged();
        }

        private void AddEvent(RoutedEventItem eventItem)
        {
            _eventItems.Add(eventItem);
            OnEventsChanged();
        }

        private void OnRoutedEventsChanged()
        {
            if( EventHandlersChanged != null)
            {
                EventHandlersChanged(this, EventArgs.Empty);
            }
        }

        private void OnEventsChanged()
        {
            if( EventsChanged != null)
            {
                EventsChanged(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}
