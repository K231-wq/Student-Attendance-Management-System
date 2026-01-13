using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Student_Attendance_Management_System.Helpers;
using Student_Attendance_Management_System.Model;
using Student_Attendance_Management_System.Model.Responses;
using Student_Attendance_Management_System.Service;
using Student_Attendance_Management_System.Service.Pages;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Input;

namespace Student_Attendance_Management_System.ViewModel
{
    public partial class QRViewModel: ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<string> _courses = new();

        [ObservableProperty]
        private Dictionary<string, string> subjectDict = new();

        [ObservableProperty]
        private string _selectedCourse;

        [ObservableProperty]
        private string _qrValue;

        [ObservableProperty]
        private bool _isQrVisible;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        bool isBusy;

        public bool IsNotBusy => !isBusy;

        public QRViewModel()
        {
        }

        public async Task loadSubjects()
        {
            Courses.Clear();
            SubjectDict.Clear();

            Teacher teacher = await AppStorage.GetTeacherAsync();
            List<string> subjects = teacher.subjects;

            foreach (string subject in subjects)
            {
                string subjectId = subject.Split(' ')[0];
                bool success = await QRAuthService.GetSubjectAsync(subjectId);

                if (success)
                {
                    Subject s = await AppStorage.GetSubjectAsync();
                    string subjectName = $"{s.code} - {s.name}";

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        Courses.Add(subjectName);
                        SubjectDict.Add(subjectName, subjectId);
                    });

                    AppStorage.ClearSubject();
                }
                else
                {
                    await Shell.Current.DisplayAlert(
                                            "Subject Fetching Error",
                                            "Failed to load subject",
                                            "OK"
                                        );
                }
            }
        }
        [RelayCommand]
        private async Task GenerateQr()
        {
            if (IsBusy) return;

            try
            {
                if (string.IsNullOrEmpty(SelectedCourse))
                    return;
                Teacher teacher = await AppStorage.GetTeacherAsync();
                string teacherId = teacher.id;
                string subjectId = subjectDict[SelectedCourse];

                Debug.WriteLine($"TeacherId: {teacherId} / SubjectId: {subjectId}");

                IsBusy = true;

                var success = await QRAuthService.SentQrTokenAsync(new QrTokenRequest
                {
                    subjectId = subjectId,
                    teacherId = teacherId
                });
                if (success)
                {
                    string qrToken = await AppStorage.GetQrTokenAsync();
                    if (string.IsNullOrEmpty(qrToken))
                    {
                        await Shell.Current.DisplayAlert("QrToken Error", "Qr Token is missing", "OK");
                        Debug.WriteLine("Qr Token is missing");
                    }
                    Debug.WriteLine("Qr Token " + qrToken);

                    var qrData = new
                    {
                        attendanceToken = qrToken
                    };

                    QrValue = JsonSerializer.Serialize(qrData);
                    IsQrVisible = true;
                    AppStorage.ClearQrToken();
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Qr Generating Error " + ex.ToString());
                await Shell.Current.DisplayAlert("Qr Error", ex.ToString(), "OK");
            }
            finally
            {
                IsBusy = false;
            }

        }
    }
}
