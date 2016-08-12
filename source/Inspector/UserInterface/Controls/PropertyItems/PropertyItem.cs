using System;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Linq;
using System.Windows.Input;
using System.Windows.Markup.Primitives;
using ChristianMoser.WpfInspector.Utilities;
using System.Windows.Markup;
using System.Text.RegularExpressions;
using System.Windows.Threading;
using System.Diagnostics;
using ChristianMoser.WpfInspector.UserInterface.Helpers;

namespace ChristianMoser.WpfInspector.UserInterface.Controls.PropertyItems
{
    public class PropertyItem : IPropertyGridItem
    {
        #region Private Members

        private readonly PropertySortInfo _propertySortInfo;
        private readonly DependencyPropertyDescriptor _dpd;
        private bool _isSelected;
        private readonly MarkupObject _markupObject;
        private DispatcherTimer _highlightChangedTimer;
        private bool _isChanged;
        private bool _isBreakpoint;
        private bool _isOwnChange;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyItem"/> class.
        /// </summary>
        public PropertyItem(PropertyDescriptor property, object instance)
        {
            Property = property;
            Instance = instance;

            ResetCommand = new Command<object>(ResetValue, CanResetValue);
            EditBindingCommand = new Command<object>(ShowBindingEditor, o => !property.IsReadOnly);
            EditResourceCommand = new Command<object>(ShowResourceEditor, o => !property.IsReadOnly);

            _propertySortInfo = PropertySorter.GetSortInfo(property);
            _markupObject = MarkupWriter.GetMarkupObjectFor(Instance);

            property.AddValueChanged(instance, OnValueChanged);

            _dpd = DependencyPropertyDescriptor.FromProperty(property);
        }

        #endregion

        #region IDisposable Members

