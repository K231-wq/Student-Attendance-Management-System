using Microsoft.Extensions.DependencyInjection;
using Student_Attendance_Management_System.Helpers;
using Student_Attendance_Management_System.ShellList;
using System.Diagnostics;

namespace Student_Attendance_Management_System
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
        protected override async void OnStart()
        {
            base.OnStart();

            var token = await AppStorage.GetTokenAsync();

            if (!string.IsNullOrEmpty(token))
            {
                Debug.WriteLine("TOKEN FOUND");
                if (Application.Current is App app)
                {
                    app.NavigateMainPage();
                }
            }
            else
            {
                await Shell.Current.GoToAsync("//Login");
            }
        }
        public void NavigateMainPage()
        {
            MainPage = new WindowShell();
#if WINDOWS
            var window = Application.Current.Windows[0].Handler.PlatformView as Microsoft.UI.Xaml.Window;
            if (window != null)
                window.ExtendsContentIntoTitleBar = true;
#endif

        }
        public void GoToLogin()
        {
            MainPage = new AppShell();
        }
    }
}