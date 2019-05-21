using System;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;

public abstract class ValueConverterBase<T, U> : System.Windows.Data.IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        switch (value)
        {
            case T t_val:
                return Convert(t_val, targetType, parameter, culture);
            case IEnumerable<T> t_arr:
                return t_arr.Select(t => Convert(t, targetType, parameter, culture));
            default:
                return null;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        switch (value)
        {
            case U u_val:
                return ConvertBack(u_val, targetType, parameter, culture);
            case IEnumerable<U> u_arr:
                return u_arr.Select(u => ConvertBack(u, targetType, parameter, culture));
            default:
                return null;
        }
    }

    public abstract U Convert(T value, Type targetType, object parameter, CultureInfo culture);
    public abstract T ConvertBack(U value, Type targetType, object parameter, CultureInfo culture);
}
