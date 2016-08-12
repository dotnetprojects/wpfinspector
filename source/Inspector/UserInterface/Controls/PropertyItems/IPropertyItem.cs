using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ChristianMoser.WpfInspector.UserInterface.Controls.PropertyItems
{
    public interface IPropertyGridItem : IDisposable, INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; }

        /// <summary>
        /// Gets if the property item is selected
        /// </summary>
        bool IsSelected { get; set; }
    }
}
