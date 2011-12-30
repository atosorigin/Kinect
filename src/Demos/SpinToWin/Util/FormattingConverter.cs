using System;
using System.Globalization;
using System.Windows.Data;

namespace Kinect.SpinToWin.Util
{
    /// <summary>
    /// A value converter that delegates to String.Format
    /// </summary>
    [ValueConversion(typeof(object), typeof(string))]
    public class FormattingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            var formatString = parameter as string;
            return formatString != null ? string.Format(culture, formatString, value) : value.ToString();
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
