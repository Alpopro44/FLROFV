using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TodoListApp.Models;

namespace TodoListApp.Views
{
    public class TaskOncelikToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TaskOncelik oncelik)
            {
                return oncelik switch
                {
                    TaskOncelik.Dusuk => "Düşük",
                    TaskOncelik.Orta => "Orta",
                    TaskOncelik.Yuksek => "Yüksek",
                    TaskOncelik.Acil => "Acil",
                    _ => "Belirsiz"
                };
            }
            return "Belirsiz";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TaskDurumToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string kategori)
            {
                return kategori switch
                {
                    "İş" => new SolidColorBrush(Color.FromRgb(33, 150, 243)),      // Mavi
                    "Kişisel" => new SolidColorBrush(Color.FromRgb(156, 39, 176)), // Mor
                    "Alışveriş" => new SolidColorBrush(Color.FromRgb(76, 175, 80)),// Yeşil
                    "Sağlık" => new SolidColorBrush(Color.FromRgb(244, 67, 54)),   // Kırmızı
                    "Diğer" => new SolidColorBrush(Color.FromRgb(158, 158, 158)),  // Gri
                    _ => new SolidColorBrush(Color.FromRgb(158, 158, 158))         // Gri
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