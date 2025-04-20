using System;
using System.Globalization;
using System.Windows.Data;
using TodoListApp.Models;

namespace TodoListApp.Converters
{
    public class UserRoleToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is UserRole role)
            {
                switch (role)
                {
                    case UserRole.Standard:
                        return "Standart";
                    case UserRole.Admin:
                        return "Yönetici";
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
                    case "Standart":
                        return UserRole.Standard;
                    case "Yönetici":
                        return UserRole.Admin;
                    default:
                        return UserRole.Standard;
                }
            }
            return UserRole.Standard;
        }
    }
} 