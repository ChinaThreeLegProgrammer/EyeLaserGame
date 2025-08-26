using System;
using System.Globalization;
using System.Windows.Data;

namespace EyeLaserGame.Converters
{
    /// <summary>
    /// 位置转换器，用于调整坐标位置
    /// </summary>
    public class PositionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int position && parameter is string offset && int.TryParse(offset, out int offsetValue))
            {
                return position + offsetValue;
            }
            
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}