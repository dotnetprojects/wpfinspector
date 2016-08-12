using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ChristianMoser.WpfInspector.Services.Events
{
    public class RoutedEventItem
    {
        #region Private Members

        private readonly RoutedEventArgs _eventArgs;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="RoutedEventItem"/> class.
        /// </summary>
        public RoutedEventItem(RoutedEventArgs eventArgs)
        {
            _eventArgs = eventArgs;
        }

        #endregion

        /// <summary>
        /// Gets the name of the routed event
        /// </summary>
        public string Name
        {
            get { return _eventArgs.RoutedEvent.Name; }
        }
    }
}
