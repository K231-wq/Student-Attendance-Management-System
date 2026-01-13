
using System.Globalization;


namespace Student_Attendance_Management_System.Helpers
{
    public class StatusToColorConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = value as string;
            if (string.IsNullOrEmpty(status)) return Colors.Gray;

            return status.ToLower() switch
            {
                "present" => Color.FromArgb("#27ae60"), // Professional Green
                "absent" => Color.FromArgb("#e74c3c"),  // Professional Red
                _ => Colors.Gray
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
