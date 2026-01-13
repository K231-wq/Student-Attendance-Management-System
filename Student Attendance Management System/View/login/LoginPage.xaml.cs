using Student_Attendance_Management_System.ViewModel;

namespace Student_Attendance_Management_System.View.login;

public partial class LoginPage : ContentPage
{
	public LoginPage(LoginViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}