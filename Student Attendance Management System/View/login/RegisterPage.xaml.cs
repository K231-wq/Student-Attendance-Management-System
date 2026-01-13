using Student_Attendance_Management_System.ViewModel;

namespace Student_Attendance_Management_System.View.login;

public partial class RegisterPage : ContentPage
{
	public RegisterPage(RegisterViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}