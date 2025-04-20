using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TodoListApp.Converters
{
    public class DurumRenkConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string durum)
            {
                return durum.ToLower() switch
                {
                    "beklemede" => new SolidColorBrush(Color.FromRgb(33, 150, 243)),  // Mavi
                    "tamamlandı" => new SolidColorBrush(Color.FromRgb(76, 175, 80)),  // Yeşil
                    _ => new SolidColorBrush(Color.FromRgb(158, 158, 158))  // Gri (varsayılan)
                };
            }
            return new SolidColorBrush(Color.FromRgb(158, 158, 158));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 