using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using ChristianMoser.WpfInspector.Utilities;

namespace ChristianMoser.WpfInspector.Services.Triggers
{
    public class ConditionItem : IDisposable, INotifyPropertyChanged
    {

        #region Private Members

        private readonly object _instance;
        private readonly BindingBase _binding;
        private readonly object _value;
        private int _index;
        private DispatcherTimer _timer;
        private DependencyPropertyDescriptor _dpd;

        #endregion

        /// <summary>
        /// Occurs when the state of the condition changed.
        /// </summary>
        public event EventHandler StateChanged;

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionItem"/> class.
        /// </summary>
        public ConditionItem(DependencyProperty property, object instance, object value)
        {
            _instance = instance;
            _value = value;

            HookProperty(property);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionItem"/> class.
        /// </summary>
        public ConditionItem(BindingBase binding, object instance, object value)
        {
            _binding = binding;
            _instance = instance;
            _value = value;

            HookBinding();
        }

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty BindingTarget1Property = DependencyProperty.RegisterAttached("BindingTarget1", typeof(object), typeof(FrameworkElement), new FrameworkPropertyMetadata(null));
        public static object GetBindingTarget1(FrameworkElement element) { return element.GetValue(BindingTarget1Property); }
        public static void SetBindingTarget1(FrameworkElement element, object value) { element.SetValue(BindingTarget1Property, value); }

        public static readonly DependencyProperty BindingTarget2Property = DependencyProperty.RegisterAttached("BindingTarget2", typeof(object), typeof(FrameworkElement), new FrameworkPropertyMetadata(null));
        public static object GetBindingTarget2(FrameworkElement element) { return element.GetValue(BindingTarget2Property); }
        public static void SetBindingTarget2(FrameworkElement element, object value) { element.SetValue(BindingTarget2Property, value); }

        public static readonly DependencyProperty BindingTarget3Property = DependencyProperty.RegisterAttached("BindingTarget3", typeof(object), typeof(FrameworkElement), new FrameworkPropertyMetadata(null));
        public static object GetBindingTarget3(FrameworkElement element) { return element.GetValue(BindingTarget3Property); }
        public static void SetBindingTarget3(FrameworkElement element, object value) { element.SetValue(BindingTarget3Property, value); }

        public static readonly DependencyProperty BindingTarget4Property = DependencyProperty.RegisterAttached("BindingTarget4", typeof(object), typeof(FrameworkElement), new FrameworkPropertyMetadata(null));
        public static object GetBindingTarget4(FrameworkElement element) { return element.GetValue(BindingTarget4Property); }
        public static void SetBindingTarget4(FrameworkElement element, object value) { element.SetValue(BindingTarget4Property, value); }

        public static readonly DependencyProperty BindingTarget5Property = DependencyProperty.RegisterAttached("BindingTarget5", typeof(object), typeof(FrameworkElement), new FrameworkPropertyMetadata(null));
        public static object GetBindingTarget5(FrameworkElement element) { return element.GetValue(BindingTarget5Property); }
        public static void SetBindingTarget5(FrameworkElement element, object value) { element.SetValue(BindingTarget5Property, value); }

        public static readonly DependencyProperty BindingTarget6Property = DependencyProperty.RegisterAttached("BindingTarget6", typeof(object), typeof(FrameworkElement), new FrameworkPropertyMetadata(null));
        public static object GetBindingTarget6(FrameworkElement element) { return element.GetValue(BindingTarget6Property); }
        public static void SetBindingTarget6(FrameworkElement element, object value) { element.SetValue(BindingTarget6Property, value); }

        public static readonly DependencyProperty BindingTarget7Property = DependencyProperty.RegisterAttached("BindingTarget7", typeof(object), typeof(FrameworkElement), new FrameworkPropertyMetadata(null));
        public static object GetBindingTarget7(FrameworkElement element) { return element.GetValue(BindingTarget7Property); }
        public static void SetBindingTarget7(FrameworkElement element, object value) { element.SetValue(BindingTarget7Property, value); }

        public static readonly DependencyProperty BindingTarget8Property = DependencyProperty.RegisterAttached("BindingTarget8", typeof(object), typeof(FrameworkElement), new FrameworkPropertyMetadata(null));
        public static object GetBindingTarget8(FrameworkElement element) { return element.GetValue(BindingTarget8Property); }
        public static void SetBindingTarget8(FrameworkElement element, object value) { element.SetValue(BindingTarget8Property, value); }

        public static readonly DependencyProperty BindingTarget9Property = DependencyProperty.RegisterAttached("BindingTarget9", typeof(object), typeof(FrameworkElement), new FrameworkPropertyMetadata(null));
        public static object GetBindingTarget9(FrameworkElement element) { return element.GetValue(BindingTarget9Property); }
        public static void SetBindingTarget9(FrameworkElement element, object value) { element.SetValue(BindingTarget9Property, value); }

        public static readonly DependencyProperty BindingTarget10Property = DependencyProperty.RegisterAttached("BindingTarget10", typeof(object), typeof(FrameworkElement), new FrameworkPropertyMetadata(null));
        public static object GetBindingTarget10(FrameworkElement element) { return element.GetValue(BindingTarget10Property); }
        public static void SetBindingTarget10(FrameworkElement element, object value) { element.SetValue(BindingTarget10Property, value); }

        public static readonly DependencyProperty BindingTarget11Property = DependencyProperty.RegisterAttached("BindingTarget11", typeof(object), typeof(FrameworkElement), new FrameworkPropertyMetadata(null));
        public static object GetBindingTarget11(FrameworkElement element) { return element.GetValue(BindingTarget11Property); }
        public static void SetBindingTarget11(FrameworkElement element, object value) { element.SetValue(BindingTarget11Property, value); }

        public static readonly DependencyProperty BindingTarget12Property = DependencyProperty.RegisterAttached("BindingTarget12", typeof(object), typeof(FrameworkElement), new FrameworkPropertyMetadata(null));
        public static object GetBindingTarget12(FrameworkElement element) { return element.GetValue(BindingTarget12Property); }
        public static void SetBindingTarget12(FrameworkElement element, object value) { element.SetValue(BindingTarget12Property, value); }

        public static readonly DependencyProperty BindingTarget13Property = DependencyProperty.RegisterAttached("BindingTarget13", typeof(object), typeof(FrameworkElement), new FrameworkPropertyMetadata(null));
        public static object GetBindingTarget13(FrameworkElement element) { return element.GetValue(BindingTarget13Property); }
        public static void SetBindingTarget13(FrameworkElement element, object value) { element.SetValue(BindingTarget13Property, value); }

        public static readonly DependencyProperty BindingTarget14Property = DependencyProperty.RegisterAttached("BindingTarget14", typeof(object), typeof(FrameworkElement), new FrameworkPropertyMetadata(null));
        public static object GetBindingTarget14(FrameworkElement element) { return element.GetValue(BindingTarget14Property); }
        public static void SetBindingTarget14(FrameworkElement element, object value) { element.SetValue(BindingTarget14Property, value); }

        public static readonly DependencyProperty BindingTarget15Property = DependencyProperty.RegisterAttached("BindingTarget15", typeof(object), typeof(FrameworkElement), new FrameworkPropertyMetadata(null));
        public static object GetBindingTarget15(FrameworkElement element) { return element.GetValue(BindingTarget15Property); }
        public static void SetBindingTarget15(FrameworkElement element, object value) { element.SetValue(BindingTarget15Property, value); }

        public static readonly DependencyProperty BindingTarget16Property = DependencyProperty.RegisterAttached("BindingTarget16", typeof(object), typeof(FrameworkElement), new FrameworkPropertyMetadata(null));
        public static object GetBindingTarget16(FrameworkElement element) { return element.GetValue(BindingTarget16Property); }
        public static void SetBindingTarget16(FrameworkElement element, object value) { element.SetValue(BindingTarget16Property, value); }

        public static readonly DependencyProperty BindingTarget17Property = DependencyProperty.RegisterAttached("BindingTarget17", typeof(object), typeof(FrameworkElement), new FrameworkPropertyMetadata(null));
        public static object GetBindingTarget17(FrameworkElement element) { return element.GetValue(BindingTarget17Property); }
        public static void SetBindingTarget17(FrameworkElement element, object value) { element.SetValue(BindingTarget17Property, value); }

        public static readonly DependencyProperty BindingTarget18Property = DependencyProperty.RegisterAttached("BindingTarget18", typeof(object), typeof(FrameworkElement), new FrameworkPropertyMetadata(null));
        public static object GetBindingTarget18(FrameworkElement element) { return element.GetValue(BindingTarget18Property); }
        public static void SetBindingTarget18(FrameworkElement element, object value) { element.SetValue(BindingTarget18Property, value); }

        public static readonly DependencyProperty BindingTarget19Property = DependencyProperty.RegisterAttached("BindingTarget19", typeof(object), typeof(FrameworkElement), new FrameworkPropertyMetadata(null));
        public static object GetBindingTarget19(FrameworkElement element) { return element.GetValue(BindingTarget19Property); }
        public static void SetBindingTarget19(FrameworkElement element, object value) { element.SetValue(BindingTarget19Property, value); }

        public static readonly DependencyProperty BindingTarget20Property = DependencyProperty.RegisterAttached("BindingTarget20", typeof(object), typeof(FrameworkElement), new FrameworkPropertyMetadata(null));
        public static object GetBindingTarget20(FrameworkElement element) { return element.GetValue(BindingTarget20Property); }
        public static void SetBindingTarget20(FrameworkElement element, object value) { element.SetValue(BindingTarget20Property, value); }

        public static readonly DependencyProperty BindingTarget21Property = DependencyProperty.RegisterAttached("BindingTarget21", typeof(object), typeof(FrameworkElement), new FrameworkPropertyMetadata(null));
        public static object GetBindingTarget21(FrameworkElement element) { return element.GetValue(BindingTarget21Property); }
        public static void SetBindingTarget21(FrameworkElement element, object value) { element.SetValue(BindingTarget21Property, value); }

        public static readonly DependencyProperty BindingTarget22Property = DependencyProperty.RegisterAttached("BindingTarget22", typeof(object), typeof(FrameworkElement), new FrameworkPropertyMetadata(null));
        public static object GetBindingTarget22(FrameworkElement element) { return element.GetValue(BindingTarget22Property); }
        public static void SetBindingTarget22(FrameworkElement element, object value) { element.SetValue(BindingTarget22Property, value); }

        public static readonly DependencyProperty BindingTarget23Property = DependencyProperty.RegisterAttached("BindingTarget23", typeof(object), typeof(FrameworkElement), new FrameworkPropertyMetadata(null));
        public static object GetBindingTarget23(FrameworkElement element) { return element.GetValue(BindingTarget23Property); }
        public static void SetBindingTarget23(FrameworkElement element, object value) { element.SetValue(BindingTarget23Property, value); }

        public static readonly DependencyProperty BindingTarget24Property = DependencyProperty.RegisterAttached("BindingTarget24", typeof(object), typeof(FrameworkElement), new FrameworkPropertyMetadata(null));
        public static object GetBindingTarget24(FrameworkElement element) { return element.GetValue(BindingTarget24Property); }
        public static void SetBindingTarget24(FrameworkElement element, object value) { element.SetValue(BindingTarget24Property, value); }

        public static readonly DependencyProperty BindingTarget25Property = DependencyProperty.RegisterAttached("BindingTarget25", typeof(object), typeof(FrameworkElement), new FrameworkPropertyMetadata(null));
        public static object GetBindingTarget25(FrameworkElement element) { return element.GetValue(BindingTarget25Property); }
        public static void SetBindingTarget25(FrameworkElement element, object value) { element.SetValue(BindingTarget25Property, value); }

        public static readonly DependencyProperty BindingTarget26Property = DependencyProperty.RegisterAttached("BindingTarget26", typeof(object), typeof(FrameworkElement), new FrameworkPropertyMetadata(null));
        public static object GetBindingTarget26(FrameworkElement element) { return element.GetValue(BindingTarget26Property); }
        public static void SetBindingTarget26(FrameworkElement element, object value) { element.SetValue(BindingTarget26Property, value); }

        public static readonly DependencyProperty BindingTarget27Property = DependencyProperty.RegisterAttached("BindingTarget27", typeof(object), typeof(FrameworkElement), new FrameworkPropertyMetadata(null));
        public static object GetBindingTarget27(FrameworkElement element) { return element.GetValue(BindingTarget27Property); }
        public static void SetBindingTarget27(FrameworkElement element, object value) { element.SetValue(BindingTarget27Property, value); }

        public static readonly DependencyProperty BindingTarget28Property = DependencyProperty.RegisterAttached("BindingTarget28", typeof(object), typeof(FrameworkElement), new FrameworkPropertyMetadata(null));
        public static object GetBindingTarget28(FrameworkElement element) { return element.GetValue(BindingTarget28Property); }
        public static void SetBindingTarget28(FrameworkElement element, object value) { element.SetValue(BindingTarget28Property, value); }

        public static readonly DependencyProperty BindingTarget29Property = DependencyProperty.RegisterAttached("BindingTarget29", typeof(object), typeof(FrameworkElement), new FrameworkPropertyMetadata(null));
        public static object GetBindingTarget29(FrameworkElement element) { return element.GetValue(BindingTarget29Property); }
        public static void SetBindingTarget29(FrameworkElement element, object value) { element.SetValue(BindingTarget29Property, value); }

        public static readonly DependencyProperty BindingTarget30Property = DependencyProperty.RegisterAttached("BindingTarget30", typeof(object), typeof(FrameworkElement), new FrameworkPropertyMetadata(null));
        public static object GetBindingTarget30(FrameworkElement element) { return element.GetValue(BindingTarget30Property); }
        public static void SetBindingTarget30(FrameworkElement element, object value) { element.SetValue(BindingTarget30Property, value); }

        public static readonly DependencyProperty BindingTarget31Property = DependencyProperty.RegisterAttached("BindingTarget31", typeof(object), typeof(FrameworkElement), new FrameworkPropertyMetadata(null));
        public static object GetBindingTarget31(FrameworkElement element) { return element.GetValue(BindingTarget31Property); }
        public static void SetBindingTarget31(FrameworkElement element, object value) { element.SetValue(BindingTarget31Property, value); }

        public static readonly DependencyProperty BindingTarget32Property = DependencyProperty.RegisterAttached("BindingTarget32", typeof(object), typeof(FrameworkElement), new FrameworkPropertyMetadata(null));
        public static object GetBindingTarget32(FrameworkElement element) { return element.GetValue(BindingTarget32Property); }
        public static void SetBindingTarget32(FrameworkElement element, object value) { element.SetValue(BindingTarget32Property, value); }

        #endregion

        /// <summary>
        /// Gets if the condition is currently active
        /// </summary>
        public bool IsActive
        {
            get
            {
                object currentValue = _binding != null ? ((DependencyObject)_instance).GetValue(GetPropertyFromIndex(_index)) : _dpd.GetValue(_instance);

                if (currentValue != null && _value != null && currentValue.GetType() != _value.GetType())
                {
                    var converter = TypeDescriptor.GetConverter(currentValue.GetType());
                    if (converter.CanConvertFrom(_value.GetType()))
                    {
                        try
                        {
                            return currentValue.Equals(converter.ConvertFrom(_value));
                        }
                        catch(Exception)
                        {
                            return false;
                        }
                    }
                }

                if (_value == null )
                {
                    return currentValue == null;
                }

                return _value.Equals(currentValue);
            }
        }

        public string Condition
        {
            get
            {
                var binding = _binding as Binding;
                if (binding != null)
                {
                    return string.Format("{0} == {1}  Value: {2} ", binding.Path.Path, TargetValue, CurrentValue);
                }
                if (_dpd != null)
                {
                    return string.Format("{0} == {1}  Value: {2}", _dpd.DisplayName, TargetValue, CurrentValue);
                }
                return "Error: Cannot evaluate condition.";

            }
        }

        public string CurrentValue
        {
            get
            {
                object value = _binding != null ? ((DependencyObject)_instance).GetValue(GetPropertyFromIndex(_index)) : _dpd.GetValue(_instance);

                if (value == null)
                {
                    return "{x:Null}";
                }
                return value.ToString();
            }
        }

        public string TargetValue
        {
            get
            {
                if (_value == null)
                {
                    return "null";
                }
                return _value.ToString();
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_dpd != null)
            {
                UnHookProperty();
            }
            if (_binding != null)
            {
                UnHookBining();
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion


        #region Private Helpers

        private void HookProperty(DependencyProperty property)
        {
            _dpd = DependencyPropertyDescriptor.FromProperty(property, _instance.GetType());
            _dpd.AddValueChanged(_instance, OnPropertyValueChanged);
        }

        private void UnHookProperty()
        {
            _dpd.RemoveValueChanged(_instance, OnPropertyValueChanged);
        }

        private void HookBinding()
        {
            _index = GetNextFreePropertyIndex();
            BindingOperations.SetBinding((DependencyObject)_instance, GetPropertyFromIndex(_index), _binding);

            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            _timer.Tick += OnPropertyValueChanged;
            _timer.Start();
        }

        private void UnHookBining()
        {
            ((DependencyObject)_instance).ClearValue(GetPropertyFromIndex(_index));
            _timer.Stop();
        }

        private void OnPropertyValueChanged(object sender, EventArgs e)
        {
            NotifyStateChanged();
            PropertyChanged.Notify(this, "IsActive");
            PropertyChanged.Notify(this, "CurrentValue");
            PropertyChanged.Notify(this, "Condition");
        }

        private void NotifyStateChanged()
        {
            if (StateChanged != null)
            {
                StateChanged(this, EventArgs.Empty);
            }
        }

        private int GetNextFreePropertyIndex()
        {
            for( int i = 0 ; i < 16; i++)
            {
                var dependencyProperty = GetPropertyFromIndex(i);
                var localValue = ((DependencyObject) _instance).ReadLocalValue(dependencyProperty);
                if( localValue == DependencyProperty.UnsetValue)
                {
                    return i;
                }
            }
            return -1;
        }

        private static DependencyProperty GetPropertyFromIndex( int index)
        {
            switch( index)
            {
                case 0: return BindingTarget1Property;
                case 1: return BindingTarget2Property;
                case 2: return BindingTarget3Property;
                case 3: return BindingTarget4Property;
                case 4: return BindingTarget5Property;
                case 5: return BindingTarget6Property;
                case 6: return BindingTarget7Property;
                case 7: return BindingTarget8Property;
                case 8: return BindingTarget9Property;
                case 9: return BindingTarget10Property;
                case 10: return BindingTarget11Property;
                case 11: return BindingTarget12Property;
                case 12: return BindingTarget13Property;
                case 13: return BindingTarget14Property;
                case 14: return BindingTarget15Property;
                case 15: return BindingTarget16Property;
                case 16: return BindingTarget17Property;
                case 17: return BindingTarget18Property;
                case 18: return BindingTarget19Property;
                case 19: return BindingTarget20Property;
                case 20: return BindingTarget21Property;
                case 21: return BindingTarget22Property;
                case 22: return BindingTarget23Property;
                case 23: return BindingTarget24Property;
                case 24: return BindingTarget25Property;
                case 25: return BindingTarget26Property;
                case 26: return BindingTarget27Property;
                case 27: return BindingTarget28Property;
                case 28: return BindingTarget29Property;
                case 29: return BindingTarget30Property;
                case 30: return BindingTarget31Property;
                default:
                    return BindingTarget32Property;
                //default:
                //    throw new IndexOutOfRangeException("Trigger is too complex to inspect!");
            }
        }

        #endregion


    }

} 
