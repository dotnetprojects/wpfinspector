using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Collections.Generic;

namespace ChristianMoser.WpfInspector.UserInterface.Controls.PropertyItems
{
    public class GridLengthPropertyItem : PropertyItem
    {
        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="GridLengthPropertyItem"/> class.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="instance"></param>
        public GridLengthPropertyItem(PropertyDescriptor property, object instance)
            : base(property, instance)
        {
            var unitTypes = new List<GridUnitType>{ GridUnitType.Auto, GridUnitType.Pixel, GridUnitType.Star };
            UnitTypes = new ListCollectionView(unitTypes);
            UnitTypes.MoveCurrentTo(GetValue().GridUnitType);
            UnitTypes.CollectionChanged += (s, e) => UpdateGridLength(Units);
        }

        #endregion

        /// <summary>
        /// Gets or sets the unit.
        /// </summary>
        public ICollectionView UnitTypes { get; private set; }

        /// <summary>
        /// Gets or sets the units.
        /// </summary>
        public double Units
        {
            get { return GetValue().Value; }
            set
            {
                UpdateGridLength(value);
                NotifyPropertyChanged("Units");
            }
        }

        #region Private Members

        protected override void OnValueChanged(object sender, EventArgs e)
        {
            UnitTypes.MoveCurrentTo(GetValue().GridUnitType);
            NotifyPropertyChanged("Units");
            base.OnValueChanged(sender, e);
        }

        private GridLength GetValue()
        {
            return (GridLength)Property.GetValue(Instance);
        }

        private void UpdateGridLength(double units)
        {
            Property.SetValue(Instance, new GridLength(units, (GridUnitType)UnitTypes.CurrentItem));
        }

        #endregion
    }
}