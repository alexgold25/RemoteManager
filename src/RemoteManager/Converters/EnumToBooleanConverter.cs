using System;
using System.Globalization;
using System.Windows.Data;

namespace RemoteManager.Converters;

public class EnumToBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null || parameter == null) return false;
        var enumValue = value.ToString();
        var targetValue = parameter.ToString();
        return enumValue != null && enumValue.Equals(targetValue);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool b && b && parameter is string str)
        {
            return Enum.Parse(targetType, str);
        }
        return Binding.DoNothing;
    }
}
