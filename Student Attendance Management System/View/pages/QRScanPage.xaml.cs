using Student_Attendance_Management_System.ViewModel;
using System.Threading.Tasks;

namespace Student_Attendance_Management_System.View.pages;

public partial class QRScanPage : ContentPage
{
	private QRViewModel ViewModel => BindingContext as QRViewModel;
	public QRScanPage(QRViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
    protected override async void OnAppearing()
    {
        base.OnAppearing();
		if(ViewModel != null)
		{
			await ViewModel.loadSubjects();
		}
    }
}