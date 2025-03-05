#nullable disable

using System.Globalization;

namespace bsm24.Services
{
    public class IntToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value == int.Parse(parameter.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? int.Parse(parameter.ToString()) : Binding.DoNothing;
        }
    }
}