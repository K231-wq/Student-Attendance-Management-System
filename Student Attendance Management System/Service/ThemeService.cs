

namespace Student_Attendance_Management_System.Service
{
    public class ThemeService
    {
        private static string ThemeKey = "AppTheme";
        public static void Initialize()
        {
            if (Preferences.ContainsKey(ThemeKey))
            {
                string theme = Preferences.Get(ThemeKey, "System");
                SetTheme(theme);
            }
        }
        public static void SetTheme(string theme)
        {
            AppTheme appTheme = theme switch
            {
                "Light" => AppTheme.Light,
                "Dark" => AppTheme.Dark,
                _ => AppTheme.Unspecified
            };

            Application.Current.UserAppTheme = appTheme;
            Preferences.Set(ThemeKey, theme);
        }

        public static string GetTheme()
        {
            return Preferences.Get(ThemeKey, "System");
        }
    }
}
