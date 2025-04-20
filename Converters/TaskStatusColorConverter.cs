using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TodoListApp.Converters
{
    public class TaskStatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isCompleted)
            {
                return isCompleted 
                    ? new SolidColorBrush(Color.FromRgb(76, 175, 80))  // Yeşil: #4CAF50
                    : new SolidColorBrush(Color.FromRgb(244, 67, 54)); // Kırmızı: #F44336
            }
            
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 