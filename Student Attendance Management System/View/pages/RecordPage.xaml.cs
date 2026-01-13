using Student_Attendance_Management_System.ViewModel;

namespace Student_Attendance_Management_System.View.pages;

public partial class RecordPage : ContentPage
{
	private RecordViewModel ViewModel => BindingContext as RecordViewModel;
	public RecordPage(RecordViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (ViewModel != null && ViewModel.Students.Count == 0)
        {
            await ViewModel.LoadAcademicYears();
            await ViewModel.LoadAllStudents();
        }
    }
}