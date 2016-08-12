using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChristianMoser.WpfInspector.UserInterface.Controls.PropertyItems;
using System.ComponentModel;
using ChristianMoser.WpfInspector.Services.Resources;
using System.Windows;
using System.Windows.Data;
using ChristianMoser.WpfInspector.Utilities;
using ChristianMoser.WpfInspector.UserInterface.Controls;

namespace ChristianMoser.WpfInspector.UserInterface
{
    public class ResourcesEditorViewModel : INotifyPropertyChanged
    {
        #region Private Members

        private readonly List<ResourceItem> _resources = new List<ResourceItem>();
        private readonly PropertyItem _propertyItem;
        private string _filter;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourcesEditorViewModel"/> class.
        /// </summary>
        /// <param name="propertyItem">The property item.</param>
        public ResourcesEditorViewModel(PropertyItem propertyItem)
        {
            _propertyItem = propertyItem;

            var fe = propertyItem.Instance as FrameworkElement;
            if (fe != null)
            {
                _resources.AddRange(ResourceItemFactory.GetResourcesForElement(fe));
            }

            Resources = new ListCollectionView(_resources);
            Resources.Filter = FilterResources;
        }

        /// <summary>
        /// Gets or sets the filter.
        /// </summary>
        /// <value>The filter.</value>
        public string Filter
        {
            get { return _filter; }
            set
            {
                PropertyChanged.ChangeAndNotify(ref _filter, value, this, "Filter");
                Resources.Refresh();
            }
        }

        /// <summary>
        /// Gets the resources.
        /// </summary>
        /// <value>The resources.</value>
        public ICollectionView Resources { get; private set; }

        /// <summary>
        /// Applies the selected resource.
        /// </summary>
        public void ApplySelectedResource()
        {
            var resourceItem = Resources.CurrentItem as ResourceItem;
            var fe = _propertyItem.Instance as FrameworkElement;
            var dpd = DependencyPropertyDescriptor.FromProperty(_propertyItem.Property);
            if (fe != null && resourceItem != null && dpd != null)
            {
                fe.SetResourceReference(dpd.DependencyProperty, resourceItem.Name);
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Private Helpers

        private bool FilterResources(object item)
        {
            
            var resourceItem = item as ResourceItem;
            if (resourceItem != null && resourceItem.Value != null &&
                !_propertyItem.Property.PropertyType.IsAssignableFrom(resourceItem.Value.GetType()))
            {
                return false;
            }
            
            if( string.IsNullOrEmpty(_filter) || resourceItem == null )
            {
                return true;
            }

            return resourceItem.Name.ToLower().Contains(_filter.ToLower());
        }

        #endregion
    }
}
