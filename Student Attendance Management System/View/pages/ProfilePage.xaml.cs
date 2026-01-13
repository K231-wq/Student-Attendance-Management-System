using Student_Attendance_Management_System.Helpers;
using Student_Attendance_Management_System.ViewModel;

namespace Student_Attendance_Management_System.View.pages;

public partial class ProfilePage : ContentPage
{
	private UpdateTeacherViewModel ViewModel => BindingContext as UpdateTeacherViewModel;
	public ProfilePage(UpdateTeacherViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (ViewModel != null)
        {
            await ViewModel.InitializeAsync();
        }
    }
}