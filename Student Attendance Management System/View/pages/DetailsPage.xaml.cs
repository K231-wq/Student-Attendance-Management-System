using Student_Attendance_Management_System.ViewModel;

namespace Student_Attendance_Management_System.View.pages;

public partial class DetailsPage : ContentPage
{
	private DetailsStudentViewModel ViewModel => BindingContext as DetailsStudentViewModel;
	public DetailsPage(DetailsStudentViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
    protected override void OnAppearing()
    {
        base.OnAppearing();
        Task.Run(async () =>
        {
            if (ViewModel != null && ViewModel.StudentRecords.Count == 0)
            {
                await ViewModel.LoadStudentRecords();
            }
        });
    }
}