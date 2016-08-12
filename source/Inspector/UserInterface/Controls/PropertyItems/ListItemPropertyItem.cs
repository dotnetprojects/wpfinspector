using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Data;

namespace ChristianMoser.WpfInspector.UserInterface.Controls.PropertyItems
{
    public class ListItemPropertyItem : IPropertyGridItem
    {
        #region Private Members

        private readonly List<IPropertyGridItem> _propertyItems = new List<IPropertyGridItem>();
        private ICollectionView _properties;
        private bool _isSelected;
        private readonly object _instance;

        #endregion
        

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="ListItemPropertyItem"/> class.
        /// </summary>
        public ListItemPropertyItem(object instance, int index)
        {
            _instance = instance;
        }

        #endregion

        public ICollectionView Properties
        {
            get
            {
                if (_properties == null)
                {
                    _properties = new ListCollectionView(PropertyItems);
                    BuildPropertyItemList();
                }
                return _properties;
            }
        }

        protected List<IPropertyGridItem> PropertyItems
        {
            get { return _propertyItems; }
        }

        #region IPropertyItem Members

        public string Name
        {
            get { return _instance.GetType().Name; }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; }
        }

        #endregion

        protected virtual void BuildPropertyItemList()
        {
            foreach (var propertyItem in PropertyItems)
            {
                propertyItem.Dispose();
            }

            PropertyItems.Clear();
            PropertyItems.AddRange(PropertyGridItemFactory.GetPropertyItems(_instance));

            Properties.Refresh();
        }

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
