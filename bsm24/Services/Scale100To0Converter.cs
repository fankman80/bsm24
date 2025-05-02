#nullable disable

using System.Globalization;

namespace bsm24.Services;

public class Scale100To0Converter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double d100)
        {
            // skaliere auf 0…1
            return ((int)(d100 * 100)).ToString();
        }
        return "0";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException("One-way converter only.");
    }
}