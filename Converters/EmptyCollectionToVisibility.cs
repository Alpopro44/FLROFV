using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TodoListApp.Converters
{
    public class EmptyCollectionToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool invert = parameter != null && parameter.ToString() == "invert";
            
            if (value == null)
                return invert ? Visibility.Collapsed : Visibility.Visible;
                
            if (value is ICollection collection)
            {
                bool isEmpty = collection.Count == 0;
                return (isEmpty ^ invert) ? Visibility.Visible : Visibility.Collapsed;
            }
            
            if (value is int count)
            {
                bool isEmpty = count == 0;
                return (isEmpty ^ invert) ? Visibility.Visible : Visibility.Collapsed;
            }
            
            return invert ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 