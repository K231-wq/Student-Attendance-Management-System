using Student_Attendance_Management_System.ViewModel;

namespace Student_Attendance_Management_System.View.pages;

public partial class EditPage : ContentPage
{
	private EditViewModel ViewModel => BindingContext as EditViewModel;
	public EditPage(EditViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (ViewModel != null && ViewModel?.Student != null)
        {
            await ViewModel.LoadAllMajors();
            await ViewModel.LoadAllAcademicYears();
        }
    }
}