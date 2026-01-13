using Student_Attendance_Management_System.ViewModel;

namespace Student_Attendance_Management_System.View.pages;

public partial class HomePage : ContentPage
{
    private HomeViewModel ViewModel => BindingContext as HomeViewModel;
    public HomePage(HomeViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (ViewModel != null && ViewModel.Students.Count == 0)
        {
            await ViewModel.LoadAttendanceRecordsForDate();
        }
    }
}