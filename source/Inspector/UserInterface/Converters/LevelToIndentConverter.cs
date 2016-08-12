using System;
using System.Windows.Data;
using System.Globalization;
using System.Windows;

namespace ChristianMoser.WpfInspector.UserInterface.Converters
{
    /// <summary>
    /// Converts the indention level to a left-margin value
    /// </summary>
    public class LevelToMarginConverter : ValueConverterBase, IValueConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LevelToMarginConverter"/> class.
        /// </summary>
        public LevelToMarginConverter()
        {
        }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
            {
                return new Thickness(((int)value*16), 0, 0, 0);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
