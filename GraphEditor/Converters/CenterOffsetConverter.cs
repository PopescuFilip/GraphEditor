using System.Globalization;
using System.Windows.Data;

namespace GraphEditor.Converters;

public class CenterOffsetConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        var position = (double)values[0];
        var size = (double)values[1];

        return position - size / 2;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}