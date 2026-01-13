
using System.Globalization;
namespace Student_Attendance_Management_System.Helpers
{
    public class ThresholdConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return false;
            if (double.TryParse(value.ToString(), out double rate))
            {
                return rate >= 75.0;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
