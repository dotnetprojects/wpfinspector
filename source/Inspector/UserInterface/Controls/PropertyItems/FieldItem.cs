using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using ChristianMoser.WpfInspector.Utilities;

namespace ChristianMoser.WpfInspector.UserInterface.Controls.PropertyItems
{
    public class FieldItem : IPropertyGridItem
    {
        private readonly FieldInfo _fieldInfo;
        private readonly object _instance;
        private bool _isSelected;

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldItem"/> class.
        /// </summary>
        public FieldItem(FieldInfo fieldInfo, object instance)
        {
            _fieldInfo = fieldInfo;
            _instance = instance;
        }

        #endregion
        public void Dispose()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public object Value
        {
            get { return _fieldInfo.GetValue(_instance); }
            set { _fieldInfo.SetValue(_instance, value);}
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name
        {
            get { return _fieldInfo.Name; }
        }

        /// <summary>
        /// Gets or sets if the property is selected
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set { PropertyChanged.ChangeAndNotify(ref _isSelected, value, this, "IsSelected"); }
        }

    }
}
