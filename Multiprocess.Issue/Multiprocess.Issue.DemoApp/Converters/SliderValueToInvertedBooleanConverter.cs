using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multiprocess.Issue.DemoApp.Converters
{
    using System.Windows.Data;

    class SliderValueToInvertedBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return false;
            var sliderValue = 0;
            Int32.TryParse(value.ToString(), out sliderValue);

            return sliderValue == 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return false;
        }
    }
}