        public virtual void Dispose()
        {
            Property.RemoveValueChanged(Instance, OnValueChanged);
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Public Functionality

        public ICommand ResetCommand { get; private set; }
        public ICommand EditBindingCommand { get; private set; }
        public ICommand EditResourceCommand { get; private set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public virtual string Name
        {
            get
            {
                return Property.DisplayName;
            }
        }

        /// <summary>
        /// Gets the category.
        /// </summary>
        /// <value>The category.</value>
        public virtual string Category
        {
            get { return _propertySortInfo.Category; }
        }

        /// <summary>
        /// Gets the property order.
        /// </summary>
        /// <value>The property order.</value>
        public int PropertyOrder
        {
            get { return _propertySortInfo.PropertyOrder; }
        }

        /// <summary>
        /// Gets the category order.
        /// </summary>
        /// <value>The category order.</value>
        public int CategoryOrder
        {
            get { return _propertySortInfo.CategoryOrder; }
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        public virtual string Description
        {
            get { return Property.Description; }
        }

        /// <summary>
        /// Gets if the property is editable
        /// </summary>
        public bool IsEditable
        {
            get { return !Property.IsReadOnly; }
        }

        /// <summary>
        /// Gets or sets if the property is selected
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set { PropertyChanged.ChangeAndNotify(ref _isSelected, value, this, "IsSelected"); }
        }

        /// <summary>
        /// Gets if the property has recently been changed
        /// </summary>
        public bool IsChanged
        {
            get { return _isChanged; }
            private set { PropertyChanged.ChangeAndNotify(ref _isChanged, value, this, "IsChanged"); }
        }

        /// <summary>
        /// Gets if the property has a breakpoint
        /// </summary>
        public bool IsBreakpoint
        {
            get { return _isBreakpoint; }
            set { PropertyChanged.ChangeAndNotify(ref _isBreakpoint, value, this, "IsBreakpoint"); }
        }

        /// <summary>
        /// Gets the value source.
        /// </summary>
        public BaseValueSource ValueSource
        {
            get
            {
                var dependencyObject = Instance as DependencyObject;
                if (_dpd != null && dependencyObject != null)
                {
                    return DependencyPropertyHelper.GetValueSource(dependencyObject, _dpd.DependencyProperty).BaseValueSource;
                }
                return BaseValueSource.Unknown;
            }
        }

        /// <summary>
        /// Gets if the property is data bound
        /// </summary>
        public bool IsDataBound
        {
            get
            {
                var dependencyObject = Instance as DependencyObject;
                if (dependencyObject != null && _dpd != null)
                {
                    return BindingOperations.GetBindingExpressionBase(dependencyObject, _dpd.DependencyProperty) != null;
                }
                return false;
            }
        }

        public bool IsDynamicResource
        {
            get
            {

                var markupProperty = _markupObject.Properties.Where(p => p.Name == Property.Name).FirstOrDefault();
                if (markupProperty != null)
                {
                    try
                    {
                        return markupProperty.Value is DynamicResourceExtension;
                    }
                    catch (Exception)
                    {
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Gets the binding expression.
        /// </summary>
        public string BindingExpression
        {
            get
            {
                var dependencyObject = Instance as DependencyObject;
                if (dependencyObject != null && _dpd != null)
                {
                    var expression = BindingOperations.GetBindingExpressionBase(dependencyObject, _dpd.DependencyProperty);
                    if (expression != null && expression.ParentBindingBase != null)
                    {
                        var binding = expression.ParentBindingBase;
                        if (binding is MultiBinding)
                        {
                            return FormatMultiBinding((MultiBinding)binding);
                        }
                        if (binding is Binding)
                        {
                            return FormatBinding((Binding)binding, false);
                        }
                    }
                }
                return string.Empty;
            }
        }

        private static string FormatMultiBinding(MultiBinding multiBinding)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("<MultiBinding>");
            foreach(Binding binding in multiBinding.Bindings)
            {
                stringBuilder.AppendLine(FormatBinding(binding, true));
            }
            stringBuilder.AppendLine("</MultiBinding>");
            return stringBuilder.ToString();
        }

        private static string FormatBinding(Binding binding, bool useAngleBraces)
        {
            string bindingString = XamlWriter.Save(binding);

            // Convert simple bindings from element syntax to markup expression syntax
            var result = Regex.Replace(bindingString, "xmlns=\"(.)*\"", "");
            result = result.Replace("\"", "");
            if (result.Length > 5 && !useAngleBraces)
            {
                return string.Concat("{", result.Substring(1, result.Length - 4), "}");
            }

            return result;
        }

        /// <summary>
        /// Gets and sets the value.
        /// </summary>
        /// <value>The value.</value>
        public virtual object Value
        {
            get { return Property.GetValue(Instance); }
            set
            {
                _isOwnChange = true;
                try
                {

                    if (value != null && !Property.PropertyType.IsAssignableFrom(value.GetType()))
                    {
                        TypeConverter converter = TypeDescriptor.GetConverter(Property.PropertyType);
                        if (!converter.CanConvertFrom(value.GetType()))
                        {
                            MessageBox.Show(string.Format("The value {0} is not valid for the property {1}", value, Property.DisplayName), "Error setting property value", MessageBoxButton.OK,
                                        MessageBoxImage.Error);
                        }
                        object convertedValue = converter.ConvertFrom(value);
                        Property.SetValue(Instance, convertedValue);
                    }
                    else
                    {
                        Property.SetValue(Instance, value);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error setting property value", MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
                _isOwnChange = false;
            }
        }

        #endregion

        #region Protected Functionality

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <value>The property.</value>
        public PropertyDescriptor Property { get; private set; }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public object Instance { get; private set; }

        protected virtual void OnValueChanged(object sender, EventArgs e)
        {
            UpdateValueProperties();

            if (!_isOwnChange)
            {
                SetIsChanged();

                if (IsBreakpoint)
                {
                    Debugger.Break();
                }
            }

            //var logicalTreeItem = LogicalTreeItem.FindLogicalTreeItem(Instance);
            //if (logicalTreeItem != null)
            //{
            //    logicalTreeItem.Refresh();
            //}

            //var visualTreeItem = VisualTreeItem.FindVisualTreeItem(Instance);
            //if( visualTreeItem != null)
            //{
            //    visualTreeItem.Refresh();
            //}
        }

        private void SetIsChanged()
        {
            if (_highlightChangedTimer == null)
            {
                _highlightChangedTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
                _highlightChangedTimer.Tick += (s, e) =>
                                                   {
                                                       IsChanged = false;
                                                       _highlightChangedTimer.Stop();
                                                   };
            }

            _highlightChangedTimer.Stop();
            _highlightChangedTimer.Start();
            IsChanged = true;
        }

        protected void UpdateValueProperties()
        {
            NotifyPropertyChanged("Value");
            NotifyPropertyChanged("ValueSource");
            NotifyPropertyChanged("IsDataBound");
            NotifyPropertyChanged("IsDynamicResource");
            NotifyPropertyChanged("BindingExpression");
        }

        protected void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        private bool CanResetValue(object parameter)
        {
            bool directResetable = Property.CanResetValue(Instance);

            if (!directResetable && !Property.IsReadOnly && Property.PropertyType.IsClass)
            {
                return true;
            }
            return directResetable;
        }

        private void ResetValue(object obj)
        {
            _isOwnChange = true;
            try
            {
                if (Property.CanResetValue(Instance))
                {
                    Property.ResetValue(Instance);
                }
                else
                {
                    if (_dpd != null)
                    {
                        ((DependencyObject)Instance).ClearValue(_dpd.DependencyProperty);
                    }
                    else
                    {
                        Property.SetValue(Instance, null);
                    }
                }
                OnValueChanged(this, EventArgs.Empty);

            }
            catch (Exception)
            {
            }
            _isOwnChange = false;
        }

        private void ShowBindingEditor(object obj)
        {
            var window = new BindingEditorWindow(this);
            window.ShowDialog();
        }

        private void ShowResourceEditor(object parameter)
        {
            var resourceEditor = new ResourceEditor(this);
            resourceEditor.ShowDialog();
        }

        #endregion

    }
}
