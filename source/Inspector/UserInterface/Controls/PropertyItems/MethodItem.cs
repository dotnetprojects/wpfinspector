using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Input;
using ChristianMoser.WpfInspector.Utilities;

namespace ChristianMoser.WpfInspector.UserInterface.Controls.PropertyItems
{
    public class MethodItem : IPropertyGridItem
    {
        private readonly MethodInfo _methodInfo;
        private readonly object _instance;
        private bool _isSelected;

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodItem"/> class.
        /// </summary>
        public MethodItem(MethodInfo methodInfo, object instance)
        {
            _methodInfo = methodInfo;
            _instance = instance;
            InvokeCommand = new Command<object>(o =>Invoke());
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand InvokeCommand { get; private set; }

        public string Name
        {
            get { return _methodInfo.Name; }
        }

        /// <summary>
        /// Gets or sets if the property is selected
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set { PropertyChanged.ChangeAndNotify(ref _isSelected, value, this, "IsSelected"); }
        }

        private void Invoke()
        {
            var arguments = new object[] {};
            _methodInfo.Invoke(_instance, arguments);
        }

    }
}
