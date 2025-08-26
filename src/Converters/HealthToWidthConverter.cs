using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace EyeLaserGame.Converters
{
    public class HealthToWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int health)
            {
                // 假设最大血量为100，血条最大宽度为150
                double maxWidth = 150;
                double percentage = Math.Max(0, Math.Min(100, health)) / 100.0;
                return percentage * maxWidth;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}