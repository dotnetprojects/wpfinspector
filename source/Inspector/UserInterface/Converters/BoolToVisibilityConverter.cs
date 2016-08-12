using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace ChristianMoser.WpfInspector.UserInterface.Converters
{
    public class BoolToVisibilityConverter : ValueConverterBase , IValueConverter
    {
        public BoolToVisibilityConverter()
        {
            
        }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool invert = false;

            if (parameter is string)
            {
                bool.TryParse((string)parameter, out invert);
            }

            if( value is bool)
            {
                return ((bool)value) ^ invert ? Visibility.Visible : Visibility.Collapsed;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
