using System;
using System.Globalization;
using System.Windows.Data;

namespace EyeLaserGame.Converters
{
    public class HealthToStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int health)
            {
                // 根据血量返回状态字符串
                if (health > 70)
                    return "High";
                else if (health > 30)
                    return "Medium";
                else
                    return "Low";
            }
            return "High";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}