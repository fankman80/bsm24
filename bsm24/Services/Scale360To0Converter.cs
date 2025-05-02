#nullable disable

using System.Globalization;

namespace bsm24.Services;

public class Scale360To0Converter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double d360)
        {
            // skaliere auf 0…1
            return ((int)(d360 * 360)).ToString();
        }
        return "0";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException("One-way converter only.");
    }
}