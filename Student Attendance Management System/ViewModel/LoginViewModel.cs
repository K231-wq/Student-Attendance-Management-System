
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Student_Attendance_Management_System.Model.Auth;
using Student_Attendance_Management_System.Service;
using System.Diagnostics;

namespace Student_Attendance_Management_System.ViewModel
{
    public partial class LoginViewModel: ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        bool isBusy;

        public bool IsNotBusy => !isBusy;

        [ObservableProperty]
        private string title;

        [ObservableProperty]
        private string username;
        [ObservableProperty]
        private string email;
        [ObservableProperty]
        private string password;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasError))]
        private string errorMessage;

        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

        public Command GoToRegisterCommand => new Command(async () =>
        {
            await Shell.Current.GoToAsync("//Register");
        });
        public Command GoToLoginCommand => new Command(async () =>
        {
            await Shell.Current.GoToAsync("//Login");
        });

        public Command SignUpCommand { get; }
        public LoginViewModel()
        {
            Title = "Login Page";
        }

        [RelayCommand]
        public async Task TeacherLoginAsync()
        {
            if (IsBusy) return;
            try
            {
                if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
                {
                    ErrorMessage = "Email and Password cannot be empty.";
                    return;
                }

                IsBusy = true;
                var success = await AuthService.teacherLoginAsync(new LoginRequest
                {
                    email = Email,
                    password = Password
                });
                if (success == true)
                {
                    await Task.Delay(2000);
                    if (Application.Current is App app)
                    {
                        app.NavigateMainPage();
                    }
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Teacher Login failed: {ex.Message}");
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
