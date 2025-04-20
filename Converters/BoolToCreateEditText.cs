using System;
using System.Globalization;
using System.Windows.Data;

namespace TodoListApp.Converters
{
    public class BoolToCreateEditText : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isNewMode)
            {
                return isNewMode ? "Yeni Görev Oluştur" : "Görevi Düzenle";
            }
            return "Görevi Düzenle";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 