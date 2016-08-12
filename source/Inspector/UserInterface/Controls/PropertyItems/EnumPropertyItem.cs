using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Collections;

namespace ChristianMoser.WpfInspector.UserInterface.Controls.PropertyItems
{
    public class EnumPropertyItem : PropertyItem
    {

        #region Construction

        public EnumPropertyItem(PropertyDescriptor property, object instance)
            :base(property, instance)
        {
            EnumValues = new ListCollectionView(Enum.GetValues(property.PropertyType));
            EnumValues.MoveCurrentTo(property.GetValue(instance));
            EnumValues.CurrentChanged += OnEnumValueChanged;
        }

        public EnumPropertyItem(PropertyDescriptor property, object instance, Type enumsType)
            : base(property, instance)
        {
            var properties = enumsType.GetProperties();
            var values = properties.Select(propertyInfo => propertyInfo.GetValue(instance, null)).ToList();

            EnumValues = new ListCollectionView(values);
            EnumValues.MoveCurrentTo(property.GetValue(instance));
            EnumValues.CurrentChanged += OnEnumValueChanged;
        }

        public EnumPropertyItem(PropertyDescriptor property, object instance, IEnumerable values)
            : base(property, instance)
        {
            EnumValues = CollectionViewSource.GetDefaultView(values);
            EnumValues.MoveCurrentTo(property.GetValue(instance));
            EnumValues.CurrentChanged += OnEnumValueChanged;
        }

        private void OnEnumValueChanged(object sender, EventArgs e)
        {
            if (Property.IsReadOnly || EnumValues.CurrentItem == null)
            {
                return;
            }
            try
            {
                if (!Value.Equals(EnumValues.CurrentItem))
                {
                    Property.SetValue(Instance, EnumValues.CurrentItem);
                }
            }
            catch {}
            UpdateValueProperties();
        }

        protected override void OnValueChanged(object sender, EventArgs e)
        {
            EnumValues.MoveCurrentTo(Property.GetValue(Instance));
            base.OnValueChanged(sender, e);
        }

        /// <summary>
        /// Gets or sets the values.
        /// </summary>
        /// <value>The values.</value>
        public ICollectionView EnumValues { get; private set; }

        #endregion
    }
}
