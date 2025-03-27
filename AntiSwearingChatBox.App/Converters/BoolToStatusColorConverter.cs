using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Anti_Swearing_Chat_Box.App.Converters
{
    public class BoolToStatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isOnline)
            {
                return isOnline ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4CAF50")) : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#757575"));
            }
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#757575"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 