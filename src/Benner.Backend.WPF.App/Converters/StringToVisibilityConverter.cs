﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Benner.Backend.WPF.App.Converters
{
    public class StringToVisibilityConverter : IValueConverter
    {
        public static StringToVisibilityConverter Instance { get; } = new StringToVisibilityConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrWhiteSpace(value?.ToString()) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}