using System;
using System.Globalization;
using Xamarin.Forms;

namespace XamarinTV.Converters
{
    public class VideoSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new Uri($"ms-appx:///{value}");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
