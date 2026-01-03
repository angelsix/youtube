using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace BatchProcess3.ValueConverters;

public class BooleanToBoldFontConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value is bool and true ? Application.Current?.FindResource("NotoSansJP-Bold") : Application.Current?.FindResource("NotoSansJP-Regular") ;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
}