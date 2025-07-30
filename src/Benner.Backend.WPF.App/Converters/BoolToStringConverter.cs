using System;
using System.Globalization;
using System.Windows.Data;

namespace Benner.Backend.WPF.App.Converters
{
    public class BoolToStringConverter : IValueConverter
    {
        public static BoolToStringConverter Instance { get; } = new BoolToStringConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue && parameter is string parameterString)
            {
                var options = parameterString.Split('|');
                if (options.Length == 2)
                {
                    return boolValue ? options[0] : options[1];
                }
            }

            return value?.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}