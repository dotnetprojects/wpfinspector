using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;
using System.Linq;

namespace ChristianMoser.WpfInspector.UserInterface.Controls.PropertyItems
{
    public class CompositePropertyItem : PropertyItem
    {
        #region Private Members

        private readonly List<IPropertyGridItem> _propertyItems = new List<IPropertyGridItem>();
        private ICollectionView _properties;
        
        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositePropertyItem"/> class.
        /// </summary>
        public CompositePropertyItem(PropertyDescriptor property, object instance)
            : base(property, instance )
        {
            HasChildren = true;
        }

        #endregion

        public string TypeName
        {
            get
            {
                var value = Property.GetValue(Instance);
                if( value != null)
                {
                    var type = value.GetType();
                    if (type.IsPrimitive)
                    {
                        return value.ToString();
                    }
                    else
                    {
                        return type.Name;
                    }
                }
                return null;
            }
        }

        public ICollectionView Properties
        {
            get
            {
                if( _properties == null)
                {
                    _properties = new ListCollectionView(PropertyItems);
                    BuildPropertyItemList();
                }
                return _properties;
            }
        }

        public override object Value
        {
            get
            {
                var value = Property.GetValue(Instance);
                if (value != null)
                {
                    return value.ToString();
                }
                return null;
            }
            set
            {
                base.Value = value;
            }
        }

        protected List<IPropertyGridItem> PropertyItems
        {
            get { return _propertyItems; }
        }

        /// <summary>
        /// Gets if the property has children
        /// </summary>
        public virtual bool HasChildren
        {
            get; protected set;
        }

        #region Private Helpers


        protected override void OnValueChanged(object sender, EventArgs e)
        {
            if (_properties != null)
            {
                BuildPropertyItemList();
            }
            
            NotifyPropertyChanged("TypeName");
            base.OnValueChanged(sender,e);
        }

        protected virtual void BuildPropertyItemList()
        {
            foreach (var propertyItem in PropertyItems)
            {
                propertyItem.Dispose();
            }

            PropertyItems.Clear();
            PropertyItems.AddRange(PropertyGridItemFactory.GetPropertyItems(Property.GetValue(Instance)));

            Properties.Refresh();
        }

        #endregion
    }
}
