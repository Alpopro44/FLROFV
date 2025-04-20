using System;
using System.Globalization;
using System.Windows.Data;
using TodoListApp.Models;

namespace TodoListApp.Converters
{
    public class EditModeToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Todo todo)
            {
                return todo == null ? "Yeni Görev" : "Görevi Düzenle";
            }
            return "Yeni Görev";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 