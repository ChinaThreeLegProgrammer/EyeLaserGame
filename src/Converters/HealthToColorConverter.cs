using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace EyeLaserGame.Converters
{
    public class HealthToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int health)
            {
                // 根据血量返回不同颜色
                if (health > 70)
                    return Colors.Green;
                else if (health > 30)
                    return Colors.Orange;
                else
                    return Colors.Red;
            }
            return Colors.Green;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
