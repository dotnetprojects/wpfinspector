using System;
using System.Windows;
using System.ComponentModel;
using ChristianMoser.WpfInspector.Utilities;

namespace ChristianMoser.WpfInspector.Services.Events
{
    public class RoutedEventHandler : IDisposable, INotifyPropertyChanged
    {
        private readonly RoutedEvent _routedEvent;
        private bool _isEnabled;
        private readonly FrameworkElement _element;
        private readonly Action<RoutedEventItem> _addEventFunc;

        #region Construction

        public RoutedEventHandler(RoutedEvent routedEvent, FrameworkElement element, Action<RoutedEventItem> addEventFunc)
        {
            _routedEvent = routedEvent;
            _element = element;
            _addEventFunc = addEventFunc;

            IsEnabled = true;
        }

        #endregion


        /// <summary>
        /// Gets the name of the routed event
        /// </summary>
        public string Name
        {
            get { return _routedEvent.Name; }
        }

        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        public string Type
        {
            get { return _routedEvent.OwnerType.Name; }
        }

        /// <summary>
        /// Gets or sets if the routed event is tracked
        /// </summary>
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                PropertyChanged.ChangeAndNotify(ref _isEnabled, value, this, "IsEnabled");
                if( value)
                {
                    AddHandler();
                }
                else
                {
                    RemoveHandler();
                }
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            RemoveHandler();
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Private Helpers

        private void AddHandler()
        {
            _element.AddHandler(_routedEvent, (System.Windows.RoutedEventHandler)OnRoutedEvent, true);
        }

        private void RemoveHandler()
        {
            _element.RemoveHandler(_routedEvent, (System.Windows.RoutedEventHandler)OnRoutedEvent);
        }

        private void OnRoutedEvent(object sender, RoutedEventArgs e)
        {
            _addEventFunc(new RoutedEventItem(e));   
        }

        #endregion
    }
}
