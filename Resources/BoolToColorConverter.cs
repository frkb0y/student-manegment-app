// plz_fix/Resources/BoolToColorConverter.cs
using System.Globalization;

namespace plz_fix.Resources
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? "#4CAF50" : "#F44336";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}