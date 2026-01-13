using Student_Attendance_Management_System.Helpers;
using Student_Attendance_Management_System.View.pages;
using System.Windows.Input;

namespace Student_Attendance_Management_System.ShellList;

public partial class WindowShell : Shell
{
    public ICommand LogoutCommand { get; }
    public WindowShell()
    {
        InitializeComponent();

        LogoutCommand = new Command(OnLogout);
        BindingContext = this;

        Routing.RegisterRoute(nameof(DetailsPage), typeof(DetailsPage));
        Routing.RegisterRoute(nameof(EditPage), typeof(EditPage));
    }
    private async void OnLogout()
    {
        bool confirm = await DisplayAlert("Logout", "Are you sure you want to sign out?", "Yes", "No");
        if (confirm)
        {
            AppStorage.Clear();
            // Reset to Login Page
            if (Application.Current is App app)
            {
                app.GoToLogin();
            }
        }
    }
}