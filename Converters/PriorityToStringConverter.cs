using System;
using System.Globalization;
using System.Windows.Data;
using TodoListApp.Models;

namespace TodoListApp.Converters
{
    public class PriorityToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Priority priority)
            {
                switch (priority)
                {
                    case Priority.All:
                        return "Hepsi";
                    case Priority.Low:
                        return "Düşük";
                    case Priority.Medium:
                        return "Orta";
                    case Priority.High:
                        return "Yüksek";
                    default:
                        return string.Empty;
                }
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string strValue)
            {
                switch (strValue)
                {
                    case "Hepsi":
                        return Priority.All;
                    case "Düşük":
                        return Priority.Low;
                    case "Orta":
                        return Priority.Medium;
                    case "Yüksek":
                        return Priority.High;
                    default:
                        return Priority.All;
                }
            }
            return Priority.All;
        }
    }
} 